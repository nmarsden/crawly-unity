using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{

    float initialDelay = 0.4f; //9;
    float startTime;
    float flashDuration = 0.01f;
    float flashStartTime;
    bool isRendererEnabled;

    void Start() {
        flashStartTime = Time.time;
        startTime = Time.time;
        isRendererEnabled = true;
    }

    void FixedUpdate()
    {
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
