using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStealth : MonoBehaviour
{
    [HideInInspector]
    public bool isHidden = false;
    PlayerController pc;
    private void Start()
    {
        pc = GetComponent<PlayerController>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("HideZone") && pc.isCrouched)
        {
            isHidden = true;
            Debug.Log("hidden");
        }
        else if(other.CompareTag("HideZone") && !pc.isCrouched)
        {
            isHidden = false;
            Debug.Log("not hidden anymore");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HideZone"))
        {
            isHidden = false;
            Debug.Log("not hidden");
        }
    }

}
