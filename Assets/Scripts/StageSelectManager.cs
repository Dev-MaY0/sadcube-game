// Assets/Scripts/StageSelectManager.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StageSelectManager : MonoBehaviour
{
    [Header("ステージボタン（6個）")]
    public Button[] stageButtons;       // Stage1〜6のボタン
    public TMP_Text[] stageButtonTexts; // 各ボタンのテキスト

    [Header("UI")]
    public TMP_Text coinText;
    public TMP_Text levelText;
    public Button backButton;

    // 各ステージの解放に必要なレベル
    private int[] requiredLevels = { 1, 2, 3, 4, 5, 6 };

    void Start()
    {
        if (GameData.Instance == null)
        {
            var go = new GameObject("GameData");
            go.AddComponent<GameData>();
        }

        // ステージボタン登録
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageIndex = i + 1; // 1〜6
            if (stageButtons[i] != null)
                stageButtons[i].onClick.AddListener(() => OnStageButton(stageIndex));
        }

        if (backButton != null)
            backButton.onClick.AddListener(() => SceneManager.LoadScene("PetScene"));

        UpdateUI();
    }

    void UpdateUI()
    {
        if (GameData.Instance == null) return;

        int currentLevel = GameData.Instance.level;

        if (coinText != null) coinText.text = $"Coins: {GameData.Instance.coins}";
        if (levelText != null) levelText.text = $"Level: {currentLevel}";

        // 各ステージボタンの状態を更新
        for (int i = 0; i < stageButtons.Length; i++)
        {
            if (stageButtons[i] == null) continue;

            bool unlocked = currentLevel >= requiredLevels[i];

            stageButtons[i].interactable = unlocked;

            if (stageButtonTexts[i] != null)
            {
                stageButtonTexts[i].text = unlocked
                    ? $"Stage {i + 1}"
                    : $"Stage {i + 1}\nLv{requiredLevels[i]}~";
            }
        }
    }

    void OnStageButton(int stage)
    {
        // BattleSceneにステージ番号を渡す
        PlayerPrefs.SetInt("selectedStage", stage);
        SceneManager.LoadScene("BattleScene");
    }
}
