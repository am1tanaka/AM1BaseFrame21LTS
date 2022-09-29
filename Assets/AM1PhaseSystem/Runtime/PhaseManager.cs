using Codice.Client.BaseCommands.CheckIn.CodeReview;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.Events;

namespace AM1.PhaseSystem
{
    /// <summary>
    /// フェーズ管理クラス。IPhaseのインスタンスを受け取ってフェースを管理します。
    /// 状態システムと異なり、フェーズシステムは複数のインスタンスで利用できるように
    /// シングルトンは利用しません。<br></br>
    /// 更新の実行タイミングを調整したい場合はこのクラスを継承します。
    /// </summary>
    public class PhaseManager : MonoBehaviour
    {
        public enum State
        {
            /// <summary>
            /// 待機中
            /// </summary>
            Standby,
            /// <summary>
            /// 通常動作中
            /// </summary>
            Running,
            /// <summary>
            /// 終了待ち
            /// </summary>
            WaitTerminate,
            /// <summary>
            /// ルートまで戻す
            /// </summary>
            PopAll,
        }

        /// <summary>
        /// 状態
        /// </summary>
        public State CurrentState { get; protected set; }

        /// <summary>
        /// スタックの初期上限
        /// </summary>
        static int StackMax => 8;

        /// <summary>
        /// フェーズスタック
        /// </summary>
        public readonly Stack<PhaseInfo> phaseStack = new Stack<PhaseInfo>(StackMax);

        /// <summary>
        /// フェーズの要求
        /// </summary>
        public readonly Queue<PhaseInfo> requestQueue = new Queue<PhaseInfo>(StackMax);

        /// <summary>
        /// 要求インスタンス
        /// </summary>
        public readonly Stack<PhaseInfo> phaseInfoPool = new Stack<PhaseInfo>(StackMax);

        /// <summary>
        /// 現在のフェーズ情報のインスタンス
        /// </summary>
        public PhaseInfo CurrentPhaseInfo { get; protected set; }

        private void Awake()
        {
            // 要求用データを生成
            for (int i = 0; i < StackMax; i++)
            {
                phaseInfoPool.Push(new PhaseInfo());
            }
        }

        /// <summary>
        /// フェーズの切り替えと更新処理の呼び出し
        /// </summary>
        protected virtual void Update()
        {
            // 切り替え処理
            UpdateChangeRequest();

            // 更新処理
            if ((CurrentPhaseInfo != null) && (!CurrentPhaseInfo.phase.IsTerminated))
            {
                CurrentPhaseInfo.phase.Update();
            }
        }

        /// <summary>
        /// 物理更新処理
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if ((CurrentPhaseInfo != null) && (!CurrentPhaseInfo.phase.IsTerminated))
            {
                CurrentPhaseInfo.phase.FixedUpdate();
            }
        }

        /// <summary>
        /// フェーズの切り替え要求の確認と処理
        /// </summary>
        void UpdateChangeRequest()
        {
            // リクエストがないか、処理中以外の時は次のフェーズへは遷移しない
            if (requestQueue.Count == 0) return;

            switch(CurrentState)
            {
                // 未処理時
                case State.Standby:
                    ChangeAction();
                    break;

                // 処理中の時
                case State.Running:
                    // 現在の状態が切り替え不可なら待機
                    if (!CurrentPhaseInfo.phase.CanChange)
                    {
                        break;
                    }

                    // 終了呼び出し
                    CurrentPhaseInfo.toNextAction?.Invoke();
                    if (CurrentPhaseInfo.phase.IsTerminated)
                    {
                        ChangeAction();
                    }
                    else
                    {
                        CurrentState = State.WaitTerminate;
                    }
                    break;

                // 終了待機
                case State.WaitTerminate:
                    if (CurrentPhaseInfo.phase.IsTerminated)
                    {
                        ChangeAction();
                    }
                    break;

                // ルートまで戻す
                case State.PopAll:
                    if (CurrentPhaseInfo.phase.IsTerminated)
                    {
                        PopAll();
                    }
                    break;
            }
        }

        /// <summary>
        /// 登録されている切り替え処理を呼び出します。
        /// </summary>
        void ChangeAction()
        {
            var info = requestQueue.Dequeue();
            info.changeAction(info);
        }

        /// <summary>
        /// 切り替え可能かを確認して、可能なら指定の処理を登録します。
        /// </summary>
        /// <param name="action">切り替え処理</param>
        /// <param name="toNext">次の処理に切り替える処理</param>
        /// <param name="ph">切り替え対象のフェーズのインスタンス</param>
        /// <param name="reserve">切り替え中の時に失敗させずに切り替え予約したい時はtrue</param>
        /// <returns>切り替え要求が通ったらtrue</returns>
        bool Request(UnityAction<PhaseInfo> action, UnityAction toNext, IPhase ph, bool reserve)
        {
            if (!CanRequest(reserve))
            {
                // リクエスト不可
                return false;
            }

            // リクエスト可能
            EnqueueRequest(action, toNext, ph);
            return true;
        }

        /// <summary>
        /// フェーズの切り替えを要求します。
        /// </summary>
        /// <param name="reserve">切り替え中の時に予約するならtrue。失敗させるなら省略</param>
        /// <returns>要求が成功したらtrue。reserveが省略されていて切り替え中なら失敗でfalse</returns>
        public bool ChangeRequest(IPhase ph, bool reserve = false)
        {
            if (CurrentPhaseInfo == null)
            {
                return Request(Change, null, ph, reserve);
            }
            return Request(Change, CurrentPhaseInfo.phase.Terminate, ph, reserve);
        }

        /// <summary>
        /// フェーズをプッシュして切り替えを要求します。
        /// </summary>
        /// <param name="reserve">切り替え中の時に予約するならtrue。失敗させるなら省略</param>
        /// <returns>要求が成功したらtrue。reserveが省略されていて切り替え中なら失敗でfalse</returns>
        public bool PushRequest(IPhase ph, bool reserve = false)
        {
            if (CurrentPhaseInfo == null)
            {
                return Request(Push, null, ph, reserve);
            }
            return Request(Push, CurrentPhaseInfo.phase.Pause, ph, reserve);
        }

        /// <summary>
        /// 一つ前のフェーズに戻します。
        /// </summary>
        /// <param name="reserve">切り替え中の時に予約するならtrue。失敗させるなら省略</param>
        /// <returns>要求が成功したらtrue。reserveが省略されていて切り替え中なら失敗でfalse</returns>
        public bool PopRequest(bool reserve = false)
        {
            // 実行フェーズのスタックが1つ以下の時はPopできないので成功させて終わり
            if (phaseStack.Count <= 1)
            {
                return true;
            }

            return Request(Pop, CurrentPhaseInfo.phase.Terminate, null, reserve);
        }

        /// <summary>
        /// ルートのフェーズまで戻します。
        /// </summary>
        /// <param name="reserve">切り替え中の時に予約するならtrue。失敗させるなら省略</param>
        /// <returns>要求が成功したらtrue。reserveが省略されていて切り替え中なら失敗でfalse</returns>
        public bool PopAllRequest(bool reserve = false)
        {
            // 実行フェーズのスタックが1つ以下の時はすでに完了しているので成功させて終わり
            if (phaseStack.Count <= 1)
            {
                return true;
            }

            return Request(PopAll, CurrentPhaseInfo.phase.Terminate, null, reserve);
        }

        /// <summary>
        /// 切り替えリクエストを登録します。
        /// </summary>
        /// <param name="act">切り替え処理</param>
        /// <param name="toNext">次へ切り替える時の処理</param>
        /// <param name="ph">切り替えフェーズのインスタンス</param>
        void EnqueueRequest(UnityAction<PhaseInfo> act, UnityAction toNext, IPhase ph)
        {
            var req = phaseInfoPool.Pop();
            req.Set(Change, toNext, ph);
            requestQueue.Enqueue(req);
        }

        /// <summary>
        /// 要求を実行できるか確認します。
        /// </summary>
        /// <param name="reserve">切り替え不可の時に次の処理として予約する場合、true</param>
        /// <returns>true=切り替え可能</returns>
        bool CanRequest(bool reserve)
        {
            // 要求がオーバーしていたら無条件で切り替え不可
            if (phaseInfoPool.Count == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning("フェーズ切り替えの要求数が上限を越えました。切り替えをキャンセルします。");
#endif
                return false;
            }

            // reserveがtrueなら無条件で要求通し
            if (reserve) return true;

            // requestQueueが0、かつ、
            // (現在のフェーズが未設定か、現在のフェーズが切り替え可能な時、true
            return (requestQueue.Count == 0)
                && (    (phaseStack.Count == 0)
                    ||  phaseStack.Peek().phase.CanChange);
        }

        /// <summary>
        /// 現在のフェーズを指定のフェーズへ切り替えます。
        /// </summary>
        /// <param name="phaseInfo">切り替え情報のインスタンス</param>
        void Change(PhaseInfo phaseInfo)
        {
            CurrentState = State.Running;

            // 現在のフェーズがあるならプールに戻す
            if (CurrentPhaseInfo != null)
            {
                var stack = phaseStack.Pop();
                phaseInfoPool.Push(stack);
            }

            Push(phaseInfo);
        }

        /// <summary>
        /// プールからインスタンスを取り出して、スタックにフェーズを積んで初期化を実行します。
        /// </summary>
        /// <param name="phaseInfo">切り替えたいフェーズ</param>
        void Push(PhaseInfo phaseInfo)
        {
            CurrentState = State.Running;
            CurrentPhaseInfo = phaseInfo;
            phaseStack.Push(phaseInfo);
            phaseInfo.phase.Init();
        }

        /// <summary>
        /// 現在のフェーズをプールに戻して、一つ前のフェーズに戻します。
        /// </summary>
        /// <param name="phaseInfo">null</param>
        void Pop(PhaseInfo phaseInfo)
        {
            CurrentState = State.Running;
            if (phaseInfo != null)
            {
                phaseInfoPool.Push(phaseInfo);
            }

            // スタックが1つ以下の時は戻せないので何もしない
            if (phaseStack.Count <= 1) return;

            // スタックが1の時はより大きい時、
            phaseInfoPool.Push(CurrentPhaseInfo);
            CurrentPhaseInfo = phaseStack.Pop();
            CurrentPhaseInfo.phase.Resume();
        }

        /// <summary>
        /// ルートまで戻す処理を開始します。
        /// </summary>
        /// <param name="phaseInfo">フェーズ情報</param>
        void PopAll(PhaseInfo phaseInfo = null)
        {
            if (phaseInfo != null)
            {
                phaseInfoPool.Push(phaseInfo);
            }

            // phaseStackが1つ以下の時はこのメソッドは呼ばれないはずだが念のため
            if (phaseStack.Count <= 1)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"phaseStackが{phaseStack.Count}の状態でPopAll()が呼ばれました。");
#endif
                CurrentState = (CurrentPhaseInfo != null) ? State.Running : State.Standby;
                return;
            }

            // 状態を1つ戻す
            phaseInfoPool.Push(CurrentPhaseInfo);
            CurrentPhaseInfo = phaseStack.Pop();

            // 戻す処理が完了したので復帰
            if (phaseStack.Count <= 1)
            {
                CurrentState = State.Running;
                CurrentPhaseInfo.phase.Resume();
            }
            else
            {
                // 戻し処理継続
                CurrentState = State.PopAll;
                CurrentPhaseInfo.phase.Terminate();
            }
        }
    }
}