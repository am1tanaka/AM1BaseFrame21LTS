using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using AM1.BaseFrame;
using AM1.BaseFrame.Demo;

public class StateChangeTests : AM1BaseFrameTestBase
{
    [UnityTest]
    public IEnumerator StateChangeTestsWithEnumeratorPasses()
    {
        yield return BootTitle();

        // ゲーム開始
        TitleBehaviour.Instance.OnStartButtonClicked();
        yield return null;
        TitleBehaviour.Instance.OnStartButtonClicked();
        yield return WaitSecondsOrChangeDone(3f);
        Assert.That(StateChanger.IsStateStarted(GameStateChanger.Instance), Is.True, $"ゲーム確認: {StateChanger.CurrentState}");
        Assert.That(GameBehaviour.Instance, Is.Not.Null, "ゲームインスタンス");
        yield return WaitSecondsOrSceneStarted(3f, GameBehaviour.Instance);

        // クリア
        GameBehaviour.Instance.ToClear();
        yield return WaitSecondsOrChangeDone(3f);
        Assert.That(StateChanger.IsStateStarted(ResultStateChanger.Instance), Is.True, "結果確認");
        Assert.That(ResultStateChanger.Instance, Is.Not.Null, "結果インスタンス");
        yield return WaitSecondsOrSceneStarted(3f, ResultBehaviour.Instance);

        //タイトルへ
        ResultBehaviour.Instance.ToTitle();
        yield return WaitSecondsOrChangeDone(3f);
        Assert.That(StateChanger.IsStateStarted(TitleStateChanger.Instance), Is.True, $"タイトル確認2 {StateChanger.CurrentState}");
        Assert.That(TitleBehaviour.Instance, Is.Not.Null, "タイトルインスタンス2");
        yield return WaitSecondsOrSceneStarted(3f, TitleBehaviour.Instance);

        // ゲーム開始
        TitleBehaviour.Instance.OnStartButtonClicked();
        yield return null;
        TitleBehaviour.Instance.OnStartButtonClicked();
        yield return WaitSecondsOrChangeDone(3f);
        Assert.That(StateChanger.IsStateStarted(GameStateChanger.Instance), Is.True, "ゲーム確認2");
        Assert.That(GameBehaviour.Instance, Is.Not.Null, "ゲームインスタンス2");
        yield return WaitSecondsOrSceneStarted(3f, GameBehaviour.Instance);

        // ミス
        GameBehaviour.Instance.ToGameover();
        yield return WaitSecondsOrChangeDone(3f);
        Assert.That(StateChanger.IsStateStarted(ResultStateChanger.Instance), Is.True, "結果確認");
        Assert.That(ResultStateChanger.Instance, Is.Not.Null, "結果インスタンス");
        yield return WaitSecondsOrSceneStarted(3f, ResultBehaviour.Instance);

        //タイトル
        ResultBehaviour.Instance.ToTitle();
        yield return WaitSecondsOrChangeDone(3f);
        Assert.That(StateChanger.IsStateStarted(TitleStateChanger.Instance), Is.True, "タイトル確認2");
        Assert.That(TitleBehaviour.Instance, Is.Not.Null, "タイトルインスタンス2");
        yield return WaitSecondsOrSceneStarted(3f, TitleBehaviour.Instance);

        // ゲーム開始
        TitleBehaviour.Instance.OnStartButtonClicked();
        yield return null;
        TitleBehaviour.Instance.OnStartButtonClicked();
        yield return WaitSecondsOrChangeDone(3f);
        Assert.That(StateChanger.IsStateStarted(GameStateChanger.Instance), Is.True, "ゲーム確認2");
        Assert.That(GameBehaviour.Instance, Is.Not.Null, "ゲームインスタンス2");
        yield return WaitSecondsOrSceneStarted(3f, GameBehaviour.Instance);
    }

    /// <summary>
    /// 指定秒数が経過するか、シーンの切り替え完了を待つ
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitSecondsOrChangeDone(float sec)
    {
        float left = sec;

        // 切り替え開始待ち
        while (!StateChanger.IsChanging && (left > 0))
        {
            left -= Time.unscaledDeltaTime;
            yield return null;
        }
        if (left <= 0) yield break;

        // 切り替え待ち
        while (StateChanger.IsChanging && (left > 0))
        {
            left -= Time.unscaledDeltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// 指定秒数かシーンの開始を待つ
    /// </summary>
    /// <param name="sec">タイムアウト秒数</param>
    /// <param name="scene">チェックするシーンのインスタンス</param>
    /// <returns></returns>
    IEnumerator WaitSecondsOrSceneStarted(float sec, ISceneBehaviour scene)
    {
        float left = sec;

        // 開始待ち
        while (!scene.IsStarted && (left > 0))
        {
            left -= Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
