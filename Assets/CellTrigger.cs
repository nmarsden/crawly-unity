using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellTrigger : MonoBehaviour
{
    Color triggeredColor = new Color32(200, 0, 0, 100);
    Color nonTriggeredColor = new Color32(100, 0, 0, 50);

    bool isTriggered = false;
    Material material;
    float triggerTime;
    float stayDuration = 0.3f;

    void Start()
    {
        material = gameObject.GetComponent<Renderer>().material;
        triggerTime = 0;
    }

    void OnTriggerStay(Collider collider) {

        if (collider.gameObject.name == "Head" || collider.gameObject.name.StartsWith("Tail")) {
            isTriggered = true;
            triggerTime = Time.time;
        }
    }

    void Update()
    {
        if (isTriggered) {
            if (Time.time - triggerTime > stayDuration) {
                isTriggered = false;
            }
        }
        material.color = isTriggered ? triggeredColor : nonTriggeredColor;
    }

    public bool IsTriggered() {
        return isTriggered;
    }

    public Vector3 GetPosition() {
        return gameObject.transform.position;
    }
}
