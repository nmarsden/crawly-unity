using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail : MonoBehaviour
{
    TurningPoints turningPoints;
    float speed;
    bool isTip;
    bool isTurn;

    public string turningPointUID;

    public void Init(TurningPoints turningPoints, float speed) {
        this.turningPoints = turningPoints;
        this.speed = speed;
        turningPointUID = this.turningPoints.GetFirstTurningPointUID();
        isTip = true;
        isTurn = false;
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (isTurn) {
            isTurn = false;
            // Debug.Log("Turn [turningPointUID: "  + turningPointUID + "]");

            // Change velocity according to the turning point's direction
            var direction = turningPoints.GetDirection(turningPointUID);
            gameObject.GetComponent<Rigidbody>().velocity = speed * direction;

            // Snap position to turning point
            var position = turningPoints.GetPosition(turningPointUID);
            // Debug.Log("[tail (pre-snap):" + gameObject.transform.position + "][tail (post-snap):" + position + "]");
            gameObject.transform.position = position;

            // Update the turningPointUID to be the next one
            var nextTurningPointUID = turningPoints.GetNextTurningPointUID(turningPointUID);

            // Debug.Log(gameObject.name + ": Turn [turningPointUID: "  + turningPointUID + "][nextTurningPointUID: "  + nextTurningPointUID + "]");

            if (isTip) {
                turningPoints.RemoveTurningPoint(turningPointUID);
            }
            turningPointUID = nextTurningPointUID;
        }
        
    }

    public void SetTurningPointUID(string turningPointUID) {
        // Debug.Log(gameObject.name + ": SetTurningPointUID [turningPointUID:" + turningPointUID + "]");

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
