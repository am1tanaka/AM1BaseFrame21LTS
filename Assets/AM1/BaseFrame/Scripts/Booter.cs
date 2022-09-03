using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AM1.BaseFrame.General
{
    /// <summary>
    /// 状態システム起動クラス
    /// </summary>
    public class Booter : MonoBehaviour
    {
        private void Awake()
        {
            StartCoroutine(BootSequence());
        }

        IEnumerator BootSequence()
        {
#if UNITY_EDITOR
            // システムシーン以外を削除
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var sc = SceneManager.GetSceneAt(i);
                if (sc.name != gameObject.scene.name)
                {
                    yield return SceneManager.UnloadSceneAsync(sc.name);
                }
            }
#endif

            BootStateChanger.Instance.Request();
            yield return new WaitWhile(() => StateChanger.IsRequestOrChanging);
            Destroy(gameObject);
        }
    }
}