using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AM1.BaseFrame.General;

namespace AM1.BaseFrame.Demo
{
    /// <summary>
    /// 各種初期化や起動処理を行う。
    /// </summary>
    public class BootDemoStateChanger : StateChangerBase<BootDemoStateChanger>, IStateChanger
    {
        public override void Init()
        {
            // 画面を覆う
            ScreenTransitionRegistry.StartCover((int)ScreenTransitionType.FilledRadial);
        }

        public override void OnHideScreen()
        {
            // ボリューム初期化
            new VolumeSetting((int)VolumeType.BGM, new BGMVolumeSaverWithPlayerPrefs());
            BGMSourceAndClips.Instance.SetVolumeSetting(VolumeSetting.volumeSettings[(int)VolumeType.BGM]);
            new VolumeSetting((int)VolumeType.SE, new SEVolumeSaverWithPlayerPrefs());
            SESourceAndClips.Instance.SetVolumeSetting(VolumeSetting.volumeSettings[(int)VolumeType.SE]);

            // 遅延再生初期化
            SESourceAndClips.Instance.InitDelaySEPlayer(System.Enum.GetValues(typeof(SEPlayer.SE)).Length, SEPlayer.DelaySeconds, SEPlayer.DelayMax);

            // ここに最初のシーンへの切り替え要求を追記
            TitleStateChanger.Instance.Request(true);
        }
    }
}