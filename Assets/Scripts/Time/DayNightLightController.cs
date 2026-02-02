using UnityEngine;

/// <summary>
/// 昼夜光照控制器
/// 根据游戏内时间自动调整Directional Light的亮度
/// </summary>
public class DayNightLightController : MonoBehaviour
{
    [Header("光照引用")]
    [Tooltip("场景中的Directional Light（如果为空，会自动查找）")]
    [SerializeField] private Light directionalLight;
    
    [Header("光照强度配置")]
    [Tooltip("夜晚最低亮度")]
    [Range(0f, 2f)]
    [SerializeField] private float nightIntensity = 0.3f;
    
    [Tooltip("白天最高亮度")]
    [Range(0f, 3f)]
    [SerializeField] private float dayIntensity = 1.5f;
    
    [Header("时段配置")]
    [Tooltip("日出开始时间（小时）")]
    [Range(0f, 24f)]
    [SerializeField] private float sunriseStartHour = 6f;
    
    [Tooltip("日出结束时间（小时）")]
    [Range(0f, 24f)]
    [SerializeField] private float sunriseEndHour = 8f;
    
    [Tooltip("日落开始时间（小时）")]
    [Range(0f, 24f)]
    [SerializeField] private float sunsetStartHour = 18f;
    
    [Tooltip("日落结束时间（小时）")]
    [Range(0f, 24f)]
    [SerializeField] private float sunsetEndHour = 20f;
    
    [Header("颜色配置（可选）")]
    [Tooltip("是否根据时间调整光照颜色")]
    [SerializeField] private bool adjustColor = true;
    
    [Tooltip("夜晚光照颜色（偏蓝）")]
    [SerializeField] private Color nightColor = new Color(0.4f, 0.5f, 0.7f, 1f);
    
    [Tooltip("白天光照颜色（偏白/暖）")]
    [SerializeField] private Color dayColor = new Color(1f, 0.95f, 0.9f, 1f);
    
    private DayManager dayManager;
    
    private void Awake()
    {
        // 优先使用当前GameObject上的Light组件（如果组件直接挂在Light上）
        if (directionalLight == null)
        {
            directionalLight = GetComponent<Light>();
        }
        
        // 如果当前GameObject没有Light组件，则查找场景中的Directional Light
        if (directionalLight == null)
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    directionalLight = light;
                    break;
                }
            }
        }
        
        // 验证Light组件
        if (directionalLight == null)
        {
            Debug.LogWarning("[DayNightLightController] 未找到Directional Light！请确保场景中有Directional Light，或将此组件挂在Directional Light上。");
        }
        else if (directionalLight.type != LightType.Directional)
        {
            Debug.LogWarning("[DayNightLightController] 指定的Light不是Directional Light类型！当前类型: " + directionalLight.type);
            directionalLight = null;
        }
        else
        {
            Debug.Log($"[DayNightLightController] 已连接到Directional Light: {directionalLight.gameObject.name}");
        }
    }
    
    private void Start()
    {
        dayManager = DayManager.Instance;
        
        if (dayManager == null)
        {
            Debug.LogError("[DayNightLightController] 未找到DayManager！");
        }
    }
    
    private void Update()
    {
        if (directionalLight == null || dayManager == null) return;
        
        // 获取当前游戏内小时
        float currentHour = dayManager.GetCurrentHour();
        
        // 计算光照强度
        float intensity = CalculateIntensity(currentHour);
        directionalLight.intensity = intensity;
        
        // 计算光照颜色（如果启用）
        if (adjustColor)
        {
            Color color = CalculateColor(currentHour);
            directionalLight.color = color;
        }
    }
    
    /// <summary>
    /// 根据当前小时计算光照强度
    /// </summary>
    private float CalculateIntensity(float hour)
    {
        // 处理跨天的情况（例如夜晚从20:00到6:00）
        if (sunsetEndHour > sunriseStartHour)
        {
            // 夜晚时段（20:00 - 6:00）
            if (hour >= sunsetEndHour || hour < sunriseStartHour)
            {
                return nightIntensity;
            }
            // 日出时段（6:00 - 8:00）
            else if (hour >= sunriseStartHour && hour < sunriseEndHour)
            {
                float t = (hour - sunriseStartHour) / (sunriseEndHour - sunriseStartHour);
                return Mathf.Lerp(nightIntensity, dayIntensity, t);
            }
            // 白天时段（8:00 - 18:00）
            else if (hour >= sunriseEndHour && hour < sunsetStartHour)
            {
                return dayIntensity;
            }
            // 日落时段（18:00 - 20:00）
            else // hour >= sunsetStartHour && hour < sunsetEndHour
            {
                float t = (hour - sunsetStartHour) / (sunsetEndHour - sunsetStartHour);
                return Mathf.Lerp(dayIntensity, nightIntensity, t);
            }
        }
        else
        {
            // 如果配置错误（日落结束时间小于日出开始时间），使用简单逻辑
            if (hour >= sunsetEndHour && hour < sunriseStartHour)
            {
                return nightIntensity;
            }
            else if (hour >= sunriseStartHour && hour < sunriseEndHour)
            {
                float t = (hour - sunriseStartHour) / (sunriseEndHour - sunriseStartHour);
                return Mathf.Lerp(nightIntensity, dayIntensity, t);
            }
            else if (hour >= sunriseEndHour && hour < sunsetStartHour)
            {
                return dayIntensity;
            }
            else
            {
                float t = (hour - sunsetStartHour) / (sunsetEndHour - sunsetStartHour);
                return Mathf.Lerp(dayIntensity, nightIntensity, t);
            }
        }
    }
    
    /// <summary>
    /// 根据当前小时计算光照颜色
    /// </summary>
    private Color CalculateColor(float hour)
    {
        // 使用与强度相同的逻辑来计算颜色插值
        float colorT = 0f;
        
        if (sunsetEndHour > sunriseStartHour)
        {
            if (hour >= sunsetEndHour || hour < sunriseStartHour)
            {
                colorT = 0f; // 夜晚
            }
            else if (hour >= sunriseStartHour && hour < sunriseEndHour)
            {
                colorT = (hour - sunriseStartHour) / (sunriseEndHour - sunriseStartHour);
            }
            else if (hour >= sunriseEndHour && hour < sunsetStartHour)
            {
                colorT = 1f; // 白天
            }
            else
            {
                colorT = 1f - (hour - sunsetStartHour) / (sunsetEndHour - sunsetStartHour);
            }
        }
        else
        {
            if (hour >= sunsetEndHour && hour < sunriseStartHour)
            {
                colorT = 0f;
            }
            else if (hour >= sunriseStartHour && hour < sunriseEndHour)
            {
                colorT = (hour - sunriseStartHour) / (sunriseEndHour - sunriseStartHour);
            }
            else if (hour >= sunriseEndHour && hour < sunsetStartHour)
            {
                colorT = 1f;
            }
            else
            {
                colorT = 1f - (hour - sunsetStartHour) / (sunsetEndHour - sunsetStartHour);
            }
        }
        
        return Color.Lerp(nightColor, dayColor, colorT);
    }
    
    /// <summary>
    /// 在编辑器中预览光照效果（仅编辑器模式）
    /// </summary>
    #if UNITY_EDITOR
    [ContextMenu("预览当前时间的光照")]
    private void PreviewCurrentLighting()
    {
        if (dayManager == null)
        {
            dayManager = FindObjectOfType<DayManager>();
        }
        
        if (dayManager != null && directionalLight != null)
        {
            float hour = dayManager.GetCurrentHour();
            float intensity = CalculateIntensity(hour);
            Color color = adjustColor ? CalculateColor(hour) : directionalLight.color;
            
            directionalLight.intensity = intensity;
            directionalLight.color = color;
            
            Debug.Log($"[DayNightLightController] 预览光照 - 时间: {hour:F2}:00, 强度: {intensity:F2}, 颜色: {color}");
        }
    }
    #endif
}
