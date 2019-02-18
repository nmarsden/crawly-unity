using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    enum CameraMode { ORTHO, ORTHO_FOLLOW, SIDE, SIDE_FOLLOW, SIDE_FOLLOW_TURN, TOP, TOP_FOLLOW, TOP_FOLLOW_TURN, FPV, FLY_AROUND };

    public class MoveTransform
    {
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.Euler(0, 0, 0);
    }

    delegate void Action();

    IDictionary<CameraMode, Action> cameraModeToAction;

    GameObject target;
    Vector3 targetOffset;
    MoveTransform moveTransform = new MoveTransform();
    CameraMode cameraMode = CameraMode.ORTHO;
    CameraMode cameraModePreFlyAround;
    bool isShaking;
    float shakeStartTime;
    float shakeDuration;
    float shakeMagnitude = 0.2f;

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
        InitCameraTransform();
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

    public void Shake(float shakeDuration) {
        this.shakeDuration = shakeDuration;
        shakeStartTime = Time.time;
        isShaking = true;
    }

    void InitCameraMode(CameraMode cameraMode) {
        cameraModeToAction[cameraMode]();
    }

    void InitCameraTransform() {
        // Set the initial position/rotation of the camera relative to the moveTransform in order to force an initial camera animation
        if (Camera.main.orthographic) {
            // -- Orthographic View --
            // Rotate camera 30 degrees around the X-axis from the movementTransform position
            // Position camera closer so that the arena remains in view
            var scaleFactor = 0.01f;
            Camera.main.transform.position = new Vector3(moveTransform.position.x * scaleFactor, moveTransform.position.y * scaleFactor, moveTransform.position.z * scaleFactor);
            Camera.main.transform.rotation = moveTransform.rotation;
            Camera.main.transform.Rotate(-30, 0, 0);
        } else {
            // -- Perspective View --
            // Position camera 100 back from the moveTransform position
            Vector3 viewDirection = (moveTransform.rotation * Vector3.forward).normalized;
            Camera.main.transform.position = moveTransform.position - (viewDirection * 100);
            Camera.main.transform.rotation = moveTransform.rotation;
        }
    }

    void InitOrthographicCameraMode() {
        cameraMode = CameraMode.ORTHO;
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 22;
        moveTransform.position = new Vector3(56.2f, 45.5f, -56.5f);
        moveTransform.rotation = Quaternion.Euler(30, -45, 0);
    }

    void InitOrthographicFollowCameraMode() {
        cameraMode = CameraMode.ORTHO_FOLLOW;
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 22;

        var cameraDistance = 100;
        var viewAngle = Quaternion.Euler(30, -45, 0);
        Vector3 viewDirection = (viewAngle * Vector3.forward).normalized;

        moveTransform.position = target.transform.position - (viewDirection * cameraDistance);
        moveTransform.rotation = viewAngle;
        targetOffset = moveTransform.position - target.transform.position;
    }

    void InitSideCameraMode() {
        cameraMode = CameraMode.SIDE;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 40;
        moveTransform.position = new Vector3(50.6f, 45.5f, 0);
        moveTransform.rotation = Quaternion.Euler(45, -90, 0);
    }

    void InitSideFollowCameraMode() {
        cameraMode = CameraMode.SIDE_FOLLOW;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 40;

        var cameraDistance = 70;
        var viewAngle = Quaternion.Euler(45, -90, 0);
        Vector3 viewDirection = (viewAngle * Vector3.forward).normalized;

        moveTransform.position = target.transform.position - (viewDirection * cameraDistance);
        moveTransform.rotation = viewAngle;
        targetOffset = moveTransform.position - target.transform.position;
    }

    void InitSideFollowTurnCameraMode() {
        cameraMode = CameraMode.SIDE_FOLLOW_TURN;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 40;

        var direction = target.transform.rotation * Vector3.forward;
        moveTransform.position = target.transform.position + (-40f * direction) + new Vector3(0, 60f, 0);
        moveTransform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(direction, Vector3.down, 1f, 0f));
    }

    void InitTopCameraMode() {
        cameraMode = CameraMode.TOP;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 50;
        moveTransform.position = new Vector3(0, 50, 0);
        moveTransform.rotation = Quaternion.Euler(90, -90, 0);
    }

    void InitTopFollowCameraMode() {
        cameraMode = CameraMode.TOP_FOLLOW;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 50;

        var cameraDistance = 50;
        var viewAngle = Quaternion.Euler(90, -90, 0);
        Vector3 viewDirection = (viewAngle * Vector3.forward).normalized;

        moveTransform.position = target.transform.position - (viewDirection * cameraDistance);
        moveTransform.rotation = viewAngle;
        targetOffset = moveTransform.position - target.transform.position;
    }

    void InitTopFollowTurnCameraMode() {
        cameraMode = CameraMode.TOP_FOLLOW_TURN;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 50;

        var cameraDistance = 50;
        var targetYRotationAngle = target.transform.rotation.eulerAngles.y;
        var viewAngle = Quaternion.Euler(90, targetYRotationAngle, 0);
        Vector3 viewDirection = (viewAngle * Vector3.forward).normalized;

        moveTransform.position = target.transform.position - (viewDirection * cameraDistance);
        moveTransform.rotation = viewAngle;
        targetOffset = moveTransform.position - target.transform.position;
    }

    void InitFirstPersonCameraMode() {
        cameraMode = CameraMode.FPV;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 80;

        var direction = target.transform.rotation * Vector3.forward;
        moveTransform.position = target.transform.position + (-5 * direction) + new Vector3(0, 7, 0);
        moveTransform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(direction, Vector3.down, 1f, 0f));
    }

    void InitFlyAroundCameraMode() {
        cameraMode = CameraMode.FLY_AROUND;
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 40;

        var cameraDistance = 70;
        var viewAngle = Quaternion.Euler(45, -90, 0);
        Vector3 viewDirection = (viewAngle * Vector3.forward).normalized;

        moveTransform.position = target.transform.position - (viewDirection * cameraDistance);
        moveTransform.rotation = viewAngle;
        targetOffset = moveTransform.position - target.transform.position;
    }

    void LateUpdate()
    {
        if (target != null) {
            var t = 2f * Time.deltaTime;

            if (cameraMode.Equals(CameraMode.ORTHO_FOLLOW) || cameraMode.Equals(CameraMode.SIDE_FOLLOW) || cameraMode.Equals(CameraMode.TOP_FOLLOW)) 
            {
                var desiredPosition = target.transform.position + targetOffset;
                moveTransform.position = Vector3.Lerp(moveTransform.position, desiredPosition, t);
            } 
            else if (cameraMode.Equals(CameraMode.TOP_FOLLOW_TURN)) 
            {
                // Camera positioned directly above the head, looking down and rotated around Y-axis the same as the head is rotated (with rotation lerping)
                var desiredPosition = target.transform.position + targetOffset;
                moveTransform.position = Vector3.Lerp(moveTransform.position, desiredPosition, t);

                var desiredRotation = Quaternion.Euler(90, target.transform.rotation.eulerAngles.y, 0);
                moveTransform.rotation = Quaternion.Lerp(moveTransform.rotation, desiredRotation, t);
            } 
            else if (cameraMode.Equals(CameraMode.SIDE_FOLLOW_TURN)) 
            {
                // Camera positioned above & behind head, looking slightly down (with rotation lerping)
                var direction = target.transform.rotation * Vector3.forward;

                var desiredPosition = target.transform.position + (-40f * direction) + new Vector3(0, 60f, 0);
                moveTransform.position = Vector3.Lerp(moveTransform.position, desiredPosition, t);

                var desiredRotation = Quaternion.LookRotation(Vector3.RotateTowards(direction, Vector3.down, 1f, 0f));
                moveTransform.rotation = Quaternion.Lerp(moveTransform.rotation, desiredRotation, t);
            }
            else if (cameraMode.Equals(CameraMode.FPV)) {
                // Camera positioned above & behind head, looking slightly down (with rotation lerping)
                var direction = target.transform.rotation * Vector3.forward;

                var desiredPosition = target.transform.position + (-5 * direction) + new Vector3(0, 7, 0);
                moveTransform.position = Vector3.Lerp(moveTransform.position, desiredPosition, t);

                var desiredRotation = Quaternion.LookRotation(Vector3.RotateTowards(direction, Vector3.down, 1f, 0f));
                moveTransform.rotation = Quaternion.Lerp(moveTransform.rotation, desiredRotation, t);
            }
            else if (cameraMode.Equals(CameraMode.FLY_AROUND)) {
                // Update camera position/rotation according to target offset
                var desiredPosition = target.transform.position + targetOffset;
                moveTransform.position = desiredPosition;

                var heading = target.transform.position - moveTransform.position;
                var distance = heading.magnitude;
                var direction = heading / distance;

                var desiredRotation = Quaternion.LookRotation(direction, Vector3.up);
                moveTransform.rotation = desiredRotation;

                // Update targetOffset by rotating 0.3 degrees around the target position
                var rotatedPoint = RotatePointAroundPivot(moveTransform.position, target.transform.position, new Vector3(0, 0.3f, 0));
                targetOffset = rotatedPoint - target.transform.position;
            }

            // Update camera position/rotation
            if (cameraMode.Equals(CameraMode.FLY_AROUND))
            {
                Camera.main.transform.position = moveTransform.position;
                Camera.main.transform.rotation = moveTransform.rotation;
            } else {
                var t2 = 5f * Time.deltaTime;
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, moveTransform.position, t2);
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, moveTransform.rotation, t2);
            }

            // Shake camera
            if (isShaking) {
                if (Time.time - shakeStartTime > shakeDuration) {
                    isShaking = false;
                } else {
                    float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
                    float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;
                    float offsetZ = Random.Range(-1f, 1f) * shakeMagnitude;
                    Camera.main.transform.Rotate(offsetX, offsetY, offsetZ);
                }
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
