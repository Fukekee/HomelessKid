using UnityEngine;
using System;

public class DayManager : MonoBehaviour
{
    private static DayManager instance;
    public static DayManager Instance => instance;
    
    [Header("时间设置")]
    [SerializeField] private float dayDuration = 600f; // 一天时长（秒），默认10分钟
    
    private int currentDay = 1;
    private float currentDayTime = 0f;
    
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
        }
    }
    
    private void Update()
    {
        // 更新时间（如果需要在时间到达后自动进入新一天，可以在这里处理）
        // 目前我们通过过夜系统手动触发新一天
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
}

