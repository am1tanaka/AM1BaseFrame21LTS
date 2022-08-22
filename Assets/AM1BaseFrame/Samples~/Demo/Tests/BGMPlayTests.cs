using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.BaseFrame;
using AM1.BaseFrame.General;

public class BGMPlayTests : AM1BaseFrameTestBase
{
    [UnityTest]
    public IEnumerator BGMPlayTestsWithEnumeratorPasses()
    {
        // 設定ファイルをリセット
        BGMVolumeSaverWithPlayerPrefs.prefix = "debug";
        var bgmSaver = new BGMVolumeSaverWithPlayerPrefs();
        bgmSaver.Save(5);
        var seSaver = new SEVolumeSaverWithPlayerPrefs();
        seSaver.Save(5);

        yield return BootTitle();

        // BGM開始
        BGMPlayer.Play(BGMPlayer.BGM.Title);
        yield return new WaitForSeconds(1);

        // フェードアウト
        Assert.That(BGMdB, Is.GreaterThan(-0.99f), "再生前のボリューム確認");

        BGMSourceAndClips.Instance.Stop(1f);
        yield return new WaitForSeconds(0.5f);
        Assert.That(BGMSourceAndClips.Instance.AudioSourceInstance.isPlaying, Is.True, "曲は再生中");
        yield return new WaitForSeconds(0.55f);
        Assert.That(BGMdB, Is.LessThan(BGMSourceAndClips.GetdB(0.1f)), "フェードアウト完了");
        Assert.That(BGMSourceAndClips.Instance.AudioSourceInstance.isPlaying, Is.False, "フェードアウトBGM再生停止");

        // 即時再生
        BGMPlayer.Play(BGMPlayer.BGM.Game);
        yield return new WaitForSeconds(1f);
        Assert.That(BGMdB, Is.GreaterThan(BGMSourceAndClips.GetdB(0.9f)), "BGM再生ボリュームオン");
        Assert.That(BGMSourceAndClips.Instance.AudioSourceInstance.isPlaying, Is.True, "即時再生");

        // 同じ曲の同時再生抑制
        float playTime = BGMSourceAndClips.Instance.AudioSourceInstance.time;
        yield return new WaitForSeconds(0.1f);
        BGMPlayer.Play(BGMPlayer.BGM.Game);
        Assert.That(BGMSourceAndClips.Instance.AudioSourceInstance.time, Is.GreaterThan(playTime), "同じ曲なら再生抑制");

        // 即時に曲変更
        playTime = BGMSourceAndClips.Instance.AudioSourceInstance.time;
        BGMPlayer.Play(BGMPlayer.BGM.Title);
        Assert.That(BGMSourceAndClips.Instance.AudioSourceInstance.time, Is.LessThan(playTime), "違う曲なら再生");
        Assert.That(BGMSourceAndClips.Instance.AudioSourceInstance.isPlaying, Is.True, "再生");
        yield return new WaitForSeconds(1f);

        // 即時停止
        BGMSourceAndClips.Instance.Stop();
        yield return null;
        Assert.That(BGMSourceAndClips.Instance.AudioSourceInstance.isPlaying, Is.False, "即時停止");

        // フェードイン再生
        BGMPlayer.Play(BGMPlayer.BGM.Title, 1);
        yield return new WaitForSeconds(0.1f);
        Assert.That(BGMSourceAndClips.Instance.AudioSourceInstance.isPlaying, Is.True, "フェードイン再生");
        float lastdB = BGMdB;
        Assert.That(lastdB, Is.InRange(BGMSourceAndClips.GetdB(0.01f), BGMSourceAndClips.GetdB(0.25f)), "フェードインボリューム");
        yield return new WaitForSeconds(1);
        Assert.That(BGMdB, Is.GreaterThan(lastdB), "dBアップ");
        lastdB = BGMdB;
        yield return new WaitForSeconds(0.5f);
        Assert.That(BGMdB, Is.EqualTo(lastdB).Within(0.1f), "同じdB");

        // フェードアウト中に別の曲をフェードイン再生開始
        BGMSourceAndClips.Instance.Stop(1);
        yield return new WaitForSeconds(0.25f);
        Assert.That(BGMdB, Is.LessThan(lastdB), "フェードアウト開始dB");
        BGMPlayer.Play(BGMPlayer.BGM.Game, 1);
        yield return null;
        Assert.That(BGMdB, Is.LessThan(BGMSourceAndClips.GetdB(0.1f)), "ゲームBGM再生で小ボリューム");
        yield return new WaitForSeconds(0.2f);
        lastdB = BGMdB;
        Assert.That(BGMdB, Is.InRange(BGMSourceAndClips.GetdB(0.1f), BGMSourceAndClips.GetdB(0.25f)), "BGMボリュームアップ");

        // フェードイン中からフェードアウト
        BGMSourceAndClips.Instance.Stop(1f);
        yield return null;
        Assert.That(BGMdB, Is.InRange(BGMSourceAndClips.GetdB(0.01f), lastdB), "ボリュームが0以上、さっきより小さい");
        yield return new WaitForSeconds(0.5f);
        Assert.That(BGMSourceAndClips.Instance.AudioSourceInstance.isPlaying, Is.False, "時間内だがボリューム的に曲停止");

        // フェードアウト中に同じ曲を指定するとフェードインして再生継続
        BGMPlayer.Play(BGMPlayer.BGM.Game);
        yield return new WaitForSeconds(1);
        BGMSourceAndClips.Instance.Stop(1f);
        yield return new WaitForSeconds(0.5f);
        playTime = BGMSourceAndClips.Instance.AudioSourceInstance.time;
        lastdB = BGMdB;
        BGMPlayer.Play(BGMPlayer.BGM.Game);
        yield return new WaitForSeconds(0.1f);
        Assert.That(BGMSourceAndClips.Instance.AudioSourceInstance.isPlaying, Is.True, "演奏継続");
        Assert.That(BGMSourceAndClips.Instance.AudioSourceInstance.time, Is.GreaterThan(playTime), "曲継続");
        yield return new WaitForSeconds(0.25f);
        Assert.That(BGMdB, Is.GreaterThan(lastdB), "ボリュームアップ");
        yield return new WaitForSeconds(1);
        Assert.That(BGMSourceAndClips.Instance.AudioSourceInstance.isPlaying, Is.True, "1秒後に演奏継続");
    }

    /// <summary>
    /// BGM用のMixerの現在のdBを返す
    /// </summary>
    float BGMdB
    {
        get
        {
            float vol;
            BGMSourceAndClips.Instance.AudioMixerInstance.GetFloat("BGMVolume", out vol);
            return vol;
        }
    }
}
