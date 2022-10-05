using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.State;
using System.Diagnostics.Tracing;

public class StateChangeTests
{
    [UnityTest]
    public IEnumerator ChangeStateReserveTests()
    {
        var go = new GameObject();
        go.AddComponent<AM1StateStack>();
        var stateStack = go.GetComponent<AM1StateStack>();
        StateTestBench[] bench = new StateTestBench[AM1StateStack.DefaultStackMax];
        for (int i = 0; i < bench.Length; i++)
        {
            bench[i] = new StateTestBench();
        }

        Assert.That(stateStack.PopAndPushRequest(bench[0]), Is.True, $"予約成功確認");
        Assert.That(stateStack.PopAndPushRequest(bench[1]), Is.False, $"Busyにより予約失敗");

        for (int i = 1; i < bench.Length; i++)
        {
            Assert.That(stateStack.PopAndPushQueueRequest(bench[i]), Is.True, $"予約キュー成功確認 {i}");
        }
        Assert.That(stateStack.PopAndPushRequest(bench[0]), Is.False, "予約ずみのため失敗");
        Assert.That(stateStack.PopAndPushQueueRequest(bench[0]), Is.False, "予約ずみのため失敗(キュー)");
        WaitForFixedUpdate wait = new();

        for (int i = 0; i < bench.Length - 2; i++)
        {
            // 実行を確認
            yield return null;
            Assert.That(bench[i].initCount, Is.GreaterThan(0), $"初期化確認 {i}");
            Assert.That(bench[i].updateCount, Is.GreaterThan(0), $"Update {i}");
            yield return wait;
            Assert.That(bench[i].fixedUpdateCount, Is.GreaterThan(0), $"FixedUpdate {i}");

            // 次は開始していない
            Assert.That(bench[i + 1].initCount, Is.EqualTo(0), $"次の初期化は未実行");

            // 初期化
            bench[i].canChange = true;
            yield return null;
            yield return wait;
            Assert.That(bench[i + 1].initCount, Is.EqualTo(1), $"次の初期化確認 {i + 1}");
            Assert.That(bench[i + 2].initCount, Is.EqualTo(0), $"2つ先の初期化は未確認 {i + 2}");
        }

        int index = AM1StateStack.DefaultStackMax - 2;
        bench[index].canChange = true;
        yield return null;
        yield return wait;
        Assert.That(bench[index].initCount, Is.GreaterThan(0), $"初期化確認 ラスト");
        index++;
        bench[index].canChange = true;
        yield return null;
        yield return wait;
        Assert.That(bench[index].initCount, Is.GreaterThan(0), $"初期化確認 ラスト");
        yield return null;
        Assert.That(bench[index].updateCount, Is.GreaterThan(0), $"Update ラスト");
        yield return wait;
        Assert.That(bench[index].fixedUpdateCount, Is.GreaterThan(0), $"FixedUpdate ラスト");
    }

    [UnityTest]
    public IEnumerator PushAndPopReserveTests()
    {
        var go = new GameObject();
        go.AddComponent<AM1StateStack>();
        var phaseManager = go.GetComponent<AM1StateStack>();
        StateTestBench[] bench = new StateTestBench[AM1StateStack.DefaultStackMax];
        for (int i = 0; i < bench.Length; i++)
        {
            bench[i] = new StateTestBench();
        }

        // Push
        for (int i = 0; i < bench.Length; i++)
        {
            Assert.That(phaseManager.PushQueueRequest(bench[i]), Is.True, "予約成功");
        }
        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        for (int i = 0; i < bench.Length - 1; i++)
        {
            // 実行を確認
            yield return null;
            Assert.That(bench[i].initCount, Is.GreaterThan(0), $"初期化確認 {i}");
            Assert.That(bench[i].updateCount, Is.GreaterThan(0), $"Update {i}");
            yield return wait;
            Assert.That(bench[i].fixedUpdateCount, Is.GreaterThan(0), $"FixedUpdate {i}");

            // 次は開始していない
            Assert.That(bench[i + 1].initCount, Is.Zero, $"次の初期化は未実行");

            // 初期化
            bench[i].canChange = true;
            yield return null;
            yield return wait;
            Assert.That(bench[i + 1].initCount, Is.GreaterThan(0), $"InitでCanChangeがtrue {i}");

            // ポーズを完了させたので、次は初期化完了している
            yield return null;
            yield return wait;
        }

        int index = AM1StateStack.DefaultStackMax - 1;
        bench[index].canChange = true;
        yield return null;
        Assert.That(bench[index].initCount, Is.GreaterThan(0), $"初期化確認 ラスト");
        yield return null;
        Assert.That(bench[index].updateCount, Is.GreaterThan(0), $"Update ラスト");
        yield return wait;
        Assert.That(bench[index].fixedUpdateCount, Is.GreaterThan(0), $"FixedUpdate ラスト");

        // Pop
        Assert.That(phaseManager.requestQueue.Count, Is.Zero, $"Pushリクエスト消化");
        Assert.That(phaseManager.stateStack.Count, Is.EqualTo(AM1StateStack.DefaultStackMax), $"Stackが{AM1StateStack.DefaultStackMax}つ");

        for (int i = 0; i < bench.Length - 1; i++)
        {
            Assert.That(phaseManager.PopQueueRequest(), Is.True, "予約成功");
        }

        /* todo
        for (int i = 0; i < bench.Length - 2; i++)
        {
            // 実行を確認
            yield return null;
            var currentPhase = phaseManager.CurrentPhaseInfo.phase as PhaseTestBench;
            Assert.That(currentPhase.pauseCount, Is.GreaterThan(0), $"Pause確認 {i}");
            Assert.That(currentPhase.resumeCount, Is.GreaterThan(0), $"Resume確認 {i}");
            Assert.That(currentPhase.updateCount, Is.GreaterThan(0), $"Update {i}");
            yield return wait;
            Assert.That(currentPhase.fixedUpdateCount, Is.GreaterThan(0), $"FixedUpdate {i}");

            // 次は開始していない
            Assert.That(bench[i + 1].initCount, Is.Zero, $"次の初期化は未実行");

            // 初期化
            bench[i].canChange = true;
            yield return null;
            yield return wait;
            Assert.That(bench[i + 1].initCount, Is.Zero, $"CanChangeを設定したがまだ終了していない {i}");

            // 終了させたので、次は初期化完了している
            bench[i].isTerminated = true;
            yield return null;
            yield return wait;
        }
        */
    }

    [UnityTest]
    public IEnumerator StateChangeTestsWithEnumeratorPasses()
    {
        var go = new GameObject();
        go.AddComponent<AM1StateStack>();
        var phaseManager = go.GetComponent<AM1StateStack>();
        var bench = new StateTestBench();
        var bench2 = new StateTestBench();

        Assert.That(phaseManager.PopAndPushRequest(bench), Is.True, "切り替え要求");
        yield return null;
        yield return new WaitForFixedUpdate();
        Assert.That(bench.initCount, Is.EqualTo(1), $"初期化を1回実行");
        Assert.That(bench.updateCount, Is.GreaterThan(0), $"Updateを{bench.updateCount}回実行");
        Assert.That(bench.fixedUpdateCount, Is.GreaterThan(0), $"FixedUpdateを{bench.fixedUpdateCount}回実行");
        Assert.That(bench.terminateCount, Is.EqualTo(0), $"Terminateはまだ呼ばれていない。");

        // 切り替えチェック
        Assert.That(phaseManager.PopAndPushRequest(bench2), Is.False, "切り替えフラグがオフなので切り替え不可");
        bench.canChange = true;
        Assert.That(phaseManager.PopAndPushRequest(bench2), Is.True, "bench2へ切り替え登録");
        // 終了
        bench2.canChange = true;
        yield return null;
        Assert.That(bench2.initCount, Is.EqualTo(1), $"初期化を1回実行");
        Assert.That(bench2.updateCount, Is.GreaterThan(0), $"Updateを{bench2.updateCount}回実行");
        yield return new WaitForFixedUpdate();
        Assert.That(bench2.fixedUpdateCount, Is.GreaterThan(0), $"FixedUpdateを{bench2.fixedUpdateCount}回実行");
        Assert.That(bench2.terminateCount, Is.EqualTo(0), $"Terminateはまだ呼ばれていない。");


    }
}
