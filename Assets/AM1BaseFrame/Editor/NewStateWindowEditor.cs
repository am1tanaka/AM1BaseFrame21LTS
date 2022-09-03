using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AM1.BaseFrame.Editor
{
    public class NewStateWindowEditor : EditorWindow
    {
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

            // ハンドラ登録
            SetupHandler();
        }

        void SetupHandler()
        {
        }
    }
}