using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Item Database", menuName = "Game/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemData> items = new List<ItemData>();
    
    public ItemData GetItemByName(string itemName)
    {
        return items.Find(item => item.itemName == itemName);
    }
    
    public ItemData GetRandomItem()
    {
        if (items.Count == 0) return null;
        return items[Random.Range(0, items.Count)];
    }
    
    public ItemData GetRandomItemByType(ItemType type)
    {
        List<ItemData> filteredItems = items.FindAll(item => item.itemType == type);
        if (filteredItems.Count == 0) return null;
        return filteredItems[Random.Range(0, filteredItems.Count)];
    }
    
    /// <summary>
    /// 根据来源区域返回物品列表，便于按区域填充 SpawnZoneData 或编辑器工具。
    /// </summary>
    /// <param name="region">区域标识，与 ItemData.sourceRegion 一致；传 null 或空字符串返回全部物品</param>
    public List<ItemData> GetItemsByRegion(string region)
    {
        if (items == null) return new List<ItemData>();
        if (string.IsNullOrEmpty(region)) return new List<ItemData>(items);
        return items.FindAll(item => item != null && item.sourceRegion == region);
    }
}





