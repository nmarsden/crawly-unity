﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public enum TurnCommand {Right, Left, None};

    TurnCommand turnCommand;

    GameObject head;
    GameObject tail;
    bool turning = false;
    float turningStartTime;
    Vector3 direction;
    float rotation = 0;

    private GameObject[] bodies;
    private int bodiesSize;

    private float minDistance = 5; //20; //15;
    private float speed;
    private float gridSpacing;
    private float arenaWidth;
    private float yPos = 0;

    private Vector3 startPos;
    private float startTime;
    private float journeyLength;
    private float journeyTime;
    private bool isKeyPressed;

    Color bodyColor = new Color32(0, 200, 0, 255);
    Color antennaColor = new Color32(238,130,238, 255);
    Color hatColor = new Color32(0, 0, 200, 255);
    Color noseColor = new Color32(0, 100, 0, 255);
    Color eyeColor = new Color32(255, 255, 255, 255);
    Color pupilColor = new Color32(0, 0, 0, 255);
    Color mouthColor = new Color32(0, 0, 0, 255);

    Main main;
    // Vector3 currentWaypoint;
    private float playerHeight;
    private float playerWidth;

    bool isAtWaypoint = false;

    Rigidbody headRigidbody;

    Quaternion headRotation;
    // GameObject gp1;
    // GameObject gp2;

    public void Init(Main main) {
        this.main = main;
        arenaWidth = main.GetArenaWidth();
        gridSpacing = main.GetGridSpacing();
        playerHeight = main.GetPlayerHeight();
        playerWidth = main.GetPlayerWidth();
        speed = main.GetPlayerSpeed();
        // this.currentWaypoint = main.GetCurrentWaypoint();
    }

    void Start()
    {
        // Direction
        direction = Vector3.forward;

        // Turn command
        turnCommand = TurnCommand.None;

        // Head
        head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.parent = transform;    
        head.transform.localScale = new Vector3(playerWidth, playerHeight, playerWidth);        
        head.transform.position = new Vector3(0, yPos, 0);
        head.GetComponent<Renderer>().material.color = bodyColor;
        head.AddComponent<Rigidbody>();

        headRigidbody = head.GetComponent<Rigidbody>();
        headRigidbody.isKinematic = false;
        headRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        headRigidbody.velocity = direction * speed;

        head.AddComponent<Head>();
        head.GetComponent<Head>().Init(main);

        // Left Eye
        GameObject leftEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leftEye.name = "Left Eye";
        leftEye.transform.localScale = new Vector3(2, 3, 2);
        leftEye.GetComponent<Renderer>().material.color = eyeColor;
        leftEye.transform.parent = head.transform;        
        leftEye.transform.position = head.transform.position;
        leftEye.transform.Translate(1, 1, 1.5f);
        leftEye.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid altering the head's center-of-mass

        // Left Pupil
        GameObject leftPupil = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leftPupil.name = "Left Pupil";
        leftPupil.transform.localScale = new Vector3(1, 2, 1);
        leftPupil.GetComponent<Renderer>().material.color = pupilColor;
        leftPupil.transform.parent = head.transform;        
        leftPupil.transform.position = head.transform.position;
        leftPupil.transform.Translate(1, 1.5f, 2.25f);
        leftPupil.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid altering the head's center-of-mass

        // Right Eye
        GameObject rightEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightEye.name = "Right Eye";
        rightEye.transform.localScale = new Vector3(2, 3, 2);
        rightEye.GetComponent<Renderer>().material.color = eyeColor;
        rightEye.transform.parent = head.transform;        
        rightEye.transform.position = head.transform.position;
        rightEye.transform.Translate(-1, 1, 1.5f);
        rightEye.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid altering the head's center-of-mass

        // Right Pupil
        GameObject rightPupil = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightPupil.name = "Right Pupil";
        rightPupil.transform.localScale = new Vector3(1, 2, 1);
        rightPupil.GetComponent<Renderer>().material.color = pupilColor;
        rightPupil.transform.parent = head.transform;        
        rightPupil.transform.position = head.transform.position;
        rightPupil.transform.Translate(-1, 1.5f, 2.25f);
        rightPupil.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid altering the head's center-of-mass

        // Nose
        GameObject nose = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        nose.name = "Nose";
        nose.transform.localScale = new Vector3(3, 2, 2);
        nose.GetComponent<Renderer>().material.color = noseColor;
        nose.transform.parent = head.transform;        
        nose.transform.position = head.transform.position;
        nose.transform.Translate(0, 0, 2.5f);
        nose.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid altering the head's center-of-mass

        // Mouth
        GameObject mouth = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        mouth.name = "Mouth";
        mouth.transform.localScale = new Vector3(3, 0.5f, 2);
        mouth.GetComponent<Renderer>().material.color = mouthColor;
        mouth.transform.parent = head.transform;        
        mouth.transform.position = head.transform.position;
        mouth.transform.Translate(0, -1.5f, 1f);
        mouth.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
        mouth.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid altering the head's center-of-mass

        // Tail
        tail = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tail.name = "Tail 1";
        tail.transform.parent = transform;    
        tail.transform.localScale = new Vector3(playerWidth, playerHeight, playerWidth);        
        tail.transform.position = new Vector3(0, yPos, -minDistance);
        tail.GetComponent<Renderer>().material.color = bodyColor;
        tail.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid bumping the head while moving

        tail.AddComponent<Rigidbody>();

        var tailRigidbody = tail.GetComponent<Rigidbody>();
        tailRigidbody.isKinematic = false;
        tailRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        tailRigidbody.velocity = direction * speed;

        tail.AddComponent<Tail>();
        tail.GetComponent<Tail>().Init(main.GetTurningPoints(), speed);

        // Bodies
        // TODO Use a List instead of an array for the bodies
        bodies = new GameObject[100];
		bodies[0] = head;
		bodies[1] = tail;
		bodiesSize = 2;

        // UpdateCurrentWaypoint(currentWaypoint);

        // Grid Point 1
        // gp1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // gp1.name = "GP1";
        // gp1.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        // gp1.GetComponent<Renderer>().material.color = new Color32(200, 0, 0, 255);
        // gp1.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid altering the head's center-of-mass

        // // Grid Point 2
        // gp2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // gp2.name = "GP2";
        // gp2.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        // gp2.GetComponent<Renderer>().material.color = new Color32(200, 0, 0, 255);
        // gp2.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid altering the head's center-of-mass
    }

    void Update() {
        if (!turning && turnCommand == TurnCommand.None) {
            float horizontal = Input.GetAxisRaw("Horizontal");

            // Debug.Log("horizontal: " + horizontal);

            if (horizontal != 0) {
                turnCommand = (horizontal == 1) ? TurnCommand.Right : TurnCommand.Left;
            }
        }
    }
    
    void FixedUpdate() {

        if (!turning) {

            if (turnCommand != TurnCommand.None) {
                float diff;
                Vector3 gridPos1;
                Vector3 gridPos2;

                // Determine whether head is allowed to turn at its current position
                if (direction == Vector3.forward || direction == Vector3.back) {
                    //diff = Mathf.Abs(head.transform.position.z % gridSpacing);

                    var zPos1 = Mathf.Floor(head.transform.position.z / gridSpacing) * gridSpacing;
                    var zPos2 = zPos1 + gridSpacing;
                    gridPos1 = new Vector3(head.transform.position.x, head.transform.position.y, zPos1);
                    gridPos2 = new Vector3(head.transform.position.x, head.transform.position.y, zPos2);

                    // Debug.Log("Checking for turn position: [direction:" + direction + "][head: " + head.transform.position + "][behind: " + zBehind + "][ahead:" + zAhead + "]");

                } else {
                    //diff = Mathf.Abs(head.transform.position.x % gridSpacing);

                    var xPos1 = Mathf.Floor(head.transform.position.x / gridSpacing) * gridSpacing;
                    var xPos2 = xPos1 + gridSpacing;
                    gridPos1 = new Vector3(xPos1, head.transform.position.y, head.transform.position.z);
                    gridPos2 = new Vector3(xPos2, head.transform.position.y, head.transform.position.z);
                }
                // Update gp1 & gp2
                // gp1.transform.position = gridPos1 + (Vector3.down * playerHeight * 0.5f);
                // gp2.transform.position = gridPos2 + (Vector3.down * playerHeight * 0.5f);

                var distanceToGridPos1 = Vector3.Distance(head.transform.position, gridPos1);
                var distanceToGridPos2 = Vector3.Distance(head.transform.position, gridPos2);
                diff = Mathf.Min(distanceToGridPos1, distanceToGridPos2);

                // Debug.Log("Checking for turn position: [diff: " + diff + "][head: " + head.transform.position + "][gp1: " + gridPos1 + "][gp2: " + gridPos2 + "]");

                // TODO the threshold needs to take into account the velocity
                var threshold = 0.01F;
                if (diff < threshold) {
                    turning = true;

                    var headPositionBeforeSnap = head.transform.position;

                    // Snap head position to grid
                    var snapGridPos = (distanceToGridPos1 < distanceToGridPos2) ? gridPos1 : gridPos2;
                    head.transform.position = snapGridPos;

                    // if (direction == Vector3.forward || direction == Vector3.back) {
                    //     var zPos = (head.transform.position.z > 0) ? 
                    //                     Mathf.FloorToInt(head.transform.position.z / gridSpacing) * gridSpacing : 
                    //                     Mathf.CeilToInt(head.transform.position.z / gridSpacing) * gridSpacing;
                    //     head.transform.position = new Vector3(head.transform.position.x, head.transform.position.y, zPos);
                    // } else {
                    //     var xPos = (head.transform.position.x > 0) ? 
                    //                     Mathf.FloorToInt(head.transform.position.x / gridSpacing) * gridSpacing : 
                    //                     Mathf.CeilToInt(head.transform.position.x / gridSpacing) * gridSpacing;
                    //     head.transform.position = new Vector3(xPos, head.transform.position.y, head.transform.position.z);
                    // }

                    // Debug.Log("Turning Head [head (pre-snap):" + headPositionBeforeSnap + "][head (post-snap):" + head.transform.position + "]");

                    var snapDistance = Vector3.Distance(headPositionBeforeSnap, head.transform.position);
                    if (Vector3.Distance(headPositionBeforeSnap, head.transform.position) != 0) {
                        // Debug.Log("**** Turning Head [snap distance:" + snapDistance + "]");
                    }

                    var incomingDirection = direction;

                    // Update direction
                    if (turnCommand == TurnCommand.Right)
                    {
                        // Turn Right
                        direction = Quaternion.Euler(0, 90, 0) * direction;
                    } else 
                    {
                        // Turn Left;
                        direction = Quaternion.Euler(0, -90, 0) * direction;
                    }
                    // Update head rotation
                    headRotation = Quaternion.LookRotation(direction);

                    // Prevent turning again for a short time
                    startTime = Time.time;

                    // Track turning point
                    var turningPointUID = main.AddTurningPoint(head.transform.position, startTime, incomingDirection, direction);

                    // Ensure any tails without a turningPointUID are given the latest turningPointUID
                    for (int i = 1; i < bodiesSize; i++) {
                        Tail tail = bodies[i].GetComponent<Tail>();
                        if (tail.GetTurningPointUID() == null) {
                            tail.SetTurningPointUID(turningPointUID);
                        }
                    }

                    // Turn Head
                    headRigidbody.velocity = direction * speed;
                    head.transform.rotation = headRotation;

                    // Debug.Log("******************* Turn Head [head:" + head.transform.position + "][tail:" + bodies[1].transform.position + "][distance: " + Vector3.Distance(head.transform.position, bodies[1].transform.position));

                    // Reset turn command
                    turnCommand = TurnCommand.None;
                }
            }
        } else {
            // Prevent turning again for a short time
            float timePassed = (Time.time - startTime);
            if (timePassed > 0.1) {
                turning = false;
            }
        }

        // Move Head
        // headRigidbody.velocity = direction * speed;
        // head.transform.rotation = headRotation;

        // Debug.Log("[head:" + head.transform.position + "][tail:" + bodies[1].transform.position + "]");

        // Grow when 'G' pressed
        if (Input.GetKeyDown(KeyCode.G)) {
            Grow();
        }

    }

    public void Grow() {
        // Mark the existing tail tip as no longer the tip
        GameObject previousTipOfTail = bodies[bodiesSize-1];
        previousTipOfTail.GetComponent<Tail>().ClearTip();

        // Add new tail part as tip of the tail
        GameObject newPart = GameObject.Instantiate(tail);
        newPart.name = "Tail " + bodiesSize;
        newPart.transform.parent = transform;    
        // TODO Currently this could potentially position the new part outside the arena
        var tailDirection = previousTipOfTail.GetComponent<Rigidbody>().velocity / speed;
        newPart.transform.position = previousTipOfTail.transform.position - (tailDirection * minDistance);
        newPart.GetComponent<Rigidbody>().velocity = tailDirection * speed;
        newPart.GetComponent<Tail>().Init(main.GetTurningPoints(), speed);

        bodies[bodiesSize] = newPart;
        bodiesSize++;
    }

    public void beforeAtWaypoint()
    {
        isAtWaypoint = true;
    }

    public void afterAtWaypoint()
    {
        isAtWaypoint = false;
    }
}
