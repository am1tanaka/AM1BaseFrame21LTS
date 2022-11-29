using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.PackageManager;
using System.Threading;

namespace AM1.BaseFrame.Editor
{
    public class AM1BaseFrameMenu : EditorWindow
    {
        static string booterFolder;

        [MenuItem("Tools/AM1/Import BaseFrame Assets", false, 0)]
        static void ImportBaseFrameAssets()
        {
            // 保存先フォルダーの選択
            booterFolder = EditorUtility.SaveFolderPanel("起動スクリプトを保存するフォルダー(Scriptsフォルダーなど)を選択してください。", "Assets", "");
            if (string.IsNullOrEmpty(booterFolder)) {
                Debug.Log($"保存先の選択がキャンセルされたのでインポート処理を中止しました。");
                return;
            }

            // パッケージをインポート
            string baseFramePackagePath = AM1BaseFrameUtils.packageFullPath + "/Package Resources/BaseFrame.unitypackage";
            AssetDatabase.ImportPackage(baseFramePackagePath, true);
            CreateBooterScript();

            // gitignoreとeditorconfigを作成
            CreateGitIgnoreAndEditorConfig();
        }

        [MenuItem("Tools/AM1/Create gitignore and editorconfig", false, 60)]
        static void SaveGitIgnoreAndEditorConfigMenu()
        {
            if (EditorUtility.DisplayDialog(".gitignoreと.editorconfigの作成", ".gitignoreと.editorconfigを作成しますか？", "作成", "キャンセル"))
            {
                CreateGitIgnoreAndEditorConfig();
            }
        }

        static void CreateGitIgnoreAndEditorConfig()
        {
            string projectPath = Path.GetFullPath(Application.dataPath + "/..");
            string editorconfig = Path.Combine(projectPath, ".editorconfig");
            AM1BaseFrameUtils.SaveTemplateNotExists("editorconfig.txt", editorconfig);
            string gitignore = Path.Combine(projectPath, ".gitignore");
            AM1BaseFrameUtils.SaveTemplateNotExists("gitignore.txt", gitignore);
        }

        /// <summary>
        /// 起動スクリプトを生成
        /// </summary>
        static void CreateBooterScript()
        {
            // Booterスクリプトを作成
            AssetDatabase.Refresh();
            CreateScriptFromTemplate("Booter", booterFolder);
            CreateScriptFromTemplate("BootSceneStateChanger", booterFolder);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 指定のファイル名(拡張子不要)のテンプレートを読み込んで、
        /// 指定したフォルダーへスクリプトファイルとして保存。
        /// </summary>
        /// <param name="fileName">ファイル名(拡張子なし)</param>
        /// <param name="targetFolder">保存左記フォルダー</param>
        static void CreateScriptFromTemplate(string fileName, string targetFolder)
        {
            // すでにファイルがあるなら何もしない
            string booterSaveName = Path.Combine(targetFolder, $"{fileName}.cs");
            string relPath = "Assets/" + AM1BaseFrameUtils.GetRelativePath(Application.dataPath, booterSaveName);
            string booterSavePath = AssetDatabase.GenerateUniqueAssetPath(relPath);
            if (File.Exists(booterSavePath))
            {
                Debug.LogWarning($"{booterSavePath}がすでにあるのでスクリプトの作成をスキップします。");
                return;
            }

            // テンプレートから指定フォルダーへスクリプトとして保存
            string booterPath = Path.Combine(AM1BaseFrameUtils.packageRelativePath+"/Package Resources", $"{fileName}Template.cs.txt");
            string booterText = File.ReadAllText(booterPath);
            File.WriteAllText(booterSavePath, booterText);
        }
    }
}