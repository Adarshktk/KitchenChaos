using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{

    public event EventHandler OnReceipeSpawned;
    public event EventHandler OnReceipeCompleted;
    public event EventHandler OnReceipeSuccess;
    public event EventHandler OnReceipeFailed;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private ReceipeListSO receipeListSO;

    private List<ReceipeSO> waitingReceipeSOList;
    private float spawnFloatTimer;
    private float spawnFloatTimerMax = 4f ;
    private int waitingReceipeMax = 4;
    private int successfulDeliveryAmount;


    private void Awake()
    {
        Instance = this;
        waitingReceipeSOList = new List<ReceipeSO>();
    }
    private void Update()
    {
        spawnFloatTimer -= Time.deltaTime;
        if(spawnFloatTimer <= 0f)
        {
            spawnFloatTimer = spawnFloatTimerMax;

            if(GameHandler.Instance.IsGamePlaying() && waitingReceipeSOList.Count < waitingReceipeMax)
            {
                ReceipeSO waitingReceipeSO = receipeListSO.receipeSOList[UnityEngine.Random.Range(0, receipeListSO.receipeSOList.Count)];
                //Debug.Log(waitingReceipeSO.receipeName);
                waitingReceipeSOList.Add(waitingReceipeSO);

                OnReceipeSpawned?.Invoke(this, EventArgs.Empty);

            }
            
        }
    }

    public void DeliverReceipe(PlateKitchenObject plateKitchenObject)
    {
        for(int i = 0; i < waitingReceipeSOList.Count; i++)
        {
            ReceipeSO waitingReceipeSO = waitingReceipeSOList[i];
            if(waitingReceipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSoList().Count)
            {
                //has the same number of ingredients
                bool plateContentMatchesReceipe = true;
                foreach(KitchenObjectsSO receipeKitchenObjectSO in waitingReceipeSO.kitchenObjectSOList)
                {
                    //cycling through all the ingredients in receipe
                    bool ingredientFound = false;
                    foreach(KitchenObjectsSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSoList())
                    {
                        //cycling through all ingredients in the plate
                        if(plateKitchenObjectSO == receipeKitchenObjectSO)
                        {
                            //ingredients Match!
                            ingredientFound = true;
                            break;

                        }
                    }
                    if (!ingredientFound)
                    {
                        //this reeipe ingredient was not found on the plate
                        plateContentMatchesReceipe = false;
                    }
                }
                if (plateContentMatchesReceipe)
                {
                    //player delivered the correct receipe
                    //Debug.Log("player delivered the correct receipe!");
                    successfulDeliveryAmount++;
                    waitingReceipeSOList.RemoveAt(i);

                    OnReceipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnReceipeSuccess?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }
        //no matches found 
        //player is not delivered a correct recipe
        //Debug.Log("player is not delivered a correct recipe");
        OnReceipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<ReceipeSO> GetWaitingReceipeSOList()
    {
        return waitingReceipeSOList;
    }

    public int GetSuccessfulDeliveryAmount()
    {
        return successfulDeliveryAmount;
    }

}
