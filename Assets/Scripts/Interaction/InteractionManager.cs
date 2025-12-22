using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


public class InteractionManager : MonoBehaviour
{
    [Header("检测设置")]
    [SerializeField] private float interactionRange = 5f;
    [SerializeField] private LayerMask interactionLayer = -1;
    
    [Header("UI引用")]
    [SerializeField] private TextMeshProUGUI interactionPromptText; // 兼容旧方式
    [SerializeField] private UIManager uiManager; // 推荐使用UIManager
    
    [Header("输入设置")]
    [SerializeField] private InputActionAsset inputActions;
    
    private InputActionMap playerActionMap;
    private InputAction interactAction;
    private IInteractable currentInteractable;
    private Camera playerCamera;
    
    private void Awake()
    {
        // 如果Inspector中没有设置，尝试从项目中查找
        if (inputActions == null)
        {
            #if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets("InputSystem_Actions t:InputActionAsset");
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                inputActions = UnityEditor.AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);
            }
            #endif
        }
        
        if (inputActions != null)
        {
            playerActionMap = inputActions.FindActionMap("Player");
            if (playerActionMap != null)
            {
                interactAction = playerActionMap.FindAction("Interact");
            }
        }
        
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            playerCamera = FindObjectOfType<Camera>();
        }
        
        // 如果没有手动指定UIManager，尝试自动查找
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
    }
    
    private void OnEnable()
    {
        playerActionMap?.Enable();
        if (interactAction != null)
        {
            interactAction.performed += OnInteractPressed;
        }
    }
    
    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.performed -= OnInteractPressed;
        }
        playerActionMap?.Disable();
    }
    
    private void Update()
    {
        DetectInteractable();
        UpdateInteractionPrompt();
    }
    
    private void DetectInteractable()
    {
        currentInteractable = null;
        
        // 首先使用射线检测前方的交互对象
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, interactionRange, interactionLayer))
        {
            currentInteractable = hit.collider.GetComponent<IInteractable>();
        }
        
        // 如果射线没有命中任何IInteractable，使用范围检测查找附近的交互对象
        if (currentInteractable == null)
        {
            InteractableBase[] interactables = FindObjectsOfType<InteractableBase>();
            float closestDistance = float.MaxValue;
            InteractableBase closestInteractable = null;
            
            Vector3 playerPosition = transform.position;
            
            foreach (InteractableBase interactable in interactables)
            {
                if (interactable.CanInteract())
                {
                    float distance = Vector3.Distance(playerPosition, interactable.transform.position);
                    if (distance < closestDistance && distance <= interactionRange)
                    {
                        closestDistance = distance;
                        closestInteractable = interactable;
                    }
                }
            }
            
            currentInteractable = closestInteractable;
        }
    }
    
    private void UpdateInteractionPrompt()
    {
        if (currentInteractable != null && currentInteractable.CanInteract())
        {
            string prompt = currentInteractable.GetInteractionPrompt();
            
            // 优先使用UIManager
            if (uiManager != null)
            {
                uiManager.ShowInteractionPrompt(prompt);
            }
            // 兼容旧方式
            else if (interactionPromptText != null)
            {
                interactionPromptText.text = prompt;
                interactionPromptText.gameObject.SetActive(true);
            }
        }
        else
        {
            // 隐藏提示
            if (uiManager != null)
            {
                uiManager.HideInteractionPrompt();
            }
            else if (interactionPromptText != null)
            {
                interactionPromptText.gameObject.SetActive(false);
            }
        }
    }
    
    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (currentInteractable != null && currentInteractable.CanInteract())
        {
            currentInteractable.OnInteract();
        }
    }
}
