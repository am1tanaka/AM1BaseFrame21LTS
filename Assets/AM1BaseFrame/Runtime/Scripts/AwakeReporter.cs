using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AM1.BaseFrame
{
    /// <summary>
    /// Awakeの実行をStateChangerに報告します。
    /// </summary>
    public class AwakeReporter : MonoBehaviour
    {
        private void Awake()
        {
            if (StateChanger.IsReady)
            {
                StateChanger.AwakeDone(gameObject.scene.name);
            }
        }
    }
}