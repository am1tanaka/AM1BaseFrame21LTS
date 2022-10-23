using UnityEngine;
using UnityEngine.Audio;

namespace AM1.BaseFrame
{
    /// <summary>
    /// BGMや効果音の音源を管理するクラスのベースクラス。最低限必要なフィールドや処理を持つ。
    /// </summary>
    public abstract class AudioSourceAndClipsBase<T> : VolumeBase<T>
    {
        [Tooltip("対応AudioSource"), SerializeField]
        protected AudioSource audioSource = default;

        [Header("音源")]
        [Tooltip("音源リスト"), SerializeField]
        protected AudioClip[] audioClips = default;

        /// <summary>
        /// BGM用に使っているAudioSourceのインスタンスを返す
        /// </summary>
        public AudioSource AudioSourceInstance => audioSource;

        /// <summary>
        /// ミュート時などに一瞬鳴る症状を解消するために小ボリュームで空鳴らしをします。
        /// </summary>
        public void WarmUpPlayOneShot()
        {
            if ((audioClips != null) && (audioClips[0] != null))
            {
                audioSource.PlayOneShot(audioClips[0], 0.0001f);
            }
        }

        public override void SetVolumeSetting(VolumeSetting vs)
        {
            base.SetVolumeSetting(vs);
            WarmUpPlayOneShot();
        }
    }
}