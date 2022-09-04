using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.BaseFrame.General
{
    /// <summary>
    /// Title状態への切り替え
    /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDoneの順にStateChanger.csから呼ばれます。
    /// </summary>
    public class TitleStateChanger : StateChangerBase<TitleStateChanger>, IStateChanger
    {
        /// <summary>
        /// 画面を覆う演出の開始や、この状態に必要なシーンの非同期読み込みの開始など。
        /// </summary>
        public override void Init()
        {
            // 画面を覆う
            ScreenTransitionRegistry.StartCover((int)ScreenTransitionType.FilledRadial, 0.5f);

            // シーンの非同期読み込み開始
            StateChanger.LoadSceneAsync("Title", true);

        }

        /// <summary>
        /// 画面が覆われて、不要なシーンの解放が完了した時に行いたい処理を実装します。シーンの読み直しなど。
        /// </summary>
        public override void OnHideScreen()
        {
            
        }

        /// <summary>
        /// 画面が覆われていて、全てのシーンが読み込まれてAwakeが実行された後に呼ばれます。<br></br>
        /// フェードインなどの状態を始めるための処理を実装します。
        /// </summary>
        public override IEnumerator OnAwakeDone() {
            // 画面の覆いを解除
            ScreenTransitionRegistry.StartUncover(0.5f);
            yield return ScreenTransitionRegistry.WaitAll();

        }

        /// <summary>
        /// 次の状態への切り替えにおいて、画面が隠れた時に呼び出すシーンの終了処理。不要になったシーンの解放などを実装します。
        /// </summary>
        public override void Terminate() {
            // シーンの解放
            StateChanger.UnloadSceneAsync("Title");

        }
    }
}
