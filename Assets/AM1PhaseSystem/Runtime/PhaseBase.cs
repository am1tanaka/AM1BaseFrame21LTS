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

        public virtual void FixedUpdate() { }

        public virtual void Init() { }

        public virtual void Teminate() { }

        public virtual void Update() { }
    }
}