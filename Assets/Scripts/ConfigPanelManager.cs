using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfigPanelManager : MonoBehaviour
{
    [Header("設定パネル")]
    public GameObject configPanel;
    public Button openConfigButton;
    public Button closeConfigButton;

    [Header("BGMスライダー")]
    public Slider bgmSlider;
    public TMP_Text bgmValueText;

    [Header("遊び方ボタンなど他のボタン")]
    public GameObject howToPlayButton;

    void Start()
    {
      
        if (openConfigButton != null)
            openConfigButton.onClick.AddListener(OnOpenConfig);
        if (closeConfigButton != null)
            closeConfigButton.onClick.AddListener(OnCloseConfig);

        
        if (bgmSlider != null)
        {
            bgmSlider.minValue = 0f;
            bgmSlider.maxValue = 1f;

           
            bgmSlider.value = AudioManager.Instance != null
                ? AudioManager.Instance.GetBGMVolume()
                : PlayerPrefs.GetFloat("bgmVolume", 1.0f);

            bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
        }

        UpdateValueText();

        if (configPanel != null)
            configPanel.SetActive(false);
    }

    public void OnOpenConfig()
    {
        if (configPanel != null)
            configPanel.SetActive(true);

        if (bgmSlider != null && AudioManager.Instance != null)
            bgmSlider.value = AudioManager.Instance.GetBGMVolume();

        if (configPanel != null)
            configPanel.SetActive(true);


        UpdateValueText();
    }

    public void OnCloseConfig()
    {
        if (configPanel != null)
            configPanel.SetActive(false);
    }

    void OnBGMSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetBGMVolume(value);

        UpdateValueText();
    }

    void UpdateValueText()
    {
        if (bgmValueText != null && bgmSlider != null)
            bgmValueText.text = $"{Mathf.RoundToInt(bgmSlider.value * 100)}%";
    }

}
