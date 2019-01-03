using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject target;
    Vector3 targetOffset;
    public void SetToFollow(GameObject target) {
        this.target = target;
        targetOffset = transform.position - target.transform.position;
    }

    void LateUpdate()
    {
        if (target != null) {
            transform.position = target.transform.position + targetOffset;
        }
        
    }
}
