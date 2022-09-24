using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System;

[System.Serializable]
public class AM1MakeBaseFrameSettings
{
    public string PublishRepositoryFolder = "";
}

public class AM1UpdateRepository
{
    /// <summary>
    /// パッケージフォルダー
    /// </summary>
    static string PackageFolder => "Assets/AM1BaseFrame";

    /// <summary>
    /// 設定ファイル名
    /// </summary>
    static string SettingFileName => "Settings.json";

    static string thisPath = "";

    [MenuItem("Tools/AM1/Make Package/Update Repository", false, 102)]
    static void UpdateRespositoryEditor()
    {
        // 初期値読み取り
        string loadPath = Path.Combine(GetThisPath(), SettingFileName);
        AM1MakeBaseFrameSettings settings = new AM1MakeBaseFrameSettings();
        if (!File.Exists(loadPath))
        {
            // ファイルがない時は保存
            SaveSetting(settings);
        }
        else
        {
            // 読み込み
            string json = File.ReadAllText(loadPath);
            settings = JsonUtility.FromJson<AM1MakeBaseFrameSettings>(json);
        }

        // フォルダー選択
        var selectedFolder = EditorUtility.SaveFolderPanel("公開用Gitリポジトリーをクローンしたフォルダーを選択してください", settings.PublishRepositoryFolder, "");
        if (string.IsNullOrEmpty(selectedFolder))
        {
            // キャンセル
            UnityEngine.Debug.Log($"処理をキャンセルしました。");
            return;
        }

        // 選択したフォルダーの保存
        settings.PublishRepositoryFolder = selectedFolder;
        SaveSetting(settings);

        // PackageFolderから指定のフォルダーへミラーリングコピー
        // サブフォルダーあり、
        // 無くなったフォルダーやファイルの削除、
        // .gitフォルダーは除く、
        // 古いファイルやフォルダーは除外
        // robocopy src dest /S /PURGE /XD .git /XO
        string src = Path.GetFullPath(PackageFolder);
        string arg = $"{src} {selectedFolder} /S /PURGE /XD .git /XO";
        Process.Start($"robocopy.exe", arg);

        // コミット
        ExecuteCommand("git", $@"-C {selectedFolder} add .");
        ExecuteCommand("git", $@"-C {selectedFolder} -c user.name=autocommit -c user.email=autocommit@autocommit commit -m Update");

        UnityEngine.Debug.Log($"更新を完了しました。GitHub DesktopでPushしてください。");
    }

    /// <summary>
    /// 参考 http://var.blog.jp/archives/24980791.html
    /// </summary>
    static int ExecuteCommand(string command, string arguments = "")
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo(command)
            {
                Arguments = arguments,
                UseShellExecute = false,
            }
        };
        process.Start();
        process.WaitForExit();
        return process.ExitCode;
    }

    /// <summary>
    /// このスクリプトのあるパスを返す
    /// </summary>
    /// <returns></returns>
    static string GetThisPath()
    {
        if (thisPath.Length == 0)
        {
            var assets = AssetDatabase.FindAssets("AM1UpdateRepository");
            var csPath = AssetDatabase.GUIDToAssetPath(assets[0]);
            thisPath = Path.GetDirectoryName(csPath);
        }

        return thisPath;
    }

    /// <summary>
    /// 設定を保存
    /// </summary>
    /// <param name="settings">保存する設定のインスタンス</param>
    static void SaveSetting(AM1MakeBaseFrameSettings settings)
    {
        string json = JsonUtility.ToJson(settings);
        string savePath = Path.Combine(GetThisPath(), SettingFileName);
        File.WriteAllText(savePath, json);
    }
}
