# DayNightLightController 使用说明

## 功能概述

`DayNightLightController` 会根据游戏内时间自动调整场景中Directional Light的亮度和颜色，实现昼夜循环效果。

---

## 快速设置（3步）

### 步骤1：添加组件

1. 在场景中找到或创建一个GameObject（建议命名为 `DayNightLightController`）
2. 添加 `DayNightLightController` 组件

### 步骤2：配置Directional Light引用

**方式A：自动查找（推荐）**
- 保持 `Directional Light` 字段为空
- 脚本会自动查找场景中的Directional Light

**方式B：手动指定**
- 将场景中的 `Directional Light` GameObject拖入 `Directional Light` 字段

### 步骤3：调整参数（可选）

在Inspector中调整光照参数：
- **Night Intensity**: 夜晚最低亮度（默认0.3）
- **Day Intensity**: 白天最高亮度（默认1.5）
- **时段配置**: 日出/日落时间（默认6:00-8:00日出，18:00-20:00日落）

---

## 参数说明

### 光照强度配置

| 参数 | 默认值 | 说明 |
|------|--------|------|
| **Night Intensity** | 0.3 | 夜晚最低亮度（20:00-6:00） |
| **Day Intensity** | 1.5 | 白天最高亮度（8:00-18:00） |

### 时段配置

| 参数 | 默认值 | 说明 |
|------|--------|------|
| **Sunrise Start Hour** | 6 | 日出开始时间（小时） |
| **Sunrise End Hour** | 8 | 日出结束时间（小时） |
| **Sunset Start Hour** | 18 | 日落开始时间（小时） |
| **Sunset End Hour** | 20 | 日落结束时间（小时） |

### 颜色配置（可选）

| 参数 | 默认值 | 说明 |
|------|--------|------|
| **Adjust Color** | true | 是否根据时间调整光照颜色 |
| **Night Color** | (0.4, 0.5, 0.7) | 夜晚光照颜色（偏蓝） |
| **Day Color** | (1.0, 0.95, 0.9) | 白天光照颜色（偏白/暖） |

---

## 光照强度曲线

```
强度
1.5 |                    ╱───────╲
    |                   ╱         ╲
1.0 |                  ╱           ╲
    |                 ╱               ╲
0.5 |                ╱                 ╲
    |               ╱                   ╲
0.3 |──────────────╱                     ╲──────────────
    |            ╱                         ╲
0.0 |───────────┴─────────────────────────┴───────────
    0  6  8             18  20  24  0  6
    日出  白天           日落  夜晚  日出
```

**时段说明**：
- **夜晚（20:00-6:00）**: 最低亮度（nightIntensity）
- **日出（6:00-8:00）**: 从暗到亮，线性过渡
- **白天（8:00-18:00）**: 最高亮度（dayIntensity）
- **日落（18:00-20:00）**: 从亮到暗，线性过渡

---

## 使用示例

### 示例1：基础设置（默认参数）

1. 添加组件到场景
2. 保持所有参数为默认值
3. 运行游戏，光照会自动根据时间变化

### 示例2：更暗的夜晚

- **Night Intensity**: `0.1`（更暗）
- **Day Intensity**: `1.5`（保持白天明亮）

### 示例3：更长的日出/日落

- **Sunrise Start Hour**: `5`（更早开始日出）
- **Sunrise End Hour**: `9`（更晚结束日出，过渡更平滑）
- **Sunset Start Hour**: `17`（更早开始日落）
- **Sunset End Hour**: `21`（更晚结束日落）

### 示例4：禁用颜色调整

- **Adjust Color**: `false`
- 只调整亮度，不改变颜色

---

## 调试技巧

### 在编辑器中预览

1. 选中 `DayNightLightController` GameObject
2. 右键点击组件
3. 选择 `预览当前时间的光照`
4. 查看Console输出的光照参数

### 快速测试不同时间

1. 在 `DayManager` 中调整 `currentDayTime`
2. 或使用 `GameBalanceConfig` 的 `timeScale` 加速时间
3. 观察光照变化

---

## 注意事项

1. **确保DayManager存在**
   - 脚本依赖 `DayManager.Instance`
   - 如果场景中没有DayManager，会报错

2. **Directional Light类型**
   - 脚本只控制 `LightType.Directional` 类型的光源
   - 如果场景中有多个Directional Light，会自动使用第一个找到的

3. **性能考虑**
   - 每帧更新光照（Update中）
   - 如果性能有问题，可以改为按固定间隔更新（如每0.1秒）

4. **与其他光照系统兼容**
   - 如果使用其他光照系统（如Post-Processing），可能需要调整参数
   - 建议先测试基础效果，再集成其他系统

---

## 常见问题

**Q: 光照没有变化？**
- 检查DayManager是否存在且正常工作
- 检查Directional Light引用是否正确
- 查看Console是否有错误信息

**Q: 光照变化太快/太慢？**
- 调整 `timeScale` 来改变时间流速
- 或调整日出/日落时段长度

**Q: 想要更平滑的过渡？**
- 延长日出/日落时段（例如6:00-9:00）
- 或修改代码使用曲线插值（AnimationCurve）

**Q: 可以控制多个光源吗？**
- 当前版本只控制一个Directional Light
- 如需控制多个，可以修改代码添加光源列表

---

## 扩展建议

如果需要更复杂的光照效果，可以考虑：

1. **使用AnimationCurve**：更灵活的光照强度曲线
2. **控制太阳角度**：根据时间旋转Directional Light的Transform
3. **环境光调整**：同时调整Ambient Light
4. **天空盒切换**：根据时间切换不同的天空盒
5. **后处理效果**：结合Post-Processing实现更真实的昼夜效果

---

**创建时间**: 2025-12-25  
**版本**: 1.0
