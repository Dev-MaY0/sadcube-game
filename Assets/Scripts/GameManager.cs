using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("セーブ設定")]
    public bool enableAutoSave = true;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnApplicationQuit()
    {
        if (enableAutoSave) SaveGame();
    }

    public void GameOver()
    {
        Debug.Log("ゲームオーバー");
        // 例: 3秒後にシーンリロード
        Invoke(nameof(ReloadScene), 3f);
    }

    void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    // --- 簡易セーブ（PlayerPrefs） ---
    public void SaveGame()
    {
        var pet = FindObjectOfType<PetController>();
        if (pet == null) return;
        PlayerPrefs.SetFloat("hunger", pet.status.hunger);
        PlayerPrefs.SetFloat("happiness", pet.status.happiness);
        PlayerPrefs.SetFloat("health", pet.status.health);
        PlayerPrefs.SetFloat("age", pet.status.age);
        PlayerPrefs.Save();
        Debug.Log("セーブ完了");
    }

    public void LoadGame()
    {
        var pet = FindObjectOfType<PetController>();
        if (pet == null) return;
        pet.status.hunger = PlayerPrefs.GetFloat("hunger", 80f);
        pet.status.happiness = PlayerPrefs.GetFloat("happiness", 80f);
        pet.status.health = PlayerPrefs.GetFloat("health", 100f);
        pet.status.age = PlayerPrefs.GetFloat("age", 0f);
        Debug.Log("ロード完了");
    }
}
