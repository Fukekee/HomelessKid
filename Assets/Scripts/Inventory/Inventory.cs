using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public sealed class InventoryStack
{
    public ItemData item;
    public int count;
}

public class Inventory : MonoBehaviour
{
    [Header("背包设置")]
    [Tooltip("背包可容纳的总物品数量（按件数计数）。")]
    [SerializeField] private int maxCapacity = 20;

    [SerializeField]
    private List<InventoryStack> stacks = new List<InventoryStack>();

    public event Action<ItemData, int> OnItemAdded;
    public event Action<ItemData, int> OnItemRemoved;
    public event Action OnInventoryChanged;

    /// <summary>堆叠列表（每种物品一个条目）。</summary>
    public IReadOnlyList<InventoryStack> Stacks => stacks;

    /// <summary>背包当前总数量（所有堆叠的 count 之和）。</summary>
    public int ItemCount => stacks.Sum(s => Mathf.Max(0, s.count));

    public int MaxCapacity => maxCapacity;

    public bool IsFull => ItemCount >= maxCapacity;

    public int GetCount(ItemData item)
    {
        if (item == null) return 0;
        var stack = stacks.FirstOrDefault(s => s != null && s.item == item);
        return stack != null ? Mathf.Max(0, stack.count) : 0;
    }

    public bool HasItem(ItemData item, int amount = 1)
    {
        return GetCount(item) >= Mathf.Max(1, amount);
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("尝试添加空物品");
            return false;
        }

        amount = Mathf.Max(1, amount);

        if (ItemCount + amount > maxCapacity)
        {
            Debug.Log("背包已满");
            return false;
        }

        var stack = stacks.FirstOrDefault(s => s != null && s.item == item);
        if (stack == null)
        {
            stack = new InventoryStack { item = item, count = 0 };
            stacks.Add(stack);
        }

        stack.count += amount;
        OnItemAdded?.Invoke(item, amount);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (item == null) return false;
        amount = Mathf.Max(1, amount);

        var stack = stacks.FirstOrDefault(s => s != null && s.item == item);
        if (stack == null) return false;

        if (stack.count < amount) return false;

        stack.count -= amount;
        if (stack.count <= 0)
            stacks.Remove(stack);

        OnItemRemoved?.Invoke(item, amount);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool UseItem(ItemData item)
    {
        if (item == null || !item.isUsable)
            return false;

        // 使用效果由外部（SurvivalStats等）处理；这里只负责消耗 1 个
        return RemoveItem(item, 1);
    }

    public void Clear()
    {
        stacks.Clear();
        OnInventoryChanged?.Invoke();
    }
}



