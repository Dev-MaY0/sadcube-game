using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
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
    public Button loginBonusButton;   // ← 追加

    [Header("遊び方パネル")]
    public GameObject howToPlayPanel;
    public Button closeButton;

    [Header("ログインボーナス")]
    public TMP_Text bonusTimerText;   // 残り時間表示
    public TMP_Text bonusButtonText;  // ボタンのテキスト
    public GameObject bonusResultPanel; // 受け取り結果パネル
    public TMP_Text bonusResultText;  // 「+30 Coins!」などの表示
    public Button bonusCloseButton; // 結果パネルの閉じるボタン

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

        // ボタン登録
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

        UpdateUI();

        bool hasSave = PlayerPrefs.HasKey("coins");
        if (continueButton != null)
            continueButton.SetActive(hasSave);
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

        UpdateUI();

        // 結果パネルを表示
        if (bonusResultText != null) bonusResultText.text = $"+{received} Coins!";
        if (bonusResultPanel != null) bonusResultPanel.SetActive(true);
    }

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
    }

    public void OnCloseHowToPlay()
    {
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
    }

    public void OnResetButton()
    {
        PlayerPrefs.DeleteAll();
        if (GameData.Instance != null)
        {
            GameData.Instance.coins = 0;
            GameData.Instance.level = 1;
        }
        UpdateUI();
    }
}
