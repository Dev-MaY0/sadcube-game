using UnityEngine;

public class PetController : MonoBehaviour
{
    public PetStatus status = new PetStatus();

    [Header("成長段階")]
    public PetGrowth petGrowth;
    [Header("アニメーション（任意）")]
    public Animator animator;

    void Start()
    {
        status.OnPetDied += HandlePetDied;
        status.OnStatusChanged += HandleStatusChanged;
    }

    void Update()
    {
        status.Tick(Time.deltaTime);
        UpdateAnimation();
    }

    // --- ボタンから呼ぶメソッド ---
    public void OnFeedButton()
    {
        status.Feed();
        FindObjectOfType<AchievementManager>()?.OnFeed();
    }
    public void OnPlayButton()
    {
        status.Play();
        FindObjectOfType<AchievementManager>()?.OnPlay();
    }
    public void OnSleepButton()
    {
        status.Sleep();
        FindObjectOfType<AchievementManager>()?.OnSleep();
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        if (!status.isAlive)
        {
            animator.SetTrigger("Die");
            return;
        }

        if (status.hunger < 20f)
            animator.SetBool("IsHungry", true);
        else
            animator.SetBool("IsHungry", false);

        if (status.happiness > 70f)
            animator.SetBool("IsHappy", true);
        else
            animator.SetBool("IsHappy", false);
    }

    void HandleStatusChanged(PetStatus s)
    {
        // UIManagerへの通知はUIManager側でイベント購読させる
    }

    void HandlePetDied()
    {
        Debug.Log("ペットが死亡しました...");
        // ゲームオーバー処理へ
        GameManager.Instance?.GameOver();
    }
}
