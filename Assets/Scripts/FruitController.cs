// Assets/Scripts/FruitController.cs
using UnityEngine;

public class FruitController : MonoBehaviour
{
    [Header("果物設定")]
    public int fruitLevel = 0;    // 0=最小, 大きいほど高レベル
    public int scoreValue = 10;   // 合体時のスコア
    public float mergeDelay = 0.3f; // 合体までのディレイ

    private MiniGameManager gameManager;
    private bool hasMerged = false;
    private bool isDropped = false;
    private float dropTimer = 0f;

    public void Initialize(MiniGameManager gm)
    {
        gameManager = gm;
    }

    void Start()
    {
        // 落下後にゲームオーバーチェックを開始
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.gravityScale = 1f;
    }

    void Update()
    {
        // 少し経ってからゲームオーバーラインチェック（落下直後は除外）
        dropTimer += Time.deltaTime;
        if (dropTimer > 1.5f && gameManager != null)
        {
            if (transform.position.y > 4f) // ゲームオーバーライン
            {
                gameManager.TriggerGameOver();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (hasMerged) return;

        var other = col.gameObject.GetComponent<FruitController>();
        if (other == null || other.hasMerged) return;

        // 同じレベルの果物が衝突したら合体
        if (other.fruitLevel == fruitLevel)
        {
            hasMerged = true;
            other.hasMerged = true;

            gameManager?.AddScore(scoreValue * (fruitLevel + 1) * 2);
            MergeFruits(other);
        }
    }

    void MergeFruits(FruitController other)
    {
        // 合体位置は2つの中間点
        Vector3 mergePos = (transform.position + other.transform.position) / 2f;

        Destroy(other.gameObject);
        Destroy(gameObject);

        // 次のレベルの果物を生成（GameManagerから取得）
        int nextLevel = fruitLevel + 1;
        if (gameManager != null && nextLevel < gameManager.fruitPrefabs.Length)
        {
            var newFruit = Instantiate(
                gameManager.fruitPrefabs[nextLevel],
                mergePos,
                Quaternion.identity
            );
            var fc = newFruit.GetComponent<FruitController>();
            if (fc != null) fc.Initialize(gameManager);
        }
    }
}
