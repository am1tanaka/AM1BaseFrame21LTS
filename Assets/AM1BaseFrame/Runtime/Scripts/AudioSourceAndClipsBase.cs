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
    }
}