using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{

    float initialDelay;
    float startTime;
    float flashDuration = 0.01f;
    float flashStartTime;
    bool isRendererEnabled;
    bool isOn = false;

    public void TurnOn(float initialDelay = 0.4f) {
        this.initialDelay = initialDelay;
        flashStartTime = Time.time;
        startTime = Time.time;
        isRendererEnabled = true;
        isOn = true;
    }

    public void TurnOff() {
        isOn = false;
        gameObject.GetComponent<Renderer>().enabled = true;
    }

    void FixedUpdate()
    {
        if (!isOn) {
            return;
        }

        if (Time.time - startTime > initialDelay) {
            ToggleRendererEnabled();
        } else {
            flashStartTime = Time.time;
        }
    }

    private void ToggleRendererEnabled() 
    {
        if (Time.time - flashStartTime > flashDuration) {
            isRendererEnabled = !isRendererEnabled;
            gameObject.GetComponent<Renderer>().enabled = isRendererEnabled;
            flashStartTime = Time.time;
        }
    }

}
