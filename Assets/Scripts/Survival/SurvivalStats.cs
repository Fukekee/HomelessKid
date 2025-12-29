using UnityEngine;
using System;

public class SurvivalStats : MonoBehaviour
{
    [Header("配置来源")]
    [Tooltip("所有数值从 GameBalanceConfig 读取，不再使用硬编码")]
    [SerializeField] private bool useGameBalanceConfig = true;
    
    [Header("负面影响")]
    [SerializeField] private float hungerDebuffSpeedMultiplier = 0.5f; // 饱食度为0时的移动速度倍数
    
    private float currentHunger;
    private float currentHealth;
    private PlayerMovementController movementController;
    
    // 缓存配置（性能优化）
    private GameBalanceConfig config;
    private float maxHunger;
    private float maxHealth;
    
    public event Action<float, float> OnStatsChanged; // (hunger, health)
    
    public float CurrentHunger => currentHunger;
    public float CurrentHealth => currentHealth;
    public float MaxHunger => maxHunger;
    public float MaxHealth => maxHealth;
    public float HungerPercentage => maxHunger > 0 ? currentHunger / maxHunger : 0;
    public float HealthPercentage => maxHealth > 0 ? currentHealth / maxHealth : 0;
    
    private void Awake()
    {
        movementController = GetComponent<PlayerMovementController>();
        
        // 加载配置
        if (useGameBalanceConfig && GameBalance.Config != null)
        {
            config = GameBalance.Config;
            maxHunger = config.maxHunger;
            maxHealth = config.maxHealth;
            Debug.Log("[SurvivalStats] 已从 GameBalanceConfig 加载配置");
        }
        else
        {
            // 使用默认值作为备用
            maxHunger = 100f;
            maxHealth = 100f;
            Debug.LogWarning("[SurvivalStats] 未找到 GameBalanceConfig，使用默认值");
        }
    }
    
    private void Start()
    {
        // 初始化数值
        currentHunger = maxHunger;
        currentHealth = maxHealth;
        OnStatsChanged?.Invoke(currentHunger, currentHealth);
    }
    
    private void Update()
    {
        // 从配置读取饱食度下降速率
        float hungerDrainRate = config != null ? config.hungerDrainPerSecond : 0.25f;
        
        // 饱食度随时间下降
        if (currentHunger > 0)
        {
            currentHunger -= hungerDrainRate * Time.deltaTime;
            currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);
            
            // 更新移动速度debuff
            if (currentHunger <= 0 && movementController != null)
            {
                movementController.SetHungerDebuff(hungerDebuffSpeedMultiplier);
            }
            else if (currentHunger > 0 && movementController != null)
            {
                movementController.SetHungerDebuff(1f);
            }
            
            OnStatsChanged?.Invoke(currentHunger, currentHealth);
        }
    }
    
    public void RestoreHunger(float amount)
    {
        currentHunger = Mathf.Clamp(currentHunger + amount, 0f, maxHunger);
        OnStatsChanged?.Invoke(currentHunger, currentHealth);
    }
    
    /// <summary>
    /// 吃食物恢复饱食度（使用配置的恢复量）
    /// </summary>
    public void EatFood()
    {
        float restoreAmount = config != null ? config.foodRestoreAmount : 40f;
        RestoreHunger(restoreAmount);
        Debug.Log($"[SurvivalStats] 吃了食物，恢复 {restoreAmount} 饱食度");
    }
    
    public void ReduceHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        
        // 健康度为0时不立即Game Over，但会有负面影响
        // 这里可以根据需要添加更多逻辑
        OnStatsChanged?.Invoke(currentHunger, currentHealth);
    }
    
    public void RestoreHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        OnStatsChanged?.Invoke(currentHunger, currentHealth);
    }
    
    // 过夜时恢复健康度
    public void RestoreHealthOvernight()
    {
        // 从配置读取睡眠恢复量
        float restoreAmount = config != null ? config.sleepHealthRestore : 30f;
        RestoreHealth(restoreAmount);
        Debug.Log($"[SurvivalStats] 睡眠恢复健康度 {restoreAmount} 点");
    }
    
    public bool IsAlive()
    {
        // 健康度不为0就视为存活（温和的失败机制）
        return currentHealth > 0;
    }
}



