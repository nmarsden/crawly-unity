using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurningPointTrigger : MonoBehaviour
{
    string turningPointUID;

    public void Init(string turningPointUID) 
    {
        this.turningPointUID = turningPointUID;
    }
    void OnTriggerEnter(Collider collider) 
    {
        // Check if the colliding Tail's turningPointUID matches the trigger's turningPointUID
        if (collider.gameObject.name.StartsWith("Tail")) {
            Tail tail = collider.GetComponent<Tail>();

            if (tail.GetTurningPointUID() == turningPointUID) {
                tail.Turn();
            }
        }
    }    

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
