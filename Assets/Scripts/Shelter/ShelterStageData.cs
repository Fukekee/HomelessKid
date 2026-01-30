using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基地阶段所需材料
/// </summary>
[Serializable]
public class MaterialRequirement
{
    public ItemData item;
    public int amount;
}

/// <summary>
/// 基地阶段数据配置
/// </summary>
[CreateAssetMenu(fileName = "ShelterStageData", menuName = "Game/Shelter Stage Data")]
public class ShelterStageData : ScriptableObject
{
    [Header("基本信息")]
    [Tooltip("阶段ID（0, 1, 2...）")]
    public int stageId = 0;
    
    [Tooltip("阶段名称（如：无家/露宿、废棚子角落）")]
    public string stageName = "无家/露宿";
    
    [Header("升级需求")]
    [Tooltip("升级到此阶段所需的材料列表")]
    public List<MaterialRequirement> requiredMaterials = new List<MaterialRequirement>();
    
    [Header("阶段加成")]
    [Tooltip("健康恢复倍率")]
    [Range(0.5f, 3f)]
    public float healthRecoveryMultiplier = 1.0f;
    
    [Tooltip("精神恢复倍率")]
    [Range(0.5f, 3f)]
    public float moodRecoveryMultiplier = 1.0f;
    
    [Tooltip("升级到此阶段时一次性获得的mood加成")]
    [Range(0f, 50f)]
    public float moodBaseBonus = 0f;
    
    [Header("描述")]
    [TextArea(2, 4)]
    public string description = "";
}
