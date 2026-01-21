using UnityEngine;
using System.Collections.Generic;

public class TrashSpawner : MonoBehaviour
{
    [Header("配置来源")]
    [Tooltip("所有数值从 GameBalanceConfig 读取")]
    [SerializeField] private bool useGameBalanceConfig = true;
    
    [Header("生成设置")]
    [SerializeField] private GameObject groundTrashPrefab;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private List<ItemData> trashItemPool = new List<ItemData>(); // 可生成的垃圾物品池
    
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
        
        if (groundTrashPrefab == null || spawnPoints.Count == 0 || trashItemPool.Count == 0)
        {
            Debug.LogWarning("TrashSpawner配置不完整");
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
            
            // 随机选择垃圾物品
            ItemData randomTrash = trashItemPool[Random.Range(0, trashItemPool.Count)];
            
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



