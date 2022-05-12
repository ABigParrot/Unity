using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    private Enemy enemy;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(enemy, 5);
    }
}
