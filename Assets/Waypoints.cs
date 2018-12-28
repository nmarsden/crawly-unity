using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    private bool isShowWaypoints = true;
    const float WAYPOINT_WIDTH = 0.1f;
    Color waypointColor = new Color32(255, 255, 255, 255);
    GameObject waypoint;
    // GameObject waypoint2;
    private Vector3[] waypoints;
    private int waypointsSize;
    private Vector3 currentWaypoint;
    private float gridSpacing;

    public void Init(Main main) 
    {
        gridSpacing = main.GetGridSpacing();        

        // Waypoints
        waypoints = new Vector3[100];

        // TODO yPos should be zero by lowering the arena floor based on player height
        var yPos = 2.5f;
        waypoints[0] = new Vector3(0, yPos, gridSpacing * 30);
        //waypoints[0] = headPosition + (headDirection * gridSpacing * 30);
        waypointsSize = 1;
        currentWaypoint = waypoints[0];
    }

    void Start()
    {
        // Waypoint
        waypoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
        waypoint.name = "Waypoint";
        waypoint.transform.parent = transform;    
        waypoint.transform.localScale = new Vector3(WAYPOINT_WIDTH, WAYPOINT_WIDTH, WAYPOINT_WIDTH);        
        waypoint.GetComponent<Renderer>().material.color = waypointColor;
        waypoint.GetComponent<Renderer>().enabled = isShowWaypoints;
        waypoint.transform.position = currentWaypoint;

        // // Waypoint 2
        // waypoint2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // waypoint2.name = "Waypoint 2";
        // waypoint2.transform.parent = transform;    
        // waypoint2.transform.localScale = new Vector3(3, 3, 3);        
        // waypoint2.GetComponent<Renderer>().material.color = waypointColor;
        // waypoint2.GetComponent<Renderer>().enabled = isShowWaypoints;
        // waypoint2.transform.position = currentWaypoint;
    }

    void Update()
    {
    }

    public Vector3 GetCurrentWaypoint() {
        return currentWaypoint;
    }

    public Vector3 AddTurningWaypoint(Vector3 headPosition, Vector3 headDirection, bool isRightTurn) {

        Debug.Log("AddTurningWaypoint: [headPosition:" + headPosition + "][headDirection:" + headDirection + "][isRightTurn:" + isRightTurn + "]");

        // Add waypoint
        var remainder = 0F;
        var headDirectionPositive = (headDirection.x > 0.1 || headDirection.z > 0.1);
        var positionPositive = false;
        if (headDirection.x > 0.1 || headDirection.x < -0.1) {
            remainder = (headPosition.x % gridSpacing);
            positionPositive = (headPosition.x > 0);
        } else {
            remainder = (headPosition.z % gridSpacing);
            positionPositive = (headPosition.z > 0);
        }
        if (headDirectionPositive) {
            if (positionPositive) {
                waypoints[waypointsSize] = headPosition + (headDirection * (gridSpacing - remainder));
            } else {
                waypoints[waypointsSize] = headPosition - (headDirection * remainder);
            }
        } else {
            if (positionPositive) {
                waypoints[waypointsSize] = headPosition + (headDirection * remainder);
            } else {
                waypoints[waypointsSize] = headPosition + (headDirection * (gridSpacing + remainder));
            }
        }
        waypointsSize++;
        currentWaypoint = waypoints[waypointsSize-1];
        
        // Update initial waypoint
        if (isRightTurn) {
            waypoints[0] = currentWaypoint + (Quaternion.AngleAxis(90, Vector3.up) * headDirection * gridSpacing * 30);
        } else {
            waypoints[0] = currentWaypoint + (Quaternion.AngleAxis(-90, Vector3.up) * headDirection * gridSpacing * 30);
        }

        // Update drawn waypoints
        waypoint.transform.position = currentWaypoint;
        // if (waypointsSize > 1) {
        //     waypoint2.transform.position = waypoints[1];
        // }

        return currentWaypoint;
    }

    public Vector3 WaypointReached() 
    {
        // Go to next way point
        waypointsSize--;
        currentWaypoint = waypoints[waypointsSize-1];
        waypoint.transform.position = currentWaypoint;

        Debug.Log("WaypointReached: [new currentWaypoint:" + currentWaypoint + "]");

        return currentWaypoint;
    }
}
