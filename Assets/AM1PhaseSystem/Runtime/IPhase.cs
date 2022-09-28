using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.PhaseSystem
{
    public interface IPhase
    {
        /// <summary>
        /// 他のフェーズへ切り替え可能な時、true
        /// </summary>
        bool CanChange { get; }

        /// <summary>
        /// 処理が完了したらtrue。開始していなかったり、処理中の時はfalse
        /// </summary>
        bool IsTerminated { get; }

        /// <summary>
        /// フェーズ開始時の初期化処理
        /// </summary>
        void Init();

        /// <summary>
        /// 更新処理
        /// </summary>
        void Update();

        /// <summary>
        /// 物理更新処理
        /// </summary>
        void FixedUpdate();

        /// <summary>
        /// フェーズを終了する時に呼び出して終了処理を実行
        /// </summary>
        void Teminate();

    }
}