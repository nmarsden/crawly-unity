using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail : MonoBehaviour
{
    Main main;
    TurningPoints turningPoints;
    float speed;
    Vector3 direction;
    bool isTip;
    bool isTurn;
    GameObject leader;
    float tailMinDistance;
    Player.HittableStatus hittableStatus;

    Rigidbody tailRigidbody;
    Rigidbody leaderRigidbody;

    public string turningPointUID;
    bool isGrowing;
    float growDuration = 2f;
    Color32 growColor = new Color32(50, 200, 200, 255);
    Color tailColor = new Color32(0, 200, 0, 255);
    float growStartTime;
    bool isKilled;


    public void Init(Main main, float tailLength, float speed, GameObject leader, float tailMinDistance) {
        this.main = main;
        this.leader = leader;
        tailRigidbody = gameObject.GetComponent<Rigidbody>();
        leaderRigidbody = leader.GetComponent<Rigidbody>();

        direction = Vector3.Normalize(leaderRigidbody.velocity);

        hittableStatus = tailLength > 4 ? Player.HittableStatus.HITTABLE : Player.HittableStatus.UNHITTABLE;
        gameObject.name = "Tail " + tailLength;
        gameObject.transform.position = leader.transform.position;
        gameObject.GetComponent<Rigidbody>().velocity = direction * speed;

        this.turningPoints = main.GetTurningPoints();
        this.speed = speed;
        turningPointUID = this.turningPoints.GetFirstTurningPointUID();
        isTip = true;
        isTurn = false;
        this.tailMinDistance = tailMinDistance;

        if (leader.name != "Head") {
            // Growing
            isGrowing = true;
            growStartTime = Time.time;
        }
    }

    void OnTriggerEnter(Collider collider) 
    {
        if (hittableStatus.Equals(Player.HittableStatus.HITTABLE) && collider.gameObject.name == "Head") {
            main.HandleHitTail();
        }
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (isKilled) {
            return;
        }

        if (isGrowing) {
            var material = gameObject.GetComponent<Renderer>().material;
            if (Time.time - growStartTime > growDuration) {
                isGrowing = false;
                material.color = tailColor;
            } else {
                material.color = Color32.Lerp(growColor, tailColor, (Time.time - growStartTime) / growDuration);
            }
        }
        if (isTurn) {
            isTurn = false;

            // Change velocity according to the turning point's direction
            direction = turningPoints.GetDirection(turningPointUID);
            tailRigidbody.velocity = speed * direction;

            // Snap position to turning point
            gameObject.transform.position = turningPoints.GetPosition(turningPointUID);

            // Update the turningPointUID to be the next one
            var nextTurningPointUID = turningPoints.GetNextTurningPointUID(turningPointUID);

            if (isTip) {
                turningPoints.RemoveTurningPoint(turningPointUID);
            }
            turningPointUID = nextTurningPointUID;
        } else {
            // Ensure minimum distance from leader when travelling the same direction
            if (Vector3.Normalize(leaderRigidbody.velocity) == Vector3.Normalize(tailRigidbody.velocity)) {
                var distanceFromLeader = Vector3.Distance(gameObject.transform.position, leader.transform.position);
                if (distanceFromLeader < tailMinDistance) {
                    tailRigidbody.velocity = direction * (speed * 0.01f);
                } else {
                    tailRigidbody.velocity = direction * speed;
                }
            } else {
                tailRigidbody.velocity = direction * speed;
            }
        }
        
    }

    public void SetTurningPointUID(string turningPointUID) {
        this.turningPointUID = turningPointUID;
    }

    public string GetTurningPointUID() {
        return turningPointUID;
    }

    public GameObject GetLeader() {
        return leader;
    }
    
    public void ClearTip() {
        isTip = false;
    }
    public void Turn() {
        isTurn = true;
    }

    public void Kill() {
        // Switch to Killed mode
        isKilled = true;

        // Allow rotation
        tailRigidbody.constraints = RigidbodyConstraints.FreezePositionY;

        // Allow bumping into stuff
        gameObject.GetComponent<Collider>().isTrigger = false;
    }

}
