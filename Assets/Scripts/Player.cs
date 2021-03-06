﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Auto turn commands which work for levels 1-4 only
    string[] autoTurnCommands = 
        new string[] {
            "L LLL",
            "LR   ",
            "LR   ",
            "LRRR ",
            " L  L",
        };

    TurnCommand autoTurnCommand = TurnCommand.None;

    public enum TurnCommand {Right, Left, None};

    public enum HittableStatus {HITTABLE, UNHITTABLE};

    TurnCommand turnCommand;

    TurningPoints turningPoints;

    GameObject head;
    GameObject lastTail;
    bool turning = false;

    private int tailLength;

    private float tailMinDistance;
    private float speed;
    private float gridSpacing;
    private float yPos = 0;

    private float turningStartTime;

    Color bodyColor = new Color32(0, 200, 0, 255);
    Color noseColor = new Color32(0, 100, 0, 255);
    Color eyeColor = new Color32(255, 255, 255, 255);
    Color pupilColor = new Color32(0, 0, 0, 255);
    Color mouthColor = new Color32(0, 0, 0, 255);

    Color shieldBodyColor = new Color32(0, 35, 102, 255);

    Main main;
    private float playerHeight;
    private float playerWidth;

    Rigidbody headRigidbody;

    bool isPosingForTitleScreen;
    bool isKilled;
    
    bool isShrinking;
    float shrinkStartTime;
    float shrinkDuration = 1;

    bool isShielded;
    float shieldStartTime;
    float shieldDuration = 5;
    Vector3 startingDirection = Vector3.left;
    bool isAutoPilotMode = false;

    public void Init(Main main) {
        this.main = main;
        gridSpacing = main.GetGridSpacing();
        playerHeight = main.GetPlayerHeight();
        playerWidth = main.GetPlayerWidth();
        speed = main.GetPlayerSpeed();
        tailMinDistance = main.GetTailMinDistance();
        isPosingForTitleScreen = false;
    }

    public void InitForTitleScreen(Main main) {
        Init(main);

        isPosingForTitleScreen = true;
    }

    void Start()
    {
        // -- Turning Points --
        turningPoints = InitTurningPoints();

        // Direction
        var initialHeadDirection = startingDirection;

        // Turn command
        turnCommand = TurnCommand.None;

        var lowPolySphereMesh = gameObject.AddComponent<LowPolySphere>().GetLowPolySphereMesh();

        // Head
        head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(head.GetComponent<MeshFilter>().mesh);
        head.GetComponent<MeshFilter>().mesh = lowPolySphereMesh;
        head.name = "Head";
        head.transform.parent = transform;    
        head.transform.localScale = new Vector3(playerWidth, playerHeight, playerWidth);        
        head.transform.position = new Vector3(0, yPos, 0);
        head.GetComponent<Renderer>().material.color = bodyColor;
        head.GetComponent<Renderer>().material.SetInt("_SmoothnessTextureChannel", 1);
        head.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0.93f);
        head.AddComponent<Rigidbody>();

        headRigidbody = head.GetComponent<Rigidbody>();
        headRigidbody.isKinematic = false;
        headRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        if (!isPosingForTitleScreen) {
            headRigidbody.velocity = initialHeadDirection * speed;
        }

        // Left Eye
        GameObject leftEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(leftEye.GetComponent<MeshFilter>().mesh);
        leftEye.GetComponent<MeshFilter>().mesh = lowPolySphereMesh;
        leftEye.name = "Left Eye";
        leftEye.transform.localScale = new Vector3(2, 3, 2);
        leftEye.GetComponent<Renderer>().material.color = eyeColor;
        leftEye.transform.parent = head.transform;        
        leftEye.transform.position = head.transform.position;
        leftEye.transform.Translate(1, 1, 1.5f);
        leftEye.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid altering the head's center-of-mass

        // Left Pupil
        GameObject leftPupil = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(leftPupil.GetComponent<MeshFilter>().mesh);
        leftPupil.GetComponent<MeshFilter>().mesh = lowPolySphereMesh;
        leftPupil.name = "Left Pupil";
        leftPupil.transform.localScale = new Vector3(1, 2, 1);
        leftPupil.GetComponent<Renderer>().material.color = pupilColor;
        leftPupil.transform.parent = head.transform;        
        leftPupil.transform.position = head.transform.position;
        leftPupil.transform.Translate(1, 1.5f, 2.25f);
        leftPupil.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid altering the head's center-of-mass

        // Right Eye
        GameObject rightEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(rightEye.GetComponent<MeshFilter>().mesh);
        rightEye.GetComponent<MeshFilter>().mesh = lowPolySphereMesh;
        rightEye.name = "Right Eye";
        rightEye.transform.localScale = new Vector3(2, 3, 2);
        rightEye.GetComponent<Renderer>().material.color = eyeColor;
        rightEye.transform.parent = head.transform;        
        rightEye.transform.position = head.transform.position;
        rightEye.transform.Translate(-1, 1, 1.5f);
        rightEye.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid altering the head's center-of-mass

        // Right Pupil
        GameObject rightPupil = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(rightPupil.GetComponent<MeshFilter>().mesh);
        rightPupil.GetComponent<MeshFilter>().mesh = lowPolySphereMesh;
        rightPupil.name = "Right Pupil";
        rightPupil.transform.localScale = new Vector3(1, 2, 1);
        rightPupil.GetComponent<Renderer>().material.color = pupilColor;
        rightPupil.transform.parent = head.transform;        
        rightPupil.transform.position = head.transform.position;
        rightPupil.transform.Translate(-1, 1.5f, 2.25f);
        rightPupil.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid altering the head's center-of-mass

        // Nose
        GameObject nose = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(nose.GetComponent<MeshFilter>().mesh);
        nose.GetComponent<MeshFilter>().mesh = lowPolySphereMesh;
        nose.name = "Nose";
        nose.transform.localScale = new Vector3(3, 2, 2);
        nose.GetComponent<Renderer>().material.color = noseColor;
        nose.transform.parent = head.transform;        
        nose.transform.position = head.transform.position;
        nose.transform.Translate(0, 0, 2.5f);
        nose.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid altering the head's center-of-mass

        // Mouth
        GameObject mouth = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(mouth.GetComponent<MeshFilter>().mesh);
        mouth.GetComponent<MeshFilter>().mesh = lowPolySphereMesh;
        mouth.name = "Mouth";
        mouth.transform.localScale = new Vector3(3, 0.5f, 2);
        mouth.GetComponent<Renderer>().material.color = mouthColor;
        mouth.transform.parent = head.transform;        
        mouth.transform.position = head.transform.position;
        mouth.transform.Translate(0, -1.5f, 1f);
        mouth.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
        mouth.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid altering the head's center-of-mass

        // Rotate Head (Note: Doing this AFTER adding children)
        head.transform.rotation = Quaternion.LookRotation(initialHeadDirection);

        // Trigger head created event
        if (!isPosingForTitleScreen) {
            main.HandleHeadCreated(head);
        }

        // Tail
        GameObject tail = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(tail.GetComponent<MeshFilter>().mesh);
        tail.GetComponent<MeshFilter>().mesh = lowPolySphereMesh;
        tail.transform.parent = transform;    
        tail.transform.localScale = new Vector3(playerWidth, playerHeight, playerWidth);        
        tail.GetComponent<Renderer>().material.color = bodyColor;
        tail.GetComponent<Renderer>().material.SetInt("_SmoothnessTextureChannel", 1);
        tail.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0.93f);
        tail.GetComponent<Collider>().isTrigger = true; // Making a trigger to avoid bumping the head while moving
        tail.AddComponent<Rigidbody>();
        tail.AddComponent<Tail>();
        var tailRigidbody = tail.GetComponent<Rigidbody>();
        tailRigidbody.isKinematic = false;
        tailRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        tail.GetComponent<Tail>().Init(main, turningPoints, tailLength++, speed, head, tailMinDistance);

        lastTail = tail;


        if (isPosingForTitleScreen) {
            // -- Setup Pose For Title Screen --
            Grow();            
            Grow();            
            Grow();            

            // Position Head
            head.transform.position = new Vector3(-6, -0.6f, -6);
            head.transform.rotation = Quaternion.Euler(-6, 125, 0);

            var direction = head.transform.rotation * Vector3.forward;

            // Position Tail
            var t = lastTail.GetComponent<Tail>();
            var i = 1;
            while(t != null) {
                t.transform.position = head.transform.position + (i * -5 * direction);

                t = t.GetLeader().GetComponent<Tail>();
                i++;
            }

        }

    }

    private TurningPoints InitTurningPoints() {
        var turningPoints = new GameObject();
        turningPoints.name = "Turning Points";

        turningPoints.transform.parent = this.transform;

        turningPoints.AddComponent<TurningPoints>();
        turningPoints.GetComponent<TurningPoints>().Init(this.main);
        return turningPoints.GetComponent<TurningPoints>();
    }
    void Update() {
        if (!turning && turnCommand == TurnCommand.None) {

            if (isAutoPilotMode) {
                // Set turn command from calculated auto turn command
                turnCommand = autoTurnCommand;
            } else {
                float horizontal = Input.GetAxisRaw("Horizontal");

                if (horizontal != 0) {
                    turnCommand = (horizontal == 1) ? TurnCommand.Right : TurnCommand.Left;
                }
            }
        }
    }
    
    void FixedUpdate() {

        if (isPosingForTitleScreen || isKilled) {
            return;
        }

        if (isShrinking) {
            if (Time.time - shrinkStartTime > shrinkDuration) {
                isShrinking = false;
                RemoveLastTailPart();
            }
        }

        if (isShielded) {
            // Update shield status bar
            var shieldTimeRemainingPercentage = 1 - ((Time.time - shieldStartTime) / shieldDuration);
            main.UpdateShieldStatusBar(shieldTimeRemainingPercentage);

            if (Time.time - shieldStartTime > shieldDuration) {
                isShielded = false;
                // Clear shield status bar
                main.UpdateShieldStatusBar(0);
                // Clear body metallic
                SetBodyMetallic(0);
            }
        }

        Vector3 direction = Vector3.Normalize(headRigidbody.velocity);

        if (!turning) {

            if (turnCommand != TurnCommand.None) {
                float diff;
                Vector3 gridPos1;
                Vector3 gridPos2;

                // TODO Extract method getSnappedGridPositionWithinThreshold(Vector3 position, Vector3 direction, float threshold)


                // Determine whether head is allowed to turn at its current position
                if (direction == Vector3.forward || direction == Vector3.back) {

                    var zPos1 = Mathf.Floor(head.transform.position.z / gridSpacing) * gridSpacing;
                    var zPos2 = zPos1 + gridSpacing;
                    gridPos1 = new Vector3(head.transform.position.x, head.transform.position.y, zPos1);
                    gridPos2 = new Vector3(head.transform.position.x, head.transform.position.y, zPos2);

                } else {

                    var xPos1 = Mathf.Floor(head.transform.position.x / gridSpacing) * gridSpacing;
                    var xPos2 = xPos1 + gridSpacing;
                    gridPos1 = new Vector3(xPos1, head.transform.position.y, head.transform.position.z);
                    gridPos2 = new Vector3(xPos2, head.transform.position.y, head.transform.position.z);
                }
                var distanceToGridPos1 = Vector3.Distance(head.transform.position, gridPos1);
                var distanceToGridPos2 = Vector3.Distance(head.transform.position, gridPos2);
                diff = Mathf.Min(distanceToGridPos1, distanceToGridPos2);

                var incomingDirection = direction;

                // Rotate head towards the intended turn direction
                if (turnCommand == TurnCommand.Right)
                {
                    // Turn Right
                    direction = Quaternion.Euler(0, 90, 0) * direction;
                } else 
                {
                    // Turn Left;
                    direction = Quaternion.Euler(0, -90, 0) * direction;
                }

                // TODO the threshold needs to take into account the velocity
                var threshold = 0.01F;
                if (diff < threshold) {
                    // -- TURN --
                    turning = true;

                    // Snap head position to grid
                    var snapGridPos = (distanceToGridPos1 < distanceToGridPos2) ? gridPos1 : gridPos2;
                    head.transform.position = snapGridPos;

                    // Prevent turning again for a short time
                    turningStartTime = Time.time;

                    // Track new turning point
                    turningPoints.AssignNewlyCreatedTurningPoint(lastTail.GetComponent<Tail>(), head.transform.position, incomingDirection, direction);

                    // Update Head Velocity
                    headRigidbody.velocity = direction * speed;

                    // Reset turn command
                    turnCommand = TurnCommand.None;
                }
            }
        } else {
            // Prevent turning again for a short time
            float timePassed = (Time.time - turningStartTime);
            if (timePassed > 0.2) {
                turning = false;
            }
        }

        if (head != null) {
            // Update Head Rotation
            head.transform.rotation = Quaternion.Lerp(head.transform.rotation, Quaternion.LookRotation(direction), 0.07f);
        }

        // Grow when 'G' pressed
        if (Input.GetKeyDown(KeyCode.G)) {
            Grow();
        }

    }

    public void ToggleAutopilotMode() {
        isAutoPilotMode = !isAutoPilotMode;
    }

    public void Grow() {
        if (isShrinking) {
            // Cannot grow when shrinking
            return;
        }

        // Add new tail part as tip of the tail
        GameObject newPart = GameObject.Instantiate(lastTail);
        newPart.transform.parent = transform;
        newPart.GetComponent<Tail>().Init(main, turningPoints, tailLength++, speed, lastTail, tailMinDistance);
        lastTail = newPart;
    }

    private bool IsMinimumLength() {
        return lastTail.GetComponent<Tail>().GetLeader().name == "Head";
    }
    
    public bool IsNotShrinkable() {
        return isShielded || IsMinimumLength();
    }

    public void Shrink() {
        if (IsNotShrinkable()) {
            return;
        }
        isShrinking = true;
        shrinkStartTime = Time.time;
        lastTail.GetComponent<Tail>().Shrink();
    }

    public void Shield() {
        isShielded = true;
        shieldStartTime = Time.time;
        // Make body metallic to represent being shielded
        SetBodyMetallic(0.5f);
   }

    void RemoveLastTailPart() {
        // Remember old last tail part
        var oldLastTail = lastTail;

        // Second last tail part now becomes the last tail part
        lastTail = lastTail.GetComponent<Tail>().GetLeader();

        // Remove old last tail part
        turningPoints.UnassignTurningPoint(oldLastTail.GetComponent<Tail>());
        GameObject.Destroy(oldLastTail);

        // Decrease tail length
        tailLength--;
    }

    public void Kill() {
        // Switch to Killed mode
        isKilled = true;

        // Allow rotation
        headRigidbody.constraints = RigidbodyConstraints.FreezePositionY;

        // Switch tail pieces to Killed Mode
        var tail = lastTail.GetComponent<Tail>();
        while(tail != null) {
            tail.Kill();
            tail = tail.GetLeader().GetComponent<Tail>();
        }
    }

    public void HeadEnteredCell(int row, int col) {
        if (isAutoPilotMode) {

            // TODO Implement a 'Smarter' way to calculate the autoTurnCommand
            // Perhaps use AStar Algorithm or the longest path algorithm (also refer to hamiltonian circuit solver)
            // - avoid wall
            // - avoid self
            // - avoid red cube
            // - attracted to green cube
            // - attracted to blue cube
            // - attracted to non-activated tile

            // Set the autoTurnCommand using hard coded turn commands (works for levels 1-4 only)
            var turnChar = autoTurnCommands[row][col];
            if (turnChar.Equals('L')) {
                autoTurnCommand = TurnCommand.Left;
            } else if (turnChar.Equals('R')) {
                autoTurnCommand = TurnCommand.Right;
            } else if (turnChar.Equals(' ')) {
                autoTurnCommand = TurnCommand.None;
            }
        }
    }

    private void SetBodyMetallic(float metallic) {
        head.GetComponent<Renderer>().material.SetFloat("_Metallic", metallic);

        var tail = lastTail.GetComponent<Tail>();
        while(tail != null) {
            tail.gameObject.GetComponent<Renderer>().material.SetFloat("_Metallic", metallic);

            tail = tail.GetLeader().GetComponent<Tail>();
        }
    }
}
 