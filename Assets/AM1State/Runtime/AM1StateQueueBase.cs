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
        public AM1StateQueueBase(int priority) { 
            Priority = priority;
        }

        /// <summary>
        /// 優先順位
        /// </summary>
        public int Priority { get; protected set; }

        public virtual bool CanChangeToOtherState => true;

        public StateChangeAction ChangeAction { get; set; }

        public bool IsRunning { get; protected set; }

        public virtual void FixedUpdate() { }

        public virtual void Init() { IsRunning = true; }

        public virtual void LateUpdate() { }

        public virtual void Pause() { IsRunning = false; }

        public virtual void Resume() { IsRunning = true; }

        public virtual void Terminate() { IsRunning = false; }

        public virtual void Update() { }
    }
}