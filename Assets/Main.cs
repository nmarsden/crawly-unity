using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    GameObject head;
    GameObject tail;
    GameObject waypoint;
    GameObject waypoint2;
    bool turning = false;
    float turningStartTime;
    Vector3 direction;
    float rotation = 0;

    private GameObject[] bodies;
    private int bodiesSize;

    private float minDistance = 20; //15;
    private float speed = 10F;
    private float gridSpacing = 20F;
    private float yPos = 4F;

    private Vector3[] waypoints;
    private int waypointsSize;
    private Vector3 currentWaypoint;
    private Vector3 startPos;
    private float startTime;
    private float journeyLength;
    private float journeyTime;
    private bool isKeyPressed;

    // TODO here for DEBUG
    private Vector3 relativePos;

    // Start is called before the first frame update
    void Start()
    {
        Color floorColor = new Color(255, 255, 255);
        Color wallColor = new Color(0, 0, 200);
        Color bodyColor = new Color(0, 200, 0);
        Color antennaColor = new Color32(238,130,238, 255);
        Color hatColor = new Color32(0, 0, 200, 255);
        Color noseColor = new Color32(0, 100, 0, 255);
        Color eyeColor = new Color32(255, 255, 255, 255);
        Color pupilColor = new Color32(0, 0, 0, 255);
        Color waypointColor = new Color(200, 200, 0);
        Color mouthColor = new Color32(0, 0, 0, 255);

        // Floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.transform.localScale = new Vector3(100, 1, 100);        
        floor.transform.position = new Vector3(0, 0, 0);
        floor.GetComponent<Renderer>().material.color = floorColor;

        // Wall 
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.localScale = new Vector3(100, 5, 5);        
        wall.transform.position = new Vector3(0, 2.5f, 50);
        wall.GetComponent<Renderer>().material.color = wallColor;

        // Wall 
        GameObject wall2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall2.transform.localScale = new Vector3(100, 5, 5);        
        wall2.transform.position = new Vector3(0, 2.5f, -50);
        wall2.GetComponent<Renderer>().material.color = wallColor;

        // Wall 
        GameObject wall3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall3.transform.localScale = new Vector3(5, 5, 100);        
        wall3.transform.position = new Vector3(50, 2.5f, 0);
        wall3.GetComponent<Renderer>().material.color = wallColor;

        // Wall 
        GameObject wall4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall4.transform.localScale = new Vector3(5, 5, 100);        
        wall4.transform.position = new Vector3(-50, 2.5f, 0);
        wall4.GetComponent<Renderer>().material.color = wallColor;

        // Head
        head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.transform.localScale = new Vector3(5, 6, 5);        
        head.transform.position = new Vector3(0, yPos, 0);
        head.GetComponent<Renderer>().material.color = bodyColor;

        // Waypoint
        waypoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        waypoint.transform.localScale = new Vector3(5, 5, 5);        
        waypoint.GetComponent<Renderer>().material.color = waypointColor;

        waypoint2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        waypoint2.transform.localScale = new Vector3(5, 5, 5);        
        waypoint2.GetComponent<Renderer>().material.color = waypointColor;

        // Hat
        // GameObject hat = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // hat.transform.localScale = new Vector3(5, 0.5f, 5);        
        // hat.transform.position = head.transform.position;
        // hat.GetComponent<Renderer>().material.color = hatColor;
        // hat.transform.parent = head.transform;        
        // hat.transform.Translate(0, 3, 0);

        // Hat Top
        // GameObject hatTop = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // hatTop.transform.localScale = new Vector3(4, 4, 4);        
        // hatTop.transform.position = head.transform.position;
        // hatTop.GetComponent<Renderer>().material.color = hatColor;
        // hatTop.transform.parent = head.transform;        
        // hatTop.transform.Translate(0, 3, 0);

        // Left Antenna
        // GameObject leftAntenna = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        // leftAntenna.transform.localScale = new Vector3(0.5f, 4, 0.5f);
        // leftAntenna.GetComponent<Renderer>().material.color = antennaColor;
        // leftAntenna.transform.parent = head.transform;        
        // leftAntenna.transform.position = head.transform.position;
        // leftAntenna.transform.Translate(1, 3, 0);
        // leftAntenna.transform.rotation = Quaternion.AngleAxis(-30, Vector3.forward);

        // Left Antenna Tip
        // GameObject leftAntennaTip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // leftAntennaTip.GetComponent<Renderer>().material.color = antennaColor;
        // leftAntennaTip.transform.parent = leftAntenna.transform;        
        // leftAntennaTip.transform.localScale = new Vector3(1, 1, 1);
        // leftAntennaTip.transform.position = head.transform.position;
        // leftAntennaTip.transform.Translate(0, 4, 0);

        // Left Eye
        GameObject leftEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leftEye.transform.localScale = new Vector3(2, 3, 2);
        leftEye.GetComponent<Renderer>().material.color = eyeColor;
        leftEye.transform.parent = head.transform;        
        leftEye.transform.position = head.transform.position;
        leftEye.transform.Translate(1, 1, 1.5f);

        // Left Pupil
        GameObject leftPupil = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leftPupil.transform.localScale = new Vector3(1, 2, 1);
        leftPupil.GetComponent<Renderer>().material.color = pupilColor;
        leftPupil.transform.parent = head.transform;        
        leftPupil.transform.position = head.transform.position;
        leftPupil.transform.Translate(1, 1.5f, 2.25f);

        // Right Eye
        GameObject rightEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightEye.transform.localScale = new Vector3(2, 3, 2);
        rightEye.GetComponent<Renderer>().material.color = eyeColor;
        rightEye.transform.parent = head.transform;        
        rightEye.transform.position = head.transform.position;
        rightEye.transform.Translate(-1, 1, 1.5f);

        // Right Pupil
        GameObject rightPupil = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightPupil.transform.localScale = new Vector3(1, 2, 1);
        rightPupil.GetComponent<Renderer>().material.color = pupilColor;
        rightPupil.transform.parent = head.transform;        
        rightPupil.transform.position = head.transform.position;
        rightPupil.transform.Translate(-1, 1.5f, 2.25f);

        // Nose
        GameObject nose = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        nose.transform.localScale = new Vector3(3, 2, 2);
        nose.GetComponent<Renderer>().material.color = noseColor;
        nose.transform.parent = head.transform;        
        nose.transform.position = head.transform.position;
        nose.transform.Translate(0, 0, 2.5f);

        // Mouth
        GameObject mouth = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        mouth.transform.localScale = new Vector3(3, 0.5f, 2);
        mouth.GetComponent<Renderer>().material.color = mouthColor;
        mouth.transform.parent = head.transform;        
        mouth.transform.position = head.transform.position;
        mouth.transform.Translate(0, -1.5f, 1f);
        mouth.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);

        // GameObject mouth = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // mouth.transform.localScale = new Vector3(3, 0.5f, 2);
        // mouth.GetComponent<Renderer>().material.color = mouthColor;
        // mouth.transform.parent = head.transform;        
        // mouth.transform.position = head.transform.position;
        // mouth.transform.Translate(0, -1.5f, 1.5f);

        // Mouth Left
        // GameObject mouthLeft = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // mouth.transform.localScale = new Vector3(3, 0.5f, 2);
        // mouthLeft.GetComponent<Renderer>().material.color = mouthColor;
        // mouthLeft.transform.parent = head.transform;        
        // mouthLeft.transform.position = head.transform.position;
        // mouthLeft.transform.Translate(1, -1.5f, 1.5f);
        // mouthLeft.transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);

        // Tail
        tail = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tail.transform.localScale = new Vector3(5, 6, 5);        
        tail.transform.position = new Vector3(0, yPos, 0);
        tail.GetComponent<Renderer>().material.color = bodyColor;

        // Bodies
        bodies = new GameObject[100];
		bodies[0] = head;
		bodies[1] = tail;
		bodiesSize = 2;

        // Direction
        direction = head.transform.forward;

        // Waypoints
        waypoints = new Vector3[100];
        waypoints[0] = head.transform.position + (direction * 100);
        // waypoints[0] = new Vector3(0, yPos, 50);
        waypointsSize = 1;
        currentWaypoint = waypoints[0];
        waypoint.transform.position = currentWaypoint;

        // Start position
        startPos = head.transform.position;

        // Keep a note of the time the movement started.
        startTime = Time.time;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(head.transform.position, currentWaypoint);

        // Target time
        journeyTime = (journeyLength / speed);

        // ball.AddComponent<Rigidbody>();
        // ball.GetComponent<Rigidbody>().velocity = transform.forward;

        // objToSpawn.AddComponent<MeshFilter>();
        // objToSpawn.AddComponent<BoxCollider>();
        // objToSpawn.AddComponent<MeshRenderer>();

    }

    // Update - using waypoints to ensure the tail follows the exact same path as the head
    void UpdateOther()
    {

        // TODO consider using buffered input, so that multiple consecutive key presses can be acted upon while turning
        // Resource: https://answers.unity.com/questions/1305421/input-buffering.html

        // if (isKeyPressed && (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow)))
        // {
        //     isKeyPressed = false;
        // }            

        if (!turning) {
        // if (!turning && !isKeyPressed) {

            // var isLeftArrow = Input.GetKeyDown(KeyCode.LeftArrow);
            // var isRightArrow = Input.GetKeyDown(KeyCode.RightArrow);
            // if (isLeftArrow || isRightArrow)
            // {
            //     isKeyPressed = true;

            float horizontal = Input.GetAxisRaw("Horizontal");
            if (horizontal != 0) {
                turning = true;
                turningStartTime = Time.fixedTime;

                // Check if turning on grid line
                // bool onGridLine = (Mathf.Abs(head.transform.position.z) % gridSpacing) < 0.2;
                // if (onGridLine) {
                //     if (horizontal == 1) {
                //         // Turning Right
                //         direction = Quaternion.AngleAxis(90, Vector3.up) * direction;
                //         rotation = rotation + 90;
                //     } else {
                //         // Turning Left
                //         direction = Quaternion.AngleAxis(-90, Vector3.up) * direction;
                //         rotation = rotation - 90;
                //     }

                //     //Debug.Log("[Modulus: " + (head.transform.position.z % gridSpacing) + " On Grid Line! position:" + head.transform.position);
                //     // Turn
                //     waypoints[0] = head.transform.position + (direction * 100);
                //     currentWaypoint = waypoints[0];

                // } else {
                    // Add waypoint
                    //Debug.Log("[Modulus: " + (head.transform.position.z % gridSpacing) + "Add waypoint for turning! position:" + head.transform.position);

                    // var angleWithZAxis = Mathf.Abs(Vector3.SignedAngle(direction, Vector3.forward, Vector3.up));
                    // var isZAxis = (angleWithZAxis < 0.1) || ((angleWithZAxis - 180) < 0.1 && (angleWithZAxis - 180) > -0.1);


                    // var distanceToGridLine = 0F;
                    // if (isZAxis) {
                    //     distanceToGridLine = gridSpacing - (Mathf.Abs(head.transform.position.z) % gridSpacing);
                    // } else {
                    //     distanceToGridLine = gridSpacing - (Mathf.Abs(head.transform.position.x) % gridSpacing);
                    // }


                    // waypoints[waypointsSize] = head.transform.position + (direction * (distanceToGridLine));

                    // var remainder = 0F;
                    // if (direction.x > 0.1 || direction.x < -0.1) {
                    //     remainder = (head.transform.position.x % gridSpacing);
                    //     if (direction.x > 0.1) {
                    //         if (head.transform.position.x > 0) {
                    //             waypoints[waypointsSize] = head.transform.position + (direction * (gridSpacing - remainder));
                    //         } else {
                    //             waypoints[waypointsSize] = head.transform.position - (direction * remainder);
                    //         }
                    //     } else {
                    //         if (head.transform.position.x > 0) {
                    //             waypoints[waypointsSize] = head.transform.position + (direction * remainder);
                    //         } else {
                    //             waypoints[waypointsSize] = head.transform.position + (direction * (gridSpacing + remainder));
                    //         }
                    //     }
                    // } else {
                    //     remainder = (head.transform.position.z % gridSpacing);
                    //     if (direction.z > 0.1) {
                    //         if (head.transform.position.z > 0) {
                    //             waypoints[waypointsSize] = head.transform.position + (direction * (gridSpacing - remainder));
                    //         } else {
                    //             waypoints[waypointsSize] = head.transform.position - (direction * remainder);
                    //         }
                    //     } else {
                    //         if (head.transform.position.z > 0) {
                    //             waypoints[waypointsSize] = head.transform.position + (direction * remainder);
                    //         } else {
                    //             waypoints[waypointsSize] = head.transform.position + (direction * (gridSpacing + remainder));
                    //         }
                    //     }
                    // }

                    var remainder = 0F;
                    var directionPositive = (direction.x > 0.1 || direction.z > 0.1);
                    var positionPositive = false;
                    if (direction.x > 0.1 || direction.x < -0.1) {
                        remainder = (head.transform.position.x % gridSpacing);
                        positionPositive = (head.transform.position.x > 0);
                    } else {
                        remainder = (head.transform.position.z % gridSpacing);
                        positionPositive = (head.transform.position.z > 0);
                    }
                    if (directionPositive) {
                        if (positionPositive) {
                            waypoints[waypointsSize] = head.transform.position + (direction * (gridSpacing - remainder));
                        } else {
                            waypoints[waypointsSize] = head.transform.position - (direction * remainder);
                        }
                    } else {
                        if (positionPositive) {
                            waypoints[waypointsSize] = head.transform.position + (direction * remainder);
                        } else {
                            waypoints[waypointsSize] = head.transform.position + (direction * (gridSpacing + remainder));
                        }
                    }
                    waypointsSize++;
                    currentWaypoint = waypoints[waypointsSize-1];

                    // float distance = (gridSpacing - Mathf.Abs(remainder));
                    // var forward = head.transform.TransformDirection(Vector3.forward);
                    // relativePos = distance * forward;
                    //Vector3 scaledDirection = direction * distance;

                    // Vector3 alternative = head.transform.position + relativePos;

                    //Vector3 origScale = head.transform.localScale;
                    //head.transform.localScale = new Vector3(1, 1, 1);
                    //Vector3 alternative = head.transform.TransformPoint(relativePos);
                    //head.transform.localScale = origScale;

                    //Debug.Log("[head:" + head.transform.position + "][currentWaypoint:" + currentWaypoint + "][distance:" + (gridSpacing - Mathf.Abs(remainder)) + "]");
                    // Debug.Log("[currentWaypoint:" + currentWaypoint + "][alternative:" + alternative + "]");

                    //Debug.Log("[position: " + head.transform.position + "][direction:" + direction + "][currentWaypoint:" + currentWaypoint + "]");

                    // Update initial waypoint
                    // if (isRightArrow) {
                    if (horizontal == 1) {
                        // Turning Right
                        waypoints[0] = currentWaypoint + (Quaternion.AngleAxis(90, Vector3.up) * direction * 100);
                    } else {
                        // Turning Left
                        waypoints[0] = currentWaypoint + (Quaternion.AngleAxis(-90, Vector3.up) * direction * 100);
                    }
                // }
                waypoint.transform.position = currentWaypoint;

                startPos = head.transform.position;
                startTime = Time.time;
                journeyLength = Vector3.Distance(head.transform.position, currentWaypoint);
                journeyTime = (journeyLength / speed);
            }
        } else {
            // Turning - determine when next turn is allowed
            // if (Time.fixedTime - turningStartTime > 0.1) {
            //     turning = false;
            // }
        }

        // Check if waypoint reached
        if (turning && Vector3.Distance(head.transform.position, currentWaypoint) < 0.1) {

            head.transform.position = currentWaypoint;

            // Go to next way point
            waypointsSize--;
            currentWaypoint = waypoints[waypointsSize-1];
            waypoint.transform.position = currentWaypoint;
            var heading = currentWaypoint - head.transform.position;
            direction = heading / heading.magnitude;
            rotation = -Vector3.SignedAngle(heading, Vector3.forward, Vector3.up);

            startPos = head.transform.position;
            startTime = Time.time;
            journeyLength = Vector3.Distance(head.transform.position, currentWaypoint);
            journeyTime = (journeyLength / speed);

            turning = false;
        }

        // Debug.Log("[rotation:" + rotation + "][currentWaypoint:" + currentWaypoint + "][waypointsSize:" + waypointsSize + "]");

        // Move Head
        head.transform.rotation = Quaternion.Euler(0, 0, 0);

        float timePassed = (Time.time - startTime);
        float fracTime = (timePassed / journeyTime);
        head.transform.position = Vector3.Lerp(startPos, currentWaypoint, fracTime);

        // Debug.Log("[journeyTime:" + journeyTime + "][timePassed: " + timePassed + "][fracTime:" + fracTime + "]");

        // float distCovered = (Time.time - startTime) * speed;
        // float fracJourney = distCovered / journeyLength;
        // head.transform.position = Vector3.Lerp(head.transform.position, waypoints[0], fracJourney);


        // Vector3 rightTurnWaypoint = head.transform.position + (Quaternion.AngleAxis(90, Vector3.up) * direction * 60);
        // waypoint2.transform.position = rightTurnWaypoint;

        // Debug.Log("[startTime:" + startTime + "][journeyLength:" + journeyLength + "][distCovered:" + distCovered + "][fracJourney:" + fracJourney + "]");

        //Debug.Log("[T: " + T + "][distance: " + distance + "][speed: " + speed + "][head position:" + head.transform.position + "][waypoint position:" + waypoints[0] + "]");
        // Debug.Log("[head position:" + head.transform.position + "][waypoint position:" + waypoints[0] + "][right turn waypoint: " + rightTurnWaypoint + "]");

        // head.transform.Translate(direction * speed * Time.smoothDeltaTime);
        head.transform.Rotate(Vector2.up, rotation);

        // Move Tail
        // for (int i = 1; i < bodiesSize; i++) {
        //     float distance = Vector3.Distance(bodies[i - 1].transform.position, bodies[i].transform.position);

        //     float T = Time.deltaTime * distance / minDistance * speed;

        //     Vector3 newPos = bodies[i-1].transform.position;

        //     if (T > 1) {
        //         T = 1;
        //     }
        //     bodies[i].transform.position = Vector3.Lerp(bodies[i].transform.position, newPos, T);
		// }

        // Grow when 'G' pressed
        if (Input.GetKeyDown(KeyCode.G)) {
            Grow();
        }
    }


    // Update (OLD) - Each tail part is following the tail part ahead of it.
    // Note: The tail parts do not follow the exact same path as the head using this technique
    void Update()
    {
        if (!turning) {
            float horizontal = Input.GetAxisRaw("Horizontal");
            if (horizontal != 0) {
                turning = true;
                turningStartTime = Time.fixedTime;
                if (horizontal == 1) {
                    // Turning Right
                    //ball.transform.Rotate(Vector2.up, 90);
                    direction = Quaternion.AngleAxis(90, Vector3.up) * direction;
                    rotation = rotation + 90;

                } else {
                    // Turning Left
                    //ball.transform.Rotate(Vector2.up, -90);
                    direction = Quaternion.AngleAxis(-90, Vector3.up) * direction;
                    rotation = rotation - 90;
                }
            }
        } else {
            // Turning - determine when next turn is allowed
            if (Time.fixedTime - turningStartTime > 1) {
                turning = false;
            }
        }

        // Move Head
        head.transform.rotation = Quaternion.Euler(0, 0, 0);
        head.transform.Translate(direction * speed * Time.smoothDeltaTime);
        head.transform.Rotate(Vector2.up, rotation);

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

        // Grow when 'G' pressed
        if (Input.GetKeyDown(KeyCode.G)) {
            Grow();
        }
    }

    void Grow() {
        GameObject newPart = GameObject.Instantiate(tail);
        newPart.transform.position = bodies[bodiesSize-1].transform.position;

        bodies[bodiesSize] = newPart;
        bodiesSize++;
    }
}
