using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayUI : MonoBehaviour
{
    [Header("UI引用")]
    [SerializeField] private TextMeshProUGUI dayText;
    
    private DayManager dayManager;
    
    private void Awake()
    {
        dayManager = DayManager.Instance;
    }
    
    private void OnEnable()
    {
        if (dayManager != null)
        {
            dayManager.OnDayChanged += UpdateUI;
        }
    }
    
    private void OnDisable()
    {
        if (dayManager != null)
        {
            dayManager.OnDayChanged -= UpdateUI;
        }
    }
    
    private void Start()
    {
        if (dayManager != null)
        {
            UpdateUI(dayManager.CurrentDay);
        }
    }
    
    private void UpdateUI(int day)
    {
        if (dayText != null)
        {
            dayText.text = $"第 {day} 天";
        }
    }
}

