using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("基本信息")]
    public string itemName = "物品名称";
    public ItemType itemType = ItemType.Junk;
    public int value = 0; // 价值（用于出售）
    [Tooltip("来源区域，与完整物品清单一致，供 GetItemsByRegion 查询")]
    public string sourceRegion = "";
    
    [Header("使用属性")]
    public bool isUsable = false; // 是否可使用
    
    [Header("使用效果（如果可食用）")]
    public int hungerRestore = 0; // 恢复的饱食度
    
    [Header("描述")]
    [TextArea(2, 4)]
    public string description = "";
}
