using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameChangeManager : MonoBehaviour
{
    [Header("名前変更パネル")]
    public GameObject nameChangePanel;
    public TMP_InputField nameInputField;
    public Button openButton;    // 名前変更ボタン
    public Button saveButton;    // 保存ボタン
    public Button cancelButton;  // キャンセルボタン
    public TMP_Text errorText;

    [Header("ペット名表示")]
    public TMP_Text petNameText;

    void Start()
    {
        if (openButton != null) openButton.onClick.AddListener(OnOpenButton);
        if (saveButton != null) saveButton.onClick.AddListener(OnSaveButton);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnCancelButton);

        if (nameChangePanel != null) nameChangePanel.SetActive(false);
        if (errorText != null) errorText.gameObject.SetActive(false);

        UpdatePetNameDisplay();
    }

    public void OnOpenButton()
    {
        if (nameChangePanel != null) nameChangePanel.SetActive(true);

        // 現在の名前を入力欄に入れておく
        if (nameInputField != null && GameData.Instance != null)
            nameInputField.text = GameData.Instance.petName;
    }

    public void OnSaveButton()
    {
        if (nameInputField == null) return;

        string inputName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(inputName))
        {
            ShowError("名前を入力してください");
            return;
        }
        if (inputName.Length > 10)
        {
            ShowError("10文字以内で入力してください");
            return;
        }

        GameData.Instance.petName = inputName;
        GameData.Instance.Save();

        if (nameChangePanel != null) nameChangePanel.SetActive(false);

        UpdatePetNameDisplay();
    }

    public void OnCancelButton()
    {
        if (nameChangePanel != null) nameChangePanel.SetActive(false);
        if (errorText != null) errorText.gameObject.SetActive(false);
    }

    void ShowError(string message)
    {
        if (errorText == null) return;
        errorText.text = message;
        errorText.gameObject.SetActive(true);
    }

    void UpdatePetNameDisplay()
    {
        if (petNameText != null && GameData.Instance != null)
            petNameText.text = GameData.Instance.petName;
    }
}
