using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AM1.PhaseSystem
{
    /// <summary>
    /// フェーズ切り替えやインスタンスを記録するクラス
    /// </summary>
    public class PhaseInfo
    {
        /// <summary>
        /// 切り替え処理
        /// </summary>
        public UnityAction<IPhase> changeAction;

        /// <summary>
        /// 切り替えフェーズ
        /// </summary>
        public IPhase phase;

        /// <summary>
        /// 設定
        /// </summary>
        /// <param name="act">処理メソッド</param>
        /// <param name="ph">フェーズインスタンス</param>
        public void Set(UnityAction<IPhase> act, IPhase ph)
        {
            changeAction = act;
            phase = ph;
        }
    }
}