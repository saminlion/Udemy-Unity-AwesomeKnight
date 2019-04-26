using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    IDLE,
    WALK,
    RUN,
    PAUSE,
    GOBACK,
    ATTACK,
    DEATH
};

public class EnemyController : MonoBehaviour
{
    private float attackDistance = 1.5f;
    private float alertAttackDistance = 8.0f;
    private float followDistance = 15.0f;

    private float enemyTomPlayerDistance;

    [HideInInspector]
    public EnemyState enemyCurrentState = EnemyState.IDLE;
    private EnemyState enemyLastState = EnemyState.IDLE;

    private Transform playerTarget;
    private Vector3 initialPosition;

    private float moveSpeed = 2.0f;
    private float walkSpeed = 1.0f;

    //private CharacterController charController;
    private Vector3 whereToMove = Vector3.zero;

    private float currentAttackTime;
    private float waitAttackTime = 1.0f;

    private Animator anim;
    private bool finishedAnimation = true;
    private bool finishedMovement = true;

    private NavMeshAgent navAgent;
    private Vector3 whereToNavigate;

    // HEALTH SCRIPT
    private EnemyHealth enemyHealth;

    void Awake()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        navAgent = GetComponent<NavMeshAgent>();
        //charController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();

        initialPosition = transform.position;
        whereToNavigate = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // IF HEALTH IS <= 0 WE WILL SET THE STATE TO DEATH
        if (enemyHealth.health <= 0f)
        {
            enemyCurrentState = EnemyState.DEATH;
        }
        
        if (enemyCurrentState != EnemyState.DEATH)
        {
            enemyCurrentState = SetEnemyState(enemyCurrentState, enemyLastState, enemyTomPlayerDistance);

            if (finishedMovement)
            {
                GetStateControl (enemyCurrentState);
            }
            else
            {
                if(!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    finishedMovement = true;
                }
                else if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsTag("Atk1") 
                    || anim.GetCurrentAnimatorStateInfo(0).IsTag("Atk2"))
                {
                    anim.SetInteger("Atk", 0);
                }
            }
        }

        else
        {
            anim.SetBool("Death", true);

            //charController.enabled = false;
            navAgent.enabled = false;

            if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Death")
                && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
            {
                Destroy(gameObject, 2.0f);
            }
        }
    }

    EnemyState SetEnemyState(EnemyState curState, EnemyState lastState, float enemyToPlayerDis)
    {
        enemyToPlayerDis = Vector3.Distance(transform.position, playerTarget.position);

        float initialDistance = Vector3.Distance(initialPosition, transform.position);

        if (initialDistance > followDistance)
        {
            lastState = curState;
            curState = EnemyState.GOBACK;
        }

        else if (enemyToPlayerDis <= attackDistance)
        {
            lastState = curState;
            curState = EnemyState.ATTACK;
        }

        else if (enemyToPlayerDis >= alertAttackDistance && lastState == EnemyState.PAUSE || lastState == EnemyState.ATTACK)
        {
            lastState = curState;
            curState = EnemyState.PAUSE;
        }

        else if (enemyToPlayerDis <= alertAttackDistance && enemyToPlayerDis > attackDistance)
        {
            if (curState != EnemyState.GOBACK || lastState == EnemyState.WALK)
            {
                lastState = curState;
                curState = EnemyState.PAUSE;
            }
        }

        else if (enemyToPlayerDis > alertAttackDistance && lastState != EnemyState.GOBACK && lastState != EnemyState.PAUSE)
        {
            lastState = curState;
            curState = EnemyState.WALK;
        }

        return curState;
    }

    void GetStateControl(EnemyState curState)
    {
        if (curState == EnemyState.RUN || curState == EnemyState.PAUSE)
        {
            if (curState != EnemyState.ATTACK)
            {
                Vector3 targetPos = new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z);

                anim.SetBool("Walk", false);
                anim.SetBool("Run", true);

                navAgent.SetDestination(targetPos);
            }            
        }

        else if (curState == EnemyState.ATTACK)
        {
            anim.SetBool("Run", false);

            whereToMove.Set(0.0f, 0.0f, 0.0f);

            navAgent.SetDestination(transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerTarget.position - transform.position),
                5.0f * Time.deltaTime);

            if (currentAttackTime >= waitAttackTime)
            {
                int atkRange = Random.Range(1, 3);

                anim.SetInteger("Atk", atkRange);
                finishedAnimation = false;
                currentAttackTime = 0.0f;
            }
            else
            {
                anim.SetInteger("Atk", 0);
                currentAttackTime += Time.deltaTime;
            }
        }

        else if (curState == EnemyState.GOBACK)
        {
            anim.SetBool("Run", true);

            Vector3 targetPos = new Vector3(initialPosition.x, transform.position.y,
                initialPosition.z);

            navAgent.SetDestination(targetPos);

            if (Vector3.Distance(targetPos, initialPosition) <= 3.5f)
            {
                enemyLastState = curState;
                curState = EnemyState.WALK;
            }
        }

        else if (curState == EnemyState.WALK)
        {
            anim.SetBool("Run", false);
            anim.SetBool("Walk", true);

            if (Vector3.Distance(transform.position, whereToNavigate) <= 2.0f)
            {
                whereToNavigate.x = Random.Range(initialPosition.x - 5.0f, initialPosition.x + 5.0f);
                whereToNavigate.z = Random.Range(initialPosition.z - 5.0f, initialPosition.z + 5.0f);
            }
            else
            {
                navAgent.SetDestination(whereToNavigate);
            }
        }

        else
        {
            anim.SetBool("Run", false);
            anim.SetBool("Walk", false);
            whereToMove.Set(0f, 0f, 0f);
            navAgent.isStopped = true;
        }

        //charController.Move(whereToMove);
    }

}// class
