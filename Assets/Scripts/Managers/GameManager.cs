using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;
    
    [Header("游戏设置")]
    [SerializeField] private int startingMoney = 0;
    
    private int currentMoney = 0;
    
    public event Action<int> OnMoneyChanged;
    
    public int Money => currentMoney;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        currentMoney = startingMoney;
        OnMoneyChanged?.Invoke(currentMoney);
    }
    
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
        Debug.Log($"获得 {amount} 货币，当前货币: {currentMoney}");
    }
    
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            OnMoneyChanged?.Invoke(currentMoney);
            Debug.Log($"花费 {amount} 货币，剩余货币: {currentMoney}");
            return true;
        }
        else
        {
            Debug.Log("货币不足");
            return false;
        }
    }
    
    public void SetMoney(int amount)
    {
        currentMoney = amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }
}

