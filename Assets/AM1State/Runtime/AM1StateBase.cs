using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.State
{
    /// <summary>
    /// IAM1Phaseの実装親クラス
    /// </summary>
    public abstract class AM1StateBase : IAM1State
    {
        public virtual bool CanChangeToOtherState => true;

        public virtual bool IsTerminated => true;

        public virtual bool IsPaused => true;

        public virtual void FixedUpdate() { }

        public virtual void Init() { }

        public virtual void Pause() { }

        public virtual void Resume() { }

        public virtual void Terminate() { }

        public virtual void Update() { }
    }
}