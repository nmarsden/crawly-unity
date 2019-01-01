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
    const float TAIL_MIN_DISTANCE = 3f;
    const bool IS_SHOW_TURNING_POINTS = false;
    const bool IS_SHOW_CELL_TRIGGERS = false;

    GameObject turningPoints;
    GameObject arena;
    GameObject player;
    GameObject food;
    GameObject hud;

    bool isGameOver;

    void Start()
    {
        Time.timeScale = 1;
        isGameOver = false;

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

        // -- HUD --
        hud = new GameObject();
        hud.name = "HUD";
        hud.AddComponent<HUD>();
        hud.GetComponent<HUD>().Init(this);
    }

    void Update()
    {
        if (isGameOver) {
            // -- Game Over --
            // Wait for key press to Reset game
            if (Input.anyKey) 
            {
                Reset();
            }
        } else {
            // -- Game In Progress --
            // Reset when 'R' pressed
            if (Input.GetKeyDown(KeyCode.R)) 
            {
                Reset();
            }
        }
    }

    void Reset() {
        DestroyAllCreatedObjects();
        Start();
    }

    void DestroyAllCreatedObjects() {

        Object.Destroy(hud);
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

    public float GetTailMinDistance() {
        return TAIL_MIN_DISTANCE;
    }

    public bool IsShowTurningPoints() {
        return IS_SHOW_TURNING_POINTS;
    }

    public bool IsShowCellTriggers() {
        return IS_SHOW_CELL_TRIGGERS;
    }

    public string AddTurningPoint(Vector3 position, Vector3 incomingDirection, Vector3 outgoingDirection) {
        return turningPoints.GetComponent<TurningPoints>().AddTurningPoint(position, incomingDirection, outgoingDirection);
    }

    public TurningPoints GetTurningPoints() {
        return turningPoints.GetComponent<TurningPoints>();
    }

    public void HandleHitFood() 
    {
        // Reposition food in an empty position
        List<Vector3> emptyPositions = arena.GetComponent<Arena>().GetEmptyPositions();
        var position = emptyPositions[Random.Range(0, emptyPositions.Count)];
        food.GetComponent<Food>().Reposition(position.x, position.z);

        // Grow player
        player.GetComponent<Player>().Grow();
    }

    public void HandleHitWall() {
        GameOver();
    }

    public void HandleHitTail() {
        GameOver();
    }

    private void GameOver() {
        // Switch to Game Over mode
        isGameOver = true;

        // Pause Game
        Time.timeScale = 0;

        // Display Game Over message
        hud.GetComponent<HUD>().ShowGameOverMessage();
    }

}
