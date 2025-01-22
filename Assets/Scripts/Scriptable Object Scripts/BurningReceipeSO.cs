using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BurningReceipeSO : ScriptableObject
{
    public KitchenObjectsSO input;
    public KitchenObjectsSO output;

    public float burningTimerMax;

}
