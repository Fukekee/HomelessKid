using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 带权重的地面垃圾物品项（权重越大出现概率越高）
/// </summary>
[System.Serializable]
public class WeightedTrashItem
{
    public ItemData item;
    [Tooltip("出现权重，数值越大越容易刷出。例如木板3、布1 则木板约75%")]
    [Min(0.01f)]
    public float weight = 1f;
}

public class TrashSpawner : MonoBehaviour
{
    [Header("配置来源")]
    [Tooltip("所有数值从 GameBalanceConfig 读取")]
    [SerializeField] private bool useGameBalanceConfig = true;
    
    [Header("生成设置")]
    [SerializeField] private GameObject groundTrashPrefab;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    
    [Header("物品池（二选一）")]
    [Tooltip("推荐：按权重控制各物品出现概率（如木板3、布1）")]
    [SerializeField] private List<WeightedTrashItem> weightedTrashPool = new List<WeightedTrashItem>();
    [Tooltip("留空则用下方旧池子；若上方加权池有内容则优先用加权池")]
    [SerializeField] private List<ItemData> trashItemPool = new List<ItemData>();
    
    private List<GameObject> spawnedTrash = new List<GameObject>();
    private DayManager dayManager;
    
    // 缓存配置
    private GameBalanceConfig config;
    
    private void Start()
    {
        dayManager = DayManager.Instance;
        
        if (dayManager != null)
        {
            dayManager.OnNewDayStarted += SpawnTrash;
        }
        
        // 加载配置
        if (useGameBalanceConfig && GameBalance.Config != null)
        {
            config = GameBalance.Config;
            Debug.Log("[TrashSpawner] 已从 GameBalanceConfig 加载配置");
        }
        else
        {
            Debug.LogWarning("[TrashSpawner] 未找到 GameBalanceConfig，使用默认值");
        }
        
        // 第一天开始时生成垃圾
        SpawnTrash();
    }
    
    private void OnDestroy()
    {
        if (dayManager != null)
        {
            dayManager.OnNewDayStarted -= SpawnTrash;
        }
    }
    
    public void SpawnTrash()
    {
        // 清除旧的垃圾
        ClearSpawnedTrash();
        
        bool useWeighted = GetWeightedPoolValid();
        bool useFlat = !useWeighted && trashItemPool.Count > 0;
        
        if (groundTrashPrefab == null || spawnPoints.Count == 0 || (!useWeighted && !useFlat))
        {
            Debug.LogWarning("TrashSpawner配置不完整：需要 groundTrashPrefab、spawnPoints 以及 weightedTrashPool 或 trashItemPool");
            return;
        }
        
        // 从配置读取每天生成的垃圾数量（随机范围）
        int minSpawn = config != null ? config.dailyGroundTrashSpawnMin : 8;
        int maxSpawn = config != null ? config.dailyGroundTrashSpawnMax : 12;
        int trashPerDay = Random.Range(minSpawn, maxSpawn + 1);
        
        // 随机选择生成点
        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);
        int spawnCount = Mathf.Min(trashPerDay, availableSpawnPoints.Count);
        
        for (int i = 0; i < spawnCount; i++)
        {
            if (availableSpawnPoints.Count == 0) break;
            
            // 随机选择生成点
            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform spawnPoint = availableSpawnPoints[randomIndex];
            availableSpawnPoints.RemoveAt(randomIndex);
            
            // 按权重或均等随机选择物品
            ItemData randomTrash = useWeighted ? PickItemByWeight() : trashItemPool[Random.Range(0, trashItemPool.Count)];
            if (randomTrash == null) continue;
            
            // 生成垃圾
            GameObject trash = Instantiate(groundTrashPrefab, spawnPoint.position, spawnPoint.rotation);
            GroundTrash groundTrash = trash.GetComponent<GroundTrash>();
            if (groundTrash != null)
            {
                groundTrash.SetTrashItem(randomTrash);
            }
            
            spawnedTrash.Add(trash);
        }
        
        Debug.Log($"[TrashSpawner] 生成了 {spawnCount} 个垃圾（配置范围: {minSpawn}-{maxSpawn}）");
    }
    
    private bool GetWeightedPoolValid()
    {
        if (weightedTrashPool == null || weightedTrashPool.Count == 0) return false;
        float total = 0f;
        foreach (var w in weightedTrashPool)
        {
            if (w != null && w.item != null) total += w.weight;
        }
        return total > 0f;
    }
    
    private ItemData PickItemByWeight()
    {
        float totalWeight = 0f;
        foreach (var w in weightedTrashPool)
        {
            if (w != null && w.item != null) totalWeight += w.weight;
        }
        if (totalWeight <= 0f) return null;
        float r = Random.Range(0f, totalWeight);
        foreach (var w in weightedTrashPool)
        {
            if (w == null || w.item == null) continue;
            r -= w.weight;
            if (r <= 0f) return w.item;
        }
        return weightedTrashPool[weightedTrashPool.Count - 1].item;
    }
    
    private void ClearSpawnedTrash()
    {
        foreach (GameObject trash in spawnedTrash)
        {
            if (trash != null)
            {
                Destroy(trash);
            }
        }
        spawnedTrash.Clear();
    }
    
    // 在编辑器中绘制生成点
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
            }
        }
    }
}



