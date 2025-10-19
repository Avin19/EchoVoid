using UnityEngine;
using Unity.Services.LevelPlay;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private LevelPlayBannerAd bannerAd;
    private LevelPlayInterstitialAd interstitialAd;
    private LevelPlayRewardedAd rewardedVideoAd;

    private bool isInitialized = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        InitializeLevelPlay();
    }

    // 🚀 Initialize SDK
    private void InitializeLevelPlay()
    {
        Debug.Log("[AdManager] Initializing LevelPlay SDK...");

        // Optional: enable built-in integration test UI
        LevelPlay.SetMetaData("is_test_suite", "enable");

        // Register init callbacks
        LevelPlay.OnInitSuccess += OnInitSuccess;
        LevelPlay.OnInitFailed += OnInitFailed;

        // Initialize SDK using your app key
        LevelPlay.Init(AdConfig.AppKey);

        // You can verify SDK status with:
        Debug.Log($"[AdManager] Using Unity LevelPlay version: {LevelPlay.UnityVersion}");
    }

    private void OnInitSuccess(LevelPlayConfiguration config)
    {
        Debug.Log("[AdManager] ✅ LevelPlay initialized successfully!");
        isInitialized = true;
        SetupAds();
    }

    private void OnInitFailed(LevelPlayInitError error)
    {
        Debug.LogError($"[AdManager] ❌ LevelPlay initialization failed: {error}");
    }

    // 🎯 Setup all ad formats
    private void SetupAds()
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[AdManager] Cannot set up ads — SDK not initialized!");
            return;
        }

        // --- Rewarded ---
        rewardedVideoAd = new LevelPlayRewardedAd(AdConfig.RewardedVideoAdUnitId);
        rewardedVideoAd.OnAdLoaded += OnRewardedLoaded;
        rewardedVideoAd.OnAdLoadFailed += OnRewardedLoadFailed;
        rewardedVideoAd.OnAdRewarded += OnRewardedEarned;
        rewardedVideoAd.OnAdClosed += OnRewardedClosed;
        rewardedVideoAd.LoadAd();

        // --- Interstitial ---
        interstitialAd = new LevelPlayInterstitialAd(AdConfig.InterstitalAdUnitId);
        interstitialAd.OnAdLoaded += OnInterstitialLoaded;
        interstitialAd.OnAdLoadFailed += OnInterstitialLoadFailed;
        interstitialAd.OnAdClosed += OnInterstitialClosed;
        interstitialAd.LoadAd();

        // --- Banner ---
        bannerAd = new LevelPlayBannerAd(AdConfig.BannerAdUnitId);
        bannerAd.OnAdLoaded += OnBannerLoaded;
        bannerAd.OnAdLoadFailed += OnBannerLoadFailed;

        // Optionally preload a banner (e.g., top or bottom)
        bannerAd.LoadAd();
        Debug.Log("[AdManager] All ad formats configured and loading...");
    }

    // -------------------- 🎬 SHOW METHODS --------------------

    public void ShowRewardedAd()
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[AdManager] SDK not initialized yet!");
            return;
        }

        if (rewardedVideoAd != null && rewardedVideoAd.IsAdReady())
        {
            Debug.Log("[AdManager] Showing Rewarded Ad...");
            rewardedVideoAd.ShowAd();
        }
        else
        {
            Debug.Log("[AdManager] Rewarded Ad not ready — loading again...");
            rewardedVideoAd?.LoadAd();
        }
    }

    public void ShowInterstitialAd()
    {
        if (!isInitialized) return;

        if (interstitialAd != null && interstitialAd.IsAdReady())
        {
            Debug.Log("[AdManager] Showing Interstitial Ad...");
            interstitialAd.ShowAd();
        }
        else
        {
            Debug.Log("[AdManager] Interstitial Ad not ready — loading again...");
            interstitialAd?.LoadAd();
        }
    }

    public void ShowBannerAd()
    {
        bannerAd?.LoadAd();
    }

    public void HideBannerAd()
    {
        bannerAd?.HideAd();
    }
    public bool IsRewardedReady()
    {
        return rewardedVideoAd != null && rewardedVideoAd.IsAdReady();
    }
    private System.Action onRewardGrantedCallback;

    public void ShowRewardedAdWithCallback(System.Action rewardCallback)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[AdManager] SDK not initialized yet!");
            return;
        }

        if (rewardedVideoAd != null && rewardedVideoAd.IsAdReady())
        {
            Debug.Log("[AdManager] Showing Rewarded Ad with callback...");
            onRewardGrantedCallback = rewardCallback;
            rewardedVideoAd.ShowAd();
        }
        else
        {
            Debug.Log("[AdManager] Rewarded Ad not ready — loading...");
            rewardedVideoAd?.LoadAd();
        }
    }

    // -------------------- 🧩 EVENTS --------------------

    private void OnRewardedLoaded(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[AdManager] Rewarded ad loaded successfully.");
    }

    private void OnRewardedLoadFailed(LevelPlayAdError error)
    {
        Debug.LogWarning($"[AdManager] Rewarded load failed: {error}");
    }

    private void OnRewardedEarned(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        Debug.Log("[AdManager] 🎁 Rewarded Ad finished, granting reward...");
        if (onRewardGrantedCallback != null)
        {
            onRewardGrantedCallback.Invoke();
            onRewardGrantedCallback = null;
        }
        else
        {
            GameManager.Instance?.ContinueAfterAd();
        }
    }

    private void OnRewardedClosed(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[AdManager] Rewarded ad closed — reloading...");
        rewardedVideoAd?.LoadAd();
    }

    private void OnInterstitialLoaded(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[AdManager] Interstitial ad loaded successfully.");
    }

    private void OnInterstitialLoadFailed(LevelPlayAdError error)
    {
        Debug.LogWarning($"[AdManager] Interstitial load failed: {error}");
    }

    private void OnInterstitialClosed(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[AdManager] Interstitial closed — reloading...");
        interstitialAd?.LoadAd();
    }

    private void OnBannerLoaded(LevelPlayAdInfo adInfo)
    {
        Debug.Log("[AdManager] Banner ad loaded successfully.");
    }

    private void OnBannerLoadFailed(LevelPlayAdError error)
    {
        Debug.LogWarning($"[AdManager] Banner load failed: {error}");
    }

    private void OnDisable()
    {
        bannerAd?.DestroyAd();
        interstitialAd?.DestroyAd();
    }
}
