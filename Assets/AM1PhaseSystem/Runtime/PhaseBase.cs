using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.PhaseSystem
{
    /// <summary>
    /// IPhaseを継承した親クラス
    /// </summary>
    public class PhaseBase : IPhase
    {
        public virtual bool CanChange => true;

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