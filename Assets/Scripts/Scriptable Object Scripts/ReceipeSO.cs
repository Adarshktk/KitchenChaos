using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ReceipeSO : ScriptableObject
{
    public List<KitchenObjectsSO> kitchenObjectSOList;
    public string receipeName;
}
