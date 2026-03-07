// Assets/Scripts/GameData.cs
using UnityEngine;

/// <summary>
/// シーンをまたいでデータを保持するシングルトン
/// </summary>
public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    [Header("ショップ")]
    public bool hasMegane = false;  // メガネ購入済み
    public bool hasBoshi = false;  // 帽子購入済み
    public string equippedItem = "";  // 装備中アイテム（"megane" or "boshi" or ""）


    [Header("所持金・レベル")]
    public int coins = 0;
    public int level = 1;

    [Header("レベルアップコスト（レベルごと）")]
    public int[] levelUpCosts = { 50, 120, 250, 500, 1000 };

    [Header("ステージ")]
    public int clearedStage = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    /// <summary>レベルアップできるか</summary>
    public bool CanLevelUp()
    {
        return level <= levelUpCosts.Length && coins >= GetLevelUpCost();
    }

    public int GetLevelUpCost()
    {
        if (level - 1 < levelUpCosts.Length)
            return levelUpCosts[level - 1];
        return 99999; // 最大レベル
    }

    public bool TryLevelUp()
    {
        if (!CanLevelUp()) return false;
        coins -= GetLevelUpCost();
        level++;
        Save();
        return true;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        Save();
    }

    // --- セーブ・ロード ---
    public void Save()
    {
        PlayerPrefs.SetInt("coins", coins);
        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetString("petName", petName); 

        PlayerPrefs.SetInt("hasMegane", hasMegane ? 1 : 0);  
        PlayerPrefs.SetInt("hasBoshi", hasBoshi ? 1 : 0);  
        PlayerPrefs.SetString("equippedItem", equippedItem);
        PlayerPrefs.SetInt("clearedStage", clearedStage);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        coins = PlayerPrefs.GetInt("coins", 0);
        level = PlayerPrefs.GetInt("level", 1);
        petName = PlayerPrefs.GetString("petName", "");
        hasMegane = PlayerPrefs.GetInt("hasMegane", 0) == 1; 
        hasBoshi = PlayerPrefs.GetInt("hasBoshi", 0) == 1; 
        equippedItem = PlayerPrefs.GetString("equippedItem", "");
        clearedStage = PlayerPrefs.GetInt("clearedStage", 0);
    }

    [Header("ペット情報")]
    public string petName = "";
}
