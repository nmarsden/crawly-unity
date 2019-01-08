﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    enum CameraMode { ORTHO, ORTHO_FOLLOW, SIDE, SIDE_FOLLOW, SIDE_FOLLOW_TURN, TOP, TOP_FOLLOW, TOP_FOLLOW_TURN, FPV };

    delegate void Action();

    IDictionary<CameraMode, Action> cameraModeToAction;

    GameObject target;
    Vector3 targetOffset;
    CameraMode cameraMode = CameraMode.SIDE_FOLLOW_TURN;

    public void Awake() {
        cameraModeToAction = new Dictionary<CameraMode, Action>
        {
            { CameraMode.ORTHO,             InitOrthographicCameraMode },
            { CameraMode.ORTHO_FOLLOW,      InitOrthographicFollowCameraMode },
            { CameraMode.SIDE,              InitSideCameraMode },
            { CameraMode.SIDE_FOLLOW,       InitSideFollowCameraMode },
            { CameraMode.SIDE_FOLLOW_TURN,  InitSideFollowTurnCameraMode },
            { CameraMode.TOP,               InitTopCameraMode },
            { CameraMode.TOP_FOLLOW,        InitTopFollowCameraMode },
            { CameraMode.TOP_FOLLOW_TURN,   InitTopFollowTurnCameraMode },
            { CameraMode.FPV,               InitFirstPersonCameraMode },
        };

    }
    public void Init(GameObject target) {
        this.target = target;
        InitCameraMode(cameraMode);
    }

    public void ToggleCameraMode() {
        if (cameraMode.Equals(CameraMode.ORTHO)) 
        {
            InitCameraMode(CameraMode.ORTHO_FOLLOW);
        } 
        else if (cameraMode.Equals(CameraMode.ORTHO_FOLLOW)) 
        {
            InitCameraMode(CameraMode.SIDE);
        } 
        else if (cameraMode.Equals(CameraMode.SIDE)) 
        {
            InitCameraMode(CameraMode.SIDE_FOLLOW);
        } 
        else if (cameraMode.Equals(CameraMode.SIDE_FOLLOW)) 
        {
            InitCameraMode(CameraMode.SIDE_FOLLOW_TURN);
        } 
        else if (cameraMode.Equals(CameraMode.SIDE_FOLLOW_TURN)) 
        {
            InitCameraMode(CameraMode.TOP);
        } 
        else if (cameraMode.Equals(CameraMode.TOP)) 
        {
            InitCameraMode(CameraMode.TOP_FOLLOW);
        } 
        else if (cameraMode.Equals(CameraMode.TOP_FOLLOW)) 
        {
            InitCameraMode(CameraMode.TOP_FOLLOW_TURN);
        } 
        else if (cameraMode.Equals(CameraMode.TOP_FOLLOW_TURN)) 
        {
            InitCameraMode(CameraMode.FPV);
        } 
        else if (cameraMode.Equals(CameraMode.FPV)) 
        {
            InitCameraMode(CameraMode.ORTHO);
        }
    }

    void InitCameraMode(CameraMode cameraMode) {
        cameraModeToAction[cameraMode]();
    }

    void InitOrthographicCameraMode() {
        cameraMode = CameraMode.ORTHO;
        Camera.main.orthographic = true;
        Camera.main.transform.position = new Vector3(55.6f, 45.5f, -56.5f);
        Camera.main.transform.rotation = Quaternion.Euler(30, -45, 0);
    }

    void InitOrthographicFollowCameraMode() {
        cameraMode = CameraMode.ORTHO_FOLLOW;
        Camera.main.orthographic = true;

        var cameraDistance = 50;
        var viewAngle = Quaternion.Euler(30, -45, 0);
        Vector3 viewDirection = (viewAngle * Vector3.forward).normalized;

        Camera.main.transform.position = target.transform.position - (viewDirection * cameraDistance);
        Camera.main.transform.rotation = viewAngle;
        targetOffset = Camera.main.transform.position - target.transform.position;
    }

    void InitSideCameraMode() {
        cameraMode = CameraMode.SIDE;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 40;
        Camera.main.transform.position = new Vector3(50.6f, 45.5f, 0);
        Camera.main.transform.rotation = Quaternion.Euler(45, -90, 0);
    }

    void InitSideFollowCameraMode() {
        cameraMode = CameraMode.SIDE_FOLLOW;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 40;

        var cameraDistance = 70;
        var viewAngle = Quaternion.Euler(45, -90, 0);
        Vector3 viewDirection = (viewAngle * Vector3.forward).normalized;

        Camera.main.transform.position = target.transform.position - (viewDirection * cameraDistance);
        Camera.main.transform.rotation = viewAngle;
        targetOffset = Camera.main.transform.position - target.transform.position;
    }

    void InitSideFollowTurnCameraMode() {
        cameraMode = CameraMode.SIDE_FOLLOW_TURN;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 40;
    }

    void InitTopCameraMode() {
        cameraMode = CameraMode.TOP;
        Camera.main.orthographic = true;
        Camera.main.transform.position = new Vector3(0, 100, 0);
        Camera.main.transform.rotation = Quaternion.Euler(90, -90, 0);
    }

    void InitTopFollowCameraMode() {
        cameraMode = CameraMode.TOP_FOLLOW;
        Camera.main.orthographic = true;

        var cameraDistance = 100;
        var viewAngle = Quaternion.Euler(90, -90, 0);
        Vector3 viewDirection = (viewAngle * Vector3.forward).normalized;

        Camera.main.transform.position = target.transform.position - (viewDirection * cameraDistance);
        Camera.main.transform.rotation = viewAngle;
        targetOffset = Camera.main.transform.position - target.transform.position;
    }

    void InitTopFollowTurnCameraMode() {
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

    void InitFirstPersonCameraMode() {
        cameraMode = CameraMode.FPV;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 80;
    }

    void LateUpdate()
    {
        if (target != null) {
            if (cameraMode.Equals(CameraMode.ORTHO_FOLLOW) || cameraMode.Equals(CameraMode.SIDE_FOLLOW) || cameraMode.Equals(CameraMode.TOP_FOLLOW)) 
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
            else if (cameraMode.Equals(CameraMode.SIDE_FOLLOW_TURN)) 
            {
                // Camera positioned above & behind head, looking slightly down (with rotation lerping)
                var direction = target.transform.rotation * Vector3.forward;
                transform.position = Vector3.Lerp(transform.position, target.transform.position + (-40f * direction) + new Vector3(0, 60f, 0), 0.05f);
                transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(Vector3.RotateTowards(direction, Vector3.down, 1f, 0f)), 0.05f);
            }
            else if (cameraMode.Equals(CameraMode.FPV)) {
                // Camera positioned above & behind head, looking slightly down (with rotation lerping)
                var direction = target.transform.rotation * Vector3.forward;
                transform.position = target.transform.position + (-5 * direction) + new Vector3(0, 7, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(Vector3.RotateTowards(direction, Vector3.down, 1f, 0f)), 0.05f);
            }
        }
        
    }
}
