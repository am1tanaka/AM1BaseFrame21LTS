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
        /// ただし、引数にtrueを指定すると現在の遷移の実行後に遷移を発生させるキューに要求を積む。この場合は戻り値は常にtrue。<br></br>
        /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
        /// </summary>
        /// <param name="canQueue">状態遷移中か予約済みの時にキャンセルせずに続けて遷移させる場合はtrueを指定。デフォルトはfalseで要求をキャンセル</param>
        /// <returns>成功したらtrue / 失敗=false</returns>
        bool Request(bool canQueue = false);

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