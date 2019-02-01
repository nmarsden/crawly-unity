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
        int useCount;

        public TurningPoint(Vector3 position, GameObject trigger, Vector3 outgoingDirection, string turningPointUID) {
            this.position = position;
            this.trigger = trigger;
            this.outgoingDirection = outgoingDirection;
            this.turningPointUID = turningPointUID;
            this.useCount = 0;
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

        public void IncrementUseCount() {
            useCount++;
        }

        public void DecrementUseCount() {
            useCount--;
        }

        public bool isNotUsed() {
            return useCount == 0;
        }
    }

    IDictionary<int, TurningPoint> turningPoints = new SortedDictionary<int, TurningPoint>();
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

    public void AssignNewlyCreatedTurningPoint(Tail lastTail, Vector3 position, Vector3 incomingDirection, Vector3 outgoingDirection) {
        var turningPointUID = AddTurningPoint(position, incomingDirection, outgoingDirection);

        // Ensure any tails without a turningPointUID are given the latest turningPointUID
        var tail = lastTail.GetComponent<Tail>();
        while(tail != null) {
            if (tail.GetTurningPointUID() == null) {
                AssignTurningPoint(tail, turningPointUID);
            }
            tail = tail.GetLeader().GetComponent<Tail>();
        }
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
        return GetTurningPoint(turningPointUID).GetOutgoingDirection();
    }

    public Vector3 GetPosition(string turningPointUID) {
        return GetTurningPoint(turningPointUID).GetPosition();
    }

    public void AssignOldestTurningPoint(Tail tail) {
        AssignTurningPoint(tail, GetOldestTurningPointUID());
    }

    public void UnassignTurningPoint(Tail tail) {
        var turningPointUID = tail.GetTurningPointUID();
        if (turningPointUID != null) {
            DecrementUseCount(turningPointUID);
            tail.SetTurningPointUID(null);
        }
    }

    public void AssignNextTurningPoint(Tail tail) {
        var turningPointUID = tail.GetTurningPointUID();
        var nextTurningPointUID = GetNextTurningPointUID(turningPointUID);

        DecrementUseCount(turningPointUID);
        AssignTurningPoint(tail, nextTurningPointUID);
    }

    private void DecrementUseCount(string turningPointUID) {
        var turningPoint = GetTurningPoint(turningPointUID);
        turningPoint.DecrementUseCount();
        if (turningPoint.isNotUsed()) {
            RemoveTurningPoint(turningPointUID);
        }
    }    

    private string AddTurningPoint(Vector3 position, Vector3 incomingDirection, Vector3 outgoingDirection) {
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

    private TurningPoint GetTurningPoint(string turningPointUID) {
        return turningPoints[int.Parse(turningPointUID)];
    }

    private string GetOldestTurningPointUID() {
        var enumerator = turningPoints.GetEnumerator();
        return enumerator.MoveNext() ? enumerator.Current.Value.GetTurningPointUID() : null;
    }

    private string GetNextTurningPointUID(string turningPointUID) {
        var enumerator = turningPoints.GetEnumerator();
        while (enumerator.MoveNext()) {
            if (enumerator.Current.Value.GetTurningPointUID() == turningPointUID) {
                return enumerator.MoveNext() ? enumerator.Current.Value.GetTurningPointUID() : null;
            }
        }
        return null;
    }

    private void AssignTurningPoint(Tail tail, string turningPointUID) {
        tail.SetTurningPointUID(turningPointUID);
        if (turningPointUID != null) {
            GetTurningPoint(turningPointUID).IncrementUseCount();
        }
    }
    
    private void RemoveTurningPoint(string turningPointUID) {
        turningPoints[int.Parse(turningPointUID)].Cleanup();
        turningPoints.Remove(int.Parse(turningPointUID));
    }
    
}
