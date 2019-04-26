using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTornadoMove : MonoBehaviour
{
    public LayerMask enemyLayer;
    public float radius = 0.5f;
    public float damageCount = 10.0f;
    public GameObject fireExposion;

    private EnemyHealth enemyHealth;
    private bool collided;

    private float speed = 3.0f;


    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        transform.rotation = Quaternion.LookRotation(player.transform.forward);

    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CheckForDamage();
    }

    void Move()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
    }

    void CheckForDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        foreach (Collider c in hits)
        {
            if (c.isTrigger)
                continue;

            enemyHealth = c.GetComponent<EnemyHealth>();
            collided = true;
        }

        if (collided)
        {
            enemyHealth.TakeDamage(damageCount);

            Vector3 temp = transform.position;
            temp.y += 2.0f;

            Instantiate(fireExposion, temp, Quaternion.identity);

            Destroy(gameObject);
        }
    }

}// class
























