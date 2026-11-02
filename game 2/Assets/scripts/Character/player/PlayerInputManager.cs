using UnityEngine;
using UnityEngine.SceneManagement;


//A lot of the movement code here is unnecessary for keyboard inputs; it's set up like this in case we add controller support

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    
    PlayerControls playerControls;

    [Header("Movement Input")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    [Header("Camera Input")]
    [SerializeField] Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;

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

        SceneManager.activeSceneChanged += OnSceneChange;

        instance.enabled = false;
    }

    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        //enable player controls in the world scene
        if (newScene.buildIndex == SaveManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;
        }
        //otherwise, we must be at the main menu, so disable
        else
        {
            instance.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (playerControls == null) 
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
        }

        playerControls.Enable();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void Update()
    {
        HandleMovementInput();
        HandleCameraMovementInput();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        //we dont like negative numbers here
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        //clampage (idle (0), walk (0.5), run (1))
        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1;
        }
    }

    private void HandleCameraMovementInput()
    {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }
}
