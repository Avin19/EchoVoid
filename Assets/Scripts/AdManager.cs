using UnityEngine;
using Unity.Services.LevelPlay;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

#if UNITY_ANDROID
    private string appKey = "YOUR_ANDROID_APP_KEY_FROM_LEVELPLAY_DASHBOARD";
#elif UNITY_IOS
    private string appKey = "YOUR_IOS_APP_KEY_FROM_LEVELPLAY_DASHBOARD";
#endif

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

    async void Start()
    {
        await InitializeLevelPlay();
    }

    private async System.Threading.Tasks.Task InitializeLevelPlay()
    {
        Debug.Log("🚀 Initializing Unity LevelPlay SDK v9...");

        // ✅ Register event listeners first
        LevelPlay.OnInitSuccess += OnInitSuccess;
        LevelPlay.OnInitFailed += OnInitFailed;

        // Optional — enable integration test suite (for verification)
        LevelPlay.SetMetaData("is_test_suite", "enable");

        // SDK initialization
        LevelPlay.Init(appKey, SystemInfo.deviceUniqueIdentifier);

        await System.Threading.Tasks.Task.Delay(1000);
    }

    private void OnInitSuccess()
    {
        isInitialized = true;
        Debug.Log("✅ LevelPlay SDK initialized successfully!");

        // Launch the LevelPlay Integration Test Suite
        LevelPlay.LaunchTestSuite();

        // Preload rewarded ads after init
        LevelPlayRewarded.Load();
        LevelPlayRewarded.OnAdLoaded += OnRewardedLoaded;
        LevelPlayRewarded.OnAdRewarded += OnRewardedEarned;
    }

    private void OnInitFailed(LevelPlayInitError error)
    {
        Debug.LogError($"❌ LevelPlay initialization failed: {error}");
    }

    public void ShowRewardedAd()
    {
        if (!isInitialized)
        {
            Debug.LogWarning("⚠️ SDK not initialized yet!");
            return;
        }

        if (LevelPlayRewarded.IsAdAvailable())
        {
            Debug.Log("🎬 Showing rewarded ad...");
            LevelPlayRewarded.Show();
        }
        else
        {
            Debug.Log("⚠️ Rewarded ad not ready — loading...");
            LevelPlayRewarded.Load();
        }
    }

    private void OnRewardedLoaded(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"✅ Rewarded ad loaded: {adInfo.AdUnitId}");
    }

    private void OnRewardedEarned(LevelPlayAdInfo adInfo)
    {
        Debug.Log("🏆 Player rewarded!");
        GameManager.Instance?.ContinueAfterAd();
    }
}
