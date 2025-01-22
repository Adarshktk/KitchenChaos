using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    
    

    public override void Interact(Player player){

        if (!HasKitchenObject())
        {
            //there is no kitchen object here
            if (player.HasKitchenObject())
            {
                //player is carrying something
                player.GetKitchenObjects().SetKitchenObjectParent(this);
            }
            else
            {
                //player is carring nothing
            }

        }else
        {
            //there is a kitchenobject here
            if (player.HasKitchenObject())
            {
                //player is carring something
                if (player.GetKitchenObjects().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObjects().GetKitchenObjectsSO()))
                    {
                        GetKitchenObjects().DestroySelf();
                    }
                }
                else
                {
                    //player is not carrying a plate but something else
                    if (GetKitchenObjects().TryGetPlate(out plateKitchenObject))
                    {
                        //clearcounter is holding a plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObjects().GetKitchenObjectsSO()))
                        {
                            player.GetKitchenObjects().DestroySelf();
                        }

                    }
                }

            }
            else
            {
                //player is carring nothing
                GetKitchenObjects().SetKitchenObjectParent(player);
            }

        }
    }

}
