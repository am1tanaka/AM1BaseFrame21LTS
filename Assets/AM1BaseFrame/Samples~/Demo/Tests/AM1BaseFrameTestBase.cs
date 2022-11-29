using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using AM1.BaseFrame;
using AM1.BaseFrame.Demo;

public class AM1BaseFrameTestBase
{
    protected IEnumerator BootTitle()
    {
        SceneStateChanger.ResetStatics();
        SceneManager.LoadScene("DemoSystem");

        // タイトルが起動するのを待つ
        float time = 3f;
        while ((time > 0) && (!SceneStateChanger.IsStateStarted(TitleStateChanger.Instance)))
        {
            time -= Time.unscaledDeltaTime;
            yield return null;
        }
        Assert.That(time, Is.GreaterThan(0), "タイトル起動");
    }
}
