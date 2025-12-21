using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("输入设置")]
    [SerializeField] private InputActionAsset inputActions;
    
    private CharacterController characterController;
    private InputActionMap playerActionMap;
    private InputAction moveAction;
    private InputAction sprintAction;
    private Vector3 velocity;
    private bool isSprinting = false;
    
    // 饱食度为0时的减速效果
    private float hungerDebuffMultiplier = 1f;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        
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
                moveAction = playerActionMap.FindAction("Move");
                sprintAction = playerActionMap.FindAction("Sprint");
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
    
    private void Update()
    {
        HandleMovement();
        HandleGravity();
    }
    
    private void HandleMovement()
    {
        Vector2 moveInput = moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        isSprinting = sprintAction?.IsPressed() ?? false;
        
        // 计算移动方向（相对于摄像机）
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        moveDirection.Normalize();
        
        // 根据是否冲刺选择速度
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        
        // 应用饱食度debuff
        currentSpeed *= hungerDebuffMultiplier;
        
        // 移动角色
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
    }
    
    private void HandleGravity()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 小的向下速度，确保贴地
        }
        
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    
    // 设置饱食度debuff（由SurvivalStats调用）
    public void SetHungerDebuff(float multiplier)
    {
        hungerDebuffMultiplier = multiplier;
    }
}
