# GameBalanceConfig 系统文档

## 📦 系统概述

这是一套完整的游戏平衡配置系统，用于集中管理所有关键数值，支持可视化调整和实时预览。

**核心特性:**
- ✅ 所有数值集中管理
- ✅ 可视化Inspector显示
- ✅ 理论数据自动计算
- ✅ 自动警告不合理配置
- ✅ 快速预设切换
- ✅ 全局单例访问
- ✅ 支持运行时调试

---

## 📁 文件结构

```
Assets/Scripts/Config/
├── GameBalanceConfig.cs          # 核心配置ScriptableObject
├── GameBalance.cs                 # 全局访问器（单例）
├── Editor/
│   └── GameBalanceConfigEditor.cs # Inspector增强显示
├── README.md                      # 本文档
├── 快速上手步骤.md                 # 5分钟上手指南 ⭐
└── 使用说明_GameBalanceConfig.md   # 详细使用文档

Assets/
└── 游戏平衡配置_V1.0_测试基准.md     # 数值设计文档
```

---

## 🎯 各文件作用

### 1. GameBalanceConfig.cs
**类型**: ScriptableObject  
**作用**: 存储所有平衡数值的核心配置类

**包含系统:**
- ⏱️ 时间系统（一天长度、时间倍率）
- 🍞 饱食度系统（下降速度、恢复量、阈值）
- 💚 健康度系统（风险概率、伤害值、恢复量）
- 🪣 垃圾桶系统（读条时间、冷却、掉落）
- 🗑️ 地面垃圾系统（密度、刷新）
- 🛡️ 兜底机制（补偿系统）
- 🐛 调试工具

**计算属性:**
- `HungerDrainPerMinute` - 每分钟饱食度下降
- `MinutesToStarve` - 完全饿完需要时间
- `MinutesPerFood` - 一份食物支撑时间
- `FoodNeededPerDay` - 每天理论需要食物
- `GetBinLootCount()` - 随机垃圾桶掉落数量
- `RollBinRisk()` - 检查是否触发风险

---

### 2. GameBalance.cs
**类型**: MonoBehaviour 单例  
**作用**: 提供全局访问入口

**使用方法:**
```csharp
// 访问配置
float dayLength = GameBalance.Config.dayLengthSeconds;
bool hasRisk = GameBalance.Config.RollBinRisk();
```

**功能:**
- 单例模式，场景持久化
- 自动查找和缓存配置
- 提供配置摘要输出（右键菜单）

---

### 3. GameBalanceConfigEditor.cs
**类型**: UnityEditor 自定义Inspector  
**作用**: 增强配置文件的Inspector显示

**显示内容:**
- 📊 理论数据计算（自动）
- ⚠️ 警告提示（压力过大/过小）
- 🎨 快速预设按钮（轻松/标准/挑战）
- 🔢 垃圾桶理论产出

---

### 4. 快速上手步骤.md ⭐
**推荐阅读**: 新手必读  
**内容**: 5分钟完成系统搭建的详细步骤

**包括:**
- 如何创建配置资源
- 如何在场景中使用
- 如何在脚本中访问
- 如何测试和调整
- 常见问题解决

---

### 5. 使用说明_GameBalanceConfig.md
**推荐阅读**: 开发时查阅  
**内容**: 详细的API文档和使用示例

**包括:**
- 所有字段详细说明
- 计算属性用法
- 完整代码示例
- 调试技巧
- 常见问题

---

### 6. 游戏平衡配置_V1.0_测试基准.md
**推荐阅读**: 调数值时参考  
**内容**: 数值设计理念和测试标准

**包括:**
- 基准数值表
- 设计意图说明
- 测试验证目标
- 理论数据计算
- 调整建议（太松/太紧怎么办）
- 测试记录模板

---

## 🚀 快速开始（3步）

### 1️⃣ 创建配置资源
```
右键 > Create > Game > Balance Config
命名: GameBalanceConfig_V1_测试
```

### 2️⃣ 添加到场景
```
选择GameManager > Add Component > GameBalance
拖入配置文件到config字段
```

### 3️⃣ 在代码中使用
```csharp
float hungerDrain = GameBalance.Config.hungerDrainPerSecond;
```

**详细步骤请看**: `快速上手步骤.md`

---

## 📚 推荐阅读顺序

### 新手第一次使用
1. ⭐ `快速上手步骤.md` - 先搭建起来
2. 运行测试一次
3. `游戏平衡配置_V1.0_测试基准.md` - 了解数值设计

### 开发中需要调整
1. `使用说明_GameBalanceConfig.md` - 查API
2. `游戏平衡配置_V1.0_测试基准.md` - 看调整建议

### 深度定制
1. 阅读 `GameBalanceConfig.cs` 源码
2. 修改或扩展配置字段
3. 更新 `GameBalanceConfigEditor.cs` 显示

---

## 🎛️ 核心配置字段速查

| 字段 | 默认值 | 说明 |
|------|--------|------|
| dayLengthSeconds | 600 | 一天长度（秒） |
| hungerDrainPerSecond | 0.25 | 饱食下降速度 |
| foodRestoreAmount | 40 | 食物恢复量 |
| binSearchTime | 2.5 | 翻桶读条时间 |
| binCooldown | 150 | 翻桶冷却时间 |
| binRiskChance | 0.15 | 翻桶风险概率 |

**完整字段请看**: `使用说明_GameBalanceConfig.md`

---

## 🔧 常用操作

### 查看配置摘要
```
选中场景中的GameBalance组件
右键 > 显示配置摘要
查看Console输出
```

### 快速切换难度
```
选中配置文件
Inspector底部点击预设按钮：
- 轻松模式
- 标准模式
- 挑战模式
```

### 加速测试
```
将 timeScale 改成 2.0
快速测试多天循环
```

---

## ⚠️ 重要提示

1. **必须在场景中添加GameBalance组件**
   - 否则 `GameBalance.Config` 会报空

2. **配置修改要在非运行时**
   - 运行时修改不会保存
   - 停止运行后再修改

3. **建议创建多个配置文件**
   - 轻松、标准、挑战各一个
   - 方便测试不同难度

4. **关注Inspector的警告**
   - 黄色警告 = 压力可能不合适
   - 红色警告 = 配置严重不合理

---

## 📊 理论数据示例

使用默认配置（V1.0基准）：

```
一天长度: 10.0 分钟
每分钟饱食度下降: 15.0 点
完全饿完需要: 6.7 分钟
一份食物支撑: 2.7 分钟
理论每天需要食物: 3.75 份

同一桶一天可翻: 4.0 次
每桶平均掉落: 3.0 个
每桶平均食物: 1.05 个
```

**解读**: 压力适中，玩家需要规划但不会崩盘

---

## 🆘 常见问题

**Q: 为什么访问Config报空？**  
A: 检查场景中是否有GameBalance组件，config字段是否配置

**Q: 如何在不同场景用不同配置？**  
A: 在每个场景的GameBalance上拖入不同配置文件

**Q: 能否运行时动态修改？**  
A: 可以，但不会保存。需要保存请用PlayerPrefs

**Q: 如何扩展新字段？**  
A: 在GameBalanceConfig.cs中添加，建议也更新Editor显示

---

## 📞 技术支持

遇到问题？检查：
1. Console是否有错误信息
2. 是否按照"快速上手步骤"操作
3. 配置文件是否正确创建

---

## 🔄 版本信息

**当前版本**: V1.0  
**创建日期**: 2025-12-25  
**基准约束**: 基地→拾荒区单程50秒  
**目标体验**: 温柔压力、有选择、不崩盘

---

**祝开发顺利！🎮**

