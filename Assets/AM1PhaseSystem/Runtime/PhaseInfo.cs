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
        /// フェーズを切り替える処理
        /// </summary>
        public UnityAction<PhaseInfo> changeAction;

        /// <summary>
        /// 他のフェーズへ切り替える前に呼び出す終了か中断の呼び出し
        /// </summary>
        public UnityAction toNextAction;

        /// <summary>
        /// 切り替えフェーズ
        /// </summary>
        public IPhase phase;

        /// <summary>
        /// 設定
        /// </summary>
        /// <param name="act">フェーズ切り替え処理</param>
        /// <param name="chg">次へ切り替える時に呼び出す処理</param>
        /// <param name="ph">フェーズインスタンス</param>
        public void Set(UnityAction<PhaseInfo> act, UnityAction chg, IPhase ph)
        {
            changeAction = act;
            toNextAction = chg;
            phase = ph;
        }
    }
}