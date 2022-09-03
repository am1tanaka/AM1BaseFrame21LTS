using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AM1.BaseFrame.General
{
    /// <summary>
    /// ボリューム用のスライダーにアタッチする。
    /// 変更時の登録はInspectorは利用せずスクリプトで設定する。
    /// VolumeSettingに依存。
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class VolumeSlider : MonoBehaviour
    {
        [Tooltip("このスライダーが担当するボリュームの種類"), SerializeField]
        VolumeType volumeType = default;

        Slider slider;
        VolumeSetting currentVolumeSetting;

        void Start()
        {
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(OnChangeValue);

            // VolumeSettingの初期化は起動処理で実行済みで、
            // この処理が呼ばれるのはタイトルシーンを読み込んでからなので、
            // タイミング上は必ずインスタンスが存在する。
#if UNITY_EDITOR
            if (VolumeSetting.volumeSettings.Count < (int)volumeType + 1)
            {
                Debug.LogWarning($"{volumeType}のVolumeSettingが初期化されていません。");
                return;
            }
#endif
            currentVolumeSetting = VolumeSetting.volumeSettings[(int)volumeType];
            slider.value = currentVolumeSetting.Volume;
        }

        /// <summary>
        /// スライダーの変更を反映
        /// </summary>
        /// <param name="newval"></param>
        void OnChangeValue(float newval)
        {
            int newValInt = Mathf.RoundToInt(newval);
            if (newValInt != currentVolumeSetting.Volume)
            {
                currentVolumeSetting.ChangeVolume(newValInt);
            }
        }
    }
}