using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    enum CameraMode { FOLLOW, FPV };

    GameObject target;
    Vector3 targetOffset;
    CameraMode cameraMode;

    public void SetToFollow(GameObject target) {
        cameraMode = CameraMode.FOLLOW;
        this.target = target;
        targetOffset = transform.position - target.transform.position;
    }

    public void SetFirstPersonView(GameObject target) {
        cameraMode = CameraMode.FPV;
        this.target = target;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 80;
    }

    void LateUpdate()
    {
        if (target != null) {
            if (cameraMode.Equals(CameraMode.FOLLOW)) 
            {
                transform.position = target.transform.position + targetOffset;
            } 
            else if (cameraMode.Equals(CameraMode.FPV)) {
                // Camera positioned above & behind head, looking slightly down (with rotation lerping)
                var direction = target.GetComponent<Rigidbody>().velocity.normalized;
                transform.position = target.transform.position + (-5 * direction) + new Vector3(0, 7, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(Vector3.RotateTowards(direction, Vector3.down, 1f, 0f)), 0.05f);
            }
        }
        
    }
}
