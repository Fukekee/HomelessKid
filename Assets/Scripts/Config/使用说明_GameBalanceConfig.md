# GameBalanceConfig 使用说明

## 📦 快速开始（5分钟上手）

### 1. 创建配置资源
1. 在Project窗口中右键
2. 选择 `Create > Game > Balance Config`
3. 命名为 `GameBalanceConfig_V1`
4. 保存到 `Assets/Settings/` 或 `Assets/Config/` 文件夹

### 2. 在场景中使用
1. 在场景中找到或创建一个 `GameManager` 对象
2. 添加 `GameBalance` 组件
3. 将刚才创建的 `GameBalanceConfig_V1` 拖入 `config` 字段
4. 完成！

### 3. 在脚本中访问配置
```csharp
// 方式1：推荐使用（通过单例）
float dayLength = GameBalance.Config.dayLengthSeconds;
float hungerDrain = GameBalance.Config.hungerDrainPerSecond;

// 方式2：使用计算属性
float foodNeeded = GameBalance.Config.FoodNeededPerDay;
bool hasRisk = GameBalance.Config.RollBinRisk();

// 方式3：直接引用（不推荐，仅用于特殊情况）
[SerializeField] private GameBalanceConfig config;
```

---

## 🎛️ 配置字段说明

### 时间系统
- **dayLengthSeconds**: 一天的长度（秒）
  - 基准值: `600` (10分钟)
  - 建议范围: 480-720秒
- **timeScale**: 时间倍率（调试用）
  - 设为 `2.0` 可加速测试

### 饱食度系统
- **maxHunger**: 最大饱食度 (`100`)
- **hungerDrainPerSecond**: 每秒下降量 (`0.25`)
  - 太松: `0.2` → 每分钟12点
  - 标准: `0.25` → 每分钟15点
  - 太紧: `0.3` → 每分钟18点
- **foodRestoreAmount**: 食物恢复量 (`40`)
- **hungerLowThreshold**: 低饱食阈值 (`30`)
- **hungerDangerThreshold**: 危险阈值 (`10`)

### 健康度系统
- **maxHealth**: 最大健康度 (`100`)
- **healthDrainPerSecond**: 基础自然下降 (`0`)
- **binRiskChance**: 翻桶风险概率 (`0.15` = 15%)
- **binRiskDamage**: 翻桶风险伤害 (`-10`)
- **sleepHealthRestore**: 睡眠恢复量 (`30`)

### 垃圾桶系统
- **binSearchTime**: 翻找读条时间 (`2.5`秒)
- **binCooldown**: 冷却时间 (`150`秒 = 2.5分钟)
- **binLootCountMin/Max**: 掉落数量范围 (`2-4`个)
- **binFoodDropChance**: 食物掉落概率 (`0.35` = 35%)

### 地面垃圾系统
- **groundTrashEncounterInterval**: 目标遭遇率 (`25`秒/次)
- **mainAreaDensityMultiplier**: 主拾荒区密度倍率 (`1.5x`)
- 每日可刷新物品的**生成数量**已在各区域的 TrashSpawner → SpawnZoneData 的 **Min Spawn / Max Spawn** 中设定，不再由此配置控制。

---

## 💡 实用计算属性

配置提供了便捷的计算属性，无需手动计算：

```csharp
// 每分钟饱食度下降量
float perMinute = GameBalance.Config.HungerDrainPerMinute; // 15.0

// 完全饿完需要多少分钟
float timeToStarve = GameBalance.Config.MinutesToStarve; // 6.67

// 一份食物可以支撑多少分钟
float foodDuration = GameBalance.Config.MinutesPerFood; // 2.67

// 一天理论需要多少份食物
float foodNeeded = GameBalance.Config.FoodNeededPerDay; // 3.75

// 获取随机垃圾桶掉落数量
int lootCount = GameBalance.Config.GetBinLootCount(); // 2-4随机

// 检查是否触发风险
if (GameBalance.Config.RollBinRisk())
{
    // 触发风险事件
}
```

---

## 🎨 Inspector增强功能

在Inspector中查看配置时，会自动显示：

### 理论数据计算
- 一天长度（分钟）
- 每分钟饱食度下降
- 完全饿完需要（分钟）
- 一份食物支撑（分钟）
- **理论每天需要食物（份）** ← 最重要！

### 自动警告
- ⚠️ 食物需求过高（> 5份/天）
- ℹ️ 食物需求过低（< 2份/天）
- ⛔ 玩家可能在一天内被饿死

### 快速预设按钮
- **轻松模式**: 压力更小，适合新手体验
- **标准模式**: 基准数值，推荐测试
- **挑战模式**: 压力更大，适合后期调整

---

## 🔧 使用示例

### 示例1：在SurvivalStats中使用

```csharp
using UnityEngine;

public class SurvivalStats : MonoBehaviour
{
    private float currentHunger = 100f;
    private float currentHealth = 100f;
    
    private void Start()
    {
        // 初始化为最大值
        currentHunger = GameBalance.Config.maxHunger;
        currentHealth = GameBalance.Config.maxHealth;
    }
    
    private void Update()
    {
        // 使用配置的下降速度
        currentHunger -= GameBalance.Config.hungerDrainPerSecond * Time.deltaTime;
        currentHealth -= GameBalance.Config.healthDrainPerSecond * Time.deltaTime;
        
        // 检查阈值
        if (currentHunger < GameBalance.Config.hungerDangerThreshold)
        {
            // 触发危险状态
        }
    }
    
    public void EatFood()
    {
        // 使用配置的恢复量
        currentHunger += GameBalance.Config.foodRestoreAmount;
        currentHunger = Mathf.Min(currentHunger, GameBalance.Config.maxHunger);
    }
}
```

### 示例2：在TrashBin中使用

```csharp
using UnityEngine;

public class TrashBin : MonoBehaviour, IInteractable
{
    private float lastSearchTime = -999f;
    
    public void OnInteract()
    {
        // 检查冷却
        if (Time.time - lastSearchTime < GameBalance.Config.binCooldown)
        {
            Debug.Log("垃圾桶冷却中...");
            return;
        }
        
        // 开始翻找（使用配置的读条时间）
        StartCoroutine(SearchCoroutine());
    }
    
    private IEnumerator SearchCoroutine()
    {
        yield return new WaitForSeconds(GameBalance.Config.binSearchTime);
        
        // 检查风险
        if (GameBalance.Config.RollBinRisk())
        {
            // 扣减健康（使用配置的伤害值）
            // playerHealth -= GameBalance.Config.binRiskDamage;
        }
        
        // 生成掉落（使用配置的数量和概率）
        int lootCount = GameBalance.Config.GetBinLootCount();
        for (int i = 0; i < lootCount; i++)
        {
            if (Random.value < GameBalance.Config.binFoodDropChance)
            {
                // 掉落食物
            }
            else if (Random.value < GameBalance.Config.binMaterialDropChance)
            {
                // 掉落材料
            }
        }
        
        lastSearchTime = Time.time;
    }
}
```

### 示例3：在DayManager中使用

```csharp
using UnityEngine;

public class DayManager : MonoBehaviour
{
    private float dayTimer = 0f;
    
    private void Update()
    {
        // 使用配置的时间倍率和一天长度
        dayTimer += Time.deltaTime * GameBalance.Config.timeScale;
        
        if (dayTimer >= GameBalance.Config.dayLengthSeconds)
        {
            // 进入新一天
            NewDay();
        }
    }
}
```

---

## 🎯 调试技巧

### 1. 显示配置摘要
在场景中选中 `GameBalance` 组件，右键选择 `显示配置摘要`

### 2. 开启调试模式
在配置中勾选 `showDebugInfo`，会在Console显示警告

### 3. 使用无敌模式测试
勾选 `godMode`，在代码中检查：
```csharp
if (!GameBalance.Config.godMode)
{
    // 扣减健康/饱食度
}
```

### 4. 加速时间测试
将 `timeScale` 改为 `2.0` 或 `3.0`，快速测试多天循环

---

## ⚠️ 注意事项

1. **必须在场景中添加GameBalance组件**，否则会报错
2. **配置是ScriptableObject**，修改会影响所有场景
3. **运行时修改不会保存**，只在编辑器模式修改才会保存
4. **建议为不同测试创建多个配置文件**：
   - `GameBalanceConfig_V1_轻松`
   - `GameBalanceConfig_V1_标准`
   - `GameBalanceConfig_V1_挑战`

---

## 🔄 版本记录

### V1.0 (2025-12-25)
- 初始版本
- 基于"50秒单程、10分钟一天"设计
- 包含时间、饱食、健康、垃圾桶、地面垃圾系统
- 提供理论计算和自动警告

---

## 📞 常见问题

**Q: 为什么我的代码访问 GameBalance.Config 报空？**  
A: 确保场景中有 `GameBalance` 组件，并且 `config` 字段已配置。

**Q: 如何在不同场景使用不同配置？**  
A: 在每个场景的 `GameBalance` 组件上拖入不同的配置文件。

**Q: 配置修改后为什么没生效？**  
A: 检查是否在运行时修改（运行时修改不保存）。停止运行后再修改。

**Q: 能否在运行时动态修改配置？**  
A: 可以，但不会保存。如果需要保存，建议使用 PlayerPrefs 或 SaveSystem。

