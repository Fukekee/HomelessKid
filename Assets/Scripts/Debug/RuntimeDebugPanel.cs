using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 运行时调试面板 - 实时查看和调整游戏平衡配置
/// 快捷键: F1 切换显示/隐藏
/// </summary>
public class RuntimeDebugPanel : MonoBehaviour
{
    [Header("面板设置")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private KeyCode toggleKey = KeyCode.F1;
    
    [Header("UI 引用 - 时间系统")]
    [SerializeField] private Slider dayLengthSlider;
    [SerializeField] private TMP_InputField dayLengthInput;
    [SerializeField] private Slider timeScaleSlider;
    [SerializeField] private TMP_InputField timeScaleInput;
    
    [Header("UI 引用 - 饱食度系统")]
    [SerializeField] private Slider hungerDrainSlider;
    [SerializeField] private TMP_InputField hungerDrainInput;
    [SerializeField] private Slider foodRestoreSlider;
    [SerializeField] private TMP_InputField foodRestoreInput;
    
    [Header("UI 引用 - 健康度系统")]
    [SerializeField] private Slider binRiskChanceSlider;
    [SerializeField] private TMP_InputField binRiskChanceInput;
    [SerializeField] private Slider binRiskDamageSlider;
    [SerializeField] private TMP_InputField binRiskDamageInput;
    [SerializeField] private Slider sleepRestoreSlider;
    [SerializeField] private TMP_InputField sleepRestoreInput;
    
    [Header("UI 引用 - 垃圾桶系统")]
    [SerializeField] private Slider binSearchTimeSlider;
    [SerializeField] private TMP_InputField binSearchTimeInput;
    [SerializeField] private Slider binCooldownSlider;
    [SerializeField] private TMP_InputField binCooldownInput;
    
    [Header("UI 引用 - 地面垃圾系统")]
    [SerializeField] private Slider groundTrashMinSlider;
    [SerializeField] private TMP_InputField groundTrashMinInput;
    [SerializeField] private Slider groundTrashMaxSlider;
    [SerializeField] private TMP_InputField groundTrashMaxInput;
    
    [Header("UI 引用 - 理论数据显示")]
    [SerializeField] private TextMeshProUGUI theoryDataText;
    
    [Header("UI 引用 - 预设按钮")]
    [SerializeField] private Button easyPresetButton;
    [SerializeField] private Button normalPresetButton;
    [SerializeField] private Button hardPresetButton;
    
    private GameBalanceConfig config;
    private bool isVisible = false;
    
    private void Start()
    {
        // 获取配置
        if (GameBalance.Config != null)
        {
            config = GameBalance.Config;
        }
        else
        {
            Debug.LogError("[RuntimeDebugPanel] 未找到 GameBalanceConfig！");
            enabled = false;
            return;
        }
        
        // 初始化 UI
        InitializeUI();
        
        // 绑定预设按钮
        if (easyPresetButton != null)
            easyPresetButton.onClick.AddListener(ApplyEasyPreset);
        if (normalPresetButton != null)
            normalPresetButton.onClick.AddListener(ApplyNormalPreset);
        if (hardPresetButton != null)
            hardPresetButton.onClick.AddListener(ApplyHardPreset);
        
        // 默认隐藏
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }
    
    private void Update()
    {
        // 切换显示/隐藏
        if (Input.GetKeyDown(toggleKey))
        {
            TogglePanel();
        }
        
        // 如果面板可见，更新理论数据
        if (isVisible && config != null)
        {
            UpdateTheoryData();
        }
    }
    
    private void TogglePanel()
    {
        isVisible = !isVisible;
        if (panelRoot != null)
        {
            panelRoot.SetActive(isVisible);
            if (isVisible)
            {
                RefreshAllValues();
            }
        }
    }
    
    private void InitializeUI()
    {
        // 时间系统
        SetupSliderAndInput(dayLengthSlider, dayLengthInput, 300f, 1200f, config.dayLengthSeconds, 
            (value) => config.dayLengthSeconds = value);
        SetupSliderAndInput(timeScaleSlider, timeScaleInput, 0.1f, 5f, config.timeScale, 
            (value) => config.timeScale = value);
        
        // 饱食度系统
        SetupSliderAndInput(hungerDrainSlider, hungerDrainInput, 0.1f, 0.5f, config.hungerDrainPerSecond, 
            (value) => config.hungerDrainPerSecond = value);
        SetupSliderAndInput(foodRestoreSlider, foodRestoreInput, 20f, 80f, config.foodRestoreAmount, 
            (value) => config.foodRestoreAmount = value);
        
        // 健康度系统
        SetupSliderAndInput(binRiskChanceSlider, binRiskChanceInput, 0f, 0.5f, config.binRiskChance, 
            (value) => config.binRiskChance = value);
        SetupSliderAndInput(binRiskDamageSlider, binRiskDamageInput, 5f, 30f, config.binRiskDamage, 
            (value) => config.binRiskDamage = value);
        SetupSliderAndInput(sleepRestoreSlider, sleepRestoreInput, 10f, 60f, config.sleepHealthRestore, 
            (value) => config.sleepHealthRestore = value);
        
        // 垃圾桶系统
        SetupSliderAndInput(binSearchTimeSlider, binSearchTimeInput, 1f, 5f, config.binSearchTime, 
            (value) => config.binSearchTime = value);
        SetupSliderAndInput(binCooldownSlider, binCooldownInput, 60f, 300f, config.binCooldown, 
            (value) => config.binCooldown = value);
        
        // 地面垃圾系统
        SetupSliderAndInput(groundTrashMinSlider, groundTrashMinInput, 5f, 20f, config.dailyGroundTrashSpawnMin, 
            (value) => config.dailyGroundTrashSpawnMin = Mathf.RoundToInt(value));
        SetupSliderAndInput(groundTrashMaxSlider, groundTrashMaxInput, 8f, 25f, config.dailyGroundTrashSpawnMax, 
            (value) => config.dailyGroundTrashSpawnMax = Mathf.RoundToInt(value));
    }
    
    private void SetupSliderAndInput(Slider slider, TMP_InputField input, float min, float max, float initialValue, System.Action<float> onValueChanged)
    {
        if (slider == null || input == null) return;
        
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = initialValue;
        input.text = initialValue.ToString("F2");
        
        // Slider 改变时更新 Input 和配置
        slider.onValueChanged.AddListener((value) =>
        {
            input.text = value.ToString("F2");
            onValueChanged?.Invoke(value);
        });
        
        // Input 改变时更新 Slider 和配置
        input.onEndEdit.AddListener((text) =>
        {
            if (float.TryParse(text, out float value))
            {
                value = Mathf.Clamp(value, min, max);
                slider.value = value;
                onValueChanged?.Invoke(value);
            }
        });
    }
    
    private void RefreshAllValues()
    {
        if (config == null) return;
        
        // 刷新所有 UI 显示
        if (dayLengthSlider != null) dayLengthSlider.value = config.dayLengthSeconds;
        if (timeScaleSlider != null) timeScaleSlider.value = config.timeScale;
        if (hungerDrainSlider != null) hungerDrainSlider.value = config.hungerDrainPerSecond;
        if (foodRestoreSlider != null) foodRestoreSlider.value = config.foodRestoreAmount;
        if (binRiskChanceSlider != null) binRiskChanceSlider.value = config.binRiskChance;
        if (binRiskDamageSlider != null) binRiskDamageSlider.value = config.binRiskDamage;
        if (sleepRestoreSlider != null) sleepRestoreSlider.value = config.sleepHealthRestore;
        if (binSearchTimeSlider != null) binSearchTimeSlider.value = config.binSearchTime;
        if (binCooldownSlider != null) binCooldownSlider.value = config.binCooldown;
        if (groundTrashMinSlider != null) groundTrashMinSlider.value = config.dailyGroundTrashSpawnMin;
        if (groundTrashMaxSlider != null) groundTrashMaxSlider.value = config.dailyGroundTrashSpawnMax;
    }
    
    private void UpdateTheoryData()
    {
        if (theoryDataText == null || config == null) return;
        
        float dayMinutes = config.dayLengthSeconds / 60f;
        float hungerPerMinute = config.hungerDrainPerSecond * 60f;
        float minutesToStarve = config.maxHunger / hungerPerMinute;
        float minutesPerFood = config.foodRestoreAmount / hungerPerMinute;
        float foodNeededPerDay = config.FoodNeededPerDay;
        float binsPerDay = config.dayLengthSeconds / config.binCooldown;
        
        theoryDataText.text = $"<b>理论数据计算</b>\n\n" +
            $"<color=#FFD700>时间系统</color>\n" +
            $"一天长度: {dayMinutes:F1} 分钟\n" +
            $"时间倍率: {config.timeScale:F1}x\n\n" +
            
            $"<color=#FF6B6B>饱食度系统</color>\n" +
            $"每分钟下降: {hungerPerMinute:F1} 点\n" +
            $"完全饿完需要: {minutesToStarve:F1} 分钟\n" +
            $"一份食物支撑: {minutesPerFood:F1} 分钟\n" +
            $"理论每天需要: {foodNeededPerDay:F2} 份食物\n\n" +
            
            $"<color=#4ECDC4>垃圾桶系统</color>\n" +
            $"翻桶读条: {config.binSearchTime:F1} 秒\n" +
            $"翻桶冷却: {config.binCooldown / 60f:F1} 分钟\n" +
            $"同一桶一天可翻: {binsPerDay:F1} 次\n" +
            $"风险概率: {config.binRiskChance * 100:F0}%\n\n" +
            
            $"<color=#95E1D3>地面垃圾</color>\n" +
            $"每日生成: {config.dailyGroundTrashSpawnMin}-{config.dailyGroundTrashSpawnMax} 个\n\n" +
            
            $"<color=#FFAA00>压力评估</color>\n" +
            GetPressureAssessment(foodNeededPerDay);
    }
    
    private string GetPressureAssessment(float foodNeededPerDay)
    {
        if (foodNeededPerDay < 2.5f)
            return "压力: <color=#00FF00>非常轻松</color>";
        else if (foodNeededPerDay < 3.5f)
            return "压力: <color=#90EE90>轻松</color>";
        else if (foodNeededPerDay < 4.5f)
            return "压力: <color=#FFFF00>适中</color>";
        else if (foodNeededPerDay < 5.5f)
            return "压力: <color=#FFA500>偏紧张</color>";
        else
            return "压力: <color=#FF0000>非常紧张</color>";
    }
    
    // 预设配置
    private void ApplyEasyPreset()
    {
        Debug.Log("[RuntimeDebugPanel] 应用轻松模式预设");
        config.hungerDrainPerSecond = 0.2f;
        config.foodRestoreAmount = 50f;
        config.binRiskChance = 0.1f;
        config.binCooldown = 120f;
        config.dailyGroundTrashSpawnMin = 10;
        config.dailyGroundTrashSpawnMax = 15;
        RefreshAllValues();
    }
    
    private void ApplyNormalPreset()
    {
        Debug.Log("[RuntimeDebugPanel] 应用标准模式预设");
        config.hungerDrainPerSecond = 0.25f;
        config.foodRestoreAmount = 40f;
        config.binRiskChance = 0.15f;
        config.binCooldown = 150f;
        config.dailyGroundTrashSpawnMin = 8;
        config.dailyGroundTrashSpawnMax = 12;
        RefreshAllValues();
    }
    
    private void ApplyHardPreset()
    {
        Debug.Log("[RuntimeDebugPanel] 应用困难模式预设");
        config.hungerDrainPerSecond = 0.3f;
        config.foodRestoreAmount = 35f;
        config.binRiskChance = 0.2f;
        config.binCooldown = 180f;
        config.dailyGroundTrashSpawnMin = 6;
        config.dailyGroundTrashSpawnMax = 10;
        RefreshAllValues();
    }
}

