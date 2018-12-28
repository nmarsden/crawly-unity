﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    enum TurnCommand {Right, Left, None};

    TurnCommand turnCommand;

    GameObject head;
    GameObject tail;
    bool turning = false;
    float turningStartTime;
    Vector3 direction;
    float rotation = 0;

    private GameObject[] bodies;
    private int bodiesSize;

    private float minDistance = 30; //20; //15;
    private float speed = 30F;
    private float gridSpacing;
    private float arenaWidth;
    private float yPos = 0;

    private Vector3 startPos;
    private float startTime;
    private float journeyLength;
    private float journeyTime;
    private bool isKeyPressed;

    // TODO here for DEBUG
    private Vector3 relativePos;

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

    bool isAtWaypoint = false;

    Rigidbody headRigidbody;

    Quaternion headRotation;

    public void Init(Main main) {
        this.main = main;
        arenaWidth = main.GetArenaWidth();
        gridSpacing = main.GetGridSpacing();
        playerHeight = main.GetPlayerHeight();
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
        head.transform.localScale = new Vector3(gridSpacing, playerHeight, gridSpacing);        
        head.transform.position = new Vector3(0, yPos, 0);
        head.GetComponent<Renderer>().material.color = bodyColor;
        head.AddComponent<Rigidbody>();

        headRigidbody = head.GetComponent<Rigidbody>();
        headRigidbody.isKinematic = false;
        headRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

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

        // Left Pupil
        GameObject leftPupil = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leftPupil.name = "Left Pupil";
        leftPupil.transform.localScale = new Vector3(1, 2, 1);
        leftPupil.GetComponent<Renderer>().material.color = pupilColor;
        leftPupil.transform.parent = head.transform;        
        leftPupil.transform.position = head.transform.position;
        leftPupil.transform.Translate(1, 1.5f, 2.25f);

        // Right Eye
        GameObject rightEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightEye.name = "Right Eye";
        rightEye.transform.localScale = new Vector3(2, 3, 2);
        rightEye.GetComponent<Renderer>().material.color = eyeColor;
        rightEye.transform.parent = head.transform;        
        rightEye.transform.position = head.transform.position;
        rightEye.transform.Translate(-1, 1, 1.5f);

        // Right Pupil
        GameObject rightPupil = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightPupil.name = "Right Pupil";
        rightPupil.transform.localScale = new Vector3(1, 2, 1);
        rightPupil.GetComponent<Renderer>().material.color = pupilColor;
        rightPupil.transform.parent = head.transform;        
        rightPupil.transform.position = head.transform.position;
        rightPupil.transform.Translate(-1, 1.5f, 2.25f);

        // Nose
        GameObject nose = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        nose.name = "Nose";
        nose.transform.localScale = new Vector3(3, 2, 2);
        nose.GetComponent<Renderer>().material.color = noseColor;
        nose.transform.parent = head.transform;        
        nose.transform.position = head.transform.position;
        nose.transform.Translate(0, 0, 2.5f);

        // Mouth
        GameObject mouth = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        mouth.name = "Mouth";
        mouth.transform.localScale = new Vector3(3, 0.5f, 2);
        mouth.GetComponent<Renderer>().material.color = mouthColor;
        mouth.transform.parent = head.transform;        
        mouth.transform.position = head.transform.position;
        mouth.transform.Translate(0, -1.5f, 1f);
        mouth.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);

        // Tail
        tail = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tail.name = "Tail";
        tail.transform.parent = transform;    
        tail.transform.localScale = new Vector3(gridSpacing, gridSpacing, gridSpacing);        
        tail.transform.position = new Vector3(0, yPos, 0);
        tail.GetComponent<Renderer>().material.color = bodyColor;
        tail.GetComponent<Collider>().isTrigger = true;
        // TODO remove when finished debugging
        // tail.SetActive(false);

        // Bodies
        bodies = new GameObject[100];
		bodies[0] = head;
		bodies[1] = tail;
		bodiesSize = 2;

        // UpdateCurrentWaypoint(currentWaypoint);
    }

    void Update() {
        if (!turning && turnCommand == TurnCommand.None) {
            float horizontal = Input.GetAxisRaw("Horizontal");
            if (horizontal != 0) {
                turnCommand = (horizontal == 1) ? TurnCommand.Right : TurnCommand.Left;
            }
        }

        if (!turning) {

            if (turnCommand != TurnCommand.None) {
                float diff;
                // Determine whether head is allowed to turn at its current position
                if (direction == Vector3.forward || direction == Vector3.back) {
                    diff = Mathf.Abs(head.transform.position.z % gridSpacing);
                } else {
                    diff = Mathf.Abs(head.transform.position.x % gridSpacing);
                }
                if (diff < 0.3) {
                    // Snap head position to grid
                    if (direction == Vector3.forward || direction == Vector3.back) {
                        var zPos = (head.transform.position.z > 0) ? 
                                        Mathf.FloorToInt(head.transform.position.z / gridSpacing) * gridSpacing : 
                                        Mathf.CeilToInt(head.transform.position.z / gridSpacing) * gridSpacing;
                        head.transform.position = new Vector3(head.transform.position.x, head.transform.position.y, zPos);
                    } else {
                        var xPos = (head.transform.position.x > 0) ? 
                                        Mathf.FloorToInt(head.transform.position.x / gridSpacing) * gridSpacing : 
                                        Mathf.CeilToInt(head.transform.position.x / gridSpacing) * gridSpacing;
                        head.transform.position = new Vector3(xPos, head.transform.position.y, head.transform.position.z);
                    }

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
                    turning = true;

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
        headRigidbody.velocity = direction * speed;
        head.transform.rotation = headRotation;

        // Move Tail
        for (int i = 1; i < bodiesSize; i++) {
            float distance = Vector3.Distance(bodies[i - 1].transform.position, bodies[i].transform.position);
            float T = Time.deltaTime * distance / minDistance * speed;
            Vector3 newPos = bodies[i-1].transform.position;
            if (T > 1) {
                T = 1;
            }
            bodies[i].transform.position = Vector3.Lerp(bodies[i].transform.position, newPos, T);
		}

        Debug.Log("[head:" + head.transform.position + "]");

        // Debug.Log("[head:" + head.transform.position + "][currentWaypoint:" + currentWaypoint + "]");

        // Grow when 'G' pressed
        if (Input.GetKeyDown(KeyCode.G)) {
            Grow();
        }

    }

    // void Update()
    // {
    //     if (isAtWaypoint) {
    //         return;
    //     }

    //     if (!turning) {

    //         float horizontal = Input.GetAxisRaw("Horizontal");
    //         if (horizontal != 0) {
    //             turning = true;
    //             turningStartTime = Time.fixedTime;

    //             currentWaypoint = main.AddTurningWaypoint(head.transform.position, direction, horizontal == 1);

    //             UpdateCurrentWaypoint(currentWaypoint);
    //         }
    //     }

    //     // Move Head
    //     head.transform.rotation = Quaternion.Euler(0, 0, 0);

    //     float timePassed = (Time.time - startTime);
    //     float fracTime = (timePassed / journeyTime);
    //     head.transform.position = Vector3.Lerp(startPos, currentWaypoint, fracTime);

    //     head.transform.Rotate(Vector2.up, rotation);

    //     // Move Tail
    //     for (int i = 1; i < bodiesSize; i++) {
    //         float distance = Vector3.Distance(bodies[i - 1].transform.position, bodies[i].transform.position);
    //         float T = Time.deltaTime * distance / minDistance * speed;
    //         Vector3 newPos = bodies[i-1].transform.position;
    //         if (T > 1) {
    //             T = 1;
    //         }
    //         bodies[i].transform.position = Vector3.Lerp(bodies[i].transform.position, newPos, T);
	// 	}

    //     // Debug.Log("[head:" + head.transform.position + "][currentWaypoint:" + currentWaypoint + "]");

    //     // Grow when 'G' pressed
    //     if (Input.GetKeyDown(KeyCode.G)) {
    //         Grow();
    //     }
    // }

    // public void PerformTurn(Vector3 currentWaypoint) {
    //     UpdateCurrentWaypoint(currentWaypoint);
    //     turning = false;
    // }

    // void UpdateCurrentWaypoint(Vector3 currentWaypoint) {
    //     this.currentWaypoint = currentWaypoint;

    //     Debug.Log("UpdateCurrentWaypoint [head:" + head.transform.position + "][currentWaypoint:" + currentWaypoint + "]");

    //     // TODO setting the head position to the currentWaypoint is no longer correct
    //     //head.transform.position = currentWaypoint;

    //     // Go to next way point
    //     var heading = currentWaypoint - head.transform.position;
    //     direction = heading / heading.magnitude;
    //     rotation = -Vector3.SignedAngle(heading, Vector3.forward, Vector3.up);

    //     startPos = head.transform.position;
    //     startTime = Time.time;
    //     journeyLength = Vector3.Distance(head.transform.position, currentWaypoint);
    //     journeyTime = (journeyLength / speed);
    // }

    public void Grow() {
        GameObject newPart = GameObject.Instantiate(tail);
        newPart.transform.parent = transform;    
        newPart.transform.position = bodies[bodiesSize-1].transform.position;

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