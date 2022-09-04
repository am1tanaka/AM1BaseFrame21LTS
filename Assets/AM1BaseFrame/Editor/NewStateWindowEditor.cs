using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using AM1.BaseFrame.General;
using System.Security.Cryptography.X509Certificates;

namespace AM1.BaseFrame.Editor
{
    public class NewStateWindowEditor : EditorWindow
    {
        static string ScriptFolder => "Assets/AM1/BaseFrame/Scripts/";

        TextField stateName;
        Toggle makeSceneToggle;
        TextField sceneName;
        EnumField transitionEnum;
        FloatField transitionSec;
        Button createButton;

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
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AM1BaseFrame/Editor/UXML/NewStateWindowEditor.uxml");
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
            createButton.SetEnabled(stateName.value.Length > 0);
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

        }
    }
}