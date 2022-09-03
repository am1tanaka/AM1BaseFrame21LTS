using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using AM1.BaseFrame.General;
using UnityEngine.SceneManagement;
using Codice.Client.BaseCommands;
using System.IO;

namespace AM1.BaseFrame.Editor
{
    /// <summary>
    /// 状態システムのシステムシーンに必要な最小限の構成を現在のアクティブシーンに追加するスクリプト
    /// </summary>
    public class SetStateSystemToActiveSceneEditor : EditorWindow
    {
        public static string ScriptPath => "Assets/AM1/BaseFrame/Scripts/";
        public static string GeneratedScriptPath => "Assets/AM1/BaseFrame/Scripts/Generated/";
        public static string ScriptTemplatePath => "Assets/AM1BaseFrame/Package Resources/";

        public static string PrefabPath => "Assets/AM1/BaseFrame/Prefabs/";

        /// <summary>
        /// 実行結果のテキスト
        /// </summary>
        static string report;

        /// <summary>
        /// 既存のシステムオブジェクトのリスト
        /// </summary>
        static List<GameObject> existsSystemObjects = new List<GameObject>();

        [MenuItem("Tools/AM1/Set StateSystem to Active Scene", false, 0)]
        static void SetStateSystemToActiveScene()
        {
            // アクティブシーンにすでにシステムに必要なスクリプトが揃っているかを確認
            string mes = "";

            if (AreSystemComponents())
            {
                mes = report + "\n\n";
                Selection.objects = existsSystemObjects.ToArray();
            }

            mes += $"シーン'{SceneManager.GetActiveScene().name}'にシステムオブジェクトを追加しますか？";

            if (EditorUtility.DisplayDialog("Systemシーン用のオブジェクトの生成", mes, "追加", "いいえ"))
            {
                SetSystemObjects();
            }
        }

        /// <summary>
        /// システムに必要なコンポーネントが揃っているかを確認
        /// </summary>
        /// <returns>いくつか存在している時true</returns>
        static bool AreSystemComponents()
        {
            report = "";
            bool res = false;
            existsSystemObjects.Clear();

            var booter = FindObjectsOfType<Booter>();
            res |= ExistsActiveScene(booter, "Booterオブジェクト");

            var stateChanger = FindObjectsOfType<StateChanger>();
            res |= ExistsActiveScene(stateChanger, "状態切り替え管理スクリプト StateChanger");

            var bgm = FindObjectsOfType<BGMSourceAndClips>();
            res |= ExistsActiveScene(bgm, "BGM再生スクリプト BGMSourceAndClips");

            var se = FindObjectsOfType<SESourceAndClips>();
            res |= ExistsActiveScene(se, "効果音再生スクリプト SESourceAndClips");

            var transitions = FindObjectsOfType<StandardTransition>();
            res |= ExistsActiveScene(transitions, "画面切り替え StandardScreenTransition");

            return res;
        }

        static bool ExistsActiveScene(MonoBehaviour[] objs, string nm)
        {
            bool res = false;

            foreach (var obj in objs)
            {
                if (obj.gameObject.scene == SceneManager.GetActiveScene()) {
                    report += $"{nm}がすでにシーンにあります。\n\n";
                    existsSystemObjects.Add(obj.gameObject);
                    res = true;
                }
            }

            return res;
        }

        /// <summary>
        /// システムオブジェクトをシーンに配置
        /// </summary>
        static void SetSystemObjects()
        {
            var target = SceneManager.GetActiveScene();

            CreateBooter();
            AddPrefab("StateChanger");
            AddPrefab("AudioPlayer");
            AddPrefab("FadeCanvas");
        }

        static void CreateBooter()
        {
            var booterObject = new GameObject();
            booterObject.name = "Booter";
            booterObject.AddComponent(typeof(Booter));
            Undo.RegisterCreatedObjectUndo(booterObject, "Created Booter Object");
        }

        /// <summary>
        /// 指定のプレハブをシーンに追加
        /// </summary>
        /// <param name="prefab">アタッチするプレハブの名前</param>
        static void AddPrefab(string prefab)
        {
            var prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabPath}{prefab}.prefab");
            var go = PrefabUtility.InstantiatePrefab(prefabObject);
            Undo.RegisterCreatedObjectUndo(go, $"Instantiated {prefab} prefab");
        }

        /// <summary>
        /// ファイル名(拡張子不要)を指定して、テンプレートテキストを読み込んで、スクリプトファイルとして保存。
        /// </summary>
        /// <param name="fname">スクリプトのファイル名。拡張子(.cs)不要</param>
        static void CreateScript(string fname)
        {
            var scriptSource = AssetDatabase.LoadAssetAtPath<TextAsset>($"{ScriptTemplatePath}{fname}.cs.txt");
            string exportPath = $"{GeneratedScriptPath}{fname}.cs";
            File.WriteAllText(exportPath, scriptSource.text);
            AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
        }

    }
}