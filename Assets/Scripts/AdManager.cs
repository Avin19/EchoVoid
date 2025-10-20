using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    void Awake() => Instance = this;

    public void ShowRewardedAd() => Debug.Log("🎬 [AdManager] Rewarded Ad removed (stub called).");
    public void ShowInterstitial() => Debug.Log("🎬 [AdManager] Interstitial Ad removed (stub called).");
    public void ShowBanner() => Debug.Log("🎬 [AdManager] Banner Ad removed (stub called).");
}
