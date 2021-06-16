using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    // Singleton instance
    private static AdManager _instance;

    // Different game ID's for different platforms
#if UNITY_ANDROID
    private string _gameID = "3714873";
#elif UNITY_IOS
    private string _gameID = "3714872";
#else
    private string _gameID = "3714874";
#endif

    // Ad type 
    string placementVideo = "video";
    string placementIdRewardedVideo = "rewardedVideo";
    string placementBannerID = "bannerPlacement";

    void Awake()
    {
        // Create Instance
        if (_instance == null)
        {
            _instance = this;
            Object.DontDestroyOnLoad(this);
        } // if
        // If instance already exists, destroy current GameObject
        else
        {
            Destroy(this.gameObject);
        } // else
    } // Awake

    // Start is called before the first frame update
    void Start()
    {
        Advertisement.AddListener(GetInstance());
        Advertisement.Initialize(_gameID, true);
        StartCoroutine(ShowBannerWhenInitialized());
    }

    /// <summary>
    /// Function that returns the instance of AdManager.
    /// </summary>
    /// <returns>AdManager instance of singleton</returns>
    public static AdManager GetInstance()
    {
        return _instance;
    } // GetInstance

    IEnumerator ShowBannerWhenInitialized()
    {
        if (!GameManager.GetInstance().GetPlayerData()._adsRemoved)
        {
            while (!Advertisement.isInitialized)
            {
                yield return new WaitForSeconds(0.5f);
            }
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Show(placementBannerID);
        } // if
    } // ShowBannerWhenInitialized

    public void ShowVideo()
    {
        if (!GameManager.GetInstance().GetPlayerData()._adsRemoved)
        {
            if (Advertisement.IsReady(placementVideo))
            {
                Advertisement.Show(placementVideo);
            } // if
            else
            {
                Debug.Log("Rewarded video is not ready at the moment! Try again later!");
            } // else
        } // if
    } // ShowVideo

    public void ShowRewardedVideo()
    {
        if (!GameManager.GetInstance().GetPlayerData()._adsRemoved)
        {
            if (Advertisement.IsReady(placementIdRewardedVideo))
            {
                Advertisement.Show(placementIdRewardedVideo);
            } // if
            else
            {
                Debug.Log("Rewarded video is not ready at the moment! Try again later!");
            } // else
        } // if
    } // ShowRewardedVideo

    public void OnUnityAdsDidFinish(string placementId, ShowResult res)
    {
        if (placementId == placementIdRewardedVideo)
        {
            if (res == ShowResult.Finished || res == ShowResult.Skipped)
            {
                GameManager.GetInstance().AdEnded();
            } // if
            else if (res == ShowResult.Failed)
            {
                Debug.LogWarning("The ad did not finish due to an error");
            } // else if
        } // if
    } // OnUnityAdsDidFinish

    public void OnUnityAdsReady(string placementId)
    {
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.LogError("Ad errored: " + message);
    }

    public void OnUnityAdsDidStart(string placementId) { }
} // AdManager
