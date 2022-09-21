using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace AM1.BaseFrame.Editor
{
    public class AM1BaseFrameMenu : EditorWindow
    {
        [MenuItem("Tools/AM1/Import BaseFrame Assets", false, 0)]
        static void ImportBaseFrameAssets()
        {
            // 保存先フォルダーの選択
            string selectedFolder = EditorUtility.SaveFolderPanel("起動スクリプトを保存するフォルダー(Scriptsフォルダーなど)を選択してください。", "Assets", "");
            Debug.Log($"slec={selectedFolder}");
            if (string.IsNullOrEmpty(selectedFolder)) {
                Debug.Log($"保存先の選択がキャンセルされたのでインポート処理を中止しました。");
                return;
            }

            // 選択先のフォルダーにBooterとBootStateChangerを作成
            AssetDatabase.Refresh();
            CreateScriptFromTemplate("Booter", selectedFolder);
            CreateScriptFromTemplate("BootStateChanger", selectedFolder);
            AssetDatabase.Refresh();

            // パッケージをインポート
            string baseFramePackagePath = AM1BaseFrameUtils.packageFullPath + "/Package Resources/BaseFrame.unitypackage";
            AssetDatabase.ImportPackage(baseFramePackagePath, true);
        }

        /// <summary>
        /// 指定のファイル名(拡張子不要)のテンプレートを読み込んで、
        /// 指定したフォルダーへスクリプトファイルとして保存。
        /// </summary>
        /// <param name="fileName">ファイル名(拡張子なし)</param>
        /// <param name="targetFolder">保存左記フォルダー</param>
        static void CreateScriptFromTemplate(string fileName, string targetFolder)
        {
            string booterPath = Path.Combine(AM1BaseFrameUtils.packageRelativePath, $"{fileName}Template.cs.txt");
            string booterText = File.ReadAllText(booterPath);
            string booterSaveName = Path.Combine(targetFolder, $"{fileName}.cs");
            string relPath = "Assets/"+Path.GetRelativePath(Application.dataPath, booterSaveName);
            string booterSavePath = AssetDatabase.GenerateUniqueAssetPath(relPath);
            File.WriteAllText(booterSavePath, booterText);
        }
    }
}