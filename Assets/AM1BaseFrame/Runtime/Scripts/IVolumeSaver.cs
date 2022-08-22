using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.BaseFrame
{
    /// <summary>
    /// ボリュームの読み書き用インターフェース
    /// </summary>
    public interface IVolumeSaver
    {
        /// <summary>
        /// 保存したBボリュームを読み込んで返す。
        /// 保存がない時は初期値を返す。
        /// </summary>
        /// <param name="df">初期値</param>
        /// <returns>読み込んだボリュームか初期値</returns>
        int Load(int df);

        /// <summary>
        /// ボリュームを保存する。
        /// </summary>
        /// <param name="v">保存する値</param>
        void Save(int v);

        /// <summary>
        /// 保存してある設定を削除する。
        /// </summary>
        void ClearSaveData();
    }
}