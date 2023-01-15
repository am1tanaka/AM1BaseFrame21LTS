using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using AM1.BaseFrame.Editor;

namespace AM1.PackageBaseFrame.Editor
{
    /// <summary>
    /// パッケージ作成用のスクリプト。頒布するパッケージには含めない。
    /// </summary>
    public class PackageBaseFrame
    {
        static string saveFolder= "Assets/AM1BaseFrame/Package Resources";

        [MenuItem("Tools/AM1/Make Package/Package BaseFrame Assets", false, 100)]
        static void PackageBaseFrameAssets()
        {
            // 保存先フォルダーを選択
            string savePath = EditorUtility.SaveFolderPanel("パッケージの保存先", saveFolder, "");
            if (!string.IsNullOrEmpty(savePath))
            {
                string relPath = "Assets/"+ AM1BaseFrameUtils.GetRelativePath(Application.dataPath, savePath);
                string packagePath = $"{relPath}/BaseFrame.unitypackage";
                if (File.Exists(packagePath))
                {
                    // 既存の場合、上書き確認
                    if (EditorUtility.DisplayDialog("Override package?", "既存のファイルに上書きしますか？", "上書き", "キャンセル"))
                    {
                        AssetDatabase.DeleteAsset(packagePath);
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        Debug.Log($"ユーザー操作によりキャンセルしました。");
                        return;
                    }

                    saveFolder = savePath;
                    List<string> exportFileList = new List<string>();
                    GetExportFiles("Assets/AM1/BaseFrame", exportFileList);
                    AssetDatabase.ExportPackage(exportFileList.ToArray(), savePath + "/BaseFrame.unitypackage", ExportPackageOptions.Recurse);
                    AssetDatabase.Refresh();
                }

            }
        }

        static void GetExportFiles(string target, List<string> exportFileList)
        {
            var files = Directory.EnumerateFiles(target);
            foreach (var file in files)
            {
                exportFileList.Add(file);
            }

            var subFolders = AssetDatabase.GetSubFolders(target);
            foreach (string folder in subFolders)
            {
                GetExportFiles(folder, exportFileList);
            }
        }
    }
}