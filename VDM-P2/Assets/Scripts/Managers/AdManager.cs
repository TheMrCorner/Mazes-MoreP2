using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    private string gameID = "1234567";

    // Ad type 
    string placementVideo = "video";
    string placementIdRewardedVideo = "rewardedVideo";
    string placementBannerID = "bannerPlacement";

    // Start is called before the first frame update
    void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameID, true);
        StartCoroutine(ShowBannerWhenInitialized());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ShowBannerWhenInitialized()
    {
        while (!Advertisement.isInitialized)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
        Advertisement.Banner.Show(placementBannerID);
    }

    public void ShowRewardedVideo()
    {
        if (Advertisement.IsReady(placementIdRewardedVideo))
        {
            Advertisement.Show(placementIdRewardedVideo);
        }
        else
        {
            Debug.Log("Rewarded video is not ready at the moment! Try again later!");
        }
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult res)
    {
        if (res == ShowResult.Finished || res == ShowResult.Skipped)
        {
            GameManager.GetInstance().AdEnded();
        }
        else if (res == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error");
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == placementIdRewardedVideo)
        {

        }
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.LogError("Ad errored: " + message);
    }

    public void OnUnityAdsDidStart(string placementId) { }
}
