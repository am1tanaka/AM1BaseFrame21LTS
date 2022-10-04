using Codice.Client.BaseCommands.CheckIn.CodeReview;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.Events;
using static Codice.CM.Common.CmCallContext;

namespace AM1.State
{
    /// <summary>
    /// 状態切り替え管理クラス。IAM1Stateのインスタンスを受け取って状態を管理します。
    /// 汎用的に利用できるようにシングルトンは利用しません。<br></br>
    /// 通常は、このクラスを継承して状態管理用のコンポーネントを作り、
    /// 必要な各状態のインスタンスの初期化と保有を実装して、利用するゲームオブジェクトにアタッチします。
    /// </summary>
    public class AM1StateStack : MonoBehaviour
    {
        /// <summary>
        /// スタックの初期上限
        /// </summary>
        public static int DefaultStackMax => 8;

        /// <summary>
        /// フェーズスタック
        /// </summary>
        public readonly Stack<IAM1State> stateStack = new Stack<IAM1State>(DefaultStackMax);

        /// <summary>
        /// フェーズの要求
        /// </summary>
        public readonly Queue<IAM1State> requestQueue = new Queue<IAM1State>(DefaultStackMax);

        /// <summary>
        /// 現在のフェーズ情報のインスタンス
        /// </summary>
        public IAM1State CurrentStateInfo { get; protected set; }

        /// <summary>
        /// フェーズの切り替えと更新処理の呼び出し
        /// </summary>
        protected virtual void Update()
        {
            // 切り替え処理
            UpdateChangeRequest();

            // 更新処理
            CurrentStateInfo?.Update();
        }

        /// <summary>
        /// 物理更新処理
        /// </summary>
        protected virtual void FixedUpdate()
        {
            CurrentStateInfo?.FixedUpdate();
        }

        /// <summary>
        /// 現在の状態を一時停止して、指定の状態をスタックに積んで初期化
        /// </summary>
        /// <param name="nextState">切り替え先の状態</param>
        IEnumerator PushState(IAM1State nextState)
        {
            // 現在の状態があれば一時停止
            if (CurrentStateInfo != null)
            {
                CurrentStateInfo.Pause();

                // 切り替え可能になるまで待機
                while (!CurrentStateInfo.CanChangeToOtherState)
                {
                    yield return null;
                }
            }

            // 状態をプッシュ
           stateStack.Push(nextState);

            // 現在のインスタンス
            CurrentStateInfo = nextState;

            // 初期化
            CurrentStateInfo.Init();
        }

        /// <summary>
        /// 現在の状態を終了して、スタックから前の状態に切り替えて初期化
        /// </summary>
        /// <param name="currentState">現在の状態</param>
        IEnumerator PopState(IAM1State currentState)
        {
            // 現在の状態があれば終了
            if (CurrentStateInfo != null)
            {
                CurrentStateInfo.Terminate();

                // 切り替え可能になるまで待機
                while (!CurrentStateInfo.CanChangeToOtherState)
                {
                    yield return null;
                }
            }

            // スタックから状態を取り出す
            CurrentStateInfo = stateStack.Pop();

            // 再開
            CurrentStateInfo.Resume();
        }

        /// <summary>
        /// 状態の切り替え要求の確認と処理
        /// </summary>
        void UpdateChangeRequest()
        {
            // リクエストがないか、処理中以外の時は次のフェーズへは遷移しない
            if ((requestQueue.Count == 0)
                || ((CurrentStateInfo != null) && !CurrentStateInfo.CanChangeToOtherState)) {
                return;
            }

            // 要求を一つ取り出して実行
            var req = requestQueue.Dequeue();
            if (req.ChangeAction ==null)
            {
#if UNITY_EDITOR
                Debug.LogError($"状態切り替え要求データ異常:{(req.ChangeAction == null ? "切り替えアクションがnull" : "")}");
#endif
            }
            StartCoroutine(req.ChangeAction(req));
        }

        /// <summary>
        /// 現在の状態を閉じて、指定の状態へ切り替える要求をします。
        /// すでに他の切り替えが要求済みだったり、過去に同じ状態があれば失敗します。
        /// </summary>
        /// <param name="nextState">切り替えたい状態</param>
        /// <returns>要求が成功したらtrue</returns>
        public bool PopAndPushRequest(IAM1State nextState)
        {
            if (requestQueue.Count > 0) return false;
            return PopAndPushQueueRequest(nextState);
        }

        /// <summary>
        /// 現在の状態を閉じて、指定の状態へ切り替える要求をします。
        /// すでに要求がある場合はキューに登録します。
        /// </summary>
        /// <param name="st">切り替えたい状態</param>
        public bool PopAndPushQueueRequest(IAM1State nextState)
        {
            foreach (var state in stateStack)
            {
                if (state == nextState)
                {
                    return false;
                }
            }

            foreach (var req in requestQueue)
            {
                if (req == nextState)
                {
                    return false;
                }
            }

            // 現在の状態をPop
            if (CurrentStateInfo != null)
            {
                CurrentStateInfo.ChangeAction = PopState;
                requestQueue.Enqueue(CurrentStateInfo);
            }
            // 要求状態のPush
            nextState.ChangeAction = PushState;
            requestQueue.Enqueue(nextState);
            return true;
        }

        /// <summary>
        /// フェーズをプッシュして切り替えを要求します。
        /// </summary>
        /// <param name="reserve">切り替え中の時に予約するならtrue。失敗させるなら省略</param>
        /// <returns>要求が成功したらtrue。reserveが省略されていて切り替え中なら失敗でfalse</returns>
        public bool PushRequest(IAM1State ph, bool reserve = false)
        {
            if (CurrentStateInfo == null)
            {
                return Request(Push, null, ph, reserve);
            }
            return Request(Push, CurrentStateInfo.state.Pause, ph, reserve);
        }

        /// <summary>
        /// 一つ前のフェーズに戻します。
        /// </summary>
        /// <param name="reserve">切り替え中の時に予約するならtrue。失敗させるなら省略</param>
        /// <returns>要求が成功したらtrue。reserveが省略されていて切り替え中なら失敗でfalse</returns>
        public bool PopRequest(bool reserve = false)
        {
            // 実行フェーズのスタックが1つ以下の時はPopできないので成功させて終わり
            if (stateStack.Count <= 1)
            {
                return true;
            }

            return Request(Pop, CurrentStateInfo.state.Terminate, null, reserve);
        }

        /// <summary>
        /// ルートのフェーズまで戻します。
        /// </summary>
        /// <param name="reserve">切り替え中の時に予約するならtrue。失敗させるなら省略</param>
        /// <returns>要求が成功したらtrue。reserveが省略されていて切り替え中なら失敗でfalse</returns>
        public bool PopAllRequest(bool reserve = false)
        {
            // 実行フェーズのスタックが1つ以下の時はすでに完了しているので成功させて終わり
            if (stateStack.Count <= 1)
            {
                return true;
            }

            return Request(PopAll, CurrentStateInfo.state.Terminate, null, reserve);
        }
    }
}