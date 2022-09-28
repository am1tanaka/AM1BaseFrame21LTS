using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.PhaseSystem;

public class PhaseChangeTests
{
    [Test]
    public void PhaseChangeTestsSimplePasses()
    {
    }

    [UnityTest]
    public IEnumerator PhaseChangeTestsWithEnumeratorPasses()
    {
        var go = new GameObject();
        go.AddComponent<PhaseManager>();
        var phaseManager = go.GetComponent<PhaseManager>();
        var bench = new PhaseTestBench();
        var bench2 = new   PhaseTestBench();

        Assert.That(phaseManager.ChangeRequest(bench), Is.True, "切り替え要求");
        yield return null;
        yield return null;
        Assert.That(bench.initCount, Is.EqualTo(1), $"初期化を1回実行");
        Assert.That(bench.updateCount, Is.GreaterThan(0), $"Updateを{bench.updateCount}回実行");
        Assert.That(bench.fixedUpdateCount, Is.GreaterThan(0), $"FixedUpdateを{bench.fixedUpdateCount}回実行");
        Assert.That(bench.terminateCount, Is.EqualTo(0), $"Terminateはまだ呼ばれていない。");

        // 切り替えチェック
        Assert.That(phaseManager.ChangeRequest(bench2), Is.False, "切り替えフラグがオフなので切り替え不可");
        bench.canChange = true;
        Assert.That(phaseManager.ChangeRequest(bench2), Is.True, "bench2へ切り替え登録");
        yield return null;
        yield return null;
        Assert.That(bench2.initCount, Is.EqualTo(0), "前の処理を終了していないので初期化はまだ");
        // 終了
        bench.isTerminated = true;
        yield return null;
        Assert.That(bench2.initCount, Is.EqualTo(1), $"初期化を1回実行");
        Assert.That(bench2.updateCount, Is.GreaterThan(0), $"Updateを{bench2.updateCount}回実行");
        yield return null;
        Assert.That(bench2.fixedUpdateCount, Is.GreaterThan(0), $"FixedUpdateを{bench2.fixedUpdateCount}回実行");
        Assert.That(bench2.terminateCount, Is.EqualTo(0), $"Terminateはまだ呼ばれていない。");


    }
}
