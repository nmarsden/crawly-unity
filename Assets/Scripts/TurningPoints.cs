using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurningPoints : MonoBehaviour
{
    public class TurningPoint {
        Vector3 position;
        GameObject trigger;
        Vector3 outgoingDirection;
        string turningPointUID;

        public TurningPoint(Vector3 position, GameObject trigger, Vector3 outgoingDirection, string turningPointUID) {
            this.position = position;
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

        public void Cleanup() {
            Object.Destroy(trigger);
        }
    }

    SortedDictionary<int, TurningPoint> turningPoints = new SortedDictionary<int, TurningPoint>();
    float gridSpacing;
    float playerWidth;
    int totalTurningPointsAdded;
    bool isShowTurningPoints;

    public void Init(Main main) {
        gridSpacing = main.GetGridSpacing();
        playerWidth = main.GetPlayerWidth();
        isShowTurningPoints = main.IsShowTurningPoints();
        totalTurningPointsAdded = 0;
    }

    void Start()
    {
        turningPoints.Clear();
    }

    void Update()
    {
        
    }

    public string AddTurningPoint(Vector3 position, Vector3 incomingDirection, Vector3 outgoingDirection) {
        var turningPointUID = totalTurningPointsAdded.ToString();
        totalTurningPointsAdded++;

        GameObject trigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
        trigger.name = "Turning Point " + turningPointUID;
        trigger.transform.parent = transform;    
        trigger.transform.localScale = new Vector3(1, 1, 1);        

        // TODO ensure the trigger is positioned taking into account velocity of tail, so it is triggered exactly when the turn is required
        var halfGridSpacing = gridSpacing / 2;
        var halfPlayerWidth = playerWidth / 2;
        var halfTriggerWidth = 0.5f;
        var halfDistancePerTick = 0.3f; // <-- This magic number seems to work!!!
        trigger.transform.position = position + (incomingDirection * (halfPlayerWidth + halfTriggerWidth - halfDistancePerTick));

        if (isShowTurningPoints) {
            trigger.GetComponent<Renderer>().material.color = new Color32(0,0,200, 255);
        } else {
            Object.Destroy(trigger.GetComponent<Renderer>());
        }
        
        trigger.GetComponent<Collider>().isTrigger = true;
        trigger.AddComponent<TurningPointTrigger>();
        trigger.GetComponent<TurningPointTrigger>().Init(turningPointUID);

        turningPoints.Add(int.Parse(turningPointUID), new TurningPoint(position, trigger, outgoingDirection, turningPointUID));

        return turningPointUID;
    }

    public Vector3[] GetPositions() {
        var positions = new Vector3[turningPoints.Count];
        var i = 0;
        foreach (var turningPoint in turningPoints) {
            positions[i++] = turningPoint.Value.GetPosition();
        }
        return positions;
    }

    public Vector3 GetDirection(string turningPointUID) {
        return turningPoints[int.Parse(turningPointUID)].GetOutgoingDirection();
    }

    public Vector3 GetPosition(string turningPointUID) {
        return turningPoints[int.Parse(turningPointUID)].GetPosition();
    }

    public string GetFirstTurningPointUID() {
        var enumerator = turningPoints.GetEnumerator();
        return enumerator.MoveNext() ? enumerator.Current.Value.GetTurningPointUID() : null;
    }

    public string GetNextTurningPointUID(string turningPointUID) {
        var enumerator = turningPoints.GetEnumerator();
        while (enumerator.MoveNext()) {
            if (enumerator.Current.Value.GetTurningPointUID() == turningPointUID) {
                return enumerator.MoveNext() ? enumerator.Current.Value.GetTurningPointUID() : null;
            }
        }
        return null;
    }

    public void RemoveTurningPoint(string turningPointUID) {
        turningPoints[int.Parse(turningPointUID)].Cleanup();
        turningPoints.Remove(int.Parse(turningPointUID));
    }

}
