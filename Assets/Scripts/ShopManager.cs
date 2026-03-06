// Assets/Scripts/ShopManager.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("所持コイン表示")]
    public TMP_Text coinText;

    [Header("ショップパネル")]
    public GameObject buyPanel;       // buypanel

    [Header("メガネ")]
    public Image meganePreview;
    public TMP_Text meganeStatusText;
    public Button meganeButton;
    public int meganeCost = 100;

    [Header("帽子")]
    public Image boshiPreview;
    public TMP_Text boshiStatusText;
    public Button boshiButton;
    public int boshiCost = 150;

    [Header("ペットプレビュー")]
    public Image petImage;
    public Image equipmentImage;

    [Header("装備Sprite")]
    public Sprite meganeSprite;
    public Sprite boshiSprite;

    [Header("ボタン")]
    public Button openShopButton;  // ショップを開くボタン
    public Button closeShopButton; // ショップを閉じるボタン
    public Button backButton;      // 戻るボタン

    void Start()
    {
        if (GameData.Instance == null)
        {
            var go = new GameObject("GameData");
            go.AddComponent<GameData>();
        }

        // ボタン登録
        if (openShopButton != null) openShopButton.onClick.AddListener(OnOpenShop);
        if (closeShopButton != null) closeShopButton.onClick.AddListener(OnCloseShop);
        if (meganeButton != null) meganeButton.onClick.AddListener(OnMeganeButton);
        if (boshiButton != null) boshiButton.onClick.AddListener(OnBoshiButton);
        if (backButton != null) backButton.onClick.AddListener(OnBackButton);

        // buypanelは初期非表示
        if (buyPanel != null) buyPanel.SetActive(false);

        UpdateUI();
    }

    public void OnOpenShop()
    {
        if (buyPanel != null) buyPanel.SetActive(true);
        UpdateUI();
    }

    public void OnCloseShop()
    {
        if (buyPanel != null) buyPanel.SetActive(false);
    }

    void UpdateUI()
    {
        if (GameData.Instance == null) return;

        // コイン表示
        if (coinText) coinText.text = $"Coins: {GameData.Instance.coins}";

        // メガネ
        UpdateItemUI(
            meganeStatusText,
            meganeButton,
            GameData.Instance.hasMegane,
            "megane",
            meganeCost
        );

        // 帽子
        UpdateItemUI(
            boshiStatusText,
            boshiButton,
            GameData.Instance.hasBoshi,
            "boshi",
            boshiCost
        );

        // 装備表示
        UpdateEquipmentDisplay();
    }

    void UpdateItemUI(TMP_Text statusText, Button button,
                      bool hasItem, string itemId, int cost)
    {
        if (statusText == null || button == null) return;

        string equipped = GameData.Instance.equippedItem;

        if (!hasItem)
        {
            statusText.text = $"Buy: {cost} coins";
            button.interactable = GameData.Instance.coins >= cost;
        }
        else if (equipped == itemId)
        {
            statusText.text = "Equipped!";
            button.interactable = true;
        }
        else
        {
            statusText.text = "Equip";
            button.interactable = true;
        }
    }

    public void OnMeganeButton()
    {
        if (GameData.Instance == null) return;

        if (!GameData.Instance.hasMegane)
        {
            if (GameData.Instance.coins < meganeCost) return;
            GameData.Instance.coins -= meganeCost;
            GameData.Instance.hasMegane = true;
        }

        GameData.Instance.equippedItem =
            GameData.Instance.equippedItem == "megane" ? "" : "megane";

        GameData.Instance.Save();
        UpdateUI();
    }

    public void OnBoshiButton()
    {
        if (GameData.Instance == null) return;

        if (!GameData.Instance.hasBoshi)
        {
            if (GameData.Instance.coins < boshiCost) return;
            GameData.Instance.coins -= boshiCost;
            GameData.Instance.hasBoshi = true;
        }

        GameData.Instance.equippedItem =
            GameData.Instance.equippedItem == "boshi" ? "" : "boshi";

        GameData.Instance.Save();
        UpdateUI();
    }

    void UpdateEquipmentDisplay()
    {
        if (equipmentImage == null) return;

        switch (GameData.Instance.equippedItem)
        {
            case "megane":
                equipmentImage.sprite = meganeSprite;
                equipmentImage.enabled = true;
                break;
            case "boshi":
                equipmentImage.sprite = boshiSprite;
                equipmentImage.enabled = true;
                break;
            default:
                equipmentImage.enabled = false;
                break;
        }
    }

    public void OnBackButton() => SceneManager.LoadScene("PetScene");
}