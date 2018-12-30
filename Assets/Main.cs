using System.Collections;
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
    // GameObject waypoints;

    // bool handlingHitWaypoint;

    void Start()
    {
        // handlingHitWaypoint = false;


        // Turn off physics
        // Physics.autoSimulation = false;

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

        // -- Waypoints --
        // waypoints = new GameObject();
        // waypoints.name = "Waypoints";
        // waypoints.AddComponent<Waypoints>();
        // waypoints.GetComponent<Waypoints>().Init(this);

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
        // Object.Destroy(waypoints);
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
    
    // public Vector3 GetCurrentWaypoint() 
    // {
    //     return waypoints.GetComponent<Waypoints>().GetCurrentWaypoint();
    // }

    // public Vector3 AddTurningWaypoint(Vector3 headPosition, Vector3 headDirection, bool isRightTurn) 
    // {
    //     return waypoints.GetComponent<Waypoints>().AddTurningWaypoint(headPosition, headDirection, isRightTurn);
    // }

    public void HandleHitFood() 
    {
        food.GetComponent<Food>().Reposition();
        player.GetComponent<Player>().Grow();
    }

    // public void HandleHitWaypoint() 
    // {
    //     Player playerScript = player.GetComponent<Player>();

    //     if (handlingHitWaypoint) {
    //         return;
    //     }
    //     handlingHitWaypoint = true;

    //     playerScript.beforeAtWaypoint();

    //     Vector3 currentWaypoint = waypoints.GetComponent<Waypoints>().WaypointReached();
    //     playerScript.PerformTurn(currentWaypoint);

    //     playerScript.afterAtWaypoint();

    //     handlingHitWaypoint = false;
    // }

}
