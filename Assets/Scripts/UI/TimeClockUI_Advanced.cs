using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 增强版圆形时钟UI - 带时间刻度和中心文字显示
/// 可选功能，比基础版TimeClockUI更完整
/// </summary>
public class TimeClockUI_Advanced : MonoBehaviour
{
    [Header("基础UI组件")]
    [SerializeField] private Image clockFillImage; // 时钟填充图片
    [SerializeField] private Text centerTimeText; // 中心时间文字（可选）
    
    [Header("时间刻度")]
    [SerializeField] private bool showTimeMarkers = true; // 是否显示时间刻度
    [SerializeField] private GameObject timeMarkerPrefab; // 时间刻度预制体（小圆点或线）
    [SerializeField] private Transform markersContainer; // 刻度容器
    [SerializeField] private float markerDistance = 45f; // 刻度距离中心的距离
    [SerializeField] private int markerCount = 12; // 刻度数量（默认12个）
    
    [Header("颜色设置")]
    [SerializeField] private bool useColorGradient = true;
    [SerializeField] private Gradient dayColorGradient;
    
    [Header("时间文字显示")]
    [SerializeField] private bool showTimeText = true;
    [SerializeField] private string morningText = "清晨";
    [SerializeField] private string noonText = "正午";
    [SerializeField] private string afternoonText = "下午";
    [SerializeField] private string eveningText = "傍晚";
    [SerializeField] private string nightText = "深夜";
    
    private void Start()
    {
        InitializeClockFill();
        InitializeDefaultGradient();
        CreateTimeMarkers();
    }
    
    private void Update()
    {
        UpdateClock();
    }
    
    /// <summary>
    /// 初始化时钟填充图片
    /// </summary>
    private void InitializeClockFill()
    {
        if (clockFillImage != null)
        {
            clockFillImage.type = Image.Type.Filled;
            clockFillImage.fillMethod = Image.FillMethod.Radial360;
            clockFillImage.fillOrigin = (int)Image.Origin360.Top;
            clockFillImage.fillClockwise = true;
        }
        else
        {
            Debug.LogError("TimeClockUI_Advanced: clockFillImage 未设置!");
        }
    }
    
    /// <summary>
    /// 创建时间刻度
    /// </summary>
    private void CreateTimeMarkers()
    {
        if (!showTimeMarkers || timeMarkerPrefab == null || markersContainer == null)
            return;
        
        // 清除旧的刻度
        foreach (Transform child in markersContainer)
        {
            Destroy(child.gameObject);
        }
        
        // 创建新的刻度
        for (int i = 0; i < markerCount; i++)
        {
            float angle = (360f / markerCount) * i;
            float radian = (angle - 90f) * Mathf.Deg2Rad; // 减90度使第一个刻度在顶部
            
            GameObject marker = Instantiate(timeMarkerPrefab, markersContainer);
            RectTransform markerRect = marker.GetComponent<RectTransform>();
            
            if (markerRect != null)
            {
                // 计算刻度位置
                float x = Mathf.Cos(radian) * markerDistance;
                float y = Mathf.Sin(radian) * markerDistance;
                markerRect.anchoredPosition = new Vector2(x, y);
            }
        }
    }
    
    /// <summary>
    /// 更新时钟显示
    /// </summary>
    private void UpdateClock()
    {
        if (DayManager.Instance == null || clockFillImage == null)
            return;
        
        float progress = DayManager.Instance.DayProgress;
        
        // 更新填充量
        clockFillImage.fillAmount = progress;
        
        // 更新颜色
        if (useColorGradient && dayColorGradient != null)
        {
            clockFillImage.color = dayColorGradient.Evaluate(progress);
        }
        
        // 更新中心文字
        if (showTimeText && centerTimeText != null)
        {
            centerTimeText.text = GetTimeOfDayText(progress);
        }
    }
    
    /// <summary>
    /// 根据进度获取时间段文字
    /// </summary>
    private string GetTimeOfDayText(float progress)
    {
        if (progress < 0.2f)
            return morningText;
        else if (progress < 0.4f)
            return noonText;
        else if (progress < 0.6f)
            return afternoonText;
        else if (progress < 0.8f)
            return eveningText;
        else
            return nightText;
    }
    
    /// <summary>
    /// 初始化默认颜色渐变
    /// </summary>
    private void InitializeDefaultGradient()
    {
        if (!useColorGradient || dayColorGradient != null)
            return;
        
        dayColorGradient = new Gradient();
        
        GradientColorKey[] colorKeys = new GradientColorKey[5];
        colorKeys[0] = new GradientColorKey(new Color(1f, 0.92f, 0.6f), 0f);      // 清晨
        colorKeys[1] = new GradientColorKey(new Color(1f, 1f, 0.8f), 0.25f);      // 正午
        colorKeys[2] = new GradientColorKey(new Color(1f, 0.8f, 0.5f), 0.5f);     // 下午
        colorKeys[3] = new GradientColorKey(new Color(1f, 0.5f, 0.3f), 0.7f);     // 傍晚
        colorKeys[4] = new GradientColorKey(new Color(0.3f, 0.4f, 0.6f), 1f);     // 深夜
        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1f, 0f);
        alphaKeys[1] = new GradientAlphaKey(1f, 1f);
        
        dayColorGradient.SetKeys(colorKeys, alphaKeys);
    }
    
    /// <summary>
    /// 运行时动态修改刻度数量
    /// </summary>
    public void SetMarkerCount(int count)
    {
        markerCount = count;
        CreateTimeMarkers();
    }
}

