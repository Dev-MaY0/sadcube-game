// Assets/Scripts/AchievementNotification.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AchievementNotification : MonoBehaviour
{
    public static AchievementNotification Instance { get; private set; }

    [Header("通知UI")]
    public GameObject notificationPanel;  // 通知パネル
    public TMP_Text iconText;           // 絵文字アイコン
    public TMP_Text titleText;          // 「実績解放！」
    public TMP_Text achievementTitle;   // 称号名
    public TMP_Text descriptionText;    // 説明文

    [Header("設定")]
    public float displayTime = 3f;        // 表示秒数

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (notificationPanel != null)
            notificationPanel.SetActive(false);

        // 実績解放イベントを購読
        if (AchievementData.Instance != null)
            AchievementData.Instance.OnAchievementUnlocked += ShowNotification;
    }

    public void ShowNotification(Achievement achievement)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayNotification(achievement));
    }

    IEnumerator DisplayNotification(Achievement achievement)
    {
        if (notificationPanel == null) yield break;

        if (iconText) iconText.text = achievement.iconEmoji;
        if (titleText) titleText.text = "実績解放！";
        if (achievementTitle) achievementTitle.text = achievement.title;
        if (descriptionText) descriptionText.text = achievement.description;

        notificationPanel.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        notificationPanel.SetActive(false);
    }
}
