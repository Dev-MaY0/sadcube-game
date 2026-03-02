using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelUpManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text coinText;
    public TMP_Text levelText;
    public TMP_Text levelUpCostText;
    public TMP_Text levelUpEffectText;
    public Button levelUpButton;
    public Button miniGameButton;   // ← 追加
    public Button titleButton;      // ← 追加

    [Header("レベルごとの効果（PetStatusに渡す）")]
    public float[] hungerDecayReduction = { 0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };
    public float[] happinessBonus = { 0f, 5f, 10f, 15f, 20f, 25f };

    private PetController petController;

    void Start()
    {
        if (GameData.Instance == null)
        {
            var go = new GameObject("GameData");
            go.AddComponent<GameData>();
        }

        petController = FindObjectOfType<PetController>();

        // ボタンをスクリプトから登録
        if (levelUpButton != null) levelUpButton.onClick.AddListener(OnLevelUpButton);
        if (miniGameButton != null) miniGameButton.onClick.AddListener(OnMiniGameButton);
        if (titleButton != null) titleButton.onClick.AddListener(OnTitleButton);

        ApplyLevelEffects();
        UpdateUI();
    }

    public void OnLevelUpButton()
    {
        if (GameData.Instance == null) return;
        if (!GameData.Instance.TryLevelUp()) return;
        ApplyLevelEffects();
        UpdateUI();
        StartCoroutine(ShowLevelUpEffect());
    }

    void ApplyLevelEffects()
    {
        if (petController == null || GameData.Instance == null) return;
        int lv = GameData.Instance.level - 1;

        if (lv < hungerDecayReduction.Length)
            petController.status.hungerDecayRate =
                Mathf.Max(0.2f, 1.0f - hungerDecayReduction[lv]);

        if (lv < happinessBonus.Length)
            petController.status.happiness =
                Mathf.Min(100f, petController.status.happiness + happinessBonus[lv]);
    }

    void UpdateUI()
    {
        if (GameData.Instance == null) return;
        int lv = GameData.Instance.level;
        int cost = GameData.Instance.GetLevelUpCost();

        if (coinText) coinText.text = $"Coins: {GameData.Instance.coins}";
        if (levelText) levelText.text = $"Level: {lv}";
        if (levelUpCostText) levelUpCostText.text = $"Next: {cost} coins";

        if (levelUpEffectText)
        {
            int idx = lv - 1;
            float decay = idx < hungerDecayReduction.Length ? hungerDecayReduction[idx] * 100 : 0;
            float happy = idx < happinessBonus.Length ? happinessBonus[idx] : 0;
            levelUpEffectText.text =
                $"Hunger Decay: -{decay:0}%\nHappiness Bonus: +{happy:0}";
        }

        if (levelUpButton != null)
            levelUpButton.interactable = GameData.Instance.CanLevelUp();
    }

    System.Collections.IEnumerator ShowLevelUpEffect()
    {
        if (levelUpEffectText)
        {
            var original = levelUpEffectText.color;
            levelUpEffectText.color = Color.yellow;
            yield return new WaitForSeconds(0.5f);
            levelUpEffectText.color = original;
        }
        UpdateUI();
    }

    public void OnMiniGameButton() => SceneManager.LoadScene("MiniGameScene");
    public void OnTitleButton() => SceneManager.LoadScene("TitleScene");
}