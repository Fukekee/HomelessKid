using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 圆形时钟UI - 显示当前一天的时间进度
/// 使用Image的Filled方式，360度填充表示时间流逝
/// </summary>
public class TimeClockUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image clockFillImage; // 时钟填充图片
    
    [Header("可选：颜色变化")]
    [SerializeField] private bool useColorGradient = true; // 是否使用颜色渐变
    [SerializeField] private Gradient dayColorGradient; // 一天中颜色的渐变（从早到晚）
    
    private void Start()
    {
        // 初始化时钟填充图片
        if (clockFillImage != null)
        {
            clockFillImage.type = Image.Type.Filled;
            clockFillImage.fillMethod = Image.FillMethod.Radial360;
            clockFillImage.fillOrigin = (int)Image.Origin360.Top; // 从顶部开始（12点钟方向）
            clockFillImage.fillClockwise = true; // 顺时针填充
        }
        else
        {
            Debug.LogError("TimeClockUI: clockFillImage 未设置!");
        }
        
        // 初始化默认渐变（如果没有设置）
        if (useColorGradient && dayColorGradient == null)
        {
            InitializeDefaultGradient();
        }
    }
    
    private void Update()
    {
        UpdateClock();
    }
    
    /// <summary>
    /// 更新时钟显示
    /// </summary>
    private void UpdateClock()
    {
        if (DayManager.Instance == null || clockFillImage == null)
            return;
        
        // 获取当前一天的进度 (0-1)
        float progress = DayManager.Instance.DayProgress;
        
        // 更新填充量
        clockFillImage.fillAmount = progress;
        
        // 更新颜色（可选）
        if (useColorGradient && dayColorGradient != null)
        {
            clockFillImage.color = dayColorGradient.Evaluate(progress);
        }
    }
    
    /// <summary>
    /// 初始化默认颜色渐变
    /// 早晨（淡黄）→ 中午（明亮）→ 傍晚（橙红）→ 夜晚（深蓝）
    /// </summary>
    private void InitializeDefaultGradient()
    {
        dayColorGradient = new Gradient();
        
        GradientColorKey[] colorKeys = new GradientColorKey[4];
        colorKeys[0] = new GradientColorKey(new Color(1f, 0.92f, 0.6f), 0f);      // 早晨 - 淡黄
        colorKeys[1] = new GradientColorKey(new Color(1f, 1f, 0.8f), 0.25f);      // 中午 - 明亮
        colorKeys[2] = new GradientColorKey(new Color(1f, 0.6f, 0.4f), 0.5f);     // 傍晚 - 橙红
        colorKeys[3] = new GradientColorKey(new Color(0.3f, 0.4f, 0.7f), 1f);     // 夜晚 - 深蓝
        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1f, 0f);
        alphaKeys[1] = new GradientAlphaKey(1f, 1f);
        
        dayColorGradient.SetKeys(colorKeys, alphaKeys);
    }
}

