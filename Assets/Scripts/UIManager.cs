using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{ 

    [Header("スライダー")]
    public Slider hungerSlider;
    public Slider happinessSlider;
    public Slider healthSlider;

    [Header("テキスト")]
    public TMP_Text ageText;
    public TMP_Text statusMessageText;

    [Header("ボタン")]
    public Button feedButton;
    public Button playButton;
    public Button sleepButton;

    private PetController petController;

    void Start()
    {
        petController = FindObjectOfType<PetController>();
        if (petController == null) return;

        
        petController.status.OnStatusChanged += UpdateUI;
        petController.status.OnPetDied += ShowGameOver;

       
        feedButton.onClick.AddListener(petController.OnFeedButton);
        playButton.onClick.AddListener(petController.OnPlayButton);
        sleepButton.onClick.AddListener(petController.OnSleepButton);

        
        UpdateUI(petController.status);
    }

    void UpdateUI(PetStatus s)
    {
        if (hungerSlider) hungerSlider.value = s.hunger / 100f;
        if (happinessSlider) happinessSlider.value = s.happiness / 100f;
        if (healthSlider) healthSlider.value = s.health / 100f;

        if (ageText) ageText.text = $"時間: {Mathf.FloorToInt(s.age)}秒";

        if (statusMessageText)
        {
            if (s.hunger < 20f) statusMessageText.text = "😢 お腹が空いてるよ！";
            else if (s.happiness < 20f) statusMessageText.text = "😞 遊んでほしいな…";
            else if (s.health < 30f) statusMessageText.text = "🤒 具合が悪いみたい…";
            else if (s.happiness > 80f) statusMessageText.text = "😄 とっても幸せ！";
            else statusMessageText.text = "😊 元気だよ！";

           
        }
    }

    void ShowGameOver()
    {
        if (statusMessageText)
            statusMessageText.text = "💀 ペットが天国へ旅立ちました…";

        feedButton.interactable = false;
        playButton.interactable = false;
        sleepButton.interactable = false;
    }
   
}
