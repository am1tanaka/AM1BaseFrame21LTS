using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AM1.BaseFrame.Assets;

namespace AM1.BaseFrame.Demo
{
    public class TitleStateChanger : SceneStateChangerBase<TitleStateChanger>, ISceneStateChanger
    {
        static float TransitionSeconds => 0.5f;

        public override void Init()
        {
            SceneStateChanger.LoadSceneAsync(nameof(SceneType.DemoTitle), true);
        }

        public override IEnumerator OnAwakeDone()
        {
            // スライダーをボリュームに割り当て
            VolumeSetting.volumeSettings[(int)VolumeType.SE].ChangeVolumeEvent.AddListener(OnChangeSEVolume);

            // フェードイン
            ScreenTransitionRegistry.StartUncover(TransitionSeconds);
            yield return ScreenTransitionRegistry.WaitAll();
            BGMPlayer.Play(BGMPlayer.BGM.Title);
        }

        void OnChangeSEVolume()
        {
            SEPlayer.Play(SEPlayer.SE.Click);
        }

        public override void Terminate()
        {
            VolumeSetting.volumeSettings[(int)VolumeType.SE].ChangeVolumeEvent.RemoveListener(OnChangeSEVolume);
            SceneStateChanger.UnloadSceneAsync(nameof(SceneType.DemoTitle));
        }
    }
}