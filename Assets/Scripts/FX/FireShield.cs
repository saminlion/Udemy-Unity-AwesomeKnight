using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireShield : MonoBehaviour
{
    private PlayerHealth playerHealth;

    void Awake()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        playerHealth.Shielded = true;
    }

    private void OnDisable()
    {
        playerHealth.Shielded = false;
    }

} // class
