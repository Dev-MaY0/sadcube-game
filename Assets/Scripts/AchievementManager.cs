// Assets/Scripts/AchievementManager.cs
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    private PetController petController;
    private int bonusReceiveCount = 0;

    void Start()
    {
        if (AchievementData.Instance == null)
        {
            var go = new GameObject("AchievementData");
            go.AddComponent<AchievementData>();
        }

        petController = FindObjectOfType<PetController>();
        bonusReceiveCount = PlayerPrefs.GetInt("bonusCount", 0);
    }

    void Update()
    {
        if (petController == null || !petController.status.isAlive) return;

        CheckPetAchievements();
        CheckCoinAchievements();
    }

    void CheckPetAchievements()
    {
        var s = petController.status;

        // 長生き（300秒）
        if (s.age >= 300f)
            AchievementData.Instance?.Unlock("pet_survived");

        // 体力100
        if (s.health >= 100f)
            AchievementData.Instance?.Unlock("full_health");

        // 幸福度100
        if (s.happiness >= 100f)
            AchievementData.Instance?.Unlock("max_happiness");
    }

    void CheckCoinAchievements()
    {
        if (GameData.Instance == null) return;
        int coins = GameData.Instance.coins;
        int level = GameData.Instance.level;

        if (coins >= 100) AchievementData.Instance?.Unlock("coins_100");
        if (coins >= 500) AchievementData.Instance?.Unlock("coins_500");
        if (coins >= 1000) AchievementData.Instance?.Unlock("coins_1000");
        if (level >= 2) AchievementData.Instance?.Unlock("level_2");
        if (level >= 6) AchievementData.Instance?.Unlock("level_max");
    }

    // --- 外部から呼ぶメソッド ---

    public void OnFeed()
    {
        AchievementData.Instance?.Unlock("first_feed");
    }

    public void OnPlay()
    {
        AchievementData.Instance?.Unlock("first_play");
    }

    public void OnSleep()
    {
        AchievementData.Instance?.Unlock("first_sleep");
    }

    public void OnMiniGameScore(int score)
    {
        if (score >= 100) AchievementData.Instance?.Unlock("minigame_100");
        if (score >= 300) AchievementData.Instance?.Unlock("minigame_300");
        if (score >= 500) AchievementData.Instance?.Unlock("minigame_500");
    }

    public void OnBonusReceived()
    {
        AchievementData.Instance?.Unlock("bonus_first");
        bonusReceiveCount++;
        PlayerPrefs.SetInt("bonusCount", bonusReceiveCount);
        if (bonusReceiveCount >= 5)
            AchievementData.Instance?.Unlock("bonus_5");
    }
}
