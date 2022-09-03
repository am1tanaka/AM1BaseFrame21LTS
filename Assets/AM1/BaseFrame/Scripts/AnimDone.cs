using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AM1.BaseFrame.General
{
    public class AnimDone : MonoBehaviour
    {
        [Tooltip("アニメが完了した時に実行する処理"), SerializeField]
        UnityEvent onDone = new UnityEvent();

        public void OnDone()
        {
            onDone.Invoke();
        }
    }
}