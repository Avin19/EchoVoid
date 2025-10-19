public static class AdConfig
{
#if UNITY_ANDROID
    public const string AppKey = "23f0d7ac5";
    public const string RewardedVideoAdUnitId = "YOUR_ANDROID_REWARDED_AD_UNIT_ID";
    public const string InterstitalAdUnitId = "YOUR_ANDROID_INTERSTITIAL_AD_UNIT_ID";
    public const string BannerAdUnitId = "YOUR_ANDROID_BANNER_AD_UNIT_ID";
#elif UNITY_IOS
    public const string AppKey = "YOUR_IOS_APP_KEY";
    public const string RewardedVideoAdUnitId = "YOUR_IOS_REWARDED_AD_UNIT_ID";
    public const string InterstitalAdUnitId = "YOUR_IOS_INTERSTITIAL_AD_UNIT_ID";
    public const string BannerAdUnitId = "YOUR_IOS_BANNER_AD_UNIT_ID";
#endif
}
