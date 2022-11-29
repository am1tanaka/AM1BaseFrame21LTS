//#define DEBUG_LOG

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AM1.BaseFrame
{
    /// <summary>
    /// シーン読み込みや解放時の非同期リスト用データ
    /// </summary>
    public struct AsyncData
    {
        /// <summary>
        /// シーン名
        /// </summary>
        public string sceneName;

        /// <summary>
        /// 非同期インスタンス
        /// </summary>
        public AsyncOperation asyncOperation;

        public AsyncData(string nm, AsyncOperation op)
        {
            sceneName = nm;
            asyncOperation = op;
        }
    }

    /// <summary>
    /// シーン切り替えを管理するクラス。
    /// </summary>
    public class SceneStateChanger : MonoBehaviour
    {
        /// <summary>
        /// 開始の準備が整ったらtrue。
        /// このフラグがfalseの時、他シーンは一度削除されるので
        /// Awake()やStart()の処理をしないようなコードを入れることで
        /// エディターでの動作が安定する。
        /// </summary>
        public static bool IsReady { get; private set; } = false;

        /// <summary>
        /// 状態切り替えが要求されているか、実行中の時、true
        /// </summary>
        public static bool IsRequestOrChanging => IsChanging || (nextStates.Count > 0);

        /// <summary>
        /// 切り替え処理中
        /// </summary>
        public static bool IsChanging { get; private set; }

        /// <summary>
        /// 現在の非同期読み込み具合。最初は0。読み込みが完了したら100
        /// </summary>
        public static int AsyncProgress { get; private set; }

        /// <summary>
        /// 次のフレームで切り替えたい状態のインスタンス
        /// </summary>
        static Queue<ISceneStateChanger> nextStates = new Queue<ISceneStateChanger>();

        /// <summary>
        /// 現在の状態
        /// </summary>
        public static ISceneStateChanger CurrentState { get; private set; }

        /// <summary>
        /// 前回の状態
        /// </summary>
        public static ISceneStateChanger LastState { get; private set; }

        /// <summary>
        /// シーン切り替えのプログレスバーの管理インスタンス
        /// </summary>
        static ISceneStateChangeProgress sceneChangeProgress;

        static List<AsyncData> asyncLoadOperationList = new List<AsyncData>();
        static List<AsyncOperation> asyncUnloadOperationList = new List<AsyncOperation>();

        /// <summary>
        /// リスタートのためstatic情報をリセット
        /// </summary>
        public static void ResetStatics()
        {
            IsChanging = false;
            nextStates.Clear();
            CurrentState = null;
            LastState = null;
            sceneChangeProgress = null;
            asyncLoadOperationList.Clear();
            asyncUnloadOperationList.Clear();
        }

        private void Update()
        {
            // 切り替え中かnextScenesが空なら何もしない
            if (IsChanging || (nextStates.Count == 0)) return;

            IsChanging = true;
            IsReady = true;
            var sc = nextStates.Dequeue();
            StartCoroutine(ChangeScene(sc));
        }

        IEnumerator ChangeScene(ISceneStateChanger next)
        {
            disallowActiveInstance = null;

            // サブシーンの非同期読み込み開始(次のシーンの初期化処理)
            next.Init();

            // 画面が隠れるのを待つ
            yield return ScreenTransitionRegistry.WaitAll();

            // 前のシーンのシーン解放などの終了処理を呼び出す
            CurrentState?.Terminate();
            yield return WaitUnloadScenes();

            // 読み込みなおしが必要なサブシーンの読み込みを開始
            next.OnHideScreen();

            // 非同期の進捗値を合算して、進捗を表示
            AsyncProgress = 0;
            if (sceneChangeProgress != null)
            {
                sceneChangeProgress.Show();
            }

            // 全てのシーンの読み込みが完了して、読み込んだシーンのAwakeが呼ばれるのを待つ
            yield return WaitAsyncAndAwake();
            if (sceneChangeProgress != null)
            {
                sceneChangeProgress.Hide();
            }

            // 次のシーンのすべてのAwake終了後の初期化処理(見える開始)
            // 画面が見える演出の完了待ち
            yield return next.OnAwakeDone();

            // シーンの切り替えフラグを解除
            IsChanging = false;
            LastState = CurrentState;
            CurrentState = next;
        }

        /// <summary>
        /// 指定の状態への切り替え要求を受け取る。
        /// すでに他のシーンへの切り替え中はキャンセル。
        /// </summary>
        /// <param name="target">切り替えたい状態への切り替え処理インスタンス</param>
        /// <param name="canQueue">切り替え中や要求が出ている時の追加要求をキューに詰む場合はtrue。デフォルトfalse</param>
        /// <returns>切り替え要求を受け取ったらtrue</returns>
        public static bool ChangeRequest(ISceneStateChanger target, bool canQueue = false)
        {
            // すでに変更中だったり、変更要望を受け取っていたらキャンセル
            if (!canQueue && IsRequestOrChanging) return false;

            // 次のフレームでシーン切り替えをするためのデータ設定
            nextStates.Enqueue(target);
            return true;
        }

        /// <summary>
        /// 指定の状態への切り替えが完了していることを確認。
        /// </summary>
        /// <param name="target">目的の状態</param>
        /// <returns>切り替え処理が完了していて、指定の状態ならtrue</returns>
        public static bool IsStateStarted(ISceneStateChanger target)
        {
            if (IsRequestOrChanging) return false;

            return CurrentState == target;
        }

        /// <summary>
        /// allowSceneActivateを設定しているインスタンス。未設定時はnull
        /// </summary>
        static AsyncOperation disallowActiveInstance;

        /// <summary>
        /// 指定のシーンを非同期読み込み開始。
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <param name="disallowActive">シーンの自動読み込みを無効化したい時、true</param>
        public static void LoadSceneAsync(string sceneName, bool disallowActive)
        {
            DebugLog($"LoadSceneAsync({sceneName}, {disallowActive}) {disallowActiveInstance}");

            var sc = SceneManager.GetSceneByName(sceneName);

            // シーンがない時のみ読み込み開始
            if ((sc == null) || !sc.IsValid())
            {
                var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                asyncLoadOperationList.Add(new AsyncData(sceneName, op));
                if (disallowActive && (disallowActiveInstance == null))
                {
                    disallowActiveInstance = op;
                    op.allowSceneActivation = false;
                }
            }
        }

        /// <summary>
        /// 指定のシーンを非同期解放開始。
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <param name="disallowActive">シーンの自動読み込みを無効化したい時、true</param>
        public static void UnloadSceneAsync(string sceneName)
        {
            DebugLog($"UnloadSceneAsync({sceneName})");

            var sc = SceneManager.GetSceneByName(sceneName);

            // シーンが有効な時のみ解放
            if ((sc != null) && sc.IsValid())
            {
                var op = SceneManager.UnloadSceneAsync(sceneName);
                asyncUnloadOperationList.Add(op);
            }
        }

        /// <summary>
        /// 指定のシーンのAwakeが完了したら呼び出す。
        /// </summary>
        /// <param name="sceneName">Awake完了したシーン名</param>
        public static void AwakeDone(string sceneName)
        {
            DebugLog($"AwakeDone({sceneName})");

            for (int i = 0; i < asyncLoadOperationList.Count; i++)
            {
                if (asyncLoadOperationList[i].sceneName == sceneName)
                {
                    asyncLoadOperationList.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// プログレスバーのインスタンスを渡す
        /// </summary>
        /// <param name="pr"></param>
        public static void SetProgressInterface(ISceneStateChangeProgress pr)
        {
            sceneChangeProgress = pr;
        }

        /// <summary>
        /// 全てのシーンの読み込みと解放待ち
        /// </summary>
        public static IEnumerator WaitAsyncAndAwake()
        {
            DebugLog($"WaitAsyncAndAwake()");

            // 読み込みシーンのAwake待ち
            while (asyncLoadOperationList.Count > 0)
            {
                yield return null;
            }

            // 解放待ち
            yield return WaitUnloadScenes();
        }

        /// <summary>
        /// シーンのActivateを開始して、シーンが全て解放されるまで待つ。
        /// </summary>
        public static IEnumerator WaitUnloadScenes()
        {
            if (disallowActiveInstance != null)
            {
                disallowActiveInstance.allowSceneActivation = true;
                disallowActiveInstance = null;
            }
            for (int i = 0; i < asyncUnloadOperationList.Count; i++)
            {
                while (!asyncUnloadOperationList[i].isDone)
                {
                    yield return null;
                }
            }
            asyncUnloadOperationList.Clear();
        }

        [System.Diagnostics.Conditional("DEBUG_LOG")]
        public static void DebugLog(object mes)
        {
            Debug.Log(mes);
        }

    }
}