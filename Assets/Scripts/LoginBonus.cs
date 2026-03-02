using UnityEngine;
using System;

public class LoginBonus : MonoBehaviour
{
    // 何時間に1回か
    const float BONUS_INTERVAL_HOURS = 1f;
    const int BONUS_COINS = 30;  // もらえるコイン数

    public static LoginBonus Instance { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>ボーナスを受け取れる状態かどうか</summary>
    public bool CanReceiveBonus()
    {
        string lastStr = PlayerPrefs.GetString("lastBonusTime", "");
        if (string.IsNullOrEmpty(lastStr)) return true;

        DateTime lastTime = DateTime.Parse(lastStr);
        TimeSpan elapsed = DateTime.Now - lastTime;

        return elapsed.TotalHours >= BONUS_INTERVAL_HOURS;
    }

    /// <summary>次のボーナスまでの残り時間を文字列で返す</summary>
    public string GetRemainingTime()
    {
        string lastStr = PlayerPrefs.GetString("lastBonusTime", "");
        if (string.IsNullOrEmpty(lastStr)) return "00:00";

        DateTime lastTime = DateTime.Parse(lastStr);
        DateTime nextTime = lastTime.AddHours(BONUS_INTERVAL_HOURS);
        TimeSpan remaining = nextTime - DateTime.Now;

        if (remaining.TotalSeconds <= 0) return "00:00";

        return $"{remaining.Minutes:00}:{remaining.Seconds:00}";
    }

    /// <summary>ボーナスを受け取る（コインを加算して時刻を保存）</summary>
    public int ReceiveBonus()
    {
        if (!CanReceiveBonus()) return 0;

        PlayerPrefs.SetString("lastBonusTime", DateTime.Now.ToString());
        PlayerPrefs.Save();

        GameData.Instance?.AddCoins(BONUS_COINS);
        return BONUS_COINS;
    }
}
