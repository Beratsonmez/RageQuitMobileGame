using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class BannerScripts : MonoBehaviour
{
    private BannerView bannerView;
    private string bannerId = "ca-app-pub-2209097412803177/4407769166";

    public static BannerScripts Instance;

    private void Awake()
    {
        if (Instance== null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        RequestBanner();
        StartCoroutine(CloseIt());
    }

    
    void Update()
    {
        
    }

    public void RequestBanner()
    {
        bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
        AdRequest reguest = new AdRequest.Builder().Build();
        bannerView.LoadAd(reguest);
    }

    public void HideBanner()
    {
        bannerView.Destroy();
    }

    IEnumerator CloseIt()
    {
        yield return new WaitForSeconds(4);
        HideBanner();
    }
}
