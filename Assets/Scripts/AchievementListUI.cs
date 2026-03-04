using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementListUI : MonoBehaviour
{
    [Header("実績一覧パネル")]
    public GameObject achievementPanel;
    public Transform contentParent;
    public GameObject achievementItemPrefab;
    public Button openButton;
    public Button closeButton;
    public TMP_Text progressText;

    void Start()
    {
        if (openButton != null) openButton.onClick.AddListener(OnOpenPanel);
        if (closeButton != null) closeButton.onClick.AddListener(OnClosePanel);
        if (achievementPanel != null) achievementPanel.SetActive(false);
    }

    public void OnOpenPanel()
    {
        if (achievementPanel != null) achievementPanel.SetActive(true);
        RefreshList();
    }

    public void OnClosePanel()
    {
        if (achievementPanel != null) achievementPanel.SetActive(false);
    }

    void RefreshList()
    {
        if (AchievementData.Instance == null)
        {
            Debug.LogWarning("AchievementDataが見つかりません");
            return;
        }

        // 既存アイテムを削除
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var achievement in AchievementData.Instance.achievements)
        {
            var item = Instantiate(achievementItemPrefab, contentParent);

            // 名前で直接取得（確実）
            var titleText = item.transform.Find("TitleText")?.GetComponent<TMP_Text>();
            var descText = item.transform.Find("DescText")?.GetComponent<TMP_Text>();

            if (titleText == null || descText == null)
            {
                Debug.LogWarning("AchievementItemにTitleTextまたはDescTextが見つかりません");
                continue;
            }

            if (achievement.isUnlocked)
            {
                titleText.text = $"★ {achievement.title}";  // 絵文字→★に変更
                descText.text = achievement.description;
                titleText.color = Color.white;
                descText.color = Color.white;
            }
            else
            {
                titleText.text = "???";
                descText.text = "まだ解放されていません";
                titleText.color = Color.gray;
                descText.color = Color.gray;
            }
        }

        // 進捗更新
        if (progressText != null)
        {
            int total = AchievementData.Instance.achievements.Count;
            int unlocked = AchievementData.Instance.UnlockedCount();
            progressText.text = $"{unlocked} / {total} 解放済み";
        }
    }
}
