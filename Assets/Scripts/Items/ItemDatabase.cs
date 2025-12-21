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
}

