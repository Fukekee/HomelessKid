using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SurvivalUI : MonoBehaviour
{
    [Header("UI引用")]
    [SerializeField] private TextMeshProUGUI hungerText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI moodText;
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider moodSlider;
    
    private SurvivalStats survivalStats;
    
    private void Awake()
    {
        survivalStats = FindObjectOfType<SurvivalStats>();
    }
    
    private void OnEnable()
    {
        if (survivalStats != null)
        {
            survivalStats.OnStatsChanged += UpdateUI;
        }
    }
    
    private void OnDisable()
    {
        if (survivalStats != null)
        {
            survivalStats.OnStatsChanged -= UpdateUI;
        }
    }
    
    private void Start()
    {
        if (survivalStats != null)
        {
            UpdateUI(survivalStats.CurrentHunger, survivalStats.CurrentHealth, survivalStats.CurrentMood);
        }
    }
    
    private void UpdateUI(float hunger, float health, float mood)
    {
        // 更新文本
        if (hungerText != null)
        {
            hungerText.text = $"饱食度: {hunger:F0}/{survivalStats.MaxHunger}";
        }
        
        if (healthText != null)
        {
            healthText.text = $"健康度: {health:F0}/{survivalStats.MaxHealth}";
        }
        
        if (moodText != null)
        {
            moodText.text = $"精神度: {mood:F0}/{survivalStats.MaxMood}";
        }
        
        // 更新滑块
        if (hungerSlider != null)
        {
            hungerSlider.value = survivalStats.HungerPercentage;
        }
        
        if (healthSlider != null)
        {
            healthSlider.value = survivalStats.HealthPercentage;
        }
        
        if (moodSlider != null)
        {
            moodSlider.value = survivalStats.MoodPercentage;
        }
    }
}

