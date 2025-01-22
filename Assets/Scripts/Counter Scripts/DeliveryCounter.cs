using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    public override void Interact(Player player)
    {
        
        if (player.GetKitchenObjects().TryGetPlate(out PlateKitchenObject plateKitchenObject))
        {
            //only accepts plates
            DeliveryManager.Instance.DeliverReceipe(plateKitchenObject);

            player.GetKitchenObjects().DestroySelf();
        }
        
    }
}
