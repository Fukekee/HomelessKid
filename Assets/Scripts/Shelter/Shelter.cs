using UnityEngine;
using UnityEngine.UI;

public class Shelter : InteractableBase
{
    [Header("UI引用")]
    [SerializeField] private GameObject sleepPanel;
    [SerializeField] private Button sleepButton;
    [SerializeField] private Button cancelButton;
    
    private SurvivalStats survivalStats;
    private DayManager dayManager;
    private TrashSpawner trashSpawner;
    
    private void Start()
    {
        survivalStats = FindObjectOfType<SurvivalStats>();
        dayManager = DayManager.Instance;
        trashSpawner = FindObjectOfType<TrashSpawner>();
        
        if (sleepButton != null)
        {
            sleepButton.onClick.AddListener(OnSleep);
        }
        
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(CloseSleepPanel);
        }
        
        if (sleepPanel != null)
        {
            sleepPanel.SetActive(false);
        }
    }
    
    public override void OnInteract()
    {
        if (!CanInteract()) return;
        
        OpenSleepPanel();
    }
    
    public override string GetInteractionPrompt()
    {
        return "按E进入小屋";
    }
    
    private void OpenSleepPanel()
    {
        if (sleepPanel != null)
        {
            sleepPanel.SetActive(true);
        }
    }
    
    private void CloseSleepPanel()
    {
        if (sleepPanel != null)
        {
            sleepPanel.SetActive(false);
        }
    }
    
    private void OnSleep()
    {
        // 恢复健康度
        if (survivalStats != null)
        {
            survivalStats.RestoreHealthOvernight();
        }
        
        // 触发新的一天
        if (dayManager != null)
        {
            dayManager.StartNewDay();
        }
        
        // 刷新垃圾
        if (trashSpawner != null)
        {
            trashSpawner.SpawnTrash();
        }
        
        CloseSleepPanel();
        
        Debug.Log("过夜完成，进入新的一天");
    }
}


