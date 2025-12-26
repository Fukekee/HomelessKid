# 圆形时钟UI设置指南

## 功能说明
圆形时钟UI用于显示一天中的时间进度，使用圆形填充的方式，像真实时钟一样顺时针填充。

## Unity中的设置步骤

### 1. 创建UI结构
在Canvas下创建以下层级结构：

```
Canvas
  └─ TimeClockPanel
      ├─ ClockBackground (Image - 时钟底盘)
      └─ ClockFill (Image - 时钟填充，这个会动态变化)
```

### 2. 设置ClockBackground（时钟底盘）
1. 添加 **Image** 组件
2. 选择一个圆形精灵图片（Sprite）
   - 可以使用Unity自带的 `Knob` 或 `UI/Skin/UISprite`
   - 或者导入你自己的圆形图片
3. 设置颜色为半透明深色，比如：`RGBA(50, 50, 50, 150)`
4. 调整 RectTransform：
   - 设置宽高相等（比如 100x100）

### 3. 设置ClockFill（时钟填充）
1. 添加 **Image** 组件
2. 选择相同的圆形精灵图片
3. 设置颜色为亮色，比如：`RGBA(255, 255, 255, 200)`
4. 调整 RectTransform：
   - 设置与ClockBackground相同的大小和位置
   - 使用Anchors让它覆盖在底盘上
5. **重要**：在Inspector中将Image Type设置为 **Filled**
   - Fill Method: **Radial 360**
   - Fill Origin: **Top**
   - Fill Amount: `0` (这个会由脚本控制)
   - Clockwise: **勾选**

### 4. 添加TimeClockUI脚本
1. 选中 `TimeClockPanel` GameObject
2. 在Inspector中点击 **Add Component**
3. 搜索并添加 **TimeClockUI** 脚本
4. 设置脚本参数：
   - **Clock Fill Image**: 拖入 `ClockFill` 对象的Image组件
   - **Use Color Gradient**: 勾选（可选，让时钟颜色随时间变化）

### 5. 测试
1. 确保场景中有 **DayManager** GameObject
2. 运行游戏
3. 你应该看到圆形时钟按照时间进度顺时针填充

## 可选：自定义颜色渐变

如果想自定义一天中不同时间的颜色：

1. 在TimeClockUI脚本中勾选 `Use Color Gradient`
2. 点击 `Day Color Gradient` 展开
3. 在Gradient编辑器中设置：
   - 0%（早晨）：淡黄色
   - 25%（中午）：明亮白色
   - 50%（傍晚）：橙红色
   - 100%（夜晚）：深蓝色

## 位置建议

推荐将时钟UI放置在屏幕的：
- **右上角**：与天数显示搭配
- **左上角**：独立显示
- **屏幕中上方**：更显眼

可以通过调整TimeClockPanel的Anchors来固定位置。

## 示例布局代码参考

如果需要通过代码创建，可以参考：

```csharp
// 设置为右上角
RectTransform rt = timeClockPanel.GetComponent<RectTransform>();
rt.anchorMin = new Vector2(1, 1);
rt.anchorMax = new Vector2(1, 1);
rt.pivot = new Vector2(1, 1);
rt.anchoredPosition = new Vector2(-20, -20); // 距离右上角20像素
rt.sizeDelta = new Vector2(100, 100);
```

## 故障排除

### 问题1：时钟不动
- 检查DayManager是否存在场景中
- 检查DayManager的Update是否在调用AddTime
- 检查TimeClockUI的clockFillImage是否已连接

### 问题2：填充方向错误
- 确保Image Type是 **Filled**
- Fill Method是 **Radial 360**
- Fill Origin是 **Top**
- Clockwise要勾选

### 问题3：看不到时钟
- 检查Canvas的Render Mode
- 确保ClockFill的Image组件启用
- 检查颜色的Alpha值不为0

## 扩展功能建议

你可以在现有基础上添加：
1. **时间刻度**：在圆周上添加12个小点表示时间刻度
2. **中心文字**：显示具体时间（如"上午 / 下午"）
3. **动画效果**：使用DoTween让填充更平滑
4. **特殊时段提示**：某些时段改变边框颜色

