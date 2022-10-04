using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AM1.State
{
    /// <summary>
    /// 状態切り替えやインスタンスを記録するクラス
    /// </summary>
    public class AM1StateInfo
    {
        /// <summary>
        /// フェーズを切り替える処理
        /// </summary>
        public UnityAction<AM1StateInfo> changeAction;

        /// <summary>
        /// 他のフェーズへ切り替える前に呼び出す終了か中断の呼び出し
        /// </summary>
        public UnityAction toNextAction;

        /// <summary>
        /// 切り替えフェーズ
        /// </summary>
        public IAM1State phase;

        /// <summary>
        /// 設定
        /// </summary>
        /// <param name="act">フェーズ切り替え処理</param>
        /// <param name="chg">次へ切り替える時に呼び出す処理</param>
        /// <param name="ph">フェーズインスタンス</param>
        public void Set(UnityAction<AM1StateInfo> act, UnityAction chg, IAM1State ph)
        {
            changeAction = act;
            toNextAction = chg;
            phase = ph;
        }
    }
}