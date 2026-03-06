using UnityEngine;
using TMPro;

public enum GrowthStage
{
    Child,
    Adult,
    Elder
}

public class PetGrowth : MonoBehaviour
{
    [Header("成長段階のSprite")]
    public Sprite childSprite;
    public Sprite adultSprite;
    public Sprite elderSprite;

    [Header("成長段階の境界レベル")]
    public int adultLevel = 3;
    public int elderLevel = 5;

    [Header("UI")]
    public TMP_Text stageText;

    private GrowthStage currentStage = GrowthStage.Child;
    private SpriteRenderer spriteRenderer;
    private PetController petController;
    private int lastLevel = 1;

    public event System.Action<GrowthStage> OnStageChanged;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        petController = GetComponent<PetController>();

        if (GameData.Instance != null)
        {
            lastLevel = GameData.Instance.level;
            ApplyStage(GetStageFromLevel(lastLevel));
        }
    }

    void Update()
    {
        if (GameData.Instance == null) return;

        int currentLevel = GameData.Instance.level;

        if (currentLevel != lastLevel)
        {
            lastLevel = currentLevel;
            GrowthStage newStage = GetStageFromLevel(currentLevel);

            if (newStage != currentStage)
            {
                currentStage = newStage;
                ApplyStage(currentStage);
                OnStageChanged?.Invoke(currentStage);
            }
            else
            {
                UpdateSprite(currentStage);
            }
        }
    }

    GrowthStage GetStageFromLevel(int level)
    {
        if (level >= elderLevel) return GrowthStage.Elder;
        if (level >= adultLevel) return GrowthStage.Adult;
        return GrowthStage.Child;
    }

    void ApplyStage(GrowthStage stage)
    {
        UpdateSprite(stage);

        if (stageText != null)
        {
            switch (stage)
            {
                case GrowthStage.Child: stageText.text = "Child"; break;
                case GrowthStage.Adult: stageText.text = "Adult"; break;
                case GrowthStage.Elder: stageText.text = "Elder"; break;
            }
        }
    }

    void UpdateSprite(GrowthStage stage)
    {
        if (spriteRenderer == null) return;

        switch (stage)
        {
            case GrowthStage.Child:
                if (childSprite != null) spriteRenderer.sprite = childSprite;
                break;
            case GrowthStage.Adult:
                if (adultSprite != null) spriteRenderer.sprite = adultSprite;
                break;
            case GrowthStage.Elder:
                if (elderSprite != null) spriteRenderer.sprite = elderSprite;
                break;
        }

        // PetEquipmentManagerに通知
        var equipment = GetComponent<PetEquipmentManager>();
        if (equipment != null)
        {
            equipment.UpdateEquipment();
        }
        else
        {
            if (spriteRenderer.sprite != null)
            {
                float targetSize = 1.0f;
                float spriteSize = spriteRenderer.sprite.bounds.size.x;
                float scale = targetSize / spriteSize;
                transform.localScale = new Vector3(scale, scale, 1f);
            }
        }
    }

    public GrowthStage GetCurrentStage() => currentStage;
}
