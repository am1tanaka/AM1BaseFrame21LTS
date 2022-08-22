using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AM1.BaseFrame
{
    /// <summary>
    /// ボリューム１つ分を管理するクラス。
    /// </summary>
    public class VolumeSetting
    {
        /// <summary>
        /// VolumeTypeに対応するインスタンスを登録します。
        /// </summary>
        public static readonly List<VolumeSetting> volumeSettings = new List<VolumeSetting>();

        /// <summary>
        /// ボリュームの最大値
        /// </summary>
        public static int volumeMax = 5;

        /// <summary>
        /// 現在のボリューム
        /// </summary>
        public int Volume { get; private set; }

        IVolumeSaver volumeSaver;

        /// <summary>
        /// ボリューム変更時に実行したい処理を登録
        /// </summary>
        public UnityEvent ChangeVolumeEvent { get; private set; } = new UnityEvent();

        /// <summary>
        /// 必要なパラメーターを指定して初期化。
        /// </summary>
        /// <param name="index">volumeSettingsに登録するインデックス</param>
        /// <param name="saver">値の読み書き用インターフェース</param>
        public VolumeSetting(int index, IVolumeSaver saver)
        {
            // 足りない時はnullを追加
            while (volumeSettings.Count <= index)
            {
                volumeSettings.Add(null);
            }
            volumeSettings[index] = this;
            SetSaver(saver);
        }

        /// <summary>
        /// ボリュームを読み書きするインスタンスを設定
        /// </summary>
        /// <param name="saver">使用するインスタンス</param>
        public void SetSaver(IVolumeSaver saver)
        {
            volumeSaver = saver;
            Volume = volumeSaver.Load((volumeMax + 1) / 2);
            ChangeVolumeEvent.Invoke();
        }

        /// <summary>
        /// ボリュームを指定の値で上書き
        /// </summary>
        /// <param name="vol">新しいボリューム</param>
        public void ChangeVolume(int vol)
        {
            int newval = Mathf.Clamp(vol, 0, volumeMax);
            if (newval != Volume)
            {
                Volume = newval;
                volumeSaver?.Save(Volume);
                ChangeVolumeEvent.Invoke();
            }
        }
    }
}