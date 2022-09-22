using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class SetDemoTagAndLayer
{
    [MenuItem("Tools/AM1/Set Demo Tags and Layers", false, 400)]
    static void SetDemoTagsAndLayersEditor()
    {
        if (EditorUtility.DisplayDialog("タグとレイヤーの読み込み", "Demo用のタグとレイヤーを読み込みますか？", "読み込む", "キャンセル"))
        {
            var assets = AssetDatabase.FindAssets("SetDemoTagAndLayer");
            string path = "";
            foreach(var asset in assets)
            {
                path = AssetDatabase.GUIDToAssetPath(asset);
                if (path.Contains("AM1 Base Framework") || path.Contains("AM1BaseFramework"))
                {
                    break;
                }
                path = "";
            }
            if (path.Length == 0)
            {
                Debug.LogError("デモフォルダーの構造にエラーがあります。再インストールしてください。");
                return;
            }

            var filePath = Path.Combine(Path.GetDirectoryName(path), "TagManager.asset.txt");
            if (!File.Exists(filePath))
            {
                Debug.LogError("必要なファイルがありません。デモを再インストールしてください。");
                return;
            }

            var text = File.ReadAllBytes(filePath);
            string saveFileName = Path.Combine(Directory.GetCurrentDirectory(), "ProjectSettings/TagManager.asset");
            File.WriteAllBytes(saveFileName, text);
            AssetDatabase.Refresh();
        }
    }
}
