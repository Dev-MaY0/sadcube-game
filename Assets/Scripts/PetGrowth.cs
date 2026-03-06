using UnityEngine;
using TMPro;

public enum GrowthStage
{
    Child,  // レベル1〜2
    Adult,  // レベル3〜4
    Elder   // レベル5〜6
}

public class PetGrowth : MonoBehaviour
{
    [Header("成長段階のSprite")]
    public Sprite childSprite;   // レベル1〜2
    public Sprite adultSprite;   // レベル3〜4
    public Sprite elderSprite;   // レベル5〜6

    [Header("成長段階の境界レベル")]
    public int adultLevel = 3;   
    public int elderLevel = 7; 

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
        UpdateStageText(stage);
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

        // Spriteのサイズを固定
        if (spriteRenderer.sprite != null)
        {
            float targetSize = 2.0f; // ← この値で表示サイズを調整
            float spriteSize = spriteRenderer.sprite.bounds.size.x;
            float scale = targetSize / spriteSize;
            transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    void UpdateStageText(GrowthStage stage)
    {
        if (stageText == null) return;
        switch (stage)
        {
            case GrowthStage.Child: stageText.text = "Child"; break;
            case GrowthStage.Adult: stageText.text = "Adult"; break;
            case GrowthStage.Elder: stageText.text = "Elder"; break;
        }
    }



    public GrowthStage GetCurrentStage() => currentStage;
}
