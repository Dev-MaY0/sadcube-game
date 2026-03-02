using UnityEngine;
using System;

[Serializable]
public class PetStatus
{
    [Header("基本パラメータ (0〜100)")]
    public float hunger = 80f;   // 満腹度（0で空腹）
    public float happiness = 80f;   // 幸福度
    public float health = 100f;  // 体力
    public float age = 0f;    // 年齢（ゲーム内時間）
    public bool isAlive = true;

    [Header("減少速度（/秒）")]
    public float hungerDecayRate = 1.0f;
    public float happinessDecayRate = 0.5f;

    public event Action OnPetDied;
    public event Action<PetStatus> OnStatusChanged;

    /// <summary>毎フレーム呼ぶ（deltaTimeを渡す）</summary>
    public void Tick(float deltaTime)
    {
        if (!isAlive) return;

        hunger = Mathf.Clamp(hunger - hungerDecayRate * deltaTime, 0, 100);
        happiness = Mathf.Clamp(happiness - happinessDecayRate * deltaTime, 0, 100);
        age += deltaTime;

        // 空腹・不幸が続くと体力が減る
        if (hunger < 20f)
            health = Mathf.Clamp(health - 2f * deltaTime, 0, 100);
        if (happiness < 10f)
            health = Mathf.Clamp(health - 1f * deltaTime, 0, 100);

        // 健康状態が回復しやすいとき
        if (hunger > 60f && happiness > 60f)
            health = Mathf.Clamp(health + 0.5f * deltaTime, 0, 100);

        OnStatusChanged?.Invoke(this);

        if (health <= 0f && isAlive)
        {
            isAlive = false;
            OnPetDied?.Invoke();
        }
    }

    // --- アクション ---
    public void Feed(float amount = 30f)
    {
        if (!isAlive) return;
        hunger = Mathf.Clamp(hunger + amount, 0, 100);
        happiness = Mathf.Clamp(happiness + 5f, 0, 100);
        OnStatusChanged?.Invoke(this);
    }

    public void Play(float amount = 25f)
    {
        if (!isAlive) return;
        happiness = Mathf.Clamp(happiness + amount, 0, 100);
        hunger = Mathf.Clamp(hunger - 10f, 0, 100); // 遊ぶとお腹が減る
        OnStatusChanged?.Invoke(this);
    }

    public void Sleep(float amount = 20f)
    {
        if (!isAlive) return;
        health = Mathf.Clamp(health + amount, 0, 100);
        hunger = Mathf.Clamp(hunger - 15f, 0, 100);
        OnStatusChanged?.Invoke(this);
    }
}