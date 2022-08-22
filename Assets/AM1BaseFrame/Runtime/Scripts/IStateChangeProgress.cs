using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.BaseFrame
{
    public interface IStateChangeProgress
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