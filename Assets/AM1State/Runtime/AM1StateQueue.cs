using System.Collections.Generic;
using UnityEngine;

namespace AM1.State
{
    /// <summary>
    /// キューの状態
    /// </summary>
    public class AM1StateQueue : MonoBehaviour
    {
        /// <summary>
        /// 状態キュー
        /// </summary>
        public readonly LinkedList<AM1StateQueueBase> stateQueue = new LinkedList<AM1StateQueueBase>();

        /// <summary>
        /// 現在の状態
        /// </summary>
        public AM1StateQueueBase CurrentState { get; protected set; }

        /// <summary>
        /// キューの状態に関わらず、現在の状態を終了したい時
        /// </summary>
        bool isTerminateCurrentState;

        /// <summary>
        /// キューの更新処理
        /// </summary>
        private void Update()
        {
            // 終了要求
            if (isTerminateCurrentState)
            {
                isTerminateCurrentState = false;
                if (CurrentState != null)
                {
                    // 現在の処理を終了
                    CurrentState.Terminate();
                    CurrentState = null;
                }
            }

            // キューに登録がある時
            if (stateQueue.Count > 0)
            {
                // 現在の状態が切り替え可能なら解除
                if ((CurrentState != null) && CurrentState.CanChangeToOtherState)
                {
                    // 現在の処理を終了
                    CurrentState.Terminate();
                    CurrentState = null;
                }

                // 現在の処理がなければ切り替え
                if (CurrentState == null)
                {
                    var cur = stateQueue.First;
                    while (cur != null)
                    {
                        // 最後の1つか、切り替え不許可の時、現在の状態に設定
                        if ((stateQueue.Count == 1) || !cur.Value.CanChangeToOtherState)
                        {
                            CurrentState = cur.Value;
                            stateQueue.RemoveFirst();
                            CurrentState.Init();
                            break;
                        }
                        else
                        {
                            cur = cur.Next;
                            stateQueue.RemoveFirst();
                        }
                    }
                }
            }

            // 更新処理
            CurrentState?.Update();
        }

        private void OnDestroy()
        {
            CurrentState?.Terminate();
        }

        private void FixedUpdate()
        {
            CurrentState?.FixedUpdate();
        }

        private void LateUpdate()
        {
            CurrentState?.LateUpdate();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                CurrentState?.Pause();
            }
            else
            {
                CurrentState?.Resume();
            }
        }

        /// <summary>
        /// 指定の状態をプライオリティを指定して登録。プライオリティは大きいほど優先される。
        /// </summary>
        /// <param name="state">IAM1Stateのインスタンス</param>
        public void Enqueue(AM1StateQueueBase state)
        {
            for (var current = stateQueue.First; current != null; current = current.Next)
            {
                // 登録中のキューの優先度が指定の優先度を下回ったらその前に登録
                if (current.Value.Priority < state.Priority)
                {
                    stateQueue.AddBefore(current, state);
                    return;
                }
            }

            // 最後に追加
            stateQueue.AddLast(state);
        }

        /// <summary>
        /// 指定の優先度以下の状態を削除。現在の状態はそのまま。
        /// </summary>
        /// <param name="priority">この値以下の優先度の状態を削除</param>
        public void Cancel(int priority)
        {
            var current = stateQueue.Last;
            while (current != null && current.Value.Priority <= priority)
            {
                stateQueue.RemoveLast();
                current = stateQueue.Last;
            }
        }

        /// <summary>
        /// キューの予約を全て削除。
        /// </summary>
        public void CancelAll()
        {
            stateQueue.Clear();
        }

        /// <summary>
        /// 次のフレームで現在の状態の終了を要求する。
        /// </summary>
        public void RequestTerminateCurrentState()
        {
            isTerminateCurrentState = true;
        }
    }
}