using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.State
{
    /// <summary>
    /// キュー用の状態のベースクラス
    /// </summary>
    public class AM1StateQueueBase : IAM1State
    {
        /// <summary>
        /// コンストラクタ。優先順位を設定する場合は引数に値を設定。
        /// </summary>
        /// <param name="priority">設定したい優先順位。大きいほど優先される。省略すると0</param>
        public AM1StateQueueBase(int priority = 0) { 
            Priority = priority;
        }

        /// <summary>
        /// 優先順位
        /// </summary>
        public int Priority { get; protected set; }

        public virtual bool CanChangeToOtherState => true;

        public StateChangeAction ChangeAction { get; set; }

        public bool IsRunning { get; protected set; }

        /// <summary>
        /// Updateが実行された時にdeltaTimeを加算する経過秒数。
        /// 時間経過に応じた処理を実装したい時などに利用する。
        /// 任意のタイミングで0を代入して計測し直せる。
        /// </summary>
        public float updateTime;

        public virtual void FixedUpdate() { }

        public virtual void Init() { IsRunning = true; }

        public virtual void LateUpdate() { }

        public virtual void Pause() { IsRunning = false; }

        public virtual void Resume() { IsRunning = true; }

        public virtual void Terminate() { IsRunning = false; }

        public virtual void Update() {
            updateTime += Time.deltaTime;
        }
    }
}