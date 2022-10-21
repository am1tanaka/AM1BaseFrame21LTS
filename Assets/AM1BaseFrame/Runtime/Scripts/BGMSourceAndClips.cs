using UnityEngine;
using UnityEngine.Audio;

namespace AM1.BaseFrame
{
    /// <summary>
    /// BGM用のAudioClipの登録とBGMの再生開始、停止。フェードイン、フェードアウトを受け取ったらBGMSEVolumeに秒数と停止のコールバックを渡す。
    /// </summary>
    public class BGMSourceAndClips : AudioSourceAndClipsBase<BGMSourceAndClips>
    {
        /// <summary>
        /// AudioMixerの対応名
        /// </summary>
        protected override string VolumeKey => "BGMVolume";

        enum FadeState
        {
            None = -1,
            FadeIn,     // ボリュームアップ
            FadeOut,    // ボリュームダウン
        }

        FadeState currentFadeState = FadeState.None;

        /// <summary>
        /// 0から1、或いは、1から0にフェードするのにかかる秒数。この値から1回分のボリュームの増減値を算出して
        /// fadeCurrentVolumeに反映させる
        /// </summary>
        float fadeSeconds;

        /// <summary>
        /// フェード中のボリュームを0～1で表す
        /// </summary>
        float currentFadeVolume;

        /// <summary>
        /// 再生中のインデックス。曲を停止したら-1
        /// </summary>
        int currentIndex = -1;

        private void Awake()
        {
            Instance = this;
            currentFadeState = FadeState.None;
        }

        private void FixedUpdate()
        {
            if (currentFadeState == FadeState.None) return;

            float tick = Time.deltaTime * (1f / fadeSeconds);
            if (currentFadeState == FadeState.FadeIn)
            {
                currentFadeVolume += tick;
                if (currentFadeVolume >= 1f)
                {
                    // フェードイン完了
                    currentFadeVolume = 1;
                    currentFadeState = FadeState.None;
                }
            }
            else
            {
                currentFadeVolume -= tick;
                if (currentFadeVolume < 0)
                {
                    // フェードアウト完了
                    currentFadeVolume = 0;
                    currentFadeState = FadeState.None;
                    audioSource.Stop();
                    currentIndex = -1;
                }
            }

            // ボリュームを反映
            UpdateVolumeWithFade(currentFadeVolume);
        }

        /// <summary>
        /// 指定のインデックスをBGMとして再生
        /// </summary>
        /// <param name="index">再生したいAudioClipをインデックスで指定</param>
        /// <param name="time">フェードイン秒数。省略するか0で即時再生開始。</param>
        public void Play(int index, float time = 0)
        {
            // 無効な指示や同じ曲を再生中なら無視
            if (    (index<0) 
                ||  (index >= audioClips.Length)
                ||  (audioClips[index] == null)
                ||  (audioSource == null))
            {
                return;
            }

            // 同じ曲が再生中の時の処理
            if (index == currentIndex)
            {
                PlaySameIndex(time);
            }
            else
            {
                // 違う曲が再生中の時
                PlayDifferentIndex(index, time);
            }
        }

        /// <summary>
        /// 同じインデックスの時の再生処理
        /// </summary>
        void PlaySameIndex(float time)
        {
            // フェード指定 あり / 状態 None = すでに再生中なので処理不要
            // フェード指定 なし / 状態 None = すでに再生中なので処理不要
            if (currentFadeState == FadeState.None) return;

            // フェード指定 あり / 状態 FadeIn = FadeInにして時間設定
            // フェード指定 あり / 状態 FadeOut = FadeInにして時間設定
            // フェード指定 なし / 状態 FadeIn = FadeInにして時間そのまま
            // フェード指定 なし / 状態 FadeOut = FadeInにして時間そのまま
            currentFadeState = FadeState.FadeIn;

            // 時間設定があれば反映
            if (time >= 1f / 60f)
            {
                fadeSeconds = time;
            }
        }

        /// <summary>
        /// 違う曲を演奏中の時の再生処理
        /// </summary>
        /// <param name="index">再生するインデックス</param>
        /// <param name="time">フェードイン秒数。1/60秒未満なら即時再生</param>
        void PlayDifferentIndex(int index, float time)
        {
            if (time >= 1f/60f)
            {
                // フェードインあり
                currentFadeVolume = 0;
                fadeSeconds = time;
                currentFadeState = FadeState.FadeIn;
            }
            else
            {
                // フェードなし
                currentFadeVolume = 1;
                currentFadeState = FadeState.None;
            }
            UpdateVolumeWithFade();

            // 再生開始
            currentIndex = index;
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }

        /// <summary>
        /// BGMを停止。引数の秒数フェードアウト
        /// </summary>
        /// <param name="time">フェードアウトさせたい時に秒数を指定。省略すると即時停止</param>
        public void Stop(float time = 0)
        {
            fadeSeconds = time;

            // 1フレーム以内の時間なら即時停止
            if (time < 1f/60f)
            {
                currentIndex = -1;
                audioSource.Stop();
                currentFadeState = FadeState.None;
                return;
            }

            // フェードしていなければフェードボリュームを最大に設定
            if (currentFadeState == FadeState.None)
            {
                // フェードしていなかったらフェード開始
                currentFadeVolume = 1;
            }

            // 状態切り替え
            currentFadeState = FadeState.FadeOut;
        }
    }
}