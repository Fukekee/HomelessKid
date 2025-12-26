// 临时禁用，等Unity稳定后再启用
#if UNITY_EDITOR && FALSE
using UnityEditor;
using UnityEngine;

/// <summary>
/// GameBalanceConfig的自定义Inspector显示
/// 显示理论数据和警告
/// </summary>
[CustomEditor(typeof(GameBalanceConfig))]
public class GameBalanceConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 绘制默认Inspector
        DrawDefaultInspector();
        
        GameBalanceConfig config = (GameBalanceConfig)target;
        
        // 绘制分隔线
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("理论数据计算", EditorStyles.boldLabel);
        
        // 显示理论数据
        EditorGUI.BeginDisabledGroup(true);
        
        EditorGUILayout.FloatField("一天长度（分钟）", config.dayLengthSeconds / 60f);
        EditorGUILayout.FloatField("每分钟饱食度下降", config.HungerDrainPerMinute);
        EditorGUILayout.FloatField("完全饿完需要（分钟）", config.MinutesToStarve);
        EditorGUILayout.FloatField("一份食物支撑（分钟）", config.MinutesPerFood);
        EditorGUILayout.FloatField("理论每天需要食物（份）", config.FoodNeededPerDay);
        
        EditorGUI.EndDisabledGroup();
        
        // 显示警告
        EditorGUILayout.Space(5);
        
        if (config.FoodNeededPerDay > 5f)
        {
            EditorGUILayout.HelpBox(
                $"警告：一天需要 {config.FoodNeededPerDay:F1} 份食物，压力可能过大！",
                MessageType.Warning
            );
        }
        else if (config.FoodNeededPerDay < 2f)
        {
            EditorGUILayout.HelpBox(
                $"提示：一天只需要 {config.FoodNeededPerDay:F1} 份食物，压力可能过小。",
                MessageType.Info
            );
        }
        else
        {
            EditorGUILayout.HelpBox(
                $"当前配置：一天需要 {config.FoodNeededPerDay:F1} 份食物，压力适中。",
                MessageType.Info
            );
        }
        
        if (config.MinutesToStarve < config.dayLengthSeconds / 60f)
        {
            EditorGUILayout.HelpBox(
                "警告：玩家可能在一天内被完全饿死！",
                MessageType.Error
            );
        }
        
        // 显示翻桶理论数据
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("垃圾桶理论数据", EditorStyles.boldLabel);
        
        EditorGUI.BeginDisabledGroup(true);
        
        float binsPerDay = (config.dayLengthSeconds / config.binCooldown);
        EditorGUILayout.FloatField("同一桶一天可翻（次）", binsPerDay);
        
        float avgBinLoot = (config.binLootCountMin + config.binLootCountMax) / 2f;
        EditorGUILayout.FloatField("每桶平均掉落（个）", avgBinLoot);
        
        float avgFoodPerBin = avgBinLoot * config.binFoodDropChance;
        EditorGUILayout.FloatField("每桶平均食物（个）", avgFoodPerBin);
        
        EditorGUI.EndDisabledGroup();
        
        // 快速预设按钮
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("快速预设", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("轻松模式"))
        {
            Undo.RecordObject(config, "设置轻松模式");
            config.hungerDrainPerSecond = 0.2f;
            config.foodRestoreAmount = 50f;
            config.binRiskChance = 0.1f;
            EditorUtility.SetDirty(config);
        }
        
        if (GUILayout.Button("标准模式"))
        {
            Undo.RecordObject(config, "设置标准模式");
            config.hungerDrainPerSecond = 0.25f;
            config.foodRestoreAmount = 40f;
            config.binRiskChance = 0.15f;
            EditorUtility.SetDirty(config);
        }
        
        if (GUILayout.Button("挑战模式"))
        {
            Undo.RecordObject(config, "设置挑战模式");
            config.hungerDrainPerSecond = 0.3f;
            config.foodRestoreAmount = 35f;
            config.binRiskChance = 0.2f;
            EditorUtility.SetDirty(config);
        }
        
        EditorGUILayout.EndHorizontal();
    }
}
#endif

