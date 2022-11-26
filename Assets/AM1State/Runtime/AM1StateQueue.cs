using System.Collections;
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
        /// 指定の状態をプライオリティを指定して登録。プライオリティは大きいほど優先される。
        /// </summary>
        /// <param name="state">IAM1Stateのインスタンス</param>
        public void Enqueue(AM1StateQueueBase state)
        {
            var current = stateQueue.First;
            while (current != null && current.Value.Priority >= state.Priority)
            {
                current = current.Next;
            }
            if (current == null)
            {
                stateQueue.AddFirst(state);
            }
            else
            {
                stateQueue.AddAfter(current, state);
            }
        }

        /// <summary>
        /// 指定の優先度以下の状態を削除
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
    }
}