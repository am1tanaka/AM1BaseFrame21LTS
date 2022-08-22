using UnityEngine;
using UnityEngine.Audio;

namespace AM1.BaseFrame
{
    /// <summary>
    /// システム効果音などの位置のない効果音の簡易管理クラス
    /// </summary>
    public class SESourceAndClips : AudioSourceAndClipsBase<SESourceAndClips>
    {
        /// <summary>
        /// AudioMixerの対応名
        /// </summary>
        protected override string VolumeKey => "SEVolume";

        public DelaySEPlayer DelaySEPlayerInstance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void FixedUpdate()
        {
            DelaySEPlayerInstance?.FixedUpdate();
        }

        /// <summary>
        /// 指定のインデックスの効果音を再生。
        /// 既定の時間内に同一の効果音が再生していた場合、
        /// DelaySEPlayerに遅延再生を登録
        /// </summary>
        /// <param name="index">再生したいSEのインデックス</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Play(int index)
        {
            if (DelaySEPlayerInstance == null)
            {
                audioSource.PlayOneShot(audioClips[index]);
            }
            else
            {
                DelaySEPlayerInstance.Play(index, audioClips[index]);
            }
        }

        /// <summary>
        /// 指定のインデックスのAudioClipを返す。
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <returns>該当するAudioClipインスタンス</returns>
        public AudioClip GetAudioClip(int index)
        {
            return audioClips[index];
        }

        /// <summary>
        /// 遅延再生クラスの初期化
        /// </summary>
        /// <param name="indexCount">管理するインデックスの数</param>
        /// <param name="delay">遅延秒数</param>
        /// <param name="max">最大遅延数</param>
        public void InitDelaySEPlayer(int indexCount, float delay, int max)
        {
            DelaySEPlayerInstance = new DelaySEPlayer(indexCount, delay, max, audioSource);
        }
    }
}