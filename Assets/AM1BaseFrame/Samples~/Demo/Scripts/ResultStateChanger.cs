using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AM1.BaseFrame.Demo
{
    public class ResultStateChanger : StateChangerBase<ResultStateChanger>, IStateChanger
    {
        public enum ResultType
        {
            None = -1,
            Success,
            Miss
        }

        static float FadeOutSeconds => 1;

        /// <summary>
        /// 結果の種類
        /// </summary>
        public static ResultType CurrentResultType { get; private set; }

        public override void Init()
        {
            StateChanger.LoadSceneAsync(nameof(SceneType.DemoGameResult), false);
            BGMSourceAndClips.Instance.Stop(FadeOutSeconds);
        }

        /// <summary>
        /// 結果の種類を指定して状態切り替え要求
        /// </summary>
        /// <param name="tp">結果タイプ</param>
        /// <returns>要求成功の時true</returns>
        public bool Request(ResultType tp)
        {
            CurrentResultType = tp;
            return Request();
        }

        public override IEnumerator OnAwakeDone()
        {
            ResultBehaviour.Instance.StartAnim(CurrentResultType);
            yield return null;
        }

        public override void Terminate()
        {
            StateChanger.UnloadSceneAsync(nameof(SceneType.DemoGame));
            StateChanger.UnloadSceneAsync(nameof(SceneType.DemoGameResult));
        }
    }
}