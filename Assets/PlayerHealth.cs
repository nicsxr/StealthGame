using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.LogError("Player Health " + health);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            
        }
    }
}
