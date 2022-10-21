using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using AM1.BaseFrame;
using AM1.BaseFrame.Assets;

public class VolumeTests : AM1BaseFrameTestBase
{
    [UnityTest]
    public IEnumerator CallFunctionVolumeTests()
    {
        VolumeSaverWithPlayerPrefsBase.prefix = "test";
        yield return BootTitle();
        BGMPlayer.Play(BGMPlayer.BGM.Title);

        var bgmVolume = new BGMVolumeSaverWithPlayerPrefs();
        VolumeSetting.volumeSettings[(int)VolumeType.BGM].ChangeVolume(1);
        int vol = bgmVolume.Load(5);
        Assert.That(vol, Is.EqualTo(1), "BGM vol=1");
        yield return new WaitForSeconds(1);
        VolumeSetting.volumeSettings[(int)VolumeType.BGM].ChangeVolume(0);
        vol = bgmVolume.Load(5);
        Assert.That(vol, Is.EqualTo(0), "BGM vol=0");
        yield return new WaitForSeconds(1);
        VolumeSetting.volumeSettings[(int)VolumeType.BGM].ChangeVolume(10);
        vol = bgmVolume.Load(0);
        Assert.That(vol, Is.EqualTo(5), "BGM vol=5");
        yield return new WaitForSeconds(1);

        // SE
        var seVolume = new SEVolumeSaverWithPlayerPrefs();
        SEPlayer.Play(SEPlayer.SE.Start);
        vol = seVolume.Load(1);
        VolumeSetting.volumeSettings[(int)VolumeType.SE].ChangeVolume(1);
        SEPlayer.Play(SEPlayer.SE.Start);
        vol = seVolume.Load(1);
        Assert.That(vol, Is.EqualTo(1), "SE vol=1");
        yield return new WaitForSeconds(1);
        VolumeSetting.volumeSettings[(int)VolumeType.SE].ChangeVolume(0);
        SEPlayer.Play(SEPlayer.SE.Start);
        vol = seVolume.Load(5);
        Assert.That(vol, Is.EqualTo(0), "SE vol=0");
        yield return new WaitForSeconds(1);
        VolumeSetting.volumeSettings[(int)VolumeType.SE].ChangeVolume(10);
        SEPlayer.Play(SEPlayer.SE.Start);
        vol = seVolume.Load(0);
        Assert.That(vol, Is.EqualTo(5), "SE vol=5");
        yield return new WaitForSeconds(1);
    }

    [UnityTest]
    public IEnumerator SliderVolumeTests()
    {
        VolumeSaverWithPlayerPrefsBase.prefix = "test";
        var bgmVolume = new BGMVolumeSaverWithPlayerPrefs();
        var seVolume = new SEVolumeSaverWithPlayerPrefs();
        bgmVolume.Save(4);
        seVolume.Save(1);
        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        yield return BootTitle();

        var bgmSlider = GameObject.Find("BGMSlider").GetComponent<Slider>();
        var seSlider = GameObject.Find("SESlider").GetComponent<Slider>();
        Assert.That(bgmSlider, Is.Not.Null, "BGMスライダー取得");
        Assert.That(seSlider, Is.Not.Null, "SEスライダー取得");

        yield return wait;
        // 値チェック
        Assert.That(bgmSlider.value, Is.EqualTo(4).Within(0.01f), "BGMボリューム設定");
        Assert.That(seSlider.value, Is.EqualTo(1).Within(0.01f), "SEボリューム設定");

        // 設定チェック
        bgmSlider.value = 5;
        seSlider.value = 0;
        yield return wait;
        Assert.That(bgmVolume.Load(3), Is.EqualTo(5), "BGMスライダーでボリューム変更");
        Assert.That(seVolume.Load(3), Is.EqualTo(0), "SEスライダーでボリューム変更");
    }
}
