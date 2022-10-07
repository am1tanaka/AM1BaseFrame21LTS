using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.State
{
    /// <summary>
    /// IAM1Phaseの実装親クラス
    /// </summary>
    public class AM1StateBase : IAM1State
    {
        public virtual bool CanChangeToOtherState => true;

        public StateChangeAction ChangeAction { get; set; }

        public bool IsRunning { get; protected set; }

        public virtual void FixedUpdate() { }

        public virtual void Init() { IsRunning = true; }

        public virtual void Pause() { IsRunning = false; }

        public virtual void Resume() { IsRunning = true; }

        public virtual void Terminate() { IsRunning = false; }

        public virtual void Update() { }
    }
}