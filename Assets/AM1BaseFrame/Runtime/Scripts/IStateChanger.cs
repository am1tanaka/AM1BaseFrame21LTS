using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.BaseFrame
{
    /// <summary>
    /// 状態切り替え処理のインターフェース<br></br>
    /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
    /// </summary>
    public interface IStateChanger
    {
        /// <summary>
        /// このシーンに切り替えることを要求。<br></br>
        /// すでに他のシーンへの要求が出ていたらfalseを返してこのシーンへの移行はキャンセル。<br></br>
        /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
        /// </summary>
        /// <returns>成功したらtrue / 失敗=false</returns>
        bool Request();

        /// <summary>
        /// このシーンに必要なSceneの非同期読み込みの開始など。<br></br>
        /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
        /// </summary>
        void Init();

        /// <summary>
        /// フェードアウトなどで画面が隠れた時に行いたい処理。シーンの読み直しなど。<br></br>
        /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
        /// </summary>
        void OnHideScreen();

        /// <summary>
        /// 全てのシーンのAwake完了後のフェードインなどの処理。<br></br>
        /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
        /// </summary>
        IEnumerator OnAwakeDone();

        /// <summary>
        /// 次のシーンへの切り替えで画面が隠れた時に呼び出すシーンの終了処理。シーンの解放などを実装。<br></br>
        /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
        /// </summary>
        void Terminate();
    }
}