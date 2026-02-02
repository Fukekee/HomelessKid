using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Shelter : InteractableBase
{
    [Header("UI引用")]
    [SerializeField] private GameObject sleepPanel;
    [SerializeField] private Button sleepButton;
    [SerializeField] private Button cancelButton;
    
    [Header("基地Stage配置")]
    [Tooltip("所有阶段的配置列表（按stageId顺序）")]
    [SerializeField] private List<ShelterStageData> stageDataList = new List<ShelterStageData>();
    
    [Header("模型切换")]
    [Tooltip("基地外观模型的父节点，升级后会在此节点下切换为对应阶段的预制体")]
    [SerializeField] private Transform modelContainer;
    
    private SurvivalStats survivalStats;
    private DayManager dayManager;
    private TrashSpawner trashSpawner;
    private Inventory inventory;
    
    // Stage管理
    private int currentStage = 0;
    private ShelterStageData currentStageData;
    private GameObject currentModelInstance; // 当前显示的阶段模型实例
    
    private void Start()
    {
        survivalStats = FindObjectOfType<SurvivalStats>();
        dayManager = DayManager.Instance;
        trashSpawner = FindObjectOfType<TrashSpawner>();
        inventory = FindObjectOfType<Inventory>();
        
        // 初始化Stage数据
        UpdateCurrentStageData();
        
        // 设置玩家在基地内标记
        if (survivalStats != null)
        {
            survivalStats.SetInShelter(true);
        }
        
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
    
    protected override void Update()
    {
        base.Update(); // 调用基类的Update来更新isPlayerInRange
        
        // 根据玩家是否在范围内设置基地标记
        if (survivalStats != null)
        {
            survivalStats.SetInShelter(isPlayerInRange);
        }
    }
    
    private void UpdateCurrentStageData()
    {
        // 从列表中查找当前阶段的配置
        currentStageData = stageDataList.Find(s => s != null && s.stageId == currentStage);
        if (currentStageData == null && stageDataList.Count > 0)
        {
            // 如果找不到，使用第一个作为默认
            currentStageData = stageDataList[0];
            currentStage = currentStageData.stageId;
        }
        RefreshStageModel();
    }
    
    /// <summary>
    /// 根据当前阶段刷新基地外观模型（升级后切换为对应阶段的预制体）
    /// </summary>
    private void RefreshStageModel()
    {
        if (modelContainer == null) return;
        
        if (currentModelInstance != null)
        {
            Destroy(currentModelInstance);
            currentModelInstance = null;
        }
        
        if (currentStageData != null && currentStageData.stageModelPrefab != null)
        {
            currentModelInstance = Instantiate(currentStageData.stageModelPrefab, modelContainer);
            currentModelInstance.transform.localPosition = Vector3.zero;
            currentModelInstance.transform.localRotation = Quaternion.identity;
            currentModelInstance.transform.localScale = Vector3.one;
        }
    }
    
    public override void OnInteract()
    {
        if (!CanInteract()) return;
        
        OpenSleepPanel();
    }
    
    public override string GetInteractionPrompt()
    {
        if (CanUpgradeToStage(currentStage + 1))
        {
            return "按E进入小屋（可升级）";
        }
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
        // 恢复健康度和精神度（应用Stage倍率）
        if (survivalStats != null && currentStageData != null)
        {
            // 健康恢复
            float baseHealthRestore = GameBalance.Config != null ? GameBalance.Config.sleepHealthRestore : 30f;
            float healthRestore = baseHealthRestore * currentStageData.healthRecoveryMultiplier;
            survivalStats.RestoreHealth(healthRestore);
            
            // 精神恢复
            float baseMoodRestore = GameBalance.Config != null ? GameBalance.Config.moodRestoreFromSleep : 15f;
            float moodRestore = baseMoodRestore * currentStageData.moodRecoveryMultiplier;
            survivalStats.RestoreMood(moodRestore);
            
            Debug.Log($"[Shelter] 睡眠恢复 - 健康: {healthRestore:F1} (倍率: {currentStageData.healthRecoveryMultiplier:F2}), 精神: {moodRestore:F1} (倍率: {currentStageData.moodRecoveryMultiplier:F2})");
        }
        else if (survivalStats != null)
        {
            // 如果没有Stage数据，使用默认恢复
            survivalStats.RestoreHealthOvernight();
            survivalStats.RestoreMoodFromSleep();
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
    
    /// <summary>
    /// 检查是否可以升级到指定阶段
    /// </summary>
    public bool CanUpgradeToStage(int targetStage)
    {
        if (targetStage <= currentStage)
        {
            return false; // 不能降级
        }
        
        // 查找目标阶段的配置
        ShelterStageData targetStageData = stageDataList.Find(s => s != null && s.stageId == targetStage);
        if (targetStageData == null)
        {
            return false; // 阶段配置不存在
        }
        
        // 检查是否有足够的材料
        if (inventory == null)
        {
            return false;
        }
        
        foreach (var requirement in targetStageData.requiredMaterials)
        {
            if (requirement.item == null) continue;
            
            int requiredAmount = requirement.amount;
            int currentAmount = inventory.GetCount(requirement.item);
            
            if (currentAmount < requiredAmount)
            {
                return false; // 材料不足
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 升级到指定阶段
    /// </summary>
    public bool UpgradeToStage(int targetStage)
    {
        if (!CanUpgradeToStage(targetStage))
        {
            Debug.LogWarning($"[Shelter] 无法升级到Stage {targetStage}");
            return false;
        }
        
        ShelterStageData targetStageData = stageDataList.Find(s => s != null && s.stageId == targetStage);
        if (targetStageData == null)
        {
            return false;
        }
        
        // 消耗材料
        if (inventory != null)
        {
            foreach (var requirement in targetStageData.requiredMaterials)
            {
                if (requirement.item == null) continue;
                
                bool removed = inventory.RemoveItem(requirement.item, requirement.amount);
                if (!removed)
                {
                    Debug.LogError($"[Shelter] 升级时无法移除材料: {requirement.item.itemName} x{requirement.amount}");
                    return false;
                }
                Debug.Log($"[Shelter] 消耗材料: {requirement.item.itemName} x{requirement.amount}");
            }
        }
        
        // 更新阶段
        currentStage = targetStage;
        UpdateCurrentStageData();
        
        // 应用一次性mood加成
        if (survivalStats != null && targetStageData.moodBaseBonus > 0)
        {
            survivalStats.RestoreMood(targetStageData.moodBaseBonus);
            Debug.Log($"[Shelter] 升级到Stage {targetStage} ({targetStageData.stageName})，获得mood加成: {targetStageData.moodBaseBonus}");
        }
        else
        {
            Debug.Log($"[Shelter] 升级到Stage {targetStage} ({targetStageData.stageName})");
        }
        
        return true;
    }
    
    /// <summary>
    /// 获取当前阶段
    /// </summary>
    public int GetCurrentStage()
    {
        return currentStage;
    }
    
    /// <summary>
    /// 获取当前阶段数据
    /// </summary>
    public ShelterStageData GetCurrentStageData()
    {
        return currentStageData;
    }
    
    /// <summary>
    /// 获取指定阶段的配置数据
    /// </summary>
    public ShelterStageData GetStageData(int stageId)
    {
        return stageDataList.Find(s => s != null && s.stageId == stageId);
    }
}





