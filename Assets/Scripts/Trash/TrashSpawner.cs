using UnityEngine;
using System.Collections.Generic;

public class TrashSpawner : MonoBehaviour
{
    [Header("生成设置")]
    [SerializeField] private GameObject groundTrashPrefab;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private List<ItemData> trashItemPool = new List<ItemData>(); // 可生成的垃圾物品池
    
    [Header("生成数量")]
    [SerializeField] private int trashPerDay = 10; // 每天生成的垃圾数量
    
    private List<GameObject> spawnedTrash = new List<GameObject>();
    private DayManager dayManager;
    
    private void Start()
    {
        dayManager = DayManager.Instance;
        
        if (dayManager != null)
        {
            dayManager.OnNewDayStarted += SpawnTrash;
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
        
        Debug.Log($"生成了 {spawnCount} 个垃圾");
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

