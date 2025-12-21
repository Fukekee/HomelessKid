using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("交互设置")]
    [SerializeField] protected float interactionRange = 3f;
    [SerializeField] protected string interactionPrompt = "按E交互";
    
    protected bool isPlayerInRange = false;
    
    protected virtual void Update()
    {
        // 检测玩家是否在范围内
        CheckPlayerInRange();
    }
    
    private void CheckPlayerInRange()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        
        float distance = Vector3.Distance(transform.position, player.transform.position);
        isPlayerInRange = distance <= interactionRange;
    }
    
    public abstract void OnInteract();
    
    public virtual string GetInteractionPrompt()
    {
        return interactionPrompt;
    }
    
    public virtual bool CanInteract()
    {
        return isPlayerInRange;
    }
    
    // 在场景视图中显示交互范围（仅编辑器）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
