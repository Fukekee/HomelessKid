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
    private float currentMood;
    private PlayerMovementController movementController;
    private DayManager dayManager;
    
    // 缓存配置（性能优化）
    private GameBalanceConfig config;
    private float maxHunger;
    private float maxHealth;
    private float maxMood;
    
    // 是否在基地内（由Shelter系统设置）
    private bool isInShelter = false;
    
    public event Action<float, float, float> OnStatsChanged; // (hunger, health, mood)
    
    public float CurrentHunger => currentHunger;
    public float CurrentHealth => currentHealth;
    public float CurrentMood => currentMood;
    public float MaxHunger => maxHunger;
    public float MaxHealth => maxHealth;
    public float MaxMood => maxMood;
    public float HungerPercentage => maxHunger > 0 ? currentHunger / maxHunger : 0;
    public float HealthPercentage => maxHealth > 0 ? currentHealth / maxHealth : 0;
    public float MoodPercentage => maxMood > 0 ? currentMood / maxMood : 0;
    
    /// <summary>
    /// 设置是否在基地内（由Shelter系统调用）
    /// </summary>
    public void SetInShelter(bool inShelter)
    {
        isInShelter = inShelter;
    }
    
    private void Awake()
    {
        movementController = GetComponent<PlayerMovementController>();
        dayManager = DayManager.Instance;
        
        // 加载配置
        if (useGameBalanceConfig && GameBalance.Config != null)
        {
            config = GameBalance.Config;
            maxHunger = config.maxHunger;
            maxHealth = config.maxHealth;
            maxMood = config.maxMood;
            Debug.Log("[SurvivalStats] 已从 GameBalanceConfig 加载配置");
        }
        else
        {
            // 使用默认值作为备用
            maxHunger = 100f;
            maxHealth = 100f;
            maxMood = 100f;
            Debug.LogWarning("[SurvivalStats] 未找到 GameBalanceConfig，使用默认值");
        }
    }
    
    private void Start()
    {
        // 初始化数值
        currentHunger = maxHunger;
        currentHealth = maxHealth;
        currentMood = maxMood;
        OnStatsChanged?.Invoke(currentHunger, currentHealth, currentMood);
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
        }
        
        // Mood下降逻辑：夜晚在外时下降
        if (dayManager != null && !isInShelter)
        {
            if (dayManager.IsNightTime())
            {
                // 从配置读取夜晚下降速率（每小时下降量，转换为每秒）
                float moodDrainRate = config != null ? config.moodDecayAtNightOutside / 3600f : 20f / 3600f;
                currentMood -= moodDrainRate * Time.deltaTime;
                currentMood = Mathf.Clamp(currentMood, 0f, maxMood);
            }
        }
        
        OnStatsChanged?.Invoke(currentHunger, currentHealth, currentMood);
    }
    
    public void RestoreHunger(float amount)
    {
        currentHunger = Mathf.Clamp(currentHunger + amount, 0f, maxHunger);
        OnStatsChanged?.Invoke(currentHunger, currentHealth, currentMood);
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
        OnStatsChanged?.Invoke(currentHunger, currentHealth, currentMood);
    }
    
    public void RestoreHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        OnStatsChanged?.Invoke(currentHunger, currentHealth, currentMood);
    }
    
    public void RestoreMood(float amount)
    {
        currentMood = Mathf.Clamp(currentMood + amount, 0f, maxMood);
        OnStatsChanged?.Invoke(currentHunger, currentHealth, currentMood);
    }
    
    public void ReduceMood(float amount)
    {
        currentMood = Mathf.Clamp(currentMood - amount, 0f, maxMood);
        OnStatsChanged?.Invoke(currentHunger, currentHealth, currentMood);
    }
    
    // 过夜时恢复健康度和精神度
    public void RestoreHealthOvernight()
    {
        // 从配置读取睡眠恢复量
        float healthRestoreAmount = config != null ? config.sleepHealthRestore : 30f;
        RestoreHealth(healthRestoreAmount);
        Debug.Log($"[SurvivalStats] 睡眠恢复健康度 {healthRestoreAmount} 点");
    }
    
    /// <summary>
    /// 睡觉恢复mood（从配置读取）
    /// </summary>
    public void RestoreMoodFromSleep()
    {
        float restoreAmount = config != null ? config.moodRestoreFromSleep : 15f;
        RestoreMood(restoreAmount);
        Debug.Log($"[SurvivalStats] 睡眠恢复精神度 {restoreAmount} 点");
    }
    
    public bool IsAlive()
    {
        // 健康度不为0就视为存活（温和的失败机制）
        return currentHealth > 0;
    }
}



