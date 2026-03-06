using UnityEngine;

public class PetEquipmentManager : MonoBehaviour
{
    [Header("装備画像（Petの子オブジェクト）")]
    public SpriteRenderer equipmentRenderer;

    [Header("メガネ（成長段階別）")]
    public Sprite childMeganeSprite;
    public Sprite adultMeganeSprite;
    public Sprite elderMeganeSprite;

    [Header("帽子（成長段階別）")]
    public Sprite childBoshiSprite;
    public Sprite adultBoshiSprite;
    public Sprite elderBoshiSprite;

    private PetGrowth petGrowth;
    private SpriteRenderer petRenderer;
    private GrowthStage lastStage = GrowthStage.Child;
    private string lastEquipped = "";

    void Start()
    {
        if (GameData.Instance == null)
        {
            var go = new GameObject("GameData");
            go.AddComponent<GameData>();
        }

        petGrowth = GetComponent<PetGrowth>();
        petRenderer = GetComponent<SpriteRenderer>();

        UpdateEquipment();
    }

    void Update()
    {
        if (GameData.Instance == null) return;

        // PetGrowthではなくレベルから直接段階を取得
        GrowthStage currentStage = GetStageFromLevel();
        string currentEquipped = GameData.Instance.equippedItem;

        if (currentStage != lastStage ||
            currentEquipped != lastEquipped)
        {
            lastStage = currentStage;
            lastEquipped = currentEquipped;
            UpdateEquipment();
        }
    }

    // レベルから直接成長段階を計算
    GrowthStage GetStageFromLevel()
    {
        if (GameData.Instance == null) return GrowthStage.Child;

        int level = GameData.Instance.level;

        int adultLevel = petGrowth != null ? petGrowth.adultLevel : 3;
        int elderLevel = petGrowth != null ? petGrowth.elderLevel : 5;

        if (level >= elderLevel) return GrowthStage.Elder;
        if (level >= adultLevel) return GrowthStage.Adult;
        return GrowthStage.Child;
    }

    public void UpdateEquipment()
    {
        if (equipmentRenderer == null || GameData.Instance == null) return;

        string equipped = GameData.Instance.equippedItem;

        // 装備なし
        if (string.IsNullOrEmpty(equipped))
        {
            equipmentRenderer.enabled = false;
            if (petRenderer != null)
            {
                petRenderer.enabled = true;
                AdjustSize(petRenderer.sprite);
            }
            return;
        }

        // レベルから直接段階を取得
        GrowthStage stage = GetStageFromLevel();

        Sprite targetSprite = GetSprite(equipped, stage);

        if (targetSprite != null)
        {
            equipmentRenderer.sprite = targetSprite;
            equipmentRenderer.enabled = true;
            if (petRenderer != null) petRenderer.enabled = false;
            AdjustSize(targetSprite);
        }
        else
        {
            equipmentRenderer.enabled = false;
            if (petRenderer != null)
            {
                petRenderer.enabled = true;
                AdjustSize(petRenderer.sprite);
            }
        }
    }

    void AdjustSize(Sprite sprite)
    {
        if (sprite == null) return;
        float targetSize = 3.0f;
        float spriteSize = sprite.bounds.size.x;
        if (spriteSize > 0)
        {
            float scale = targetSize / spriteSize;
            transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    Sprite GetSprite(string itemId, GrowthStage stage)
    {
        switch (itemId)
        {
            case "megane":
                switch (stage)
                {
                    case GrowthStage.Child: return childMeganeSprite;
                    case GrowthStage.Adult: return adultMeganeSprite;
                    case GrowthStage.Elder: return elderMeganeSprite;
                }
                break;
            case "boshi":
                switch (stage)
                {
                    case GrowthStage.Child: return childBoshiSprite;
                    case GrowthStage.Adult: return adultBoshiSprite;
                    case GrowthStage.Elder: return elderBoshiSprite;
                }
                break;
        }
        return null;
    }
}



