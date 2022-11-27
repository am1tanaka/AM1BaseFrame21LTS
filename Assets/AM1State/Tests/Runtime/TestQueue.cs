using AM1.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// テスト用のキュー
/// </summary>
public class TestQueue : AM1StateQueueBase
{
    /// <summary>
    /// テスト用に次の状態への切り替えを外部から変更できるように設定
    /// </summary>
    public bool CanChange;
    public override bool CanChangeToOtherState => CanChange;

    public int Index { get; private set; }

    public TestQueue(int priority, int index) : base(priority) { 
        Index = index;
    }
}
