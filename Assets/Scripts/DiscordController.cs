using Discord;
using UnityEngine;

public class DiscordController : MonoBehaviour
{
    // 【重要】ここにDeveloper PortalでコピーしたApplication IDを貼り付けます
    // 例: public long applicationId = 123456789012345678;
    public long applicationId = 1479805837592756244;

    private Discord.Discord discord;

    void Start()
    {
        try
        {
            // Discordとの接続初期化
            discord = new Discord.Discord(applicationId, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
            UpdateStatus();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Discordの起動に失敗しました。アプリが立ち上がっていますか？: " + e.Message);
        }
    }

    void UpdateStatus()
    {
        if (discord == null) return;

        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity
        {
            State = "SadCubeゲームをプレイ中",
            Details = "メインステージ",
            Assets = {
                LargeImage = "icon_main", // Portalで設定した名前があれば反映されます
                LargeText = "My Project"
            }
        };

        activityManager.UpdateActivity(activity, (res) => {
            if (res == Discord.Result.Ok) Debug.Log("Discordステータスを更新しました！");
        });
    }

    void Update()
    {
        // Discord側へ情報を送り続けるために必要です
        if (discord != null) discord.RunCallbacks();
    }

    void OnApplicationQuit()
    {
        if (discord != null) discord.Dispose();
    }
}
