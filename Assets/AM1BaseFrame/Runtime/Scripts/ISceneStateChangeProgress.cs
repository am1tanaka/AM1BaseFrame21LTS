using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.BaseFrame
{
    /// <summary>
    /// シーン切り替えの進捗を表すインターフェース
    /// </summary>
    public interface ISceneStateChangeProgress
    {
        /// <summary>
        /// プログレスバーの表示
        /// </summary>
        void Show();

        /// <summary>
        /// プログレスバーの非表示
        /// </summary>
        void Hide();
    }
}