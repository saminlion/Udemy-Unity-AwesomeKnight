using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControlPath : MonoBehaviour
{
    public Transform[] walkPoints;
    private int walkIndex = 0;

    private Transform playerTarget;

    private Animator anim;

    private NavMeshAgent navAgent;

    private float walkDistance = 8.0f;
    private float attackDistance = 2.0f;

    private float currentAttackTime;
    private float waitAttackTime = 1.0f;

    private Vector3 nextDestination;

    private EnemyHealth enemyHealth;

    void Awake()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void MoveAndAttack()
    {
        float distance = Vector3.Distance(transform.position, playerTarget.position);

        if (distance > walkDistance)
        {
            if (navAgent.remainingDistance <= 0.5f)
            {
                navAgent.isStopped = false;

                anim.SetBool("Walk", true);
                anim.SetBool("Run", false);
                anim.SetInteger("Atk", 0);

                nextDestination = walkPoints[walkIndex].position;
                navAgent.SetDestination(nextDestination);

                if (walkIndex == walkPoints.Length - 1)
                {
                    walkIndex = 0;
                }
                else
                {
                    walkIndex++;
                }
            }
        }

        else
        {
            if (distance > attackDistance)
            {
                navAgent.isStopped = false;

                anim.SetBool("Walk", false);
                anim.SetBool("Run", true);
                anim.SetInteger("Atk", 0);

                navAgent.SetDestination(playerTarget.position);
            }

            else
            {
                navAgent.isStopped = true;

                navAgent.isStopped = false;

                anim.SetBool("Run", false);

                Vector3 targetPosition = new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z);

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPosition - transform.position), 5.0f * Time.deltaTime);

                if (currentAttackTime >= waitAttackTime)
                {
                    int atkRange = Random.Range(1, 3);

                    anim.SetInteger("Atk", atkRange);

                    currentAttackTime = 0.0f;
                }

                else
                {
                    anim.SetInteger("Atk", 0);

                    currentAttackTime += Time.deltaTime;
                }

                anim.SetInteger("Atk", 0);

                navAgent.SetDestination(playerTarget.position);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHealth.health > 0)
        {
            MoveAndAttack();
        }

        else
        {
            anim.SetBool("Death", true);

            navAgent.enabled = false;

            if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Death")
                && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
            {
                Destroy(gameObject, 2.0f);
            }
        }
 
    }

}// class






























