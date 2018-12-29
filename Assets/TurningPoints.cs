using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurningPoints : MonoBehaviour
{
    public class TurningPoint {
        Vector3 position;
        float time;
        GameObject trigger;
        Vector3 outgoingDirection;
        string turningPointUID;

        public TurningPoint(Vector3 position, float time, GameObject trigger, Vector3 outgoingDirection, string turningPointUID) {
            this.position = position;
            this.time = time;
            this.trigger = trigger;
            this.outgoingDirection = outgoingDirection;
            this.turningPointUID = turningPointUID;
        }

        public Vector3 GetPosition() {
            return position;
        }

        public Vector3 GetOutgoingDirection() {
            return outgoingDirection;
        }

        public string GetTurningPointUID() {
            return turningPointUID;
        }
    }

    List<TurningPoint> turningPoints;

    void Start()
    {
        turningPoints = new List<TurningPoint>();
    }

    void Update()
    {
        
    }

    public string AddTurningPoint(Vector3 position, float time, Vector3 incomingDirection, Vector3 outgoingDirection) {
        var turningPointUID = "TP_" + turningPoints.Count.ToString();

        GameObject trigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
        trigger.name = "Turning Point " + turningPoints.Count;
        trigger.transform.parent = transform;    
        trigger.transform.localScale = new Vector3(1, 1, 1);        

        // TODO pass in gridSpacing instead of hard coded as 5F
        var gridSize = 5F;
        trigger.transform.position = position + (incomingDirection * ((0.5f * gridSize) + 0.5f));

        trigger.GetComponent<Renderer>().material.color = new Color32(0,0,200, 255);
        trigger.GetComponent<Collider>().isTrigger = true;
        trigger.AddComponent<TurningPointTrigger>();
        trigger.GetComponent<TurningPointTrigger>().Init(turningPointUID);

        turningPoints.Add(new TurningPoint(position, time, trigger, outgoingDirection, turningPointUID));

        return turningPointUID;
    }

    public Vector3[] GetPositions() {
        var positions = new Vector3[turningPoints.Count];
        var i = 0;
        foreach (var turningPoint in turningPoints) {
            positions[i++] = turningPoint.GetPosition();
        }
        return positions;
    }

    public Vector3 GetDirection(string turningPointUID) {
        // TODO don't rely on the UID containing the index, instead find the turningPoint with the given UID
        var index = int.Parse(turningPointUID.Split('_')[1]);
        return turningPoints[index].GetOutgoingDirection();
    }

    public string GetFirstTurningPointUID() {
        if (turningPoints.Count > 0) {
            return turningPoints[0].GetTurningPointUID();
        } else {
            return null;
        }
    }

    public string GetNextTurningPointUID(string turningPointUID) {
        // TODO don't rely on the UID containing the index, instead find the turningPoint with the given UID
        var index = int.Parse(turningPointUID.Split('_')[1]);
        if (index < turningPoints.Count-1) {
            return turningPoints[++index].GetTurningPointUID();
        } else {
            return null;
        }
    }

}
