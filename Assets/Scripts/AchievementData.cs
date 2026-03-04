// Assets/Scripts/AchievementData.cs
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Achievement
{
    public string id;           // 実績ID（ユニークなキー）
    public string title;        // 称号名
    public string description;  // 条件の説明
    public bool isUnlocked;   // 解放済みかどうか
    public string iconEmoji;    // アイコン絵文字

    public Achievement(string id, string title, string description, string iconEmoji)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.iconEmoji = iconEmoji;
        this.isUnlocked = false;
    }
}

public class AchievementData : MonoBehaviour
{
    public static AchievementData Instance { get; private set; }

    // 全実績リスト
    public List<Achievement> achievements = new List<Achievement>();

    // 実績解放時のイベント
    public event Action<Achievement> OnAchievementUnlocked;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitAchievements();
        Load();
    }

    void InitAchievements()
    {
        achievements = new List<Achievement>
{
    new Achievement("first_feed",    "はじめての食事",   "初めてごはんをあげた",      ""),
    new Achievement("first_play",    "はじめてのあそび", "初めて遊んであげた",        ""),
    new Achievement("first_sleep",   "はじめてのねむり", "初めて寝かせた",            ""),
    new Achievement("pet_survived",  "長生き",           "ペットが300秒生存した",     ""),
    new Achievement("full_health",   "健康優良",         "体力を100に保った",         ""),
    new Achievement("max_happiness", "超ハッピー",       "幸福度を100にした",         ""),
    new Achievement("coins_100",     "小金持ち",         "コインを100枚貯めた",       ""),
    new Achievement("coins_500",     "大金持ち",         "コインを500枚貯めた",       ""),
    new Achievement("coins_1000",    "大富豪",           "コインを1000枚貯めた",      ""),
    new Achievement("level_2",       "成長中",           "レベル2になった",           ""),
    new Achievement("level_max",     "マスター",         "最大レベルに達した",        ""),
    new Achievement("minigame_100",  "ゲーム初心者",     "ミニゲームで100点達成",     ""),
    new Achievement("minigame_300",  "ゲーム上級者",     "ミニゲームで300点達成",     ""),
    new Achievement("minigame_500",  "スイカマスター",   "ミニゲームで500点達成",     ""),
    new Achievement("bonus_first",   "はじめてのボーナス","ログインボーナスを初めて受け取った",""),
    new Achievement("bonus_5",       "常連さん",         "ログインボーナスを5回受け取った",  ""),
};
    }

    /// <summary>IDで実績を解放する</summary>
    public void Unlock(string id)
    {
        var achievement = achievements.Find(a => a.id == id);
        if (achievement == null || achievement.isUnlocked) return;

        achievement.isUnlocked = true;
        Save();
        OnAchievementUnlocked?.Invoke(achievement);
        Debug.Log($"実績解放: {achievement.title}");
    }

    /// <summary>解放済みかどうか確認</summary>
    public bool IsUnlocked(string id)
    {
        var achievement = achievements.Find(a => a.id == id);
        return achievement != null && achievement.isUnlocked;
    }

    // --- セーブ・ロード ---
    public void Save()
    {
        foreach (var a in achievements)
            PlayerPrefs.SetInt($"ach_{a.id}", a.isUnlocked ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        foreach (var a in achievements)
            a.isUnlocked = PlayerPrefs.GetInt($"ach_{a.id}", 0) == 1;
    }

    // 解放済み実績の数
    public int UnlockedCount()
    {
        int count = 0;
        foreach (var a in achievements)
            if (a.isUnlocked) count++;
        return count;
    }
}