using UnityEngine;

/// <summary>
/// 游戏平衡配置访问器 - 提供全局单例访问
/// 使用方法: GameBalance.Config.dayLengthSeconds
/// </summary>
public class GameBalance : MonoBehaviour
{
    [Header("配置引用")]
    [Tooltip("拖入GameBalanceConfig资源")]
    public GameBalanceConfig config;
    
    private static GameBalance _instance;
    private static GameBalanceConfig _config;
    
    /// <summary>
    /// 获取配置实例（推荐使用这个）
    /// </summary>
    public static GameBalanceConfig Config
    {
        get
        {
            if (_config == null)
            {
                // 尝试从场景中查找
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameBalance>();
                }
                
                if (_instance != null && _instance.config != null)
                {
                    _config = _instance.config;
                }
                else
                {
                    Debug.LogError("[GameBalance] 未找到GameBalanceConfig！请在场景中添加GameBalance组件并配置config字段。");
                }
            }
            return _config;
        }
    }
    
    private void Awake()
    {
        // 单例处理
        if (_instance == null)
        {
            _instance = this;
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (_instance != this)
        {
            Debug.LogWarning("[GameBalance] 场景中存在多个GameBalance实例，销毁重复的。");
            Destroy(gameObject);
            return;
        }
        
        // 缓存配置
        if (config != null)
        {
            _config = config;
            Debug.Log($"[GameBalance] 配置加载完成 - 一天长度: {config.dayLengthSeconds}秒");
        }
        else
        {
            Debug.LogError("[GameBalance] config字段未配置！请拖入GameBalanceConfig资源。");
        }
    }
    
    /// <summary>
    /// 在Inspector中显示当前配置信息
    /// </summary>
    [ContextMenu("显示配置摘要")]
    public void ShowConfigSummary()
    {
        if (config == null)
        {
            Debug.LogWarning("配置未设置！");
            return;
        }
        
        Debug.Log($"=== 游戏平衡配置摘要 ===\n" +
                  $"一天长度: {config.dayLengthSeconds}秒 ({config.dayLengthSeconds/60f:F1}分钟)\n" +
                  $"理论每天需要食物: {config.FoodNeededPerDay:F1}份\n" +
                  $"一份食物支撑时间: {config.MinutesPerFood:F1}分钟\n" +
                  $"完全饿完需要: {config.MinutesToStarve:F1}分钟\n" +
                  $"翻桶风险: {config.binRiskChance * 100:F0}%\n" +
                  $"翻桶冷却: {config.binCooldown}秒 ({config.binCooldown/60f:F1}分钟)");
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        // 编辑器中修改时刷新缓存
        if (config != null)
        {
            _config = config;
        }
    }
#endif
}

