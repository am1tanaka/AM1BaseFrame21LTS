using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.BaseFrame;
using AM1.BaseFrame.General;

public class AudioSettingSaverTests
{
    [Test]
    public void AudioSettingSaverTestsSimplePasses()
    {
        BGMVolumeSaverWithPlayerPrefs.prefix = "test";
        var saver = new BGMVolumeSaverWithPlayerPrefs();
        saver.ClearSaveData();
        Assert.That(saver.Load(5), Is.EqualTo(5), "BGM=5");
        saver.Save(3);
        Assert.That(saver.Load(5), Is.EqualTo(3), "BGM=3");
        saver.ClearSaveData();
        Assert.That(saver.Load(5), Is.EqualTo(5), "BGM=5 after clear");

        SEVolumeSaverWithPlayerPrefs.prefix = "test";
        var sesaver = new SEVolumeSaverWithPlayerPrefs();
        sesaver.ClearSaveData();
        Assert.That(sesaver.Load(5), Is.EqualTo(5), "SE=5");
        sesaver.Save(3);
        Assert.That(sesaver.Load(5), Is.EqualTo(3), "SE=3");
        sesaver.ClearSaveData();
        Assert.That(sesaver.Load(5), Is.EqualTo(5), "SE=5 after clear");

    }

}
