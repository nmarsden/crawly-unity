using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    enum CameraMode { ORTHO, ORTHO_FOLLOW, SIDE, SIDE_FOLLOW, SIDE_FOLLOW_TURN, TOP, TOP_FOLLOW, TOP_FOLLOW_TURN, FPV, FLY_AROUND };

    delegate void Action();

    IDictionary<CameraMode, Action> cameraModeToAction;

    GameObject target;
    Vector3 targetOffset;
    CameraMode cameraMode = CameraMode.ORTHO;
    CameraMode cameraModePreFlyAround;

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
            { CameraMode.FLY_AROUND,        InitFlyAroundCameraMode },
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

    public void ActivateFlyAroundMode() {
        if (cameraMode.Equals(CameraMode.FLY_AROUND)) {
            return;
        }
        cameraModePreFlyAround = cameraMode;
        InitCameraMode(CameraMode.FLY_AROUND);
    }

    public void DeactivateFlyAroundMode() {
        if (!cameraMode.Equals(CameraMode.FLY_AROUND)) {
            return;
        }
        InitCameraMode(cameraModePreFlyAround);
    }

    void InitCameraMode(CameraMode cameraMode) {
        cameraModeToAction[cameraMode]();
    }

    void InitOrthographicCameraMode() {
        cameraMode = CameraMode.ORTHO;
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 22;
        Camera.main.transform.position = new Vector3(56.2f, 45.5f, -56.5f);
        Camera.main.transform.rotation = Quaternion.Euler(30, -45, 0);
    }

    void InitOrthographicFollowCameraMode() {
        cameraMode = CameraMode.ORTHO_FOLLOW;
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 22;

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
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 50;
        Camera.main.transform.position = new Vector3(0, 50, 0);
        Camera.main.transform.rotation = Quaternion.Euler(90, -90, 0);
    }

    void InitTopFollowCameraMode() {
        cameraMode = CameraMode.TOP_FOLLOW;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 50;

        var cameraDistance = 50;
        var viewAngle = Quaternion.Euler(90, -90, 0);
        Vector3 viewDirection = (viewAngle * Vector3.forward).normalized;

        Camera.main.transform.position = target.transform.position - (viewDirection * cameraDistance);
        Camera.main.transform.rotation = viewAngle;
        targetOffset = Camera.main.transform.position - target.transform.position;
    }

    void InitTopFollowTurnCameraMode() {
        cameraMode = CameraMode.TOP_FOLLOW_TURN;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 50;

        var cameraDistance = 50;
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

    void InitFlyAroundCameraMode() {
        cameraMode = CameraMode.FLY_AROUND;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 40;

        var cameraDistance = 70;
        var viewAngle = Quaternion.Euler(45, -90, 0);
        Vector3 viewDirection = (viewAngle * Vector3.forward).normalized;

        Camera.main.transform.position = target.transform.position - (viewDirection * cameraDistance);
        Camera.main.transform.rotation = viewAngle;
        targetOffset = Camera.main.transform.position - target.transform.position;
    }

    void LateUpdate()
    {
        if (target != null) {
            if (cameraMode.Equals(CameraMode.ORTHO_FOLLOW) || cameraMode.Equals(CameraMode.SIDE_FOLLOW) || cameraMode.Equals(CameraMode.TOP_FOLLOW)) 
            {
                var t = 2f * Time.deltaTime;

                var desiredPosition = target.transform.position + targetOffset;
                transform.position = Vector3.Lerp(transform.position, desiredPosition, t);
            } 
            else if (cameraMode.Equals(CameraMode.TOP_FOLLOW_TURN)) 
            {
                // Camera positioned directly above the head, looking down and rotated around Y-axis the same as the head is rotated (with rotation lerping)
                var t = 2f * Time.deltaTime;

                var desiredPosition = target.transform.position + targetOffset;
                transform.position = Vector3.Lerp(transform.position, desiredPosition, t);

                var desiredRotation = Quaternion.Euler(90, target.transform.rotation.eulerAngles.y, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, t);
            } 
            else if (cameraMode.Equals(CameraMode.SIDE_FOLLOW_TURN)) 
            {
                // Camera positioned above & behind head, looking slightly down (with rotation lerping)
                var direction = target.transform.rotation * Vector3.forward;
                var t = 2f * Time.deltaTime;

                var desiredPosition = target.transform.position + (-40f * direction) + new Vector3(0, 60f, 0);
                transform.position = Vector3.Lerp(transform.position, desiredPosition, t);

                var desiredRotation = Quaternion.LookRotation(Vector3.RotateTowards(direction, Vector3.down, 1f, 0f));
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, t);
            }
            else if (cameraMode.Equals(CameraMode.FPV)) {
                // Camera positioned above & behind head, looking slightly down (with rotation lerping)
                var direction = target.transform.rotation * Vector3.forward;
                var t = 2f * Time.deltaTime;

                var desiredPosition = target.transform.position + (-5 * direction) + new Vector3(0, 7, 0);
                transform.position = Vector3.Lerp(transform.position, desiredPosition, t);

                var desiredRotation = Quaternion.LookRotation(Vector3.RotateTowards(direction, Vector3.down, 1f, 0f));
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, t);
            }
            else if (cameraMode.Equals(CameraMode.FLY_AROUND)) {
                // Update camera position/rotation according to target offset
                var desiredPosition = target.transform.position + targetOffset;
                transform.position = desiredPosition;

                var heading = target.transform.position - transform.position;
                var distance = heading.magnitude;
                var direction = heading / distance;

                var desiredRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = desiredRotation;

                // Update targetOffset by rotating 0.3 degrees around the target position
                var rotatedPoint = RotatePointAroundPivot(transform.position, target.transform.position, new Vector3(0, 0.3f, 0));
                targetOffset = rotatedPoint - target.transform.position;
            }
        }
    }

    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) 
    {
        var dir = point - pivot;
        var rotatedDir = Quaternion.Euler(angles) * dir;
        var rotatedPoint = rotatedDir + pivot;
        return rotatedPoint;
    }
}
