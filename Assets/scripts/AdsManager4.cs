using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager4 : MonoBehaviour, IUnityAdsListener
{
    public static AdsManager4 instance;
    string Ad_ID = "Rewarded_video";
    string Ad_ID_Reward = "Rewarded_Android";

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize("4483387");
        if(Advertisement.IsReady("BannerAd"))
        {
            showBanner();
        }
        Advertisement.Load(Ad_ID);
    }
    
    void showBanner()
    {
        if(Advertisement.IsReady("BannerAd"))
        {
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Show("BannerAd");
        }
        else
        {
            StartCoroutine(RepeatBannerAd());
        }
    }
    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }
    public void showInterstitial(string AD_ID)
    {
        if(Advertisement.IsReady(AD_ID))
        {
            Debug.Log("showing int ad");
            Advertisement.Show(AD_ID);
        }
        else
        {
            Debug.Log("Int ad not ready");
        }
    }
    public void showRewardedVideoAd(string AD_ID)
    {
        if(Advertisement.IsReady(AD_ID))
        {
            Advertisement.Show(AD_ID);
        }
    }
    IEnumerator RepeatBannerAd()
    {
        yield return new WaitForSeconds(1);
        showBanner();
    }
    
    // Update is called once per frame
    
    public void OnUnityAdsDidError(string message)
    {
        Debug.Log("Error" + message);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if(placementId == Ad_ID_Reward)
        {
            int temp = GameManager.instance.timeLeft;
            GameManager.instance.timeLeft = temp + 610;
        }
        else if(placementId == Ad_ID)
        {
            Debug.Log("User rewarded");
        }
        
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Ad started");
    }

    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("Ad is ready");
    }
}
