using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public Camera camObject;
    public PlayerManager player;
    [SerializeField] Transform cameraPivotTransform;

    [Header("Camera Settings")]
    private float smoothTime = 1; // bigger number = camera takes longer to catch up during movement
    [SerializeField] private float leftAndRightRotationSpeed = 220;
    [SerializeField] private float upAndDownRotationSpeed = 220;
    [SerializeField] float minPivot = -30; // lowest angle the camera can look down
    [SerializeField] float maxPivot = 60; // highest angle the camera can look up
    [SerializeField] float cameraCollisionRadius = 0.2f;
    [SerializeField] LayerMask collideWithLayers;

    [Header("Camera Values")]
    private Vector3 camVelocity;
    private Vector3 camObjectPosition;
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    private float cameraZPosition;
    private float targetCameraZPosition;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        cameraZPosition = camObject.transform.localPosition.z;
    }

    public void HandleCameraActions()
    {
        if (player != null)
        {
            HandleFollowTarget();
            HandleRotation();
            HandleCollisions();
        }
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref camVelocity, smoothTime * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotation()
    {
        // rotate left and right
        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
        // rotate up and down (clamped by angle limits)
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minPivot, maxPivot);

        // rotate this gameObject left and right
        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;
        cameraRotation.y = leftAndRightLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = targetRotation;

        //rotate the pivot gameObject up and down
        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;

        RaycastHit hit;
        Vector3 direction = camObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        camObjectPosition.z = Mathf.Lerp(camObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        camObject.transform.localPosition = camObjectPosition;
    }
}
