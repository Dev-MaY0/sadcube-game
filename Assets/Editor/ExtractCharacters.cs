// Assets/Editor/ExtractCharacters.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ExtractCharacters : EditorWindow
{
    [MenuItem("Tools/日本語文字を抽出する")]
    public static void Extract()
    {
        string[] targetFiles = {
            "Assets/Scripts/UIManager.cs",
            "Assets/Scripts/PetController.cs",
            "Assets/Scripts/PetStatus.cs",
            "Assets/Scripts/GameManager.cs",
            "Assets/Scripts/GameData.cs",
            "Assets/Scripts/TitleSceneManager.cs",
            "Assets/Scripts/MiniGameManager.cs",
            "Assets/Scripts/FruitController.cs",
            "Assets/Scripts/LevelUpManager.cs",
            "Assets/Scripts/LoginBonus.cs",
            "Assets/Scripts/AchievementData.cs",
            "Assets/Scripts/AchievementManager.cs",
            "Assets/Scripts/AchievementNotification.cs",
            "Assets/Scripts/AchievementListUI.cs",
            "Assets/Scripts/PetNameInputManager.cs",
            "Assets/Scripts/NameChangeManager.cs",
        };

        var charSet = new SortedSet<char>();

        // 基本ASCII文字
        for (char c = ' '; c <= '~'; c++)
            charSet.Add(c);

        // 各スクリプトから文字列リテラルを抽出
        foreach (var path in targetFiles)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"ファイルが見つかりません: {path}");
                continue;
            }

            string code = File.ReadAllText(path, Encoding.UTF8);

            bool inString = false;
            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] == '"' && (i == 0 || code[i - 1] != '\\'))
                {
                    inString = !inString;
                    continue;
                }
                if (inString)
                    charSet.Add(code[i]);
            }
        }

        // 念のため手動で追加する文字
        string extraChars =
            "あいうえおかきくけこさしすせそたちつてとなにぬねの" +
            "はひふへほまみむめもやゆよらりるれろわをん" +
            "アイウエオカキクケコサシスセソタチツテトナニヌネノ" +
            "ハヒフヘホマミムメモヤユヨラリルレロワヲン" +
            "ぁぃぅぇぉっゃゅょァィゥェォッャュョ" +
            "がぎぐげござじずぜぞだぢづでどばびぶべぼぱぴぷぺぽ" +
            "ガギグゲゴザジズゼゾダヂヅデドバビブベボパピプペポ" +
            "ーっ。、！？「」『』【】…・〜";

        foreach (char c in extraChars)
            charSet.Add(c);

        string result = new string(new List<char>(charSet).ToArray());
        string outputPath = "Assets/tmp_characters.txt";
        File.WriteAllText(outputPath, result, Encoding.UTF8);
        AssetDatabase.Refresh();

        Debug.Log($"抽出完了！文字数: {result.Length}");
        EditorUtility.DisplayDialog("抽出完了",
            $"文字数: {result.Length} 文字\n保存先: {outputPath}\n\nFont Asset CreatorのCustom Character Listに貼り付けてください。",
            "OK");
    }
}
