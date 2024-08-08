using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 5;
    private float health;

    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
