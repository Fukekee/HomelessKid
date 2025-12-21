using System.Collections.Generic;
using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("背包设置")]
    [SerializeField] private int maxCapacity = 20;
    
    private List<ItemData> items = new List<ItemData>();
    
    public event Action<ItemData> OnItemAdded;
    public event Action<ItemData> OnItemRemoved;
    public event Action OnInventoryChanged;
    
    public List<ItemData> Items => items;
    public int ItemCount => items.Count;
    public int MaxCapacity => maxCapacity;
    public bool IsFull => items.Count >= maxCapacity;
    
    public bool AddItem(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("尝试添加空物品");
            return false;
        }
        
        if (IsFull)
        {
            Debug.Log("背包已满");
            return false;
        }
        
        items.Add(item);
        OnItemAdded?.Invoke(item);
        OnInventoryChanged?.Invoke();
        return true;
    }
    
    public bool RemoveItem(ItemData item)
    {
        if (items.Remove(item))
        {
            OnItemRemoved?.Invoke(item);
            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }
    
    public bool RemoveItemAt(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            ItemData item = items[index];
            items.RemoveAt(index);
            OnItemRemoved?.Invoke(item);
            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }
    
    public bool UseItem(ItemData item)
    {
        if (item == null || !item.isUsable)
        {
            return false;
        }
        
        // 使用物品的逻辑由SurvivalStats处理
        // 这里只负责从背包移除
        if (RemoveItem(item))
        {
            return true;
        }
        
        return false;
    }
    
    public bool HasItem(ItemData item)
    {
        return items.Contains(item);
    }
    
    public void Clear()
    {
        items.Clear();
        OnInventoryChanged?.Invoke();
    }
}

