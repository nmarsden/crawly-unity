﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    const float GRID_SPACING = 7f; //20f; //5f;
    const float ARENA_WIDTH = GRID_SPACING * 11; //105f;
    const float PLAYER_HEIGHT = 5f;
    const float PLAYER_WIDTH = 5f;

    const float PLAYER_SPEED = 10f;

    GameObject arena;
    GameObject player;
    GameObject food;
    GameObject turningPoints;

    void Start()
    {
        // -- Turning Points --
        turningPoints = new GameObject();
        turningPoints.name = "Turning Points";
        turningPoints.AddComponent<TurningPoints>();
        turningPoints.GetComponent<TurningPoints>().Init(this);

        // -- Arena --
        arena = new GameObject();
        arena.name = "Arena";
        arena.AddComponent<Arena>();
        arena.GetComponent<Arena>().Init(this);

        // -- Player --
        player = new GameObject();
        player.name = "Player";
        player.AddComponent<Player>();
        player.GetComponent<Player>().Init(this);

        // -- Food Container --
        food = new GameObject();
        food.name = "Food Container";
        food.AddComponent<Food>();
        food.GetComponent<Food>().Init(this);
    }

    void Update()
    {
        // Reset when 'R' pressed
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            Reset();
        }
    }

    void Reset() {
        DestroyAllCreatedObjects();
        Start();
    }

    void DestroyAllCreatedObjects() {
        Object.Destroy(food);
        Object.Destroy(player);
        Object.Destroy(arena);
        Object.Destroy(turningPoints);
    }

    public float GetArenaWidth() 
    {
        return ARENA_WIDTH;
    }

    public float GetGridSpacing() 
    {
        return GRID_SPACING;
    }

    public float GetPlayerHeight() 
    {
        return PLAYER_HEIGHT;
    }

    public float GetPlayerWidth() 
    {
        return PLAYER_WIDTH;
    }

    public float GetPlayerSpeed() 
    {
        return PLAYER_SPEED;
    }

    public string AddTurningPoint(Vector3 position, float time, Vector3 incomingDirection, Vector3 outgoingDirection) {
        return turningPoints.GetComponent<TurningPoints>().AddTurningPoint(position, time, incomingDirection, outgoingDirection);
    }

    public TurningPoints GetTurningPoints() {
        return turningPoints.GetComponent<TurningPoints>();
    }

    public void HandleHitFood() 
    {
        food.GetComponent<Food>().Reposition();
        player.GetComponent<Player>().Grow();
    }
}
