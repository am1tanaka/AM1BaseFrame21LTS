using UnityEngine;
using UnityEngine.Audio;

namespace AM1.BaseFrame
{
    /// <summary>
    /// BGMや効果音の音源を管理するクラスのベースクラス。最低限必要なフィールドや処理を持つ。
    /// </summary>
    public abstract class AudioSourceAndClipsBase<T> : MonoBehaviour
    {
        public static T Instance { get; protected set; }

        [Tooltip("対応AudioSource"), SerializeField]
        protected AudioSource audioSource = default;
        [Tooltip("対応AudioMixer"), SerializeField]
        protected AudioMixer audioMixer = default;

        [Header("音源")]
        [Tooltip("音源リスト"), SerializeField]
        protected AudioClip[] audioClips = default;

        /// <summary>
        /// AudioMixerの対応名
        /// </summary>
        protected abstract string VolumeKey { get; }

        /// <summary>
        /// この値より小さい場合は消音
        /// </summary>
        static float MinVolume => 0.01f;

        /// <summary>
        /// 0=消音、1=最大音量とした時の対応するdBの値を返す。
        /// dB = 20 * Log10(X)
        /// log10(X) = dB / 20
        /// 
        /// </summary>
        /// <param name="v">0～1で表したボリューム。1の時は戻り値は0。</param>
        /// <returns>対応するdB値。-80～0</returns>
        public static float GetdB(float v)
        {
            if (v < MinVolume) return -80;
            return 20f * Mathf.Log10(v);
        }

        /// <summary>
        /// BGM用に使っているAudioSourceのインスタンスを返す
        /// </summary>
        public AudioSource AudioSourceInstance => audioSource;

        /// <summary>
        /// BGM用に使っているAudioMixerGroupのインスタンスを返す。
        /// </summary>
        public AudioMixer AudioMixerInstance => audioMixer;

        protected VolumeSetting volumeSetting;

        /// <summary>
        /// ボリュームを更新してAudioMixerに設定
        /// </summary>
        protected virtual void UpdateVolumeWithFade()
        {
            UpdateVolumeWithFade(1);
        }

        /// <summary>
        /// ボリュームを設定
        /// </summary>
        /// <param name="fadeVolume">フェード値を0～1で指定</param>
        protected virtual void UpdateVolumeWithFade(float fadeVolume)
        {
            float vol = fadeVolume;
            if (volumeSetting != null)
            {
                vol *= Mathf.Clamp01(((float)volumeSetting.Volume / (float)VolumeSetting.volumeMax));
            }
            audioMixer.SetFloat(VolumeKey, BGMSourceAndClips.GetdB(vol));
        }

        /// <summary>
        /// ボリューム設定のインスタンスを受け取る。受け取ったらボリューム更新処理を相手に渡す。
        /// </summary>
        /// <param name="vs">対応する処理のインスタンス</param>
        public void SetVolumeSetting(VolumeSetting vs)
        {
            volumeSetting = vs;
            volumeSetting.ChangeVolumeEvent.AddListener(UpdateVolumeWithFade);
            UpdateVolumeWithFade();
        }
    }
}