using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform receipeTemplate;

    private void Awake()
    {
        
        receipeTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnReceipeSpawned += DeliveryManager_OnReceipeSpawned;
        DeliveryManager.Instance.OnReceipeCompleted += DeliveryManager_OnReceipeCompleted;
        
        UpdateVisual();
    }

    private void DeliveryManager_OnReceipeCompleted(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void DeliveryManager_OnReceipeSpawned(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach(Transform child in container)
        {
            if (child == receipeTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach(ReceipeSO receipeSO in DeliveryManager.Instance.GetWaitingReceipeSOList())
        {
            Transform receipeTransform = Instantiate(receipeTemplate, container);
            receipeTransform.gameObject.SetActive(true);
            receipeTransform.GetComponent<DeliveryManagerSingleUI>().SetReceipeSO(receipeSO);
        }

    }



}
