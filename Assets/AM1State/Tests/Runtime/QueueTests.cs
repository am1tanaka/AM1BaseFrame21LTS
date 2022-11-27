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
        for (int i = 0; i < priority; i++)
        {
            for (int j = 0; j < count; j++)
            {
                testQueues[i, j] = new TestQueue(i, i * count + j);
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
        AssertPriority(new int[] { 2, 2, 2, 1, 1, 1, 0, 0, 0 });

        // 変更不可の状態で切り替えがされない
        yield return null;
        AssertPriority(new int[] { 2, 2, 1, 1, 1, 0, 0, 0 });
        yield return null;
        yield return null;
        AssertPriority(new int[] { 2, 2, 1, 1, 1, 0, 0, 0 });

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

    void AssertPriority(int[] priorities)
    {
        // 優先度順に並んでいるはず
        int index = 0;
        foreach (var current in stateQueue.stateQueue)
        {
            Assert.That(current.Priority, Is.EqualTo(priorities[index]), $"{index}: 優先度{priorities[index]}回目");
            index++;
        }
    }
}
