using UnityEngine;
using System;

public class DayManager : MonoBehaviour
{
    private static DayManager instance;
    public static DayManager Instance => instance;
    
    [Header("配置来源")]
    [Tooltip("所有数值从 GameBalanceConfig 读取")]
    [SerializeField] private bool useGameBalanceConfig = true;
    
    private int currentDay = 1;
    private float currentDayTime = 0f;
    
    // 缓存配置
    private GameBalanceConfig config;
    private float dayDuration;
    
    public event Action<int> OnDayChanged; // 天数变化事件
    public event Action OnNewDayStarted; // 新一天开始事件
    
    public int CurrentDay => currentDay;
    public float CurrentDayTime => currentDayTime;
    public float DayProgress => currentDayTime / dayDuration; // 0-1之间的进度
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // 加载配置
        if (useGameBalanceConfig && GameBalance.Config != null)
        {
            config = GameBalance.Config;
            dayDuration = config.dayLengthSeconds;
            Debug.Log($"[DayManager] 已从 GameBalanceConfig 加载配置 - 一天长度: {dayDuration}秒");
        }
        else
        {
            // 使用默认值作为备用
            dayDuration = 600f;
            Debug.LogWarning("[DayManager] 未找到 GameBalanceConfig，使用默认值 600秒");
        }
    }
    
    private void Update()
    {
        // 应用时间倍率（从配置读取）
        float timeScale = config != null ? config.timeScale : 1f;
        
        // 更新当前一天的时间
        AddTime(Time.deltaTime * timeScale);
        
        // 注意：不自动进入新一天，需要通过小屋睡觉来触发
        // 即使时间超过dayDuration，也要等玩家回到小屋过夜
    }
    
    public void StartNewDay()
    {
        currentDay++;
        currentDayTime = 0f;
        
        OnDayChanged?.Invoke(currentDay);
        OnNewDayStarted?.Invoke();
        
        Debug.Log($"新的一天开始了！第 {currentDay} 天");
    }
    
    public void AddTime(float deltaTime)
    {
        currentDayTime += deltaTime;
    }
    
    /// <summary>
    /// 判断当前是否为夜晚时间
    /// </summary>
    public bool IsNightTime()
    {
        if (config == null) return false;
        
        // 将当前时间转换为游戏内小时（0-24）
        // 假设一天24小时，currentDayTime是秒数
        float hoursInDay = 24f;
        float currentHour = (currentDayTime / dayDuration) * hoursInDay;
        
        int nightStart = config.nightStartHour;
        int nightEnd = config.nightEndHour;
        
        // 处理跨天的情况（如21:00-06:00）
        if (nightStart > nightEnd)
        {
            // 夜晚跨越午夜，例如21:00-06:00
            return currentHour >= nightStart || currentHour < nightEnd;
        }
        else
        {
            // 夜晚在同一天内
            return currentHour >= nightStart && currentHour < nightEnd;
        }
    }
    
    /// <summary>
    /// 获取当前游戏内小时（0-24）
    /// </summary>
    public float GetCurrentHour()
    {
        float hoursInDay = 24f;
        return (currentDayTime / dayDuration) * hoursInDay;
    }
}



