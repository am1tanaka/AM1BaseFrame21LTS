using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.BaseFrame;

public class dBTests
{
    [Test]
    public void dBTestsSimplePasses()
    {
        Assert.That(BGMSourceAndClips.GetdB(1), Is.EqualTo(0).Within(0.01f), "v=1の時");
        Assert.That(BGMSourceAndClips.GetdB(0.75f), Is.InRange(-6f, 0), "v=0.75の時");
        Assert.That(BGMSourceAndClips.GetdB(0.5f), Is.EqualTo(-6).Within(0.1f), "v=0.5の時");
        Assert.That(BGMSourceAndClips.GetdB(0.25f), Is.EqualTo(-12).Within(0.1f), "v=0.25の時");
        Assert.That(BGMSourceAndClips.GetdB(0.125f), Is.EqualTo(-18).Within(0.1f), "v=0.125の時");
        Assert.That(BGMSourceAndClips.GetdB(0.011f), Is.LessThan(-38), "v=0.011の時");
        Assert.That(BGMSourceAndClips.GetdB(0), Is.LessThan(-79), "v=0の時");
    }
}
