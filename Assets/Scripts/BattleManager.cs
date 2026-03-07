using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    [Header("ペット")]
    public Image petImage;
    public Slider petHpSlider;
    public TMP_Text petHpText;
    public Sprite[] petSprites; // Child/Adult/Elderの順

    [Header("敵")]
    public Image enemyImage;
    public Slider enemyHpSlider;
    public TMP_Text enemyHpText;
    public Sprite[] enemySprites; // Stage1〜6の順

    [Header("UI")]
    public TMP_Text battleLogText;  // 戦闘ログ
    public TMP_Text stageText;
    public Button attackButton;
    public Button backButton;

    [Header("結果パネル")]
    public GameObject resultPanel;
    public TMP_Text resultText;
    public TMP_Text rewardText;
    public Button resultBackButton;

    // ステータス
    private int currentStage;
    private int petMaxHp;
    private int petHp;
    private int petAttack;
    private int enemyMaxHp;
    private int enemyHp;
    private int enemyAttack;

    private bool isBattleOver = false;

    // ステージごとの敵ステータス
    private int[] enemyMaxHps = { 50, 100, 200, 350, 500, 800 };
    private int[] enemyAttacks = { 5, 10, 20, 35, 50, 80 };
    private int rewardCoins = 1000;

    void Start()
    {
        if (GameData.Instance == null)
        {
            var go = new GameObject("GameData");
            go.AddComponent<GameData>();
        }

        currentStage = PlayerPrefs.GetInt("selectedStage", 1);

        SetupBattle();

        if (attackButton != null) attackButton.onClick.AddListener(OnAttackButton);
        if (backButton != null) backButton.onClick.AddListener(OnBackButton);
        if (resultBackButton != null) resultBackButton.onClick.AddListener(OnBackButton);

        if (resultPanel != null) resultPanel.SetActive(false);
    }

    void SetupBattle()
    {
        int level = GameData.Instance.level;

        // ペットのステータス（レベルに応じて強くなる）
        petMaxHp = 50 + (level - 1) * 30;
        petHp = petMaxHp;
        petAttack = 10 + (level - 1) * 8;

        // 敵のステータス
        int idx = Mathf.Clamp(currentStage - 1, 0, enemyMaxHps.Length - 1);
        enemyMaxHp = enemyMaxHps[idx];
        enemyHp = enemyMaxHp;
        enemyAttack = enemyAttacks[idx];

        // UI更新
        UpdateHpUI();

        if (stageText != null)
            stageText.text = $"Stage {currentStage}";

        if (battleLogText != null)
            battleLogText.text = "Attackボタンを押して戦おう！";

        if (enemyImage != null && enemySprites != null
     && enemySprites.Length > 0)
        {
            int idx2 = Mathf.Clamp(currentStage - 1, 0, enemySprites.Length - 1);
            if (enemySprites[idx2] != null)
                enemyImage.sprite = enemySprites[idx2];
        }

        // ペット画像も同様に修正
        if (petImage != null && petSprites != null
            && petSprites.Length > 0)
        {
            int spriteIdx = level >= 5 ? 2 : level >= 3 ? 1 : 0;
            spriteIdx = Mathf.Clamp(spriteIdx, 0, petSprites.Length - 1);
            if (petSprites[spriteIdx] != null)
                petImage.sprite = petSprites[spriteIdx];
        }
    }

    public void OnAttackButton()
    {
        if (isBattleOver) return;

        StartCoroutine(BattleTurn());
    }

    IEnumerator BattleTurn()
    {
        // ボタンを無効化（連打防止）
        if (attackButton != null) attackButton.interactable = false;

        // ペットの攻撃
        int petDmg = Random.Range(petAttack - 3, petAttack + 5);
        enemyHp = Mathf.Max(0, enemyHp - petDmg);
        UpdateHpUI();

        if (battleLogText != null)
            battleLogText.text = $"あなたの攻撃！ {petDmg}ダメージ！";

        yield return new WaitForSeconds(0.8f);

        // 敵が倒れた
        if (enemyHp <= 0)
        {
            BattleEnd(true);
            yield break;
        }

        // 敵の攻撃
        int enemyDmg = Random.Range(enemyAttack - 2, enemyAttack + 4);
        petHp = Mathf.Max(0, petHp - enemyDmg);
        UpdateHpUI();

        if (battleLogText != null)
            battleLogText.text = $"敵の攻撃！ {enemyDmg}ダメージ！";

        yield return new WaitForSeconds(0.8f);

        // ペットが倒れた
        if (petHp <= 0)
        {
            BattleEnd(false);
            yield break;
        }

        // ボタンを再び有効化
        if (attackButton != null) attackButton.interactable = true;

        if (battleLogText != null)
            battleLogText.text = "Attackボタンを押して戦おう！";
    }

    void BattleEnd(bool isWin)
    {
        isBattleOver = true;

        if (attackButton != null) attackButton.interactable = false;
        if (resultPanel != null) resultPanel.SetActive(true);

        if (isWin)
        {
            // 勝利
            GameData.Instance.coins += rewardCoins;
            if (currentStage > GameData.Instance.clearedStage)
                GameData.Instance.clearedStage = currentStage;
            GameData.Instance.Save();

            if (resultText != null) resultText.text = "勝利！";
            if (rewardText != null) rewardText.text = $"+{rewardCoins} coins！";
        }
        else
        {
            // 敗北
            if (resultText != null) resultText.text = "敗北...";
            if (rewardText != null) rewardText.text = "もう一度挑戦しよう！";
        }
    }

    void UpdateHpUI()
    {
        // ペットHP
        if (petHpSlider != null)
        {
            petHpSlider.maxValue = petMaxHp;
            petHpSlider.value = petHp;
        }
        if (petHpText != null)
            petHpText.text = $"HP: {petHp} / {petMaxHp}";

        // 敵HP
        if (enemyHpSlider != null)
        {
            enemyHpSlider.maxValue = enemyMaxHp;
            enemyHpSlider.value = enemyHp;
        }
        if (enemyHpText != null)
            enemyHpText.text = $"HP: {enemyHp} / {enemyMaxHp}";
    }

    public void OnBackButton() => SceneManager.LoadScene("StageSelectScene");
}
