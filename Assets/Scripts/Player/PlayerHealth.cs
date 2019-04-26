using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100.0f;

    private bool isSielded;

    private Animator anim;

    private Image health_Img;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        health_Img = GameObject.Find("Health Icon").GetComponent<Image>();
    }

    public bool Shielded
    {
        get
        {
            return isSielded;
        }

        set
        {
            isSielded = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TakeDamage(float amount)
    {
        if (!isSielded)
        {
            health -= amount;

            health_Img.fillAmount = health / 100f;

            print("Player Took Damage");

            if (health <= 0)
            {
                // DEATH
                anim.SetBool("Death", true);

                if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Death")
                    && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    //PLAYER DIED
                    //DESTROY PLAYER

                }
            }
        }
    }

    public void HealPlayer(float healAmount)
    {
        health += healAmount;

        if (health > 100f)
            health = 100f;

        health_Img.fillAmount = health / 100f;
    }
}
