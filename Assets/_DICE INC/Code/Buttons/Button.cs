using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Button : MonoBehaviour
{
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private float moveDistance;
    
    [ShowInInspector, ReadOnly] private protected bool isActive;
   
    void OnMouseDown()
    {
        if (!isActive) return;
        
        spriteTransform.localPosition = new Vector3(0, -moveDistance, 0);
    }

    void OnMouseUp()
    {
        if (!isActive) return;
        
        spriteTransform.localPosition = new Vector3(0, 0, 0);
        ButtonAction();
    }
    
    protected abstract void ButtonAction();

}
