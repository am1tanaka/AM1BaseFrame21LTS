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
        /// <summary>
        /// スタックの初期上限
        /// </summary>
        static int StackMax => 8;

        /// <summary>
        /// フェーズスタック
        /// </summary>
        public readonly Stack<PhaseChangeRequest> phaseStack = new Stack<PhaseChangeRequest>(StackMax);

        /// <summary>
        /// フェーズの要求
        /// </summary>
        public readonly Queue<PhaseChangeRequest> requestQueue = new Queue<PhaseChangeRequest>(StackMax);

        /// <summary>
        /// 要求インスタンス
        /// </summary>
        public readonly Stack<PhaseChangeRequest> requestPool = new Stack<PhaseChangeRequest>(StackMax);

        private void Awake()
        {
            // 要求用データを生成
            for (int i = 0; i < StackMax; i++)
            {
                requestPool.Push(new PhaseChangeRequest());
            }
        }

        /// <summary>
        /// フェーズの切り替えと更新処理の呼び出し
        /// </summary>
        protected virtual void Update()
        {
        }

        /// <summary>
        /// 物理更新処理
        /// </summary>
        protected virtual void FixedUpdate()
        {
            
        }

        /// <summary>
        /// 切り替え可能かを確認して、可能なら指定の処理を登録します。
        /// </summary>
        /// <param name="action">切り替え処理</param>
        /// <param name="ph">切り替え対象のフェーズのインスタンス</param>
        /// <param name="reserve">切り替え中の時に失敗させずに切り替え予約したい時はtrue</param>
        /// <returns>切り替え要求が通ったらtrue</returns>
        bool Request(UnityAction<IPhase> action, IPhase ph, bool reserve)
        {
            if (!CanRequest(reserve))
            {
                // リクエスト不可
                return false;
            }

            // リクエスト可能
            EnqueueRequest(action, ph);
            return true;
        }

        /// <summary>
        /// フェーズの切り替えを要求します。
        /// </summary>
        /// <param name="reserve">切り替え中の時に予約するならtrue。失敗させるなら省略</param>
        /// <returns>要求が成功したらtrue。reserveが省略されていて切り替え中なら失敗でfalse</returns>
        public bool ChangeRequest(IPhase ph, bool reserve = false)
        {
            return Request(Change, ph, reserve);
        }

        /// <summary>
        /// フェーズをプッシュして切り替えを要求します。
        /// </summary>
        /// <param name="reserve">切り替え中の時に予約するならtrue。失敗させるなら省略</param>
        /// <returns>要求が成功したらtrue。reserveが省略されていて切り替え中なら失敗でfalse</returns>
        public bool ChangePush(IPhase ph, bool reserve = false)
        {
            return Request(Push, ph, reserve);
        }

        /// <summary>
        /// 一つ前のフェーズに戻します。
        /// </summary>
        /// <param name="reserve">切り替え中の時に予約するならtrue。失敗させるなら省略</param>
        /// <returns>要求が成功したらtrue。reserveが省略されていて切り替え中なら失敗でfalse</returns>
        public bool ChangePop(bool reserve = false)
        {
            return Request(Pop, null, reserve);
        }

        /// <summary>
        /// ルートのフェーズまで戻します。
        /// </summary>
        /// <param name="reserve">切り替え中の時に予約するならtrue。失敗させるなら省略</param>
        /// <returns>要求が成功したらtrue。reserveが省略されていて切り替え中なら失敗でfalse</returns>
        public bool ChangePopAll(bool reserve = false)
        {
            return Request(PopAll, null, reserve);
        }

        /// <summary>
        /// 切り替えリクエストを登録します。
        /// </summary>
        /// <param name="act">切り替え処理</param>
        /// <param name="ph">切り替えフェーズのインスタンス</param>
        void EnqueueRequest(UnityAction<IPhase> act, IPhase ph)
        {
            var req = requestPool.Pop();
            req.Set(Change, ph);
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
            if (requestPool.Count == 0)
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
        /// 指定のフェーズへ切り替えます。
        /// </summary>
        /// <param name="phase">切り替え先のフェーズ</param>
        void Change(IPhase phase)
        {

        }

        void Push(IPhase phase)
        {

        }

        void Pop(IPhase phase)
        {

        }

        void PopAll(IPhase phase)
        {

        }

    }
}