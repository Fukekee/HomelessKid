using UnityEngine;
using System;

public class SurvivalStats : MonoBehaviour
{
    [Header("饱食度设置")]
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float hungerDecayRate = 5f; // 每秒下降的饱食度
    
    [Header("健康度设置")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthRestoreRate = 10f; // 过夜时每秒恢复的健康度
    
    [Header("负面影响")]
    [SerializeField] private float hungerDebuffSpeedMultiplier = 0.5f; // 饱食度为0时的移动速度倍数
    
    private float currentHunger;
    private float currentHealth;
    private PlayerMovementController movementController;
    
    public event Action<float, float> OnStatsChanged; // (hunger, health)
    
    public float CurrentHunger => currentHunger;
    public float CurrentHealth => currentHealth;
    public float MaxHunger => maxHunger;
    public float MaxHealth => maxHealth;
    public float HungerPercentage => currentHunger / maxHunger;
    public float HealthPercentage => currentHealth / maxHealth;
    
    private void Awake()
    {
        movementController = GetComponent<PlayerMovementController>();
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
        // 饱食度随时间下降
        if (currentHunger > 0)
        {
            currentHunger -= hungerDecayRate * Time.deltaTime;
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
        // 每次过夜恢复50%健康度
        RestoreHealth(maxHealth * 0.5f);
    }
    
    public bool IsAlive()
    {
        // 健康度不为0就视为存活（温和的失败机制）
        return currentHealth > 0;
    }
}


