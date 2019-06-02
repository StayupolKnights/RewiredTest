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

    public int playerBodyCount = 6;
    public bool allowedToMove = true;



    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        foreach (Transform child in transform)
        {
            playerBody.Add(child.gameObject);
        }
    }

    public void MovePlayer(Vector2Int moveDirection)
    {
        Quaternion playerRotation = playerBody[0].transform.rotation;
        ResetToGrid(playerRotation);

        if (allowedToMove)
        {
            SaveLastLocations();
            MoveHead(moveDirection);
            MoveBodyParts();
            GrowSnake();
        }

        ResetToGrid(playerRotation);
    }

    public void PlayerPlugIn()
    {
        playerBody[0].tag = "Player Head";
        playerBody[playerBody.Count - 1].tag = "Active Player Head";
        playerBody.Reverse();
        playerInput.ReversePlayerDirection();
        SaveLastLocations();
    }

    void SaveLastLocations()
    {
        lastBodyLocations.Clear();

        foreach (GameObject bodyPart in playerBody)
        {
            lastBodyLocations.Add(bodyPart.transform.position);
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

    void ResetToGrid(Quaternion rotation)
    {
        playerBody[0].transform.position = new Vector3(
            Mathf.RoundToInt(playerBody[0].transform.position.x),
            Mathf.RoundToInt(playerBody[0].transform.position.y),
            0.0f
        );
        playerBody[0].transform.rotation = rotation;
    }
}
