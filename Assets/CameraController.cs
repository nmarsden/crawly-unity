using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    enum CameraMode { SIDE, SIDE_FOLLOW, TOP, TOP_FOLLOW, TOP_FOLLOW_TURN, FPV };

    GameObject target;
    Vector3 targetOffset;
    CameraMode cameraMode;

    public void Init(GameObject target) {
        this.target = target;
        SwitchToTopFollowTurnView();
    }

    public void ToggleView() {
        if (cameraMode.Equals(CameraMode.SIDE)) 
        {
            SwitchToSideFollowView();
        } 
        else if (cameraMode.Equals(CameraMode.SIDE_FOLLOW)) 
        {
            SwitchToTopView();
        } 
        else if (cameraMode.Equals(CameraMode.TOP)) 
        {
            SwitchToTopFollowView();
        } 
        else if (cameraMode.Equals(CameraMode.TOP_FOLLOW)) 
        {
            SwitchToTopFollowTurnView();
        } 
        else if (cameraMode.Equals(CameraMode.TOP_FOLLOW_TURN)) 
        {
            SwitchToFirstPersonView();
        } 
        else if (cameraMode.Equals(CameraMode.FPV)) 
        {
            SwitchToSideView();
        }
    }

    void SwitchToSideView() {
        cameraMode = CameraMode.SIDE;
        Camera.main.orthographic = true;
        Camera.main.transform.position = new Vector3(55.6f, 45.5f, -56.5f);
        Camera.main.transform.rotation = Quaternion.Euler(30, -45, 0);
    }

    void SwitchToSideFollowView() {
        cameraMode = CameraMode.SIDE_FOLLOW;
        Camera.main.orthographic = true;

        var cameraDistance = 50;
        var viewAngle = Quaternion.Euler(30, -45, 0);
        Vector3 viewDirection = (viewAngle * Vector3.forward).normalized;

        Camera.main.transform.position = target.transform.position - (viewDirection * cameraDistance);
        Camera.main.transform.rotation = viewAngle;
        targetOffset = Camera.main.transform.position - target.transform.position;
    }

    void SwitchToTopView() {
        cameraMode = CameraMode.TOP;
        Camera.main.orthographic = true;
        Camera.main.transform.position = new Vector3(0, 100, 0);
        Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    void SwitchToTopFollowView() {
        cameraMode = CameraMode.TOP_FOLLOW;
        Camera.main.orthographic = true;

        var cameraDistance = 100;
        var viewAngle = Quaternion.Euler(90, 0, 0);
        Vector3 viewDirection = (viewAngle * Vector3.forward).normalized;

        Camera.main.transform.position = target.transform.position - (viewDirection * cameraDistance);
        Camera.main.transform.rotation = viewAngle;
        targetOffset = Camera.main.transform.position - target.transform.position;
    }

    void SwitchToTopFollowTurnView() {
        cameraMode = CameraMode.TOP_FOLLOW_TURN;
        Camera.main.orthographic = true;

        var cameraDistance = 100;
        var targetYRotationAngle = target.transform.rotation.eulerAngles.y;
        var viewAngle = Quaternion.Euler(90, targetYRotationAngle, 0);
        Vector3 viewDirection = (viewAngle * Vector3.forward).normalized;

        Camera.main.transform.position = target.transform.position - (viewDirection * cameraDistance);
        Camera.main.transform.rotation = viewAngle;
        targetOffset = Camera.main.transform.position - target.transform.position;
    }

    void SwitchToFirstPersonView() {
        cameraMode = CameraMode.FPV;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 80;
    }

    void LateUpdate()
    {
        if (target != null) {
            if (cameraMode.Equals(CameraMode.SIDE_FOLLOW) || cameraMode.Equals(CameraMode.TOP_FOLLOW)) 
            {
                transform.position = target.transform.position + targetOffset;
            } 
            else if (cameraMode.Equals(CameraMode.TOP_FOLLOW_TURN)) 
            {
                // Camera positioned directly above the head, looking down and rotated around Y-axis the same as the head is rotated (with rotation lerping)
                transform.position = target.transform.position + targetOffset;

                var targetYRotationAngle = target.transform.rotation.eulerAngles.y;
                var viewAngle = Quaternion.Euler(90, targetYRotationAngle, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, viewAngle, 0.05f);
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
