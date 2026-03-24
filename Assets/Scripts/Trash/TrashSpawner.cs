using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 带权重的地面垃圾物品项（权重越大出现概率越高）。
/// 可刷新物品：由此类与 TrashSpawner 管理；不可刷新物品在场景中手动放置，不参与刷新。
/// </summary>
[System.Serializable]
public class WeightedTrashItem
{
    public ItemData item;
    [Tooltip("出现权重，数值越大越容易刷出。例如木板3、布1 则木板约75%")]
    [Min(0.01f)]
    public float weight = 1f;
    [Tooltip("该物品每隔几日刷新一次。1=每天，2=每两天，3=每三天… 仅对可刷新物品生效")]
    [Min(1)]
    public int refreshIntervalDays = 1;
}

/// <summary>
/// 单个区域的生成配置：生成点、物品池、密度倍率（默认1）
/// </summary>
[System.Serializable]
public class SpawnZoneData
{
    [Tooltip("区域 ID，与 ItemData.sourceRegion 一致，如 Strawberry Fields、Redfruit Lane")]
    public string zoneId = "";
    [Tooltip("显示名（可选）")]
    public string displayName = "";
    [Tooltip("本区域的生成点")]
    public List<Transform> spawnPoints = new List<Transform>();
    [Tooltip("本区域物品池（可按权重）")]
    public List<WeightedTrashItem> weightedTrashPool = new List<WeightedTrashItem>();
    [Tooltip("本区域垃圾密度倍率，生成数 = 基础数量 × 此值，默认 1")]
    [Min(0.01f)]
    public float densityMultiplier = 1f;
    [Tooltip("本区域每日生成数量最小值")]
    [Min(0)]
    public int minSpawn = 1;
    [Tooltip("本区域每日生成数量最大值")]
    [Min(0)]
    public int maxSpawn = 10;
}

/// <summary>
/// 仅负责「可刷新」的地面物品：在新一天按区域与物品池生成，每个物品的刷新周期由 WeightedTrashItem.refreshIntervalDays 控制。
/// 「不可刷新」的地面物品请在场景中手动放置 GroundTrash，不会在此被清除或重新生成。
/// </summary>
public class TrashSpawner : MonoBehaviour
{
    [Header("配置来源")]
    [Tooltip("所有数值从 GameBalanceConfig 读取")]
    [SerializeField] private bool useGameBalanceConfig = true;
    
    [Header("生成设置")]
    [SerializeField] private GameObject groundTrashPrefab;
    [Tooltip("按区域配置：每个区域有独立生成点与物品池，以及密度倍率（默认1）")]
    [SerializeField] private List<SpawnZoneData> spawnZones = new List<SpawnZoneData>();
    
    private List<GameObject> spawnedTrash = new List<GameObject>();
    private DayManager dayManager;
    private GameBalanceConfig config;
    
    private void Start()
    {
        dayManager = DayManager.Instance;
        
        if (dayManager != null)
        {
            dayManager.OnNewDayStarted += OnNewDayStarted;
        }
        
        if (useGameBalanceConfig && GameBalance.Config != null)
        {
            config = GameBalance.Config;
            Debug.Log("[TrashSpawner] 已从 GameBalanceConfig 加载配置");
        }
        else
        {
            Debug.LogWarning("[TrashSpawner] 未找到 GameBalanceConfig，使用默认值");
        }
        
        // 第一天开始时生成
        SpawnTrash();
    }
    
    private void OnDestroy()
    {
        if (dayManager != null)
        {
            dayManager.OnNewDayStarted -= OnNewDayStarted;
        }
    }
    
    /// <summary>
    /// 每天新日都会执行一次生成；具体哪些物品出现由各 WeightedTrashItem.refreshIntervalDays 决定。
    /// </summary>
    private void OnNewDayStarted()
    {
        SpawnTrash();
    }
    
    public void SpawnTrash()
    {
        ClearSpawnedTrash();
        
        if (groundTrashPrefab == null || spawnZones == null || spawnZones.Count == 0)
        {
            Debug.LogWarning("[TrashSpawner] 配置不完整：需要 groundTrashPrefab 和至少一个 spawnZones 配置");
            return;
        }
        
        int totalSpawned = 0;
        
        foreach (SpawnZoneData zone in spawnZones)
        {
            if (zone == null || zone.spawnPoints == null || zone.spawnPoints.Count == 0)
                continue;
            if (!GetZonePoolValid(zone))
            {
                Debug.LogWarning($"[TrashSpawner] 区域 \"{zone.zoneId}\" 物品池无效，已跳过");
                continue;
            }
            
            int zoneMin = Mathf.Min(zone.minSpawn, zone.maxSpawn);
            int zoneMax = Mathf.Max(zone.minSpawn, zone.maxSpawn);
            int baseCount = Random.Range(zoneMin, zoneMax + 1);
            int zoneCount = Mathf.Max(0, Mathf.RoundToInt(baseCount * zone.densityMultiplier));
            zoneCount = Mathf.Min(zoneCount, zone.spawnPoints.Count);
            
            List<Transform> available = new List<Transform>(zone.spawnPoints);
            for (int i = 0; i < zoneCount; i++)
            {
                if (available.Count == 0) break;
                int idx = Random.Range(0, available.Count);
                Transform point = available[idx];
                available.RemoveAt(idx);
                
                int currentDay = dayManager != null ? dayManager.CurrentDay : 1;
                ItemData item = PickItemByWeightFromDueToday(zone.weightedTrashPool, currentDay);
                if (item == null) continue;
                
                GameObject trash = Instantiate(groundTrashPrefab, point.position, point.rotation);
                var groundTrash = trash.GetComponent<GroundTrash>();
                if (groundTrash != null)
                    groundTrash.SetTrashItem(item);
                spawnedTrash.Add(trash);
                totalSpawned++;
            }
        }
        
        Debug.Log($"[TrashSpawner] 本次生成 {totalSpawned} 个垃圾（按区域与密度倍率）");
    }
    
    private bool GetZonePoolValid(SpawnZoneData zone)
    {
        if (zone?.weightedTrashPool == null || zone.weightedTrashPool.Count == 0) return false;
        float total = 0f;
        foreach (var w in zone.weightedTrashPool)
        {
            if (w != null && w.item != null) total += w.weight;
        }
        return total > 0f;
    }
    
    /// <summary>
    /// 从池子中只选「今天该刷新」的条目（按 refreshIntervalDays），再按权重抽取；若无则从全池抽取。
    /// </summary>
    private ItemData PickItemByWeightFromDueToday(List<WeightedTrashItem> pool, int currentDay)
    {
        if (pool == null || pool.Count == 0) return null;
        List<WeightedTrashItem> due = new List<WeightedTrashItem>();
        foreach (var w in pool)
        {
            if (w == null || w.item == null) continue;
            int interval = w.refreshIntervalDays <= 0 ? 1 : w.refreshIntervalDays;
            if ((currentDay - 1) % interval == 0)
                due.Add(w);
        }
        List<WeightedTrashItem> use = due.Count > 0 ? due : pool;
        float totalWeight = 0f;
        foreach (var w in use)
        {
            if (w != null && w.item != null) totalWeight += w.weight;
        }
        if (totalWeight <= 0f) return null;
        float r = Random.Range(0f, totalWeight);
        foreach (var w in use)
        {
            if (w == null || w.item == null) continue;
            r -= w.weight;
            if (r <= 0f) return w.item;
        }
        return use[use.Count - 1].item;
    }
    
    private void ClearSpawnedTrash()
    {
        foreach (GameObject trash in spawnedTrash)
        {
            if (trash != null)
                Destroy(trash);
        }
        spawnedTrash.Clear();
    }
    
    private void OnDrawGizmos()
    {
        if (spawnZones == null) return;
        Gizmos.color = Color.green;
        foreach (SpawnZoneData zone in spawnZones)
        {
            if (zone?.spawnPoints == null) continue;
            foreach (Transform t in zone.spawnPoints)
            {
                if (t != null)
                    Gizmos.DrawWireSphere(t.position, 0.5f);
            }
        }
    }
}
