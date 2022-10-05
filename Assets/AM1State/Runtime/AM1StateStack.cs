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
        /// 要求を受け取れないかを確認
        /// </summary>
        /// <returns>要求がすでにある、あるいは、現在の状態が切り替え不可のとき、true</returns>
        bool IsBusy => (requestQueue.Count > 0) || ((CurrentStateInfo != null) && !CurrentStateInfo.CanChangeToOtherState);

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
                || ((CurrentStateInfo != null) && !CurrentStateInfo.CanChangeToOtherState))
            {
                return;
            }

            // 要求を一つ取り出して実行
            var req = requestQueue.Dequeue();
            if (req.ChangeAction == null)
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
        /// <returns>要求が成功するか、すでに要求の状態ならtrue</returns>
        public bool PopAndPushRequest(IAM1State nextState)
        {
            if (IsBusy) return false;
            return PopAndPushQueueRequest(nextState);
        }

        /// <summary>
        /// 現在の状態を閉じて、指定の状態へ切り替える要求をします。
        /// すでに要求がある場合はキューに登録します。
        /// </summary>
        /// <param name="st">切り替えたい状態</param>
        /// <returns>要求が成功するか、すでに要求の状態ならtrue</returns>
        public bool PopAndPushQueueRequest(IAM1State nextState)
        {
            // 要求状態なら何もせずに成功
            if (CurrentStateInfo == nextState)
            {
                return true;
            }

            if (ContainsStackOrRequestQueue(nextState))
            {
                return false;
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
        /// 指定のインスタンスがスタックか要求にある時、true
        /// </summary>
        /// <param name="st">確認する状態のインスタンス</param>
        /// <returns>スタックか要求に含まれているとき、true</returns>
        bool ContainsStackOrRequestQueue(IAM1State st)
        {
            return ContainsStack(st) || ContainsRequestQueue(st);
        }

        /// <summary>
        /// 指定の状態がスタックにあることを確認します。
        /// </summary>
        /// <param name="st">確認する状態のインスタンス</param>
        /// <returns>指定の状態が見つかればtrue</returns>
        bool ContainsStack(IAM1State st)
        {
            foreach (var state in stateStack)
            {
                if (state == st)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 指定の状態が要求内にあったらtrue
        /// </summary>
        /// <param name="st">確認する状態のインスタンス</param>
        /// <returns>指定の状態がすでに要求にあったらtrue</returns>
        bool ContainsRequestQueue(IAM1State st)
        {
            foreach (var req in requestQueue)
            {
                if (req == st)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 状態をプッシュして切り替えを要求します。
        /// </summary>
        /// <param name="nextState">切り替えたい状態のインスタンス</param>
        /// <returns>要求が成功したらtrue。reserveが省略されていて切り替え中なら失敗でfalse</returns>
        public bool PushRequest(IAM1State nextState)
        {
            if (IsBusy) return false;
            return PushQueueRequest(nextState);
        }

        /// <summary>
        /// 状態をプッシュして切り替えを要求します。
        /// 要求があったり、切り替え中のときは、登録キューに積みます。
        /// </summary>
        /// <param name="nextState">切り替えたい状態のインスタンス</param>
        /// <returns>要求が成功したらtrue。reserveが省略されていて切り替え中なら失敗でfalse</returns>
        public bool PushQueueRequest(IAM1State nextState)
        {
            if (CurrentStateInfo == nextState)
            {
                return true;
            }
            if (ContainsStackOrRequestQueue(nextState))
            {
                return false;
            }

            // 要求状態のPush
            nextState.ChangeAction = PushState;
            requestQueue.Enqueue(nextState);
            return true;
        }

        /// <summary>
        /// 指定のフェーズに戻します。
        /// 予約があったり、切り替え中のときは登録失敗します。
        /// 指定を省略すると一つ前に戻します。
        /// 指定のものがない場合は失敗します。
        /// </summary>
        /// <returns>要求が成功するか、現在の状態がなければtrue</returns>
        /// <param name="backState"></param>
        /// <returns></returns>
        public bool PopRequest(IAM1State backState = null)
        {
            if (IsBusy) return false;
            return PopQueueRequest(backState);
        }

        /// <summary>
        /// 一つ前のフェーズに戻します。
        /// 予約があったり、切り替え中のときは、キューに処理を積みます。
        /// </summary>
        /// <returns>要求が成功するか、現在の状態がなければtrue</returns>
        public bool PopQueueRequest(IAM1State backState = null)
        {
            if (CurrentStateInfo == null)
            {
                return true;
            }

            // すでにキューに積まれているか、指定した状態がスタックにないなら失敗
            if ((backState != null) && (ContainsRequestQueue(backState) || !ContainsStack(backState)))
            {
                return false;
            }

            var targetState = (backState == null) ? CurrentStateInfo : backState;
            foreach (var stack in stateStack)
            {
                stack.ChangeAction = PopState;
                requestQueue.Enqueue(stack);
                if (stack == backState)
                {
                    break;
                }
            }

            return true;
        }

        /// <summary>
        /// ルートの状態まで戻します。すでにルートになっていたり、状態がなければ何もせず成功で終了します。
        /// 要求があったり、切り替え中のときは失敗します。
        /// </summary>
        /// <returns>要求が成功するか、すでにルートになっていたらtrue。</returns>
        public bool PopToRootRequest()
        {
            if (IsBusy)
            {
                return false;
            }
            return PopToRootQueueRequest();
        }

        /// <summary>
        /// ルートの状態まで戻します。すでにルートになっていたり、状態がなければ何もせず成功で終了します。
        /// </summary>
        /// <returns>原則、trueのみ</returns>
        public bool PopToRootQueueRequest()
        {
            int i = stateStack.Count;
            foreach (var state in stateStack)
            {
                // ルートか状態がないので成功して終わり
                i--;
                if (i <= 0)
                {
                    break;
                }

                state.ChangeAction = PopState;
                requestQueue.Enqueue(state);
            }

            return true;
        }

        /// <summary>
        /// 状態を全て戻します。最初から状態がなければ何もせず成功で終了します。
        /// 要求があったり、切り替え中のときは失敗します。
        /// </summary>
        /// <returns>要求が成功するか、すでに状態がなければtrue。</returns>
        public bool PopAllRequest()
        {
            if (IsBusy)
            {
                return false;
            }
            return PopAllQueueRequest();
        }

        /// <summary>
        /// 全ての状態を戻して状態なしにします。
        /// </summary>
        /// <returns>true</returns>
        public bool PopAllQueueRequest()
        {
            if (stateStack.Count == 0)
            {
                return true;
            }

            foreach (var state in stateStack)
            {
                state.ChangeAction = PopState;
                requestQueue.Enqueue(state);
            }

            return true;
        }

        /// <summary>
        /// リクエストキューを削除します。
        /// </summary>
        public void ClearRequestQueue()
        {
            requestQueue.Clear();
        }
    }
}