using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("交互提示UI")]
    [SerializeField] private Text interactionPromptText;
    
    [Header("进度条UI")]
    [SerializeField] private GameObject progressBarPanel;
    [SerializeField] private Slider progressBar;
    
    [Header("背包UI")]
    [SerializeField] private InventoryUI inventoryUI;
    
    private float currentProgressDuration = 0f;
    private float currentProgressTime = 0f;
    private bool isShowingProgress = false;
    
    private void Update()
    {
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
    
    public void ToggleInventory()
    {
        if (inventoryUI != null)
        {
            inventoryUI.ToggleInventory();
        }
    }
}

