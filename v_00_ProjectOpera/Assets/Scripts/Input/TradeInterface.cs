using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeInterface : MonoBehaviour
{
    public void OnClickSellCrops()
    {
        if(GameManager.Instance.PlayerCrops > 0)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, ResourceTypes.CROP);
            GameManager.Instance.AddCreditsToPlayer(5);
        }
    }

    public void OnClickSellParts()
    {
        if(GameManager.Instance.PlayerParts > 0)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, ResourceTypes.PART);
            GameManager.Instance.AddCreditsToPlayer(5);
        }
    }

    public void OnClickSellNitrogen()
    {
        if(GameManager.Instance.PlayerNitrogen > 0)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, ResourceTypes.NITROGEN);
            GameManager.Instance.AddCreditsToPlayer(5);
        }
    }
}
