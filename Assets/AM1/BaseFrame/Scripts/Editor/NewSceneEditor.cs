using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using System.IO;

namespace AM1.BaseFrame.General.Editor
{
    /// <summary>
    /// 新規シーンの作成。起動報告スクリプトを仕込んだオブジェクトを設定
    /// </summary>
    public class NewSceneEditor : EditorWindow
    {
        static string ImportedEditorFolder => "Assets/AM1/BaseFrame/Scripts/Editor";

        TextField sceneName;
        Button createButton;

        string savePathField = "Assets";

        [MenuItem("Tools/AM1/New BaseFrame Scene", false, 2)]
        static void NewBaseFrameScene()
        {
            var wnd = GetWindow<NewSceneEditor>();
            wnd.titleContent = new GUIContent("New BaseFrame Scene");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{ImportedEditorFolder}/UXML/NewBaseFrameSceneWindow.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            sceneName = root.Query<TextField>("SceneName").First();
            createButton = root.Query<Button>("CreateButton").First();

            // 状態設定
            UpdateElement();

            // 処理を登録
            sceneName.RegisterCallback<InputEvent>((InputEvent iev) => UpdateElement());
            createButton.RegisterCallback<ClickEvent>((ClickEvent cev) => NewSceneAndClearText(sceneName.text));
        }

        void UpdateElement()
        {
            createButton.SetEnabled(sceneName.text.Length > 0);
        }

        /// <summary>
        /// シーンを作成して、成功したらテキストをクリア。
        /// </summary>
        /// <param name="scName">作成するシーン名</param>
        void NewSceneAndClearText(string scName)
        {
            createButton.SetEnabled(false);

            // フォルダー選択
            string folder = EditorUtility.SaveFolderPanel("シーンの保存先フォルダー", savePathField, "");
            if (!string.IsNullOrEmpty(folder))
            {
                var saved = NewScene(scName, folder);
                sceneName.value = "";
                savePathField = Path.GetDirectoryName(saved);
            }

            UpdateElement();
        }

        /// <summary>
        /// シーン作成
        /// </summary>
        /// <param name="scName">シーン名</param>
        /// <param name="savePath">保存先フォルダー</param>
        /// <returns>保存先フォルダーのプロジェクトからの相対パス</returns>
        public static string NewScene(string scName, string savePath)
        {
            // 新しいシーンを作成
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

            // オブジェクト作成
            var go = new GameObject();
            go.name = $"{scName}Behaviour";
            go.AddComponent<AwakeReporter>();
            Undo.RegisterCreatedObjectUndo(go, $"Created {scName}Behaviour Object.");

            // フォルダー選択
            string scenePath = Path.Combine(savePath, scName + ".unity");

            // シーンの保存
            var relPath = "Assets/"+Path.GetRelativePath(Application.dataPath, scenePath);
            var path = AssetDatabase.GenerateUniqueAssetPath(relPath);
            EditorSceneManager.SaveScene(newScene, path);
            AssetDatabase.Refresh();

            return path;
        }
    }
}