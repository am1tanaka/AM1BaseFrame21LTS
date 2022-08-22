using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.BaseFrame
{
    /// <summary>
    /// フェードやアイキャッチ表示などの画面を隠すクラスに実装させる
    /// インターフェース
    /// </summary>
    public interface IScreenTransition
    {
        /// <summary>
        /// 画面を覆うか、表示する演出を開始。
        /// </summary>
        /// <param name="cover">覆う時true、表示する時false</param>
        /// <param name="sec">表示までの秒数。省略か0で即時</param>
        /// <returns>開始したらtrue。すでに切り替え中の時はfalseを返して処理をキャンセル</returns>
        bool StartCover(bool cover, float sec = 0);

        /// <summary>
        /// 切り替え待ち。0秒の時は即時終了
        /// </summary>
        IEnumerator Wait();

        /// <summary>
        /// 切り替え中の時true
        /// </summary>
        bool IsTransitioning { get; }

        /// <summary>
        /// アニメの画面が覆われているパラメーターの状態を返す
        /// </summary>
        bool IsCover { get; }
    }
}