using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.BaseFrame
{
    /// <summary>
    /// 状態切り替え処理の親クラス
    /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
    /// </summary>
    public abstract class StateChangerBase<T> : SimpleSingleton<T> where T : IStateChanger, new()
    {
        /// <summary>
        /// このシーンに切り替えることを要求。<br></br>
        /// すでに他のシーンへの要求が出ていたらfalseを返してこのシーンへの移行はキャンセル。<br></br>
        /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
        /// </summary>
        /// <returns>成功したらtrue / 失敗=false</returns>
        public virtual bool Request(bool canQueue=false)
        {
            return StateChanger.ChangeRequest(Instance, canQueue);
        }

        /// <summary>
        /// このシーンに必要なSceneの非同期読み込みの開始など。<br></br>
        /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
        /// </summary>
        public virtual void Init() { }

        /// <summary>
        /// フェードアウトなどで画面が隠れた時に行いたい処理。シーンの読み直しなど。<br></br>
        /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
        /// </summary>
        public virtual void OnHideScreen() { }

        /// <summary>
        /// 全てのシーンのAwake完了後のフェードインなどの処理。<br></br>
        /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
        /// </summary>
        public virtual IEnumerator OnAwakeDone()
        {
            yield return null;
        }

        /// <summary>
        /// 次のシーンへの切り替えで画面が隠れた時に呼び出すシーンの終了処理。シーンの解放などを実装。<br></br>
        /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDone
        /// </summary>
        public virtual void Terminate() { }
    }
}