using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePurchase : MonoBehaviour
{
    public void PurchaseNoAdds()
    {
#if UNITY_ANDROID
        GameManager.GetInstance().RemoveAds();
#endif
#if UNITY_EDITOR
        Debug.LogWarning("Fake Purchase is only for Android");
#endif
    }

    public void PurchaseHints(int nHints)
    {
#if UNITY_ANDROID
        GameManager.GetInstance().AddHints(nHints);
#endif
#if UNITY_EDITOR
        Debug.LogWarning("Fake Purchase is only for Android");
#endif
    }
}
