using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{

    public static event EventHandler OnAnyCut;

    public static void ResetStaticData()
    {
        OnAnyCut = null;
    }


    public event EventHandler OnCut;

    public event EventHandler<IHasProgress.OnProgressChangeEventArgs> OnProgressChange;
    


    [SerializeField] private CuttingReceipeSO[] cuttingReceipeSOArray;

    private int cuttingProgress;
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //there is no kitchen object here
            if (player.HasKitchenObject())
            {
                //player is carrying something
                if (HasReceipeWithInput(player.GetKitchenObjects().GetKitchenObjectsSO()))
                {
                    //player carrying something that can be cut
                    player.GetKitchenObjects().SetKitchenObjectParent(this);

                    CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOWithInput(GetKitchenObjects().GetKitchenObjectsSO());

                    cuttingProgress = 0;

                    OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
                    {
                        progressNormalized = (float)cuttingProgress / cuttingReceipeSO.cuttingProgressMax
                    });
                }
                
            }
            else
            {
                //player is carring nothing
            }

        }
        else
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
            }
            else
            { 
                //player is carring nothing
                GetKitchenObjects().SetKitchenObjectParent(player);
            }

        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasReceipeWithInput(GetKitchenObjects().GetKitchenObjectsSO()))
        {
            //There is a kitchen Object here AND it can be cut

            cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);

            CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOWithInput(GetKitchenObjects().GetKitchenObjectsSO());

            
            OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingReceipeSO.cuttingProgressMax
            });

            if (cuttingProgress >= cuttingReceipeSO.cuttingProgressMax)
            {

                KitchenObjectsSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObjects().GetKitchenObjectsSO());
                GetKitchenObjects().DestroySelf();

                KitchenObjects.SpawnKitchenObject(outputKitchenObjectSO, this);

            }
        }   
    }

    private bool HasReceipeWithInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOWithInput(inputKitchenObjectSO);
        return cuttingReceipeSO != null;
    }

    private KitchenObjectsSO GetOutputForInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOWithInput(inputKitchenObjectSO);
        if(cuttingReceipeSO != null)
        {
            return cuttingReceipeSO.output;
        }
        return null;
    }
    private CuttingReceipeSO GetCuttingReceipeSOWithInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        foreach (CuttingReceipeSO cuttingReceipeSO in cuttingReceipeSOArray)
        {
            if (cuttingReceipeSO.input == inputKitchenObjectSO)
            {
                return cuttingReceipeSO;
            }
        }
        return null;
    }
}
