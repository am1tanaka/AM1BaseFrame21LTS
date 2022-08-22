using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AM1.BaseFrame.Demo
{
    public class Booter : MonoBehaviour
    {
        private void Awake()
        {
            BootStateChanger.Instance.Request();
            StartCoroutine(BootSequence());
        }

        IEnumerator BootSequence()
        {
            yield return new WaitUntil(() => StateChanger.IsStateStarted(BootStateChanger.Instance));

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

            TitleStateChanger.Instance.Request();
            Destroy(gameObject);
        }
    }
}