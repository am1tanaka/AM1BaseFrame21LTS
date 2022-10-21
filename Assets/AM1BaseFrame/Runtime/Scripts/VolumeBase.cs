using UnityEngine;
using UnityEngine.Audio;

namespace AM1.BaseFrame
{
    /// <summary>
    /// BGMや効果音のボリュームミキサーを管理するベースクラス。
    /// </summary>
    public abstract class VolumeBase<T> : MonoBehaviour
    {
        public static T Instance { get; protected set; }

        [Tooltip("対応AudioMixer"), SerializeField]
        protected AudioMixer audioMixer = default;

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
        /// BGM用に使っているAudioMixerGroupのインスタンスを返す。
        /// </summary>
        public AudioMixer AudioMixerInstance => audioMixer;

        protected VolumeSetting volumeSetting;

        /// <summary>
        /// ボリュームを更新してAudioMixerに設定。利用する場合はFixedUpdateから呼び出します。
        /// </summary>
        public virtual void UpdateVolumeWithFade()
        {
            UpdateVolumeWithFade(1);
        }

        /// <summary>
        /// フェードなどの通常のボリューム設定以外のボリュームを適用する際に利用する更新処理。
        /// 利用する場合はFixedUpdateから呼び出します。
        /// </summary>
        /// <param name="fadeVolume">フェード値を0～1で指定</param>
        public virtual void UpdateVolumeWithFade(float fadeVolume)
        {
            float vol = fadeVolume;
            if (volumeSetting != null)
            {
                vol *= Mathf.Clamp01(((float)volumeSetting.Volume / (float)VolumeSetting.volumeMax));
            }

            float newdB = GetdB(vol);
            float currentdB = newdB;
            audioMixer.GetFloat(VolumeKey, out currentdB);
            if (!Mathf.Approximately(newdB, currentdB))
            {
                audioMixer.SetFloat(VolumeKey, GetdB(vol));
            }
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