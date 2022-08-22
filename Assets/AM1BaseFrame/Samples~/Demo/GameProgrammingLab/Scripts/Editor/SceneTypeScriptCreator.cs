using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

/// <summary>
/// シーンの種類を管理するスクリプトを作成するクラス
/// </summary>
/// <see href="https://game-programming-lab.com/unity/editor-extension-in-unity/auto-create-scene-type-in-unity/">Game Programming Lab</see>
public static class SceneTypeScriptCreator
{
    private const string MENU_ITEM_NAME = "Tools/Scene Type/Create";
    private const string SCRIPT_FILE_PATH = "Assets/AM1Framework/Demo/Scripts/Constants/SceneType.cs";

    /// <summary>
    /// シーンリストを変更した時の処理
    /// </summary>
    [MenuItem(MENU_ITEM_NAME)]
    private static void Create()
    {
        if (!CanCreate()) return;

        CreateDirectoryIfNotExists();
        var script_string = CreateScriptString();

        File.WriteAllText(SCRIPT_FILE_PATH, script_string, Encoding.UTF8);
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

        EditorUtility.DisplayDialog(SCRIPT_FILE_PATH, "シーンの種類の作成が完了しました。", "OK");
    }

    /// <summary>
    /// 作成できるか判別する値を取得
    /// </summary>
    private static bool CanCreate() => !EditorApplication.isPlaying && !EditorApplication.isCompiling;

    /// <summary>
    /// スクリプトの文字列を作成
    /// </summary>
    private static string CreateScriptString()
    {
        var contents = EditorBuildSettings.scenes
            .Select(scene => Path.GetFileNameWithoutExtension(scene.path))
            .Distinct()
            .Select(scene_name => scene_name)
            .Aggregate(String.Empty, (concated, scene_name) => concated + $"    {scene_name},{Environment.NewLine}")
        ;

        var builder = new StringBuilder()
            .AppendLine("/// <summary>")
            .AppendLine("/// シーンの種類を管理する列挙型")
            .AppendLine("/// <summary>")
            .AppendLine("public enum SceneType")
            .AppendLine("{")
            .Append(contents)
            .AppendLine("}")
        ;

        return builder.ToString();
    }

    /// <summary>
    /// アッパーキャメルケースからスネークケースに変換
    /// </summary>
    private static string ToSnakeCaseFromUpperCamelCase(string value)
    {
        return Regex.Replace(value, "([a-z])([A-Z])", "$1_$2").ToUpper();
    }

    /// <summary>
    /// ディレクトリが存在しない場合、作成
    /// </summary>
    private static void CreateDirectoryIfNotExists()
    {
        var directory_path = Path.GetDirectoryName(SCRIPT_FILE_PATH);
        if (Directory.Exists(directory_path)) return;

        Directory.CreateDirectory(directory_path);
    }
}