using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateIconsSingleUI : MonoBehaviour
{
    [SerializeField] Image img;
    public void SetKitchenObjectSO(KitchenObjectsSO kitchenObjectSO)
    {
        img.sprite = kitchenObjectSO.sprite;
    }
}
