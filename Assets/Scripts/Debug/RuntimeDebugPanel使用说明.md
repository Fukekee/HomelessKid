# RuntimeDebugPanel 使用说明

## 📋 功能概述

运行时调试面板是一个强大的工具，允许您在游戏运行时实时查看和调整游戏平衡配置，无需停止游戏或重新加载场景。

## 🎮 快速使用

### 打开/关闭面板
- 按 **F1** 键切换调试面板的显示/隐藏

### 调整参数
1. 使用滑块（Slider）快速调整数值
2. 或直接在输入框（InputField）中输入精确数值
3. 修改会立即生效，影响游戏系统

### 快速预设
点击底部的预设按钮快速切换难度：
- **轻松模式**: 饱食度下降慢，食物恢复多，风险低
- **标准模式**: 平衡的游戏体验（V1.0 基准）
- **困难模式**: 饱食度下降快，食物恢复少，风险高

## 📊 面板内容

### 时间系统
- **一天长度**: 调整一天的持续时间（秒）
- **时间倍率**: 加速/减速游戏时间流逝

### 饱食度系统
- **饱食下降速率**: 每秒下降的饱食度
- **食物恢复量**: 吃一份食物恢复的饱食度

### 健康度系统
- **翻桶风险概率**: 翻垃圾桶时受伤的概率（0-1）
- **翻桶风险伤害**: 受伤时损失的健康度
- **睡眠恢复量**: 睡觉时恢复的健康度

### 垃圾桶系统
- **翻桶读条时间**: 翻找垃圾桶需要的时间（秒）
- **翻桶冷却时间**: 同一个垃圾桶的冷却时间（秒）

### 地面垃圾系统
- **每日生成最小值**: 每天生成地面垃圾的最小数量
- **每日生成最大值**: 每天生成地面垃圾的最大数量

### 理论数据显示
面板右侧实时显示根据当前配置计算的理论数据：
- 一天长度（分钟）
- 每分钟饱食度下降
- 完全饿完需要的时间
- 一份食物支撑的时间
- 理论每天需要的食物数量
- 垃圾桶相关数据
- **压力评估**: 根据食物需求自动评估游戏压力等级

## 🎯 使用场景

### 测试不同难度
1. 打开调试面板（F1）
2. 点击"轻松模式"预设
3. 玩几分钟感受体验
4. 切换到"困难模式"对比

### 微调特定参数
1. 打开调试面板
2. 找到想调整的参数（如饱食下降速率）
3. 拖动滑块或输入数值
4. 观察右侧理论数据变化
5. 在游戏中验证效果

### 快速迭代测试
1. 使用时间倍率（2x-5x）加速游戏
2. 快速体验多天循环
3. 找到合适的数值后记录
4. 在配置文件中永久保存

## ⚠️ 重要提示

### 修改不会保存
- 运行时的修改**仅在当前游戏会话有效**
- 停止游戏后，所有修改会丢失
- 如果找到满意的配置，请手动记录数值
- 然后在 Unity Editor 中修改 GameBalanceConfig 资产文件

### 如何永久保存配置
1. 在调试面板中调整到满意的数值
2. 记录所有修改的参数
3. 停止游戏
4. 在 Project 窗口找到 `Assets/Resource/GameBalanceConfig_V1_测试.asset`
5. 在 Inspector 中手动输入记录的数值
6. 保存项目

### 性能影响
- 调试面板在隐藏时几乎没有性能影响
- 显示时每帧更新理论数据，性能消耗很小
- 可以在最终发布版本中移除此脚本

## 🔧 设置步骤

### 1. 创建调试面板 UI
由于 Unity Prefab 需要在编辑器中手动创建，请按以下步骤操作：

1. 在场景中创建 Canvas（如果没有）
2. 在 Canvas 下创建一个 Panel，命名为 `RuntimeDebugPanel`
3. 添加 `RuntimeDebugPanel.cs` 脚本到 Panel 上
4. 按照脚本中的字段创建对应的 UI 元素：
   - 各种 Slider 和 InputField
   - 理论数据显示的 TextMeshProUGUI
   - 三个预设按钮
5. 将 UI 元素拖拽到脚本的对应字段

### 2. 快速创建方案
如果您希望快速测试，可以：
1. 只创建基础的 Panel 和 Text
2. 在脚本中注释掉 UI 相关的代码
3. 使用 Debug.Log 输出调试信息
4. 后续再完善 UI

## 📝 代码集成

### 在其他脚本中使用
调试面板修改的是 `GameBalanceConfig` 实例，所有已集成的系统会自动响应：

```csharp
// 所有使用 GameBalance.Config 的系统都会自动使用新数值
float hungerDrain = GameBalance.Config.hungerDrainPerSecond;
```

### 扩展调试面板
如果您添加了新的配置参数：

1. 在 `RuntimeDebugPanel.cs` 中添加对应的 Slider 和 InputField 字段
2. 在 `InitializeUI()` 中添加初始化代码
3. 在 `RefreshAllValues()` 中添加刷新代码
4. 如果需要，在 `UpdateTheoryData()` 中添加显示逻辑

## 🎨 UI 布局建议

### 推荐布局
```
RuntimeDebugPanel (Panel)
├── Header (Text: "游戏平衡调试面板 [F1]")
├── LeftPanel (垂直布局)
│   ├── TimeGroup (时间系统参数)
│   ├── HungerGroup (饱食度系统参数)
│   ├── HealthGroup (健康度系统参数)
│   ├── BinGroup (垃圾桶系统参数)
│   └── TrashGroup (地面垃圾系统参数)
├── RightPanel (理论数据显示)
│   └── TheoryDataText (TextMeshProUGUI)
└── BottomPanel (预设按钮)
    ├── EasyButton
    ├── NormalButton
    └── HardButton
```

### 每个参数组的结构
```
ParameterGroup (垂直布局)
├── GroupTitle (Text)
├── Parameter1
│   ├── Label (Text)
│   ├── Slider
│   └── InputField
├── Parameter2
│   ├── Label (Text)
│   ├── Slider
│   └── InputField
...
```

## 🐛 故障排除

### 面板不显示
- 检查 `panelRoot` 是否正确赋值
- 确认 Canvas 存在且激活
- 查看 Console 是否有错误信息

### 参数修改不生效
- 确认 GameBalance 组件在场景中
- 检查 GameBalanceConfig 资产是否正确配置
- 查看各系统脚本是否正确集成配置

### 理论数据不更新
- 检查 `theoryDataText` 是否赋值
- 确认 TextMeshProUGUI 组件存在
- 查看 Console 是否有空引用错误

## 📚 相关文档

- `GameBalanceConfig.cs` - 配置数据结构
- `GameBalance.cs` - 配置访问器
- `游戏平衡配置_V1.0_测试基准.md` - 数值设计文档

---

**提示**: 这是一个开发工具，建议在最终发布时移除或禁用。

