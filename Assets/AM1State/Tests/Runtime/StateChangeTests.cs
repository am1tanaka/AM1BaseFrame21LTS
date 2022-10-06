using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.State;
using System.Diagnostics.Tracing;
using UnityEditor.VersionControl;
using System.Linq;

public class StateChangeTests
{
    [UnityTest]
    public IEnumerator PopTests()
    {
        // Popのデータ作成
        var go = new GameObject();
        go.AddComponent<AM1StateStack>();
        var stateStack = go.GetComponent<AM1StateStack>();
        StateTestBench[] bench = new StateTestBench[6];
        for (int i = 0; i < bench.Length; i++)
        {
            bench[i] = new StateTestBench();
        }
        yield return SetStackFull(stateStack, bench);

        // 通常の要求
        // 1つ戻す
        Debug.Log($"--1つ戻す");
        yield return null;
        stateStack.PopRequest();
        yield return WaitChangeDone(stateStack);
        Assert.That(stateStack.CurrentStateInfo, Is.EqualTo(bench[4]), "通常 1つ戻す");

        // 2つ戻す
        Debug.Log($"--2つ戻す");
        yield return null;
        Assert.That(stateStack.PopRequest(bench[2]), Is.True, "2つ戻す要求");
        Assert.That(stateStack.IsBusy, Is.True, "切り替え要求発動");
        yield return WaitChangeDone(stateStack);
        Assert.That(stateStack.CurrentStateInfo, Is.EqualTo(bench[2]), "通常 2つ戻す");

        // ルートまで戻す
        stateStack.PopToRootRequest();
        yield return WaitChangeDone(stateStack);
        Assert.That(stateStack.CurrentStateInfo, Is.EqualTo(bench[0]), "通常 ルートまで戻す");

        // 全部戻す
        yield return SetStackFull(stateStack, bench);
        Assert.That(stateStack.stateStack.Count, Is.EqualTo(bench.Length), "スタック満タン");
        stateStack.PopAllRequest();
        yield return WaitChangeDone(stateStack);
        Assert.That(stateStack.CurrentStateInfo, Is.Null, "通常 全て戻す");
        Assert.That(stateStack.stateStack.Count, Is.Zero, "通常 スタックなし");

        // キューに積む
        // 1つ戻す

        // 2つ戻す

        // ルートまで戻す

        // 全部戻す


    }

    /// <summary>
    /// 与えられたスタックに、与えられた状態を全て積み上げます。
    /// </summary>
    /// <param name="stateStack"></param>
    /// <param name="bench"></param>
    /// <returns></returns>
    IEnumerator SetStackFull(AM1StateStack stateStack, StateTestBench[] bench)
    {
        for (int i = stateStack.stateStack.Count; i < bench.Length; i++)
        {
            stateStack.PushQueueRequest(bench[i]);
        }

        // ４つ登録が終わるまで待つ
        while (stateStack.requestQueue.Count > 0)
        {
            yield return WaitChangeDone(stateStack);
        }
    }

    IEnumerator WaitChangeDone(AM1StateStack stack)
    {
        while (stack.IsBusy)
        {
            if (stack.CurrentStateInfo != null)
            {
                StateTestBench stb = (stack.CurrentStateInfo as StateTestBench);
                stb.canChange = true;
            }
            yield return null;
        }
    }

    [UnityTest]
    public IEnumerator PopAndPushTests()
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
            stateStack.PopAndPushQueueRequest(bench[i]);
        }
        Assert.That(stateStack.PopAndPushRequest(bench[0]), Is.False, "予約ずみのため失敗");
        WaitForFixedUpdate wait = new();

        for (int i = 0; i < bench.Length - 2; i++)
        {
            Debug.Log($"Loop {i} requestQueueCount={stateStack.requestQueue.Count}");
            // 実行を確認
            yield return null;  // Init実行+canChange=false
            Assert.That(bench[i].initCount, Is.GreaterThan(0), $"初期化確認 {i}");
            Assert.That(bench[i].updateCount, Is.GreaterThan(0), $"Update {i}");
            yield return wait;
            Assert.That(bench[i].fixedUpdateCount, Is.GreaterThan(0), $"FixedUpdate {i}");

            // 次は開始していない
            Assert.That(bench[i + 1].initCount, Is.Zero, $"次の初期化は未実行");
            Assert.That(bench[i + 1].terminateCount, Is.Zero, $"まだTerminateCount {i}");

            // 初期化
            bench[i].canChange = true;  // InitのcanChangeを解除
            yield return null;  // Pop
            yield return wait;
            Assert.That(bench[i].terminateCount, Is.GreaterThan(0), $"TerminateCount {i}");

            Assert.That(bench[i + 1].initCount, Is.Zero, $"次の初期化はまだ {i + 1}");
            bench[i].canChange = true;  // TerminateのcanChangeを解除
            yield return null;  // Pop
            Assert.That(bench[i + 1].initCount, Is.EqualTo(1), $"次の初期化確認 {i + 1}");
            Assert.That(bench[i + 2].initCount, Is.Zero, $"2つ先の初期化は未確認 {i + 2}");
        }

        int index = AM1StateStack.DefaultStackMax - 2;
        bench[index].canChange = true;
        yield return null;
        yield return wait;
        Assert.That(bench[index].initCount, Is.GreaterThan(0), $"初期化確認 {index}");
        bench[index].canChange = true;
        index++;
        yield return null;
        yield return wait;
        bench[index].canChange = true;
        yield return null;
        Assert.That(bench[index].initCount, Is.GreaterThan(0), $"初期化確認  {index}");
        yield return null;
        Assert.That(bench[index].updateCount, Is.GreaterThan(0), $"Update  {index}");
        yield return wait;
        Assert.That(bench[index].fixedUpdateCount, Is.GreaterThan(0), $"FixedUpdate  {index}");
    }

    /// <summary>
    /// PushとPopをそれぞれテスト
    /// </summary>
    [UnityTest]
    public IEnumerator PushAndPopTests()
    {
        var go = new GameObject();
        go.AddComponent<AM1StateStack>();
        var stateStack = go.GetComponent<AM1StateStack>();
        WaitForFixedUpdate wait = new();
        StateTestBench[] bench = new StateTestBench[4];
        for (int i = 0; i < bench.Length; i++)
        {
            bench[i] = new StateTestBench();
        }

        // Push
        Assert.That(stateStack.PushRequest(bench[0]), Is.True, "予約成功");
        Assert.That(stateStack.PushRequest(bench[1]), Is.False, "登録ありの予約失敗");
        yield return null;
        yield return wait;
        Assert.That(stateStack.PushRequest(bench[1]), Is.False, "切り替え不可なので登録失敗");
        bench[0].canChange = true;
        Assert.That(stateStack.PushRequest(bench[1]), Is.True, "切り替えを許可したので登録成功");
        yield return null;
        yield return wait;
        Assert.That(bench[0].pauseCount, Is.GreaterThan(0), $"一時停止 1");
        Assert.That(bench[0].resumeCount, Is.Zero, $"復帰はまだ 0");

        bench[0].canChange = true;
        yield return null;
        Assert.That(bench[1].initCount, Is.GreaterThan(0), $"初期化確認 1");
        Assert.That(bench[1].updateCount, Is.GreaterThan(0), $"Update 1");
        yield return wait;
        Assert.That(bench[1].fixedUpdateCount, Is.GreaterThan(0), $"FixedUpdate 1");
        Assert.That(bench[1].pauseCount, Is.Zero, $"一時停止まだ 0");
        Assert.That(bench[1].resumeCount, Is.Zero, $"復帰はまだ 0");

        // PushQueueRequest
        for (int i = 2; i < bench.Length; i++)
        {
            stateStack.PushQueueRequest(bench[i]);
        }

        // 実行を確認
        Assert.That(bench[2].initCount, Is.Zero, $"未初期化 {2}");
        bench[1].canChange = true;
        yield return null;
        bench[1].canChange = true;
        yield return null;
        Assert.That(bench[2].initCount, Is.GreaterThan(0), $"初期化確認 {2}");
        Assert.That(bench[2].updateCount, Is.GreaterThan(0), $"Update {2}");
        yield return wait;
        Assert.That(bench[2].fixedUpdateCount, Is.GreaterThan(0), $"FixedUpdate {2}");
        Assert.That(bench[2].pauseCount, Is.Zero, $"Pauseまだ {2}");
        Assert.That(bench[2].resumeCount, Is.Zero, $"Resumeまだ {2}");
        Assert.That(bench[1].pauseCount, Is.GreaterThan(0), $"Pause実行 {1}");
        Assert.That(bench[1].resumeCount, Is.Zero, $"Resumeまだ {1}");

        // 次は開始していない
        Assert.That(bench[3].initCount, Is.Zero, $"次の初期化は未実行");

        // 初期化
        bench[2].canChange = true;
        yield return null;
        bench[2].canChange = true;
        yield return null;

        // 最後
        Assert.That(bench[3].initCount, Is.GreaterThan(0), $"InitでCanChangeがtrue {3}");
        Assert.That(bench[3].updateCount, Is.GreaterThan(0), $"Update ラスト");
        yield return wait;
        Assert.That(bench[3].fixedUpdateCount, Is.GreaterThan(0), $"FixedUpdate ラスト");

        // 既存の状態はプッシュ不可
        Assert.That(stateStack.PushRequest(bench[0]), Is.False, "既存の状態はプッシュ不可");

        //////////
        // Pop
        Assert.That(stateStack.requestQueue.Count, Is.Zero, $"Pushリクエスト消化");
        Assert.That(stateStack.stateStack.Count, Is.EqualTo(bench.Length), $"Stackが{bench.Length}つ");

        // まだ現在の状態を変更下にしてない
        Assert.That(stateStack.PopRequest(), Is.False, "最後の状態の変更を不許可なのでPop失敗");
        bench[3].canChange = true;
        Assert.That(stateStack.PopRequest(), Is.True, "一手戻す");
        yield return null;
        yield return wait;
        Assert.That(stateStack.PopRequest(), Is.False, "最後の状態の変更を不許可なのでPop失敗");
        bench[3].canChange = true;
        yield return wait;
        Assert.That(stateStack.CurrentStateInfo, Is.EqualTo(bench[2]), "一手戻っている");
        Assert.That(bench[2].resumeCount, Is.EqualTo(1), "Resume 2");
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
        yield return null;
        bench.canChange = true;
        yield return null;
        Assert.That(bench2.initCount, Is.EqualTo(1), $"初期化を1回実行");
        Assert.That(bench2.updateCount, Is.GreaterThan(0), $"Updateを{bench2.updateCount}回実行");
        yield return new WaitForFixedUpdate();
        Assert.That(bench2.fixedUpdateCount, Is.GreaterThan(0), $"FixedUpdateを{bench2.fixedUpdateCount}回実行");
        Assert.That(bench2.terminateCount, Is.EqualTo(0), $"Terminateはまだ呼ばれていない。");
    }
}
