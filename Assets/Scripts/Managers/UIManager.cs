using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("交互提示UI")]
    [SerializeField] private TextMeshProUGUI interactionPromptText;
    
    [Header("进度条UI")]
    [SerializeField] private GameObject progressBarPanel;
    [SerializeField] private Slider progressBar;
    
    [Header("获得物品提示")]
    [Tooltip("显示「获得了：xxx」的面板，留空则不显示获得提示")]
    [SerializeField] private GameObject itemObtainedPanel;
    [SerializeField] private TextMeshProUGUI itemObtainedText;
    [Tooltip("提示显示时长（秒）")]
    [SerializeField] private float itemObtainedDisplayDuration = 2f;
    
    [Header("背包UI")]
    [SerializeField] private InventoryUI inventoryUI;

    [Header("快捷键")]
    [SerializeField] private bool enableInventoryHotkey = true;
    
    private float currentProgressDuration = 0f;
    private float currentProgressTime = 0f;
    private bool isShowingProgress = false;
    private Coroutine hideItemObtainedRoutine;
    private Inventory cachedInventory;

    private void Awake()
    {
        if (inventoryUI == null)
            inventoryUI = FindObjectOfType<InventoryUI>(true);
    }
    
    private void Start()
    {
        // 订阅背包添加物品事件，统一显示获得提示（翻桶、拾取地面垃圾、商店等）
        cachedInventory = FindObjectOfType<Inventory>();
        if (cachedInventory != null && itemObtainedPanel != null && itemObtainedText != null)
        {
            cachedInventory.OnItemAdded += ShowItemObtained;
        }
    }
    
    private void OnDestroy()
    {
        if (cachedInventory != null)
        {
            cachedInventory.OnItemAdded -= ShowItemObtained;
        }
    }
    
    private void Update()
    {
        // I 键打开/关闭背包
        if (enableInventoryHotkey && Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }

        // 更新进度条
        if (isShowingProgress)
        {
            currentProgressTime += Time.deltaTime;
            if (progressBar != null)
            {
                progressBar.value = Mathf.Clamp01(currentProgressTime / currentProgressDuration);
            }
            
            if (currentProgressTime >= currentProgressDuration)
            {
                HideProgressBar();
            }
        }
    }
    
    public void ShowInteractionPrompt(string prompt)
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.text = prompt;
            interactionPromptText.gameObject.SetActive(true);
        }
    }
    
    public void HideInteractionPrompt()
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }
    
    public void ShowProgressBar(float duration)
    {
        if (progressBarPanel != null)
        {
            progressBarPanel.SetActive(true);
        }
        
        if (progressBar != null)
        {
            progressBar.value = 0f;
        }
        
        currentProgressDuration = duration;
        currentProgressTime = 0f;
        isShowingProgress = true;
    }
    
    public void HideProgressBar()
    {
        if (progressBarPanel != null)
        {
            progressBarPanel.SetActive(false);
        }
        
        isShowingProgress = false;
        currentProgressTime = 0f;
        currentProgressDuration = 0f;
    }
    
    /// <summary>
    /// 显示获得物品提示（翻桶、拾取地面垃圾等获得物品时调用）
    /// </summary>
    public void ShowItemObtained(ItemData item, int amount)
    {
        if (item == null || itemObtainedPanel == null || itemObtainedText == null) return;
        
        string msg = amount > 1
            ? $"获得了：{item.itemName} x{amount}"
            : $"获得了：{item.itemName}";
        itemObtainedText.text = msg;
        itemObtainedPanel.SetActive(true);
        
        if (hideItemObtainedRoutine != null)
        {
            StopCoroutine(hideItemObtainedRoutine);
        }
        hideItemObtainedRoutine = StartCoroutine(HideItemObtainedAfterDelay());
    }
    
    private IEnumerator HideItemObtainedAfterDelay()
    {
        yield return new WaitForSeconds(itemObtainedDisplayDuration);
        if (itemObtainedPanel != null)
        {
            itemObtainedPanel.SetActive(false);
        }
        hideItemObtainedRoutine = null;
    }
    
    public void ToggleInventory()
    {
        if (inventoryUI != null)
        {
            inventoryUI.ToggleInventory();

            // 打开背包时解锁鼠标，关闭时锁回去，方便点击UI
            if (inventoryUI.IsOpen)
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
}

