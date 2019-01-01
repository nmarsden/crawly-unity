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


    public void Init(Main main, Player.HittableStatus hittableStatus, float speed, Vector3 direction, GameObject leader, float tailMinDistance) {
        this.main = main;
        this.hittableStatus = hittableStatus;
        this.turningPoints = main.GetTurningPoints();
        this.speed = speed;
        this.direction = direction;
        turningPointUID = this.turningPoints.GetFirstTurningPointUID();
        isTip = true;
        isTurn = false;
        this.leader = leader;
        tailRigidbody = gameObject.GetComponent<Rigidbody>();
        leaderRigidbody = leader.GetComponent<Rigidbody>();
        this.tailMinDistance = tailMinDistance;
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
        if (isTurn) {
            isTurn = false;

            // Change velocity according to the turning point's direction
            direction = turningPoints.GetDirection(turningPointUID);
            tailRigidbody.velocity = speed * direction;

            // Snap position to turning point
            gameObject.transform.position = turningPoints.GetPosition(turningPointUID);;

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

    public void ClearTip() {
        isTip = false;
    }
    public void Turn() {
        isTurn = true;
    }
}
