using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.BaseFrame
{
    /// <summary>
    /// 効果音用のSourceAndClipsから管理して、Play()で同時再生時に遅延再生させる。
    /// </summary>
    public class DelaySEPlayer
    {
        public class DelayData
        {
            public float time;
            public AudioClip clip;
        }

        public List<DelayData> PlayDataList { get; private set; }
        Queue<DelayData> dataPool;
        float[] lastPlayTime;
        float delaySeconds;
        AudioSource audioSource;

        /// <summary>
        /// 遅延再生クラスの初期化
        /// </summary>
        /// <param name="indexCount">管理するインデックス数</param>
        /// <param name="delay">同時再生を禁止する秒数</param>
        /// <param name="delayMax">遅延再生バッファの上限。これ以上は溜めない</param>
        public DelaySEPlayer(int indexCount, float delay, int delayMax, AudioSource audio)
        {
            lastPlayTime = new float[indexCount];
            delaySeconds = delay;
            PlayDataList = new List<DelayData>(delayMax);
            dataPool = new Queue<DelayData>(delayMax);
            for (int i = 0; i < delayMax; i++)
            {
                dataPool.Enqueue(new DelayData());
            }
        
            audioSource = audio;
        }

        /// <summary>
        /// 指定のインデックスの音の再生を試みる。
        /// </summary>
        /// <param name="index">対応するインデックス</param>
        /// <param name="clip">再生予定のオーディオクリップ</param>
        public void Play(int index, AudioClip clip)
        {
            if (!clip) return;

            if (lastPlayTime[index]+delaySeconds <= Time.time)
            {
                // 前回再生から時間が経過していたのですぐに再生して終了
                audioSource.PlayOneShot(clip);
                lastPlayTime[index] = Time.time;
                return;
            }

            // キューオーバーなら無視
            if (dataPool.Count == 0) return;

            // キューに詰む
            InsertData(index, clip);
        }

        /// <summary>
        /// 指定のデータをtimeの昇順にリストに加える。
        /// </summary>
        /// <param name="index">lastPlayTimeのインデックス</param>
        /// <param name="clip">再生するAudioClip</param>
        void InsertData(int index, AudioClip clip)
        {
            var dt = dataPool.Dequeue();
            dt.clip = clip;
            dt.time = lastPlayTime[index] + delaySeconds;
            lastPlayTime[index] = dt.time;

            for (int i=0;i<PlayDataList.Count;i++)
            {
                if (dt.time < PlayDataList[i].time)
                {
                    PlayDataList.Insert(i, dt);
                    return;
                }
            }

            // 最後のデータならAdd
            PlayDataList.Add(dt);
        }

        /// <summary>
        /// 対応するAudioSourceAndClipsのFixedUpdate()から呼び出す
        /// </summary>
        public void FixedUpdate()
        {
            while ((PlayDataList.Count > 0) && (Time.time >= PlayDataList[0].time))
            {
                audioSource.PlayOneShot(PlayDataList[0].clip);
                dataPool.Enqueue(PlayDataList[0]);
                PlayDataList.RemoveAt(0);
            }
        }
    }
}

