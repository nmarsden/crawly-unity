using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellTrigger : MonoBehaviour
{
    Color triggeredColor = new Color32(200, 0, 0, 100);
    Color nonTriggeredColor = new Color32(100, 0, 0, 50);

    bool isTriggered = false;
    Material triggerMaterial;
    float triggerTime;
    float stayDuration = 0.3f;
    Cell cell;

    void Start()
    {
        cell = gameObject.transform.parent.GetComponent<Cell>();
        triggerMaterial = gameObject.GetComponent<Renderer>().material;
        triggerTime = 0;
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.name == "Head" && !cell.IsWall()) 
        {
            cell.TriggerEnter();
        }
    }

    void OnTriggerStay(Collider collider) {

        if (collider.gameObject.name == "Head" || collider.gameObject.name.StartsWith("Tail")) {

            cell.Activate();

            isTriggered = true;
            triggerTime = Time.time;
        }
    }

    void Update()
    {
        if (isTriggered) {
            // Stay triggered for the 'stayDuration'
            if (Time.time - triggerTime > stayDuration) {
                isTriggered = false;
            }
        }
        triggerMaterial.color = isTriggered ? triggeredColor : nonTriggeredColor;
    }

    public bool IsTriggered() {
        return isTriggered;
    }

    public Vector3 GetPosition() {
        return gameObject.transform.position;
    }
}
