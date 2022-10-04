using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.State
{
    public interface IAM1State
    {
        /// <summary>
        /// 他の状態へ切り替え可能な時、true
        /// </summary>
        bool CanChangeToOtherState { get; }

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
        void Terminate();

        /// <summary>
        /// 他のフェーズへのスタック切り替える前の処理
        /// </summary>
        void Pause();

        /// <summary>
        /// 他のフェーズから戻る時の処理
        /// </summary>
        void Resume();
    }
}