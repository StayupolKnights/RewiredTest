﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Player Head":
                Debug.Log("Hit a player head");
                other.GetComponentInParent<PlayerScript>().allowedToMove = false;
                break;
                
            case "Outlet":
                Debug.Log("Hit an outlet");
                break;
        }
    }
}