using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("摄像机设置")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float verticalRotationLimit = 80f;
    
    [Header("输入设置")]
    [SerializeField] private InputActionAsset inputActions;
    
    private Camera playerCamera;
    private float verticalRotation = 0f;
    private InputActionMap playerActionMap;
    private InputAction lookAction;
    
    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        // 如果没有在Inspector中设置，尝试自动加载
        if (inputActions == null)
        {
            inputActions = Resources.Load<InputActionAsset>("InputSystem_Actions");
        }
        
        if (inputActions != null)
        {
            playerActionMap = inputActions.FindActionMap("Player");
            if (playerActionMap != null)
            {
                lookAction = playerActionMap.FindAction("Look");
            }
        }
    }
    
    private void OnEnable()
    {
        playerActionMap?.Enable();
    }
    
    private void OnDisable()
    {
        playerActionMap?.Disable();
    }
    
    private void Start()
    {
        // 锁定鼠标光标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Update()
    {
        HandleMouseLook();
        
        // 按ESC解锁鼠标（用于测试）
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
    
    private void HandleMouseLook()
    {
        Vector2 lookInput = lookAction?.ReadValue<Vector2>() ?? Vector2.zero;
        
        // 水平旋转（Y轴）
        float mouseX = lookInput.x * mouseSensitivity;
        transform.parent.Rotate(Vector3.up * mouseX);
        
        // 垂直旋转（X轴）- 限制在摄像机本身
        float mouseY = lookInput.y * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
