using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public TextMeshProUGUI pHealthText;

    private void Start()
    {
        pHealthText.text = "Health: " + health.ToString();
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        pHealthText.text = "Health: " + health.ToString();
    }
}
