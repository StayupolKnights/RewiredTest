﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    PlayerInput playerInput;

    public GameColor playerColor;
    public GameObject bodyPartPrefab;

    public List<GameObject> playerBody = new List<GameObject>();
    List<Vector3> lastBodyLocations = new List<Vector3>();
    List<Vector3> lastBodyRotations = new List<Vector3>();

    public int maxBatteryLife = 10;
    public int currentBatteryLife;

    public int playerBodyCount = 6;
    public bool allowedToMove = true;

    public float unpauseTime = 0;


    public GameObject BatteryObject;

public float PowerInt;
    GameMaster master; 
    void Start()
    {
        currentBatteryLife = maxBatteryLife;
        
        playerInput = GetComponent<PlayerInput>();
        foreach (Transform child in transform)
        {
            playerBody.Add(child.gameObject);
        }
        master = GameObject.Find("Game Master").GetComponent<GameMaster>();
    }
    

    public void MovePlayer(Vector2Int moveDirection)
    {
        allowedToMove = (Time.time >= unpauseTime);
        if (master.gameRunning == false) {allowedToMove = false;}
        
        if (currentBatteryLife > 0)
        {
            currentBatteryLife--;
            updateBlueBattery();
            ResetToGrid();

            if (allowedToMove)
            {
                SaveLastLocations();
                MoveHead(moveDirection);
                MoveBodyParts();
                GrowSnake();
            }

            ResetToGrid();
        }
        
    }

    public void PlayerPlugIn()
    {
        PausePlayer();
        
        playerBody[0].tag = "Player Head";
        playerBody[playerBody.Count - 1].tag = "Active Player Head";
        playerBody.Reverse();
        SaveLastLocations();
        playerInput.SetPlayerDirection(lastBodyRotations[0]);
        // SaveLastLocations();
    }

    void PausePlayer()
    {
        unpauseTime = Time.time + master.playerPauseTime;
    }

    void SaveLastLocations()
    {
        lastBodyLocations.Clear();
        lastBodyRotations.Clear();

        foreach (GameObject bodyPart in playerBody)
        {
            lastBodyLocations.Add(bodyPart.transform.position);
            lastBodyRotations.Add(bodyPart.transform.eulerAngles);
        }
    }

    void MoveHead(Vector2Int moveDirection)
    {
        if (moveDirection == new Vector2Int(0, 1)) //UP
        {
            playerBody[0].transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (moveDirection == new Vector2Int(1, 0)) //RIGHT
        {
            playerBody[0].transform.eulerAngles = new Vector3(0, 0, -90);
        }
        if (moveDirection == new Vector2Int(0, -1)) //DOWN
        {
            playerBody[0].transform.eulerAngles = new Vector3(0, 0, 180);
        }
        if (moveDirection == new Vector2Int(-1, 0)) //LEFT
        {
            playerBody[0].transform.eulerAngles = new Vector3(0, 0, 90);
        }

        // maybe check for collision
        playerBody[0].transform.Translate(new Vector3(moveDirection.x, moveDirection.y, 0), Space.World);
    }

    void MoveBodyParts()
    {
        for (int i = 1; i < playerBody.Count; i++)
        {
            playerBody[i].transform.position = lastBodyLocations[i - 1];
            if (i != playerBody.Count - 1)
            {
                playerBody[i].transform.eulerAngles = lastBodyRotations[i - 1];
            }
            else
            {
                playerBody[i].transform.eulerAngles = new Vector3(
                    0,
                    0,
                    lastBodyRotations[i - 1].z + 180.0f
                );
            }
        }
    }

    void GrowSnake()
    {
        if (playerBodyCount > playerBody.Count)
        {
            GameObject newBodyPart = Instantiate(bodyPartPrefab, lastBodyLocations[lastBodyLocations.Count - 1], Quaternion.identity);
            newBodyPart.transform.parent = this.gameObject.transform;
            newBodyPart.name = "Body (" + (playerBody.Count - 1) + ")";

            playerBody.Insert((playerBody.Count - 1), newBodyPart);
        }
    }

    void ResetToGrid()
    {
        playerBody[0].transform.position = new Vector3(
            Mathf.RoundToInt(playerBody[0].transform.position.x),
            Mathf.RoundToInt(playerBody[0].transform.position.y),
            0.0f
        );

        float r = playerBody[0].transform.eulerAngles.z;
        if (r >= -45.0f && r < 45.0f)
        {
            r = 0;
        }
        else if (r >= 45.0f && r < 135.0f)
        {
            r = 90;
        }
        else if (r >= 135.0f && r < -135.0f)
        {
            r = 180;
        }
        else if (r >= -135.0f && r < -45.0f)
        {
            r = -90;
        }
    }

    void updateBlueBattery()
        {

        List<SpriteRenderer> cells = new List<SpriteRenderer>();
        foreach (Transform child in BatteryObject.transform)
         {
            cells.Add(child.gameObject.GetComponent<SpriteRenderer>());
         }

        int Powerincrement = (maxBatteryLife / 5);
        int CurrentPower = currentBatteryLife;

        PowerInt = CurrentPower/Powerincrement;
        
        if (PowerInt >= 4)
        {
            cells[0].enabled = false;
            cells[1].enabled = false;
            cells[2].enabled = false;
            cells[3].enabled = false;
            cells[4].enabled = true;
        }
        else if (PowerInt >= 3)
        {
            cells[0].enabled = false;
            cells[1].enabled = false;
            cells[2].enabled = false;
            cells[3].enabled = true;
            cells[4].enabled = false;
        }

        else if (PowerInt >= 2)
        {
            cells[0].enabled = false;
            cells[1].enabled = false;
            cells[2].enabled = true;
            cells[3].enabled = false;
            cells[4].enabled = false;
        }
        else if (PowerInt >= 1)
        {
            cells[0].enabled = false;
            cells[1].enabled = true;
            cells[2].enabled = false;
            cells[3].enabled = false;
            cells[4].enabled = false;
        }
        else if (PowerInt >= 0 && currentBatteryLife != 0) 
        {
            cells[0].enabled = true;
            cells[1].enabled = false;
            cells[2].enabled = false;
            cells[3].enabled = false;
            cells[4].enabled = false;
        }

        if (currentBatteryLife == 0)
        {
           cells[0].enabled = false;
           cells[1].enabled = false;
           cells[2].enabled = false;
           cells[3].enabled = false;
           cells[4].enabled = false;
        }


    }
}
