using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private float bgmVolume = 1.0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        
        bgmVolume = PlayerPrefs.GetFloat("bgmVolume", 1.0f);
    }

    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager
            .sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager
            .sceneLoaded -= OnSceneLoaded;
    }

    
    void OnSceneLoaded(
        UnityEngine.SceneManagement.Scene scene,
        UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        ApplyVolumeToScene();
    }

    public void ApplyVolumeToScene()
    {
        AudioSource[] sources =
            FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (var source in sources)
            source.volume = bgmVolume;
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        ApplyVolumeToScene();

        PlayerPrefs.SetFloat("bgmVolume", bgmVolume);
        PlayerPrefs.Save();
    }

    public float GetBGMVolume() => bgmVolume;
}