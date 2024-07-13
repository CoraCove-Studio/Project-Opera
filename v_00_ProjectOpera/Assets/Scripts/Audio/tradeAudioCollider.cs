using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tradeAudioCollider : MonoBehaviour
{
    [SerializeField] private TradeInterface tradeInterface;

    private void Start()
    {
        if (tradeInterface == null)
        {
            tradeInterface = GetComponentInParent<TradeInterface>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tradeInterface.PlayerLeftCollider();
        }
    }
}
