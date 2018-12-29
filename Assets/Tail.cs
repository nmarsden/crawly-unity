using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail : MonoBehaviour
{
    TurningPoints turningPoints;

    public string turningPointUID;

    public void Init(TurningPoints turningPoints) {
        this.turningPoints = turningPoints;
        turningPointUID = this.turningPoints.GetFirstTurningPointUID();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetTurningPointUID(string turningPointUID) {
        this.turningPointUID = turningPointUID;
    }

    public string GetTurningPointUID() {
        return turningPointUID;
    }

    public void Turn() {
        // Change velocity according to the turning point's direction
        var direction = turningPoints.GetDirection(turningPointUID);
        // TODO pass in speed instead of hardcoded 5
        gameObject.GetComponent<Rigidbody>().velocity = 5 * direction;

        // Update the turningPointUID to be the next one
        turningPointUID = turningPoints.GetNextTurningPointUID(turningPointUID);
    }
}
