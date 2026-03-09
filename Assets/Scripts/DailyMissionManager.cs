using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class DailyMissionManager : MonoBehaviour
{
    public static DailyMissionManager Instance;

    private GameObject missionPanel;
    private TMP_Text resetTimerText;

    private Slider feedSlider, miniGameSlider, stageSlider;
    private TMP_Text feedProgressText, miniGameProgressText, stageProgressText;
    private Button feedRewardButton, miniGameRewardButton, stageRewardButton;
    private TMP_Text feedRewardText, miniGameRewardText, stageRewardText;

    private const int FEED_GOAL = 3;
    private const int MINIGAME_GOAL = 1;
    private const int STAGE_GOAL = 1;
    private const int FEED_REWARD = 500;
    private const int MINIGAME_REWARD = 300;
    private const int STAGE_REWARD = 800;

    private int feedCount, miniGameCount, stageCount;
    private bool feedRewarded, miniGameRewarded, stageRewarded;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        CheckDailyReset();
        LoadMissionData();
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (SceneManager.GetActiveScene().name == "PetScene")
            FindAndBindUI();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PetScene")
            FindAndBindUI();
    }

    void FindAndBindUI()
    {
        Debug.Log("FindAndBindUI開始");

        // 非アクティブでも見つけられる方法でMissionPanelを検索
        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.name == "MissionPanel" && obj.scene.isLoaded)
            {
                missionPanel = obj;
                Debug.Log("MissionPanel見つかった！");
                break;
            }
        }

        resetTimerText = FindTMPInactive("ResetTimerText");

        feedSlider = FindSliderInactive("FeedSlider");
        feedProgressText = FindTMPInactive("FeedProgressText");
        feedRewardButton = FindButtonInactive("FeedRewardButton");
        feedRewardText = FindTMPInactive("FeedRewardText");
        if (feedRewardButton != null)
        {
            feedRewardButton.onClick.RemoveAllListeners();
            feedRewardButton.onClick.AddListener(OnFeedReward);
        }

        miniGameSlider = FindSliderInactive("MiniGameSlider");
        miniGameProgressText = FindTMPInactive("MiniGameProgressText");
        miniGameRewardButton = FindButtonInactive("MiniGameRewardButton");
        miniGameRewardText = FindTMPInactive("MiniGameRewardText");
        if (miniGameRewardButton != null)
        {
            miniGameRewardButton.onClick.RemoveAllListeners();
            miniGameRewardButton.onClick.AddListener(OnMiniGameReward);
        }

        stageSlider = FindSliderInactive("StageSlider");
        stageProgressText = FindTMPInactive("StageProgressText");
        stageRewardButton = FindButtonInactive("StageRewardButton");
        stageRewardText = FindTMPInactive("StageRewardText");
        if (stageRewardButton != null)
        {
            stageRewardButton.onClick.RemoveAllListeners();
            stageRewardButton.onClick.AddListener(OnStageReward);
        }

        // MissionButton（アクティブなので通常のFind）
        var missionBtn = FindButton("MissionButton");
        Debug.Log($"MissionButton: {(missionBtn != null ? "見つかった" : "見つからない")}");
        if (missionBtn != null)
        {
            missionBtn.onClick.RemoveAllListeners();
            missionBtn.onClick.AddListener(OnOpenMission);
        }

        var closeMissionBtn = FindButtonInactive("CloseMissionButton");
        if (closeMissionBtn != null)
        {
            closeMissionBtn.onClick.RemoveAllListeners();
            closeMissionBtn.onClick.AddListener(OnCloseMission);
        }

        UpdateUI();
    }

    // --- UI検索ヘルパー（通常） ---
    TMP_Text FindTMP(string objName)
    {
        var obj = GameObject.Find(objName);
        return obj != null ? obj.GetComponent<TMP_Text>() : null;
    }

    Slider FindSlider(string objName)
    {
        var obj = GameObject.Find(objName);
        return obj != null ? obj.GetComponent<Slider>() : null;
    }

    Button FindButton(string objName)
    {
        var obj = GameObject.Find(objName);
        return obj != null ? obj.GetComponent<Button>() : null;
    }

    // --- UI検索ヘルパー（非アクティブ対応） ---
    T FindComponentInactive<T>(string objName) where T : Component
    {
        var allObjects = Resources.FindObjectsOfTypeAll<T>();
        foreach (var obj in allObjects)
        {
            if (obj.name == objName && obj.gameObject.scene.isLoaded)
                return obj;
        }
        return null;
    }

    TMP_Text FindTMPInactive(string objName)
        => FindComponentInactive<TMP_Text>(objName);

    Slider FindSliderInactive(string objName)
        => FindComponentInactive<Slider>(objName);

    Button FindButtonInactive(string objName)
        => FindComponentInactive<Button>(objName);

    // --- 日付リセット ---
    void CheckDailyReset()
    {
        string savedDate = PlayerPrefs.GetString("missionDate", "");
        string today = DateTime.Now.ToString("yyyy/MM/dd");

        if (savedDate != today)
        {
            PlayerPrefs.SetString("missionDate", today);
            PlayerPrefs.SetInt("feedCount", 0);
            PlayerPrefs.SetInt("miniGameCount", 0);
            PlayerPrefs.SetInt("stageCount", 0);
            PlayerPrefs.SetInt("feedRewarded", 0);
            PlayerPrefs.SetInt("miniGameRewarded", 0);
            PlayerPrefs.SetInt("stageRewarded", 0);
            PlayerPrefs.Save();
        }
    }

    void LoadMissionData()
    {
        feedCount = PlayerPrefs.GetInt("feedCount", 0);
        miniGameCount = PlayerPrefs.GetInt("miniGameCount", 0);
        stageCount = PlayerPrefs.GetInt("stageCount", 0);

        feedRewarded = PlayerPrefs.GetInt("feedRewarded", 0) == 1;
        miniGameRewarded = PlayerPrefs.GetInt("miniGameRewarded", 0) == 1;
        stageRewarded = PlayerPrefs.GetInt("stageRewarded", 0) == 1;
    }

    void SaveMissionData()
    {
        PlayerPrefs.SetInt("feedCount", feedCount);
        PlayerPrefs.SetInt("miniGameCount", miniGameCount);
        PlayerPrefs.SetInt("stageCount", stageCount);
        PlayerPrefs.SetInt("feedRewarded", feedRewarded ? 1 : 0);
        PlayerPrefs.SetInt("miniGameRewarded", miniGameRewarded ? 1 : 0);
        PlayerPrefs.SetInt("stageRewarded", stageRewarded ? 1 : 0);
        PlayerPrefs.Save();
    }

    // --- 進捗を増やす ---
    public void AddFeedCount()
    {
        if (feedCount >= FEED_GOAL) return;
        feedCount++;
        SaveMissionData();
        UpdateUI();
    }

    public void AddMiniGameCount()
    {
        if (miniGameCount >= MINIGAME_GOAL) return;
        miniGameCount++;
        SaveMissionData();
        UpdateUI();
    }

    public void AddStageCount()
    {
        if (stageCount >= STAGE_GOAL) return;
        stageCount++;
        SaveMissionData();
        UpdateUI();
    }

    // --- 報酬受取 ---
    public void OnFeedReward()
    {
        if (feedCount < FEED_GOAL || feedRewarded) return;
        GameData.Instance.coins += FEED_REWARD;
        GameData.Instance.Save();
        feedRewarded = true;
        SaveMissionData();
        UpdateUI();
    }

    public void OnMiniGameReward()
    {
        if (miniGameCount < MINIGAME_GOAL || miniGameRewarded) return;
        GameData.Instance.coins += MINIGAME_REWARD;
        GameData.Instance.Save();
        miniGameRewarded = true;
        SaveMissionData();
        UpdateUI();
    }

    public void OnStageReward()
    {
        if (stageCount < STAGE_GOAL || stageRewarded) return;
        GameData.Instance.coins += STAGE_REWARD;
        GameData.Instance.Save();
        stageRewarded = true;
        SaveMissionData();
        UpdateUI();
    }

    // --- パネル開閉 ---
    public void OnOpenMission()
    {
        CheckDailyReset();
        LoadMissionData();
        if (missionPanel != null) missionPanel.SetActive(true);
        UpdateUI();
    }

    public void OnCloseMission()
    {
        if (missionPanel != null) missionPanel.SetActive(false);
    }

    // --- UI更新 ---
    void UpdateUI()
    {
        UpdateMissionUI(
            feedSlider, feedProgressText,
            feedRewardButton, feedRewardText,
            feedCount, FEED_GOAL, feedRewarded, FEED_REWARD
        );
        UpdateMissionUI(
            miniGameSlider, miniGameProgressText,
            miniGameRewardButton, miniGameRewardText,
            miniGameCount, MINIGAME_GOAL,
            miniGameRewarded, MINIGAME_REWARD
        );
        UpdateMissionUI(
            stageSlider, stageProgressText,
            stageRewardButton, stageRewardText,
            stageCount, STAGE_GOAL, stageRewarded, STAGE_REWARD
        );
    }

    void UpdateMissionUI(
        Slider slider, TMP_Text progressText,
        Button rewardButton, TMP_Text rewardText,
        int count, int goal, bool rewarded, int reward)
    {
        if (slider != null)
        {
            slider.maxValue = goal;
            slider.value = count;
        }

        if (progressText != null)
            progressText.text = $"{count}/{goal}";

        if (rewardButton != null && rewardText != null)
        {
            if (rewarded)
            {
                rewardText.text = "受取済み";
                rewardButton.interactable = false;
            }
            else if (count >= goal)
            {
                rewardText.text = $"受取 +{reward}";
                rewardButton.interactable = true;
            }
            else
            {
                rewardText.text = $"報酬: {reward}";
                rewardButton.interactable = false;
            }
        }
    }

    void Update()
    {
        UpdateResetTimer();
    }

    void UpdateResetTimer()
    {
        if (resetTimerText == null) return;
        DateTime now = DateTime.Now;
        DateTime midnight = DateTime.Today.AddDays(1);
        TimeSpan remaining = midnight - now;
        resetTimerText.text =
            $"リセットまで: {remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
    }
}