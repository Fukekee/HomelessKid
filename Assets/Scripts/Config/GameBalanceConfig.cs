using UnityEngine;

/// <summary>
/// 游戏平衡配置 - 所有关键数值的中央配置
/// 版本: V1.0 测试基准
/// </summary>
[CreateAssetMenu(fileName = "GameBalanceConfig", menuName = "Game/Balance Config")]
public class GameBalanceConfig : ScriptableObject
{
    [Header("=== 时间系统 ===")]
    [Tooltip("一天的长度（秒）- 基准: 900秒 = 15分钟")]
    public float dayLengthSeconds = 900f;
    
    [Tooltip("时间倍率（调试用）- 设为2.0可加速测试")]
    [Range(0.1f, 5f)]
    public float timeScale = 1f;
    
    [Tooltip("夜晚开始时间（小时）- 21:00开始算夜晚")]
    [Range(0, 23)]
    public int nightStartHour = 21;
    
    [Tooltip("夜晚结束时间（小时）- 06:00结束夜晚")]
    [Range(0, 23)]
    public int nightEndHour = 6;
    
    
    [Header("=== 饱食度系统 ===")]
    [Tooltip("最大饱食度")]
    public float maxHunger = 100f;
    
    [Tooltip("每秒下降量 - 基准: 0.25 (每分钟15点，约6.5分钟消耗完)")]
    public float hungerDrainPerSecond = 0.25f;
    
    [Tooltip("食物恢复量 - 基准: 40点 (约支撑2.5分钟探索)")]
    public float foodRestoreAmount = 40f;
    
    [Tooltip("低饱食阈值 - 低于此值触发'有点饿'提示")]
    public float hungerLowThreshold = 30f;
    
    [Tooltip("危险阈值 - 低于此值触发'很饿'状态（减速等）")]
    public float hungerDangerThreshold = 10f;
    
    
    [Header("=== 健康度系统 ===")]
    [Tooltip("最大健康度")]
    public float maxHealth = 100f;
    
    [Tooltip("基础自然下降（每秒）- 基准: 0 (暂不自然下降)")]
    public float healthDrainPerSecond = 0f;
    
    [Tooltip("翻桶风险概率 (0-1) - 基准: 0.15 (15%概率扣健康)")]
    [Range(0f, 1f)]
    public float binRiskChance = 0.15f;
    
    [Tooltip("翻桶风险伤害 - 基准: 10点")]
    public float binRiskDamage = 10f;
    
    [Tooltip("坏食物风险概率 (0-1) - 基准: 0 (后期加入)")]
    [Range(0f, 1f)]
    public float badFoodRiskChance = 0f;
    
    [Tooltip("坏食物风险伤害")]
    public float badFoodRiskDamage = 15f;
    
    [Tooltip("睡眠恢复量 - 基准: 30点 (一天可承受3次风险)")]
    public float sleepHealthRestore = 30f;
    
    
    [Header("=== 精神度系统 ===")]
    [Tooltip("最大精神度")]
    public float maxMood = 100f;
    
    [Tooltip("夜晚在外每小时下降量 - 基准: 20点/小时")]
    public float moodDecayAtNightOutside = 20f;
    
    [Tooltip("精神不稳阈值 - 低于此值触发精神不稳状态")]
    public float moodLowThreshold = 30f;
    
    [Tooltip("睡眠恢复量 - 基础恢复量")]
    public float moodRestoreFromSleep = 15f;
    
    
    [Header("=== 垃圾桶系统 ===")]
    [Tooltip("翻找读条时间（秒）- 基准: 2.5秒")]
    public float binSearchTime = 2.5f;
    
    [Tooltip("冷却时间（秒）- 基准: 150秒 (2.5分钟，同一桶一天最多翻2次)")]
    public float binCooldown = 150f;
    
    [Tooltip("掉落数量范围(最小值)")]
    public int binLootCountMin = 2;
    
    [Tooltip("掉落数量范围(最大值)")]
    public int binLootCountMax = 4;
    
    [Tooltip("食物掉落概率 (0-1) - 基准: 0.35 (平均每桶1个食物)")]
    [Range(0f, 1f)]
    public float binFoodDropChance = 0.35f;
    
    [Tooltip("材料掉落概率 (0-1) - 基准: 0.5")]
    [Range(0f, 1f)]
    public float binMaterialDropChance = 0.5f;
    
    [Tooltip("失物掉落概率 (0-1) - 基准: 0.05 (后期加入)")]
    [Range(0f, 1f)]
    public float binLostItemDropChance = 0.05f;
    
    
    [Header("=== 地面垃圾系统 ===")]
    [Tooltip("目标遭遇率：平均每X秒遇到一次 - 基准: 25秒")]
    public float groundTrashEncounterInterval = 25f;
    
    [Tooltip("主拾荒区密度倍率 - 基准: 1.5x")]
    [Range(0.5f, 3f)]
    public float mainAreaDensityMultiplier = 1.5f;
    
    [Tooltip("每日刷新地面垃圾数量 - 基准: 8-12个随机刷新点")]
    public int dailyGroundTrashSpawnMin = 8;
    public int dailyGroundTrashSpawnMax = 12;
    
    
    [Header("=== 世界兜底机制（温柔压力）===")]
    [Tooltip("连续低状态时的补偿食物概率 (0-1)")]
    [Range(0f, 1f)]
    public float pityFoodChance = 0.1f;
    
    [Tooltip("触发补偿的连续低饱食天数")]
    public int pityTriggerDays = 2;
    
    [Tooltip("基地恢复倍率 - 在基地休息时的额外恢复加成")]
    [Range(1f, 3f)]
    public float shelterRecoveryMultiplier = 1.5f;
    
    
    [Header("=== 调试工具 ===")]
    [Tooltip("是否显示调试信息")]
    public bool showDebugInfo = true;
    
    [Tooltip("无敌模式（测试用）")]
    public bool godMode = false;
    
    
    // ===== 计算属性（方便其他脚本使用）=====
    
    /// <summary>
    /// 获取垃圾桶掉落数量（随机）
    /// </summary>
    public int GetBinLootCount()
    {
        return Random.Range(binLootCountMin, binLootCountMax + 1);
    }
    
    /// <summary>
    /// 每分钟饱食度下降量
    /// </summary>
    public float HungerDrainPerMinute => hungerDrainPerSecond * 60f;
    
    /// <summary>
    /// 完全饿完需要多少分钟
    /// </summary>
    public float MinutesToStarve => maxHunger / HungerDrainPerMinute;
    
    /// <summary>
    /// 一份食物可以支撑多少分钟
    /// </summary>
    public float MinutesPerFood => foodRestoreAmount / HungerDrainPerMinute;
    
    /// <summary>
    /// 一天理论需要多少份食物
    /// </summary>
    public float FoodNeededPerDay => (dayLengthSeconds / 60f) / MinutesPerFood;
    
    /// <summary>
    /// 检查是否触发垃圾桶风险
    /// </summary>
    public bool RollBinRisk()
    {
        return Random.value < binRiskChance;
    }
    
    /// <summary>
    /// 检查是否触发坏食物风险
    /// </summary>
    public bool RollBadFoodRisk()
    {
        return Random.value < badFoodRiskChance;
    }
    
    
    // ===== 验证方法（在Inspector中显示警告）=====
    
    private void OnValidate()
    {
        // 确保数值合理
        if (binLootCountMin > binLootCountMax)
        {
            binLootCountMax = binLootCountMin;
        }
        
        if (dailyGroundTrashSpawnMin > dailyGroundTrashSpawnMax)
        {
            dailyGroundTrashSpawnMax = dailyGroundTrashSpawnMin;
        }
        
        // 在编辑器中显示理论数据
        #if UNITY_EDITOR
        if (showDebugInfo && Application.isPlaying == false)
        {
            // 这些数据会在Inspector中通过自定义Editor显示
            // 暂时通过Debug.Log查看
            float theoreticalFoodNeeded = FoodNeededPerDay;
            if (theoreticalFoodNeeded > 5f)
            {
                Debug.LogWarning($"[GameBalance] 一天理论需要 {theoreticalFoodNeeded:F1} 份食物，可能压力过大！");
            }
        }
        #endif
    }
}

