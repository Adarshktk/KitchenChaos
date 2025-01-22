using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{

    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    [SerializeField]private KitchenObjectsSO plateObjectSO;
    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4.0f;
    private int platesSpawnAmount;
    private int platesSpawnAmountMax = 4;

    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;
        if(spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;
            if(GameHandler.Instance.IsGamePlaying() && platesSpawnAmount < platesSpawnAmountMax)
            {
                platesSpawnAmount++;
                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
            
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            //player is empty handed
            if(platesSpawnAmount > 0)
            {
                //there is at least one plate is there
                platesSpawnAmount--;
                KitchenObjects.SpawnKitchenObject(plateObjectSO, player);

                OnPlateRemoved?.Invoke(this, EventArgs.Empty);

            }
        }
    }

}