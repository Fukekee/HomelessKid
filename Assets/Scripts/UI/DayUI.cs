using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayUI : MonoBehaviour
{
    [Header("UI引用")]
    [SerializeField] private TextMeshProUGUI dayText;
    
    private DayManager dayManager;
    
    private void Start()
    {
        // 在 Start 中获取 DayManager，确保它已经初始化
        dayManager = DayManager.Instance;
        
        if (dayManager != null)
        {
            // 订阅事件
            dayManager.OnDayChanged += UpdateUI;
            
            // 立即更新一次 UI
            UpdateUI(dayManager.CurrentDay);
            
            Debug.Log($"[DayUI] 已连接到 DayManager，当前第 {dayManager.CurrentDay} 天");
        }
        else
        {
            Debug.LogError("[DayUI] 无法找到 DayManager！");
        }
    }
    
    private void OnDestroy()
    {
        // 取消订阅事件
        if (dayManager != null)
        {
            dayManager.OnDayChanged -= UpdateUI;
        }
    }
    
    private void UpdateUI(int day)
    {
        if (dayText != null)
        {
            dayText.text = $"第 {day} 天";
            Debug.Log($"[DayUI] UI 已更新：第 {day} 天");
        }
    }
}

