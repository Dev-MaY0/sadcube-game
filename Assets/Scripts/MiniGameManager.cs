using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    [Header("スポーン設定")]
    public GameObject[] fruitPrefabs;
    public Transform spawnPoint;
    public Transform dropLine;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text coinPreviewText;
    public TMP_Text timerText;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public TMP_Text earnedCoinText;

    [Header("ボタン")]
    public Button retryButton;   // ← 追加
    public Button backButton;    // ← 追加
    public Button titleButton;   // ← 追加

    [Header("ゲーム設定")]
    public float gameTime = 60f;
    public int scoreTocoin = 10;

    private int score = 0;
    private float timeLeft;
    private bool isGameOver = false;
    private bool canDrop = true;
    private int nextFruitIndex = 0;
    private float dropCooldown = 0.8f;
    private float dropTimer = 0f;

    void Start()
    {
        if (GameData.Instance == null)
        {
            var go = new GameObject("GameData");
            go.AddComponent<GameData>();
        }

        timeLeft = gameTime;
        gameOverPanel.SetActive(false);

        // ボタンをスクリプトから登録
        if (retryButton != null) retryButton.onClick.AddListener(OnRetryButton);
        if (backButton != null) backButton.onClick.AddListener(OnBackButton);
        if (titleButton != null) titleButton.onClick.AddListener(OnTitleButton);

        PrepareNextFruit();
        UpdateUI();
    }

    void Update()
    {
        if (isGameOver) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f) { timeLeft = 0f; EndGame(); return; }

        if (!canDrop)
        {
            dropTimer += Time.deltaTime;
            if (dropTimer >= dropCooldown)
            {
                canDrop = true;
                dropTimer = 0f;
                PrepareNextFruit();
            }
        }

        if (canDrop && Input.GetMouseButtonDown(0))
            DropFruit();

        UpdateUI();
    }

    void PrepareNextFruit()
    {
        nextFruitIndex = Random.Range(0, Mathf.Min(3, fruitPrefabs.Length));
    }

    void DropFruit()
    {
        if (fruitPrefabs.Length == 0) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 spawnPos = new Vector3(
            Mathf.Clamp(mousePos.x, -2.5f, 2.5f),
            spawnPoint.position.y,
            0f
        );

        var fruit = Instantiate(fruitPrefabs[nextFruitIndex], spawnPos, Quaternion.identity);
        var fc = fruit.GetComponent<FruitController>();
        if (fc != null) fc.Initialize(this);

        canDrop = false;
    }

    public void AddScore(int amount) => score += amount;

    public void TriggerGameOver()
    {
        if (!isGameOver) EndGame();
    }

    void EndGame()
    {
        isGameOver = true;
        int earnedCoins = score / scoreTocoin;
        GameData.Instance?.AddCoins(earnedCoins);

        finalScoreText.text = $"Score: {score}";
        earnedCoinText.text = $"+{earnedCoins} Coins!";
        gameOverPanel.SetActive(true);
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = $"Score: {score}";
        if (coinPreviewText) coinPreviewText.text = $"Coins: +{score / scoreTocoin}";
        if (timerText) timerText.text = $"Time: {Mathf.CeilToInt(timeLeft)}";
    }

    public void OnRetryButton() => SceneManager.LoadScene("MiniGameScene");
    public void OnBackButton() => SceneManager.LoadScene("PetScene");
    public void OnTitleButton() => SceneManager.LoadScene("TitleScene");
}
