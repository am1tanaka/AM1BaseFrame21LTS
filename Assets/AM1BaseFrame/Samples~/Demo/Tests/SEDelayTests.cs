using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using AM1.BaseFrame;
using AM1.BaseFrame.Demo;
using AM1.BaseFrame.Assets;

public class SEDelayTests : AM1BaseFrameTestBase
{
    [UnityTest]
    public IEnumerator SEDelayTest()
    {
        VolumeSaverWithPlayerPrefsBase.prefix = "test";
        var seVolume = new SEVolumeSaverWithPlayerPrefs();
        seVolume.Save(3);
        yield return BootTitle();

        // SE
        SEPlayer.Play(SEPlayer.SE.Start);
        SEPlayer.Play(SEPlayer.SE.Start);
        SEPlayer.Play(SEPlayer.SE.Start);
        SEPlayer.Play(SEPlayer.SE.Start);
        SEPlayer.Play(SEPlayer.SE.Start);

        yield return new WaitForSeconds(0.02f);
        SEPlayer.Play(SEPlayer.SE.Click);
        SEPlayer.Play(SEPlayer.SE.Click);
        SEPlayer.Play(SEPlayer.SE.Click);
        SEPlayer.Play(SEPlayer.SE.Click);
        SEPlayer.Play(SEPlayer.SE.Click);

        int dalayCount = 8;
        var inst = SESourceAndClips.Instance.DelaySEPlayerInstance;
        Assert.That(SESourceAndClips.Instance.DelaySEPlayerInstance.PlayDataList.Count, Is.EqualTo(dalayCount), $"{dalayCount}つ遅延");

        for (int i = dalayCount; i >= 1; i--)
        {
            yield return new WaitWhile(() => inst.PlayDataList.Count == i);
            Assert.That(inst.PlayDataList.Count, Is.EqualTo(i - 1), $"残り{i - 1}つ");
        }

        yield return new WaitForSeconds(SEPlayer.DelaySeconds * 2);
        SEPlayer.Play(SEPlayer.SE.Start);
        SEPlayer.Play(SEPlayer.SE.Click);
        Assert.That(inst.PlayDataList.Count, Is.EqualTo(0), $"残り0");

        yield return new WaitForSeconds(1);
    }
}
