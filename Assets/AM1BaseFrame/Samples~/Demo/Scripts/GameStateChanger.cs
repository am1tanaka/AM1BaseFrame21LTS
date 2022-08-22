using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AM1.BaseFrame.Demo
{
    public class GameStateChanger : StateChangerBase<GameStateChanger>, IStateChanger
    {
        static float TransitionSeconds => 0.5f;

        public override void Init()
        {
            StateChanger.LoadSceneAsync(nameof(SceneType.DemoGame), true);
            BGMSourceAndClips.Instance.Stop(TransitionSeconds);
        }

        public override IEnumerator OnAwakeDone()
        {
            // 画面表示
            ScreenTransitionRegistry.StartUncover(TransitionSeconds);
            yield return ScreenTransitionRegistry.WaitAll();

            // 状態開始
            GameBehaviour.Instance.StartState();
        }
    }
}