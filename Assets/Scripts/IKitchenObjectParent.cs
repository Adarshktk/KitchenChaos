using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKitchenObjectParent
{
    public Transform GetKitchenObjectFollowTransform();
    public void SetKitchenObject(KitchenObjects kitchenObjects);
    public KitchenObjects GetKitchenObjects();
    public void ClearKitchenObjects();
    public bool HasKitchenObject();
}
