using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.BaseFrame
{
    public interface ISceneBehaviour
    {
        /// <summary>
        /// 操作開始フラグ
        /// </summary>
        bool IsStarted { get; }
    }
}