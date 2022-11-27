using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.State;

/// <summary>
/// キューのテスト
/// </summary>
public class QueueTests 
{
    TestQueue[,] testQueues;
    AM1StateQueue stateQueue;

    /// <summary>
    /// テスト用にシーンにオブジェクトを配置
    /// </summary>
    void Standby(int pri, int cnt)
    {
        var go = new GameObject();
        go.AddComponent<AM1StateQueue>();
        InstantiateQueues(pri, cnt);

        stateQueue = go.GetComponent<AM1StateQueue>();
    }

    /// <summary>
    /// 作成する属性の範囲と数
    /// </summary>
    /// <param name="priority"></param>
    /// <param name="count"></param>
    void InstantiateQueues(int priority, int count)
    {
        testQueues = new TestQueue[priority, count];
        int index = 0;
        for (int i = priority-1; i >=0 ; i--)
        {
            for (int j = 0; j < count; j++, index++)
            {
                testQueues[i, j] = new TestQueue(i, index);
            }
        }
    }

    /// <summary>
    /// 登録テスト
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator EnqueueTests()
    {
        Standby(3, 3);

        // 登録テスト
        Enqueue3x3();
        AssertIndex(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });

        // 変更不可の状態で切り替えがされない
        yield return null;
        AssertIndex(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });
        yield return null;
        yield return null;
        AssertIndex(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });

        // 一つ進める
        testQueues[2, 0].CanChange = true;
        yield return null;
        yield return null;
        AssertIndex(new int[] { 2, 3, 4, 5, 6, 7, 8 });

        // 3つ進める
        testQueues[2, 1].CanChange = true;
        testQueues[2, 2].CanChange = true;
        testQueues[1, 0].CanChange = true;
        yield return null;
        AssertIndex(new int[] { 5, 6, 7, 8 });

        // 全て切り替え
        testQueues[1, 1].CanChange = true;
        testQueues[1, 2].CanChange = true;
        testQueues[0, 0].CanChange = true;
        testQueues[0, 1].CanChange = true;
        testQueues[0, 2].CanChange = true;
        yield return null;
        Assert.That(stateQueue.stateQueue.Count, Is.Zero, "残りなし");
        Assert.That((stateQueue.CurrentState as TestQueue).Index, Is.EqualTo(8), "最後の状態");
    }

    [UnityTest]
    public IEnumerator CancelTests()
    {
        Standby(3, 3);
        Enqueue3x3();
        AssertIndex(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });
        yield return null;
        AssertIndex(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });
        stateQueue.Cancel(1);
        AssertIndex(new int[] { 1, 2 });
        stateQueue.Cancel(2);
        Assert.That(stateQueue.stateQueue.Count, Is.Zero, "全てキャンセル");
    }

    void Enqueue3x3()
    {
        // 優先度を逆順に3回登録
        for (int i = 0; i < 3; i++)
        {
            for (int j = 2; j >= 0; j--)
            {
                stateQueue.Enqueue(testQueues[j, i]);
            }
        }
    }

    void AssertIndex(int[] indices)
    {
        // 優先度順に並んでいるはず
        int index = 0;
        foreach (var current in stateQueue.stateQueue)
        {
            var testInst = current as TestQueue;
            Assert.That(testInst.Index, Is.EqualTo(indices[index]), $"{index}個目");
            index++;
        }
    }
}
