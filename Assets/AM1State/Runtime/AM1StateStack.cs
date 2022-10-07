//#define DEBUG_LOG

using Codice.Client.BaseCommands.CheckIn.CodeReview;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.Build.Content;
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
        /// 現在の状態のインスタンス
        /// </summary>
        public IAM1State CurrentState { get; protected set; }

        /// <summary>
        /// 要求を受け取れないかを確認
        /// </summary>
        /// <returns>要求がすでにある、あるいは、現在の状態が切り替え不可のとき、true</returns>
        public bool IsBusy => (requestQueue.Count > 0) || ((CurrentState != null) && !CurrentState.CanChangeToOtherState);

        /// <summary>
        /// 一手戻すキュー用データ
        /// </summary>
        protected readonly AM1StateBase prevCommandState = new();

        /// <summary>
        /// 全て戻すキュー用データ
        /// </summary>
        protected readonly AM1StateBase popAllCommandState = new();

        /// <summary>
        /// 最上位の状態まで戻すためのキュー用データ
        /// </summary>
        protected readonly AM1StateBase popToRootCommandState = new();

        /// <summary>
        /// 状態切り替え中フラグ。切り替えコルーチンの待機時に他の処理が動かないように
        /// このフラグで切り替え処理が完了するまでは他の切り替えが進まないようにブロック
        /// </summary>
        public bool IsChanging { get; private set; }

        public AM1StateStack()
        {
            prevCommandState.ChangeAction = PopPrevState;
            popAllCommandState.ChangeAction = PopAllState;
            popToRootCommandState.ChangeAction = PopToRootState;
        }

        /// <summary>
        /// フェーズの切り替えと更新処理の呼び出し
        /// </summary>
        protected virtual void Update()
        {
            // 切り替え処理
            UpdateChangeRequest();

            // 更新処理
            CurrentState?.Update();
        }

        /// <summary>
        /// 物理更新処理
        /// </summary>
        protected virtual void FixedUpdate()
        {
            CurrentState?.FixedUpdate();
        }

        /// <summary>
        /// 状態の切り替え要求の確認と処理
        /// </summary>
        void UpdateChangeRequest()
        {
            // リクエストがないか、処理中以外の時は次のフェーズへは遷移しない
            if ((requestQueue.Count == 0)
                || ((CurrentState != null) && !CurrentState.CanChangeToOtherState)
                || IsChanging)
            {
                return;
            }

            // 要求を一つ取り出して実行
            IsChanging = true;
            var req = requestQueue.Dequeue();
            // それ以外は設定アクション呼び出し
            Log($"Start ChangeAction Coroutine {req} change={req.ChangeAction}");
            StartCoroutine(req.ChangeAction(req));
        }

        /// <summary>
        /// 現在の状態を一時停止して、指定の状態をスタックに積んで初期化
        /// </summary>
        /// <param name="nextState">切り替え先の状態</param>
        IEnumerator PushState(IAM1State nextState)
        {
            Log($"PushState()");

            // スタックに切り替えたい状態がすでにあったら処理なし
            if (stateStack.Contains(nextState))
            {
                Log($"  ContainsStack");
                IsChanging = false;
                yield break;
            }

            // 現在の状態があれば一時停止
            if (CurrentState != null)
            {
                CurrentState.Pause();

                // 切り替え可能になるまで待機
                while (!CurrentState.CanChangeToOtherState)
                {
                    yield return null;
                }
            }

            // 状態をプッシュ
            stateStack.Push(nextState);
            Log($"PushState after push {stateStack.Count}");

            // 現在のインスタンス
            CurrentState = nextState;

            // 初期化
            CurrentState.Init();
            IsChanging = false;
        }

        /// <summary>
        /// 一手戻す処理を実行するコルーチン。
        /// </summary>
        IEnumerator PopPrevStateCoroutine()
        {
            Log($"PopPrevStateCoroutine()");
            // 現在の状態があれば終了
            if (CurrentState != null)
            {
                CurrentState.Terminate();

                // 切り替え可能になるまで待機
                while (!CurrentState.CanChangeToOtherState)
                {
                    yield return null;
                }
            }

            // スタックから状態を取り出す
            Log($"  PopPrevStateCoroutine before pop {stateStack.Count}");
            if (stateStack.Count == 0)
            {
                // スタックが0なら現在状態をnullへ
                CurrentState = null;
            }
            else
            {
                stateStack.Pop();
                if (stateStack.Count == 0)
                {
                    // スタックが0なら現在状態をnullへ
                    CurrentState = null;
                }
                else
                {
                    CurrentState = stateStack.Peek();
                    // 再開
                    CurrentState.Resume();
                }
            }
        }

        /// <summary>
        /// 一手戻します。
        /// </summary>
        /// <param name="prevState">未使用</param>
        IEnumerator PopPrevState(IAM1State prevState)
        {
            Log($"PopPrevState()");
            yield return PopPrevStateCoroutine();
            IsChanging = false;
        }

        /// <summary>
        /// 現在の状態を終了して、目的の状態まで戻します。
        /// </summary>
        /// <param name="targetState">現在の状態</param>
        IEnumerator PopState(IAM1State targetState)
        {
            Log($"PopState start");

            // スタックに指定の状態がなければ即終了
            if (!stateStack.Contains(targetState))
            {
                Log($"PopState ContainsStack");
                IsChanging = false;
                yield break;
            }

            while (CurrentState != targetState)
            {
                Log($"PopState PopPrevState");
                yield return PopPrevStateCoroutine();
            }

            Log($"PopState Done");
            IsChanging = false;
        }

        /// <summary>
        /// ルートまで戻します。
        /// </summary>
        /// <param name="rootState">未使用</param>
        IEnumerator PopToRootState(IAM1State rootState)
        {
            Log($"PopToRootState stateStackCount={stateStack.Count}");
            while (stateStack.Count > 1)
            {
                Log($"  {stateStack.Count}");
                yield return PopPrevStateCoroutine();
            }
            Log($"PopToRootState Done");
            IsChanging = false;
        }

        /// <summary>
        /// 全ての状態を戻します。
        /// </summary>
        /// <param name="allState">未使用</param>
        IEnumerator PopAllState(IAM1State allState)
        {
            Log($"PopAllState stateStackCount={stateStack.Count}");

            while (CurrentState != null)
            {
                Log($"  {stateStack.Count}");
                yield return PopPrevStateCoroutine();
            }
            Log($"PopAllState Done");
            IsChanging = false;
        }

        /// <summary>
        /// 現在の状態を閉じて、指定の状態へ切り替える要求をします。
        /// すでに他の切り替えが要求済みだったり、過去に同じ状態があれば失敗します。
        /// </summary>
        /// <param name="nextState">切り替えたい状態</param>
        /// <returns>要求が成功するか、すでに要求の状態ならtrue</returns>
        public bool PopAndPushRequest(IAM1State nextState)
        {
            if (IsBusy)
            {
                return false;
            }
            if (CurrentState == nextState)
            {
                return true;
            }
            if (stateStack.Contains(nextState))
            {
                return false;
            }

            // 一手戻しとプッシュを予約
            if (CurrentState != null)
            {
                requestQueue.Enqueue(prevCommandState);
            }

            // 要求状態のPush
            nextState.ChangeAction = PushState;
            requestQueue.Enqueue(nextState);
            return true;
        }

        /// <summary>
        /// 現在の状態を閉じて、指定の状態へ切り替える要求をします。
        /// すでに要求がある場合はキューに登録します。切り替えが必要かの判定は実行時に実施します。
        /// </summary>
        /// <param name="st">切り替えたい状態</param>
        public bool PopAndPushQueueRequest(IAM1State nextState)
        {
            if (requestQueue.Contains(nextState))
            {
                return false;
            }

            // キューに積む場合は常に一手戻しは予約
            requestQueue.Enqueue(prevCommandState);

            // 要求状態のPush
            nextState.ChangeAction = PushState;
            requestQueue.Enqueue(nextState);
            return true;
        }

        /// <summary>
        /// 状態をプッシュして切り替えを要求します。
        /// </summary>
        /// <param name="nextState">切り替えたい状態のインスタンス</param>
        /// <returns>要求が成功するか、すでに要求状態の時true</returns>
        public bool PushRequest(IAM1State nextState)
        {
            if (IsBusy)
            {
                return false;
            }
            if (CurrentState == nextState)
            {
                return true;
            }
            if (stateStack.Contains(nextState))
            {
                return false;
            }

            nextState.ChangeAction = PushState;
            requestQueue.Enqueue(nextState);
            return true;
        }

        /// <summary>
        /// 状態をプッシュして切り替えを要求します。
        /// 要求があったり、切り替え中のときは、登録キューに積みます。
        /// </summary>
        /// <param name="nextState">切り替えたい状態のインスタンス</param>
        /// <returns>要求が成功したらtrue</returns>
        public bool PushQueueRequest(IAM1State nextState)
        {
            // 同じ状態が要求済みなら失敗
            if (requestQueue.Contains(nextState))
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
        /// <param name="backState">戻したい状態があれば指定。nullで一手戻し</param>
        /// <returns>要求が成功すればtrue</returns>
        public bool PopRequest(IAM1State backState = null)
        {
            if (IsBusy)
            {
                return false;
            }
            if (CurrentState != null)
            {
                Log($"PopRequest set back state.");
                PopQueueRequest(backState);
            }

            return true;
        }

        /// <summary>
        /// 一つ前のフェーズに戻します。
        /// 予約があったり、切り替え中のときは、キューに処理を積みます。
        /// </summary>
        public bool PopQueueRequest(IAM1State backState = null)
        {
            // 引数なしなら一手戻す
            if (backState == null)
            {
                Log($"PopQueueRequest-prev");
                requestQueue.Enqueue(prevCommandState);
            }
            else
            {
                if (requestQueue.Contains(backState))
                {
                    Log($"PopQueueRequest 登録があるので失敗");
                    return false;
                }

                // 引数があるなら引数のところまで戻す
                Log($"PopQueueRequest-target");
                backState.ChangeAction = PopState;
                requestQueue.Enqueue(backState);
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
            if (stateStack.Count > 1)
            {
                PopToRootQueueRequest();
            }

            return true;
        }

        /// <summary>
        /// ルートの状態まで戻します。すでにルートになっていたり、状態がなければ何もせず成功で終了します。
        /// </summary>
        public void PopToRootQueueRequest()
        {
            requestQueue.Enqueue(popToRootCommandState);
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
            if (CurrentState != null)
            {
                PopAllQueueRequest();
            }
            return true;
        }

        /// <summary>
        /// 全ての状態を戻して状態なしにします。
        /// </summary>
        public void PopAllQueueRequest()
        {
            requestQueue.Enqueue(popAllCommandState);
        }

        /// <summary>
        /// リクエストキューを削除します。
        /// </summary>
        public void ClearRequestQueue()
        {
            requestQueue.Clear();
            IsChanging = false;
        }

        [System.Diagnostics.Conditional("DEBUG_LOG")]
        public static void Log(object mes)
        {
            Debug.Log(mes);
        }
    }
}