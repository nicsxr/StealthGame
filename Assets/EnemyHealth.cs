using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    public int health;
    public TextMeshPro txt;

    private void Start()
    {
        txt.text = health.ToString();
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        txt.text = health.ToString();
    }

    private void Update()
    {
        if(health <= 0)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
