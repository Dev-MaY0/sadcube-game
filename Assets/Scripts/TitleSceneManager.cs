using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    public Button shopButton;

    [Header("UI")]
    public TMP_Text coinText;
    public TMP_Text levelText;
    public GameObject continueButton;

    [Header("ボタン")]
    public Button startButton;
    public Button miniGameButton;
    public Button resetButton;
    public Button quitButton;
    public Button howToPlayButton;
    public Button loginBonusButton; 

    [Header("遊び方パネル")]
    public GameObject howToPlayPanel;
    public Button closeButton;

    [Header("ログインボーナス")]
    public TMP_Text bonusTimerText;  
    public TMP_Text bonusButtonText;  
    public GameObject bonusResultPanel;
    public TMP_Text bonusResultText;  
    public Button bonusCloseButton; 

    [Header("ステージセレクト")]
    public Button stageSelectButton;

    [Header("設定ボタン")]
    public GameObject configButton;

    void Start()
    {
        if (GameData.Instance == null)
        {
            var go = new GameObject("GameData");
            go.AddComponent<GameData>();
        }

        if (LoginBonus.Instance == null)
        {
            var go = new GameObject("LoginBonus");
            go.AddComponent<LoginBonus>();
        }

        
        if (startButton != null) startButton.onClick.AddListener(OnStartButton);
        if (miniGameButton != null) miniGameButton.onClick.AddListener(OnMiniGameButton);
        if (resetButton != null) resetButton.onClick.AddListener(OnResetButton);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitButton);
        if (howToPlayButton != null) howToPlayButton.onClick.AddListener(OnHowToPlayButton);
        if (closeButton != null) closeButton.onClick.AddListener(OnCloseHowToPlay);
        if (loginBonusButton != null) loginBonusButton.onClick.AddListener(OnLoginBonusButton);
        if (bonusCloseButton != null) bonusCloseButton.onClick.AddListener(OnCloseBonusResult);

        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
        if (bonusResultPanel != null) bonusResultPanel.SetActive(false);
        if (shopButton != null) shopButton.onClick.AddListener(OnShopButton);

        UpdateUI();

        bool hasSave = PlayerPrefs.HasKey("coins");
        if (continueButton != null)
            continueButton.SetActive(hasSave);

        if (stageSelectButton != null)
            stageSelectButton.onClick.AddListener(
                () => SceneManager.LoadScene("StageSelectScene")
            );
    }

    void Update()
    {
        UpdateBonusButton();
    }

    void UpdateUI()
    {
        if (GameData.Instance == null) return;
        if (coinText) coinText.text = $"Coins: {GameData.Instance.coins}";
        if (levelText) levelText.text = $"Level: {GameData.Instance.level}";
    }

    void UpdateBonusButton()
    {
        if (LoginBonus.Instance == null) return;

        bool canReceive = LoginBonus.Instance.CanReceiveBonus();

        // ボタンの有効・無効を切り替え
        if (loginBonusButton != null)
            loginBonusButton.interactable = canReceive;

        // ボタンのテキストを変える
        if (bonusButtonText != null)
            bonusButtonText.text = canReceive ? "Bonus!" : "Bonus";

        // 残り時間を表示
        if (bonusTimerText != null)
        {
            if (canReceive)
                bonusTimerText.text = "受け取れます！";
            else
                bonusTimerText.text = $"次まで: {LoginBonus.Instance.GetRemainingTime()}";
        }
    }

    public void OnLoginBonusButton()
    {
        if (LoginBonus.Instance == null) return;
        int received = LoginBonus.Instance.ReceiveBonus();
        if (received <= 0) return;

        // 実績チェックを追加
        FindObjectOfType<AchievementManager>()?.OnBonusReceived();

        UpdateUI();
        if (bonusResultText != null) bonusResultText.text = $"+{received} Coins!";
        if (bonusResultPanel != null) bonusResultPanel.SetActive(true);

        
    }

    public void OnShopButton() => SceneManager.LoadScene("ShopScene");
    public void OnCloseBonusResult()
    {
        if (bonusResultPanel != null) bonusResultPanel.SetActive(false);
    }

    public void OnStartButton() => SceneManager.LoadScene("PetScene");
    public void OnMiniGameButton() => SceneManager.LoadScene("MiniGameScene");
    public void OnQuitButton() => Application.Quit();

    public void OnHowToPlayButton()
    {
        if (howToPlayPanel != null) howToPlayPanel.SetActive(true);

        if (configButton != null)
            configButton.SetActive(false);
    }

    public void OnCloseHowToPlay()
    {
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);

        if (configButton != null)
            configButton.SetActive(true);
    }

    public void OnResetButton()
    {
        PlayerPrefs.DeleteAll();

        // GameDataをリセット
        if (GameData.Instance != null)
        {
            GameData.Instance.coins = 0;
            GameData.Instance.level = 1;
            GameData.Instance.petName = "";  // 名前をリセット
            GameData.Instance.hasMegane = false;  // ← 追加
            GameData.Instance.hasBoshi = false;  // ← 追加
            GameData.Instance.equippedItem = "";    // ← 追加
        }

        // 実績をリセット
        if (AchievementData.Instance != null)
        {
            foreach (var achievement in AchievementData.Instance.achievements)
                achievement.isUnlocked = false;
        }

        // ログインボーナスをリセット
        PlayerPrefs.DeleteKey("lastBonusTime");
        PlayerPrefs.DeleteKey("bonusCount");

        PlayerPrefs.Save();
        UpdateUI();

        Debug.Log("リセット完了");
    }
}
