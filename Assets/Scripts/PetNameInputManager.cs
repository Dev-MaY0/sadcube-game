using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PetNameInputManager : MonoBehaviour
{
    [Header("名前入力パネル")]
    public GameObject nameInputPanel;  // 名前入力画面全体
    public TMP_InputField nameInputField; // 名前入力欄
    public Button confirmButton;       // 決定ボタン
    public TMP_Text errorText;         // エラーメッセージ

    [Header("ペット名表示")]
    public TMP_Text petNameText;       // PetScene上のペット名表示

    void Start()
    {
        if (GameData.Instance == null)
        {
            var go = new GameObject("GameData");
            go.AddComponent<GameData>();
        }

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmButton);

        if (errorText != null)
            errorText.gameObject.SetActive(false);

        // 名前が未設定なら入力画面を表示
        if (string.IsNullOrEmpty(GameData.Instance.petName))
        {
            if (nameInputPanel != null)
                nameInputPanel.SetActive(true);
        }
        else
        {
            if (nameInputPanel != null)
                nameInputPanel.SetActive(false);

            UpdatePetNameDisplay();
        }
    }

    public void OnConfirmButton()
    {
        if (nameInputField == null) return;

        string inputName = nameInputField.text.Trim();

        // バリデーション
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

        // 名前を保存
        GameData.Instance.petName = inputName;
        GameData.Instance.Save();

        // パネルを閉じる
        if (nameInputPanel != null)
            nameInputPanel.SetActive(false);

        UpdatePetNameDisplay();
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
