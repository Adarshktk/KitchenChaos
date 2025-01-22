using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{

    public event EventHandler<IHasProgress.OnProgressChangeEventArgs> OnProgressChange;

    public event EventHandler<OnStateChangeEventArgs> OnStateChanged;
    public class OnStateChangeEventArgs : EventArgs
    {
        public State state;
    }
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingReceipeSO[] fryingReceipeSOArray;
    [SerializeField] private BurningReceipeSO[] burningReceipeSOArray;
    private State state;
    private float fryingTimer;
    private float burningTimer;
    private FryingReceipeSO fryingReceipeSO;
    private BurningReceipeSO burningReceipeSO;


    public void Start()
    {
        state = State.Idle;
    }
    public void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.Idle:
                    break;

                case State.Frying:

                    fryingTimer += Time.deltaTime;

                    OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
                    {
                        progressNormalized = fryingTimer / fryingReceipeSO.fryingTimerMax
                    });

                    if (fryingTimer > fryingReceipeSO.fryingTimerMax)
                    {
                        //fried
                        GetKitchenObjects().DestroySelf();
                        KitchenObjects.SpawnKitchenObject(fryingReceipeSO.output, this);
                        

                        burningReceipeSO = GetBurningReceipeSOWithInput(GetKitchenObjects().GetKitchenObjectsSO());

                        state = State.Fried;
                        burningTimer = 0f;
                        OnStateChanged?.Invoke(this, new OnStateChangeEventArgs
                        {
                            state = state
                        });

                    }
                    break;
                case State.Fried:

                    burningTimer += Time.deltaTime;

                    OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
                    {
                        progressNormalized = burningTimer / burningReceipeSO.burningTimerMax
                    });

                    if (burningTimer > burningReceipeSO.burningTimerMax)
                    {
                        //fried
                        GetKitchenObjects().DestroySelf();
                        KitchenObjects.SpawnKitchenObject(burningReceipeSO.output, this);
                        
                        state = State.Burned;
                        OnStateChanged?.Invoke(this, new OnStateChangeEventArgs
                        {
                            state = state
                        });
                        OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                    break;
                case State.Burned:
                    break;

            }
            
        }
    }

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
                    //player carrying something that can be fried
                    player.GetKitchenObjects().SetKitchenObjectParent(this);

                    fryingReceipeSO = GetFryingReceipeSOWithInput(GetKitchenObjects().GetKitchenObjectsSO());

                    
                    state = State.Frying;
                    fryingTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangeEventArgs
                    {
                        state = state
                    });
                    
                    OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
                    {
                        progressNormalized = fryingTimer / fryingReceipeSO.fryingTimerMax
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

                        state = State.Idle;
                        OnStateChanged?.Invoke(this, new OnStateChangeEventArgs
                        {
                            state = state
                        });
                        OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }

                }
            }
            else
            {
                //player is carring nothing
                GetKitchenObjects().SetKitchenObjectParent(player);

                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangeEventArgs
                {
                    state = state
                });
                OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
                {
                    progressNormalized = 0f
                });
            }

        }
    }

    private bool HasReceipeWithInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        FryingReceipeSO fryingReceipeSO = GetFryingReceipeSOWithInput(inputKitchenObjectSO);
        return fryingReceipeSO != null;
    }

    private KitchenObjectsSO GetOutputForInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        FryingReceipeSO fryingReceipeSO = GetFryingReceipeSOWithInput(inputKitchenObjectSO);
        if (fryingReceipeSO != null)
        {
            return fryingReceipeSO.output;
        }
        return null;
    }
    private FryingReceipeSO GetFryingReceipeSOWithInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        foreach (FryingReceipeSO fryingReceipeSO in fryingReceipeSOArray)
        {
            if (fryingReceipeSO.input == inputKitchenObjectSO)
            {
                return fryingReceipeSO;
            }
        }
        return null;
    }

    private BurningReceipeSO GetBurningReceipeSOWithInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        foreach (BurningReceipeSO buringReceipeSO in burningReceipeSOArray)
        {
            if (buringReceipeSO.input == inputKitchenObjectSO)
            {
                return buringReceipeSO;
            }
        }
        return null;
    }

    public bool IsFried()
    {
        return state == State.Fried;
    }
}



