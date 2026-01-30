using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 基地升级UI控制器
/// 集成到SleepPanel中，显示升级信息和按钮
/// </summary>
public class ShelterUpgradeUI : MonoBehaviour
{
    [Header("UI引用")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI upgradeInfoText;
    [SerializeField] private TextMeshProUGUI materialsText;
    
    private Shelter shelter;
    private Inventory inventory;
    
    private void Start()
    {
        shelter = FindObjectOfType<Shelter>();
        inventory = FindObjectOfType<Inventory>();
        
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }
        
        // 订阅背包变化事件，更新材料显示
        if (inventory != null)
        {
            inventory.OnInventoryChanged += UpdateMaterialsDisplay;
        }
        
        UpdateUpgradeDisplay();
    }
    
    private void OnEnable()
    {
        UpdateUpgradeDisplay();
    }
    
    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= UpdateMaterialsDisplay;
        }
    }
    
    /// <summary>
    /// 更新升级显示信息
    /// </summary>
    public void UpdateUpgradeDisplay()
    {
        if (shelter == null) return;
        
        int currentStage = shelter.GetCurrentStage();
        int nextStage = currentStage + 1;
        
        // 检查是否可以升级
        bool canUpgrade = shelter.CanUpgradeToStage(nextStage);
        
        // 更新升级按钮状态
        if (upgradeButton != null)
        {
            upgradeButton.interactable = canUpgrade;
        }
        
        // 更新信息文本
        if (upgradeInfoText != null)
        {
            if (canUpgrade)
            {
                upgradeInfoText.text = $"当前阶段: Stage {currentStage}\n可以升级到: Stage {nextStage}";
            }
            else
            {
                upgradeInfoText.text = $"当前阶段: Stage {currentStage}\n无法升级（材料不足）";
            }
        }
        
        // 更新材料显示
        UpdateMaterialsDisplay();
    }
    
    /// <summary>
    /// 更新材料需求显示
    /// </summary>
    private void UpdateMaterialsDisplay()
    {
        if (shelter == null || inventory == null) return;
        
        int nextStage = shelter.GetCurrentStage() + 1;
        ShelterStageData nextStageData = GetStageData(nextStage);
        
        if (nextStageData == null)
        {
            if (materialsText != null)
            {
                materialsText.text = "已达到最高阶段";
            }
            return;
        }
        
        // 构建材料需求文本
        string materialsInfo = "升级所需材料:\n";
        bool allMaterialsAvailable = true;
        
        foreach (var requirement in nextStageData.requiredMaterials)
        {
            if (requirement.item == null) continue;
            
            int requiredAmount = requirement.amount;
            int currentAmount = inventory.GetCount(requirement.item);
            bool hasEnough = currentAmount >= requiredAmount;
            
            if (!hasEnough)
            {
                allMaterialsAvailable = false;
            }
            
            string status = hasEnough ? "✓" : "✗";
            materialsInfo += $"{status} {requirement.item.itemName}: {currentAmount}/{requiredAmount}\n";
        }
        
        if (materialsText != null)
        {
            materialsText.text = materialsInfo;
        }
    }
    
    /// <summary>
    /// 获取指定阶段的配置数据
    /// </summary>
    private ShelterStageData GetStageData(int stageId)
    {
        if (shelter == null) return null;
        return shelter.GetStageData(stageId);
    }
    
    /// <summary>
    /// 升级按钮点击事件
    /// </summary>
    private void OnUpgradeButtonClicked()
    {
        if (shelter == null) return;
        
        int nextStage = shelter.GetCurrentStage() + 1;
        
        if (shelter.CanUpgradeToStage(nextStage))
        {
            bool success = shelter.UpgradeToStage(nextStage);
            if (success)
            {
                Debug.Log($"[ShelterUpgradeUI] 成功升级到Stage {nextStage}");
                UpdateUpgradeDisplay();
            }
            else
            {
                Debug.LogWarning($"[ShelterUpgradeUI] 升级失败");
            }
        }
        else
        {
            Debug.LogWarning($"[ShelterUpgradeUI] 无法升级，材料不足");
        }
    }
}
