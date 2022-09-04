using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using AM1.BaseFrame.General;
using System.Security.Cryptography.X509Certificates;
using Codice.Client.Common;

namespace AM1.BaseFrame.General.Editor
{
    public class NewStateWindowEditor : EditorWindow
    {
        static string ScriptFolder => "Assets/AM1/BaseFrame/Scripts";
        static string StateChangerScriptFolder => "Assets/AM1/BaseFrame/Scripts/StateChangers";
        static string ImportedEditorFolder => "Assets/AM1/BaseFrame/Scripts/Editor";

        TextField stateName;
        Toggle makeSceneToggle;
        TextField sceneName;
        EnumField transitionEnum;
        FloatField transitionSec;
        Button createButton;

        /// <summary>
        /// 状態切り替えのファイル名
        /// </summary>
        string StateChangerScriptPath => $"{StateChangerScriptFolder}/{stateName.text}StateChanger.cs";

        [MenuItem("Tools/AM1/New State", false, 1)]
        public static void ShowNewState()
        {
            NewStateWindowEditor wnd = GetWindow<NewStateWindowEditor>();
            wnd.titleContent = new GUIContent("New State Window");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{ImportedEditorFolder}/UXML/NewStateWindowEditor.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            // 列挙子の初期設定
            stateName = rootVisualElement.Query<TextField>("StateNameText").First();
            makeSceneToggle = rootVisualElement.Query<Toggle>("MakeSceneToggle").First();
            sceneName = rootVisualElement.Query<TextField>("SceneName").First();
            transitionEnum = rootVisualElement.Query<EnumField>("ScreenTransitionTypeEnum").First();
            transitionSec = rootVisualElement.Query<FloatField>("ScreenTransitionSeconds").First();
            createButton = rootVisualElement.Query<Button>("CreateButton").First();

            transitionEnum.Init(ScreenTransitionType.FilledRadial);

            UpdateActivity();

            // ハンドラ登録
            SetupHandler();
        }

        /// <summary>
        /// 有効性を現状に合わせて更新
        /// </summary>
        void UpdateActivity()
        {
            // シーン名
            sceneName.SetEnabled(makeSceneToggle.value);

            // 画面遷移スクリプトを設定
            transitionSec.SetEnabled((ScreenTransitionType)transitionEnum.value != ScreenTransitionType.None);

            // 作成ボタン
            bool isExists = File.Exists(StateChangerScriptPath);
            createButton.SetEnabled(!isExists && (stateName.value.Length > 0));
        }

        /// <summary>
        /// ハンドラを登録
        /// </summary>
        void SetupHandler()
        {
            stateName.RegisterCallback<InputEvent>(OnChangeStateName);
            makeSceneToggle.RegisterCallback<ClickEvent>((ClickEvent cvt) => UpdateActivity());
            transitionEnum.RegisterCallback<ChangeEvent<System.Enum>>((ChangeEvent<System.Enum> ch) => UpdateActivity());
            createButton.RegisterCallback<ClickEvent>(CreateButtonProc);
        }

        void OnChangeStateName(InputEvent iev)
        {            
            if (iev.previousData == sceneName.value)
            {
                sceneName.value = iev.newData;
            }

            UpdateActivity();
        }

        /// <summary>
        /// 状態の作成を実行
        /// </summary>
        /// <param name="cvt"></param>
        void CreateButtonProc(ClickEvent cvt)
        {
            string packagePath = AM1BaseFrameUtils.packageFullPath;
            string templatePath = $"{packagePath}/Package Resources/SceneStateChangerTemplate.cs.txt";
            string tempText = File.ReadAllText(templatePath);

            // 状態名を置き換え
            tempText = tempText.Replace(":StateName:", stateName.text);

            // 画面切り替えの設定
            if (transitionEnum.text != "None")
            {
                var lines = tempText.Split("\n");
                tempText = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("//ScreenTransitionRegistry."))
                    {
                        lines[i] = lines[i].Replace("//", "");
                        lines[i] = lines[i].Replace(":ScreenTransitionType:", transitionEnum.text.Replace(" ", ""));
                        lines[i] = lines[i].Replace(":ScreenTransitionSeconds:", transitionSec.text+"f");
                    }

                    tempText += lines[i].Replace("\r", "\r\n");
                }
            }

            // シーン読み込み
            if (makeSceneToggle.value)
            {
                // シーン作成
                string loadCode = "// シーンの非同期読み込み開始\r\n";
                     loadCode += $"            StateChanger.LoadSceneAsync(\"{sceneName.text}\", true);\r\n";
                tempText = tempText.Replace(":LoadScene:", loadCode);
            }
            else
            {
                // シーン作成しない
                tempText = tempText.Replace(":LoadScene:", "");
            }

            // HideScreenは一先ず不要
            tempText = tempText.Replace(":OnHideScreen:", "");

            // OnAwakeDoneに画面遷移の解除
            if (transitionEnum.text != "None")
            {
                string uncover = "// 画面の覆いを解除\r\n";
                     uncover += $"            ScreenTransitionRegistry.StartUncover({transitionSec.value}f);\r\n";
                     uncover += $"            yield return ScreenTransitionRegistry.WaitAll();\r\n";
                tempText = tempText.Replace(":OnAwakeDone:", uncover);
            }

            // Terminateでシーンの解除
            if (makeSceneToggle.value)
            {
                string terminate = "// シーンの解放\r\n";
                terminate += $"            StateChanger.UnloadSceneAsync(\"{sceneName.text}\");\r\n";
                tempText = tempText.Replace(":Terminate:", terminate);
            }
            else
            {
                tempText = tempText.Replace(":Terminate:", "");
            }

            // 保存先フォルダーのチェック
            if (!Directory.Exists(StateChangerScriptFolder))
            {
                Directory.CreateDirectory(StateChangerScriptFolder);
            }
            File.WriteAllText(StateChangerScriptPath, tempText);
            AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

            // シーンの作成
            if (makeSceneToggle.value)
            {
                NewSceneEditor.NewScene(sceneName.text);
            }
        }
    }
}