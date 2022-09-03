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
        public static string GeneratedScriptPath => "Assets/AM1/BaseFrame/Scripts/Generated/";
        public static string ScriptTemplatePath => "Assets/AM1BaseFrame/Package Resources/";

        [MenuItem("Tools/AM1/Set StateSystem to Active Scene", false, 0)]
        static void SetStateSystemToActiveScene()
        {
            var wnd = GetWindow<SetStateSystemToActiveSceneEditor>();
            wnd.titleContent = new GUIContent("Set State System To Active Scene");
        }

        /// <summary>
        /// 実行結果のテキスト
        /// </summary>
        string report;

        /// <summary>
        /// 既存のシステムオブジェクトのリスト
        /// </summary>
        List<GameObject> existsSystemObjects = new List<GameObject>();

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AM1BaseFrame/Editor/UXML/SetStateSystemToActiveSceneEditor.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            // アクティブシーンにすでにシステムに必要なスクリプトが揃っているかを確認
            var qtext = rootVisualElement.Query<Label>("QText");
            var qtextFirst = qtext.First();
 
            if (AreSystemComponents())
            {
                qtextFirst.text = report + "\n\n";
                Selection.objects = existsSystemObjects.ToArray();
            }

            qtextFirst.text += $"シーン'{SceneManager.GetActiveScene().name}'にシステムオブジェクトを追加しますか？";

            // ハンドラ登録
            SetupHandler();
        }

        /// <summary>
        /// システムに必要なコンポーネントが揃っているかを確認
        /// </summary>
        /// <returns>いくつか存在している時true</returns>
        bool AreSystemComponents()
        {
            report = "";
            bool res = false;
            existsSystemObjects.Clear();

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

        bool ExistsActiveScene(MonoBehaviour[] objs, string nm)
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

        void SetupHandler()
        {
            var buttons = rootVisualElement.Query<Button>();
            foreach(var button in buttons.ToList())
            {
                if (button.name == "AcceptButton")
                {
                    button.RegisterCallback<ClickEvent>(SetSystemObjects);
                }
                else
                {
                    button.RegisterCallback<ClickEvent>((ClickEvent evt) => Close());
                }
            }
        }

        /// <summary>
        /// システムオブジェクトをシーンに配置
        /// </summary>
        void SetSystemObjects(ClickEvent evt)
        {
            var target = SceneManager.GetActiveScene();

            CreateBooter();

            /*
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AM1BaseFrame/Editor/UXML/SetStateSystemToActiveSceneEditor.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            */

        }

        void CreateBooter()
        {
            // 作成先フォルダーの確認
            if (!Directory.Exists(GeneratedScriptPath))
            {
                Directory.CreateDirectory(GeneratedScriptPath);
            }

            // 切り替えスクリプト
            var bootStateChangerScripts = AssetDatabase.FindAssets("BootStateChanger", new[] { GeneratedScriptPath });
            if (bootStateChangerScripts.Length == 0)
            {
                // スクリプト作成
                CreateScript("BootStateChanger");
            }

            var booterScripts = AssetDatabase.FindAssets("Booter", new[] { GeneratedScriptPath });
            if (booterScripts.Length == 0)
            {
                // Booterスクリプトを作成
                CreateScript("Booter");
            }

            var booterObject = new GameObject();
            var booterScript = AssetDatabase.LoadAssetAtPath<MonoBehaviour>($"{GeneratedScriptPath}Booter.cs");
            booterObject.AddComponent(booterScript);
        }

        /// <summary>
        /// ファイル名(拡張子不要)を指定して、テンプレートテキストを読み込んで、スクリプトファイルとして保存。
        /// </summary>
        /// <param name="fname">スクリプトのファイル名。拡張子(.cs)不要</param>
        void CreateScript(string fname)
        {
            var scriptSource = AssetDatabase.LoadAssetAtPath<TextAsset>($"{ScriptTemplatePath}{fname}.cs.txt");
            string exportPath = $"{GeneratedScriptPath}{fname}.cs";
            File.WriteAllText(exportPath, scriptSource.text);
            AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
        }

    }
}