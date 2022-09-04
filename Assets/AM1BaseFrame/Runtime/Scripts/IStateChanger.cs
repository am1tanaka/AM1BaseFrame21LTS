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
        /// この状態に必要なSceneの非同期読み込みの開始など。<br></br>
        /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
        /// </summary>
        void Init();

        /// <summary>
        /// 画面が覆われて、不要なシーンの解放が完了した時に行いたい処理を実装します。シーンの読み直しなど。<br></br>
        /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
        /// </summary>
        void OnHideScreen();

        /// <summary>
        /// 画面が覆われていて、全てのシーンが読み込まれてAwakeが実行された後に呼ばれます。<br></br>
        /// フェードインなどの状態を始めるための処理を実装します。<br></br>
        /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
        /// </summary>
        IEnumerator OnAwakeDone();

        /// <summary>
        /// 次の状態への切り替えにおいて、画面が隠れた時に呼び出すシーンの終了処理。不要になったシーンの解放などを実装します。<br></br>
        /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
        /// </summary>
        void Terminate();
    }
}