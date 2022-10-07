using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.State;

public class StateTestBench : AM1StateBase
{
    /// <summary>
    /// チェック用
    /// </summary>
    public int index;

    /// <summary>
    /// 初期化、終了、一時停止、再開時にCanChangeToOtherStateに設定する値。
    /// まとめて戻す時はフリーズしないようにtrueを設定。
    /// </summary>
    public bool debugCanChange = false;

    public int initCount;
    public int updateCount;
    public int fixedUpdateCount;
    public int terminateCount;
    public int pauseCount;
    public int resumeCount;

    public override bool CanChangeToOtherState => canChange;

    /// <summary>
    /// テスト用に公開。任意のタイミングでテストから設定する
    /// </summary>
    public bool canChange;

    public void ClearCount()
    {
        initCount = 0;
        updateCount = 0;
        fixedUpdateCount = 0;
        terminateCount = 0;
        pauseCount = 0;
        resumeCount = 0;
    }

    public override void Init()
    {
        initCount++;
        canChange = debugCanChange;
        Debug.Log($"{index}:Init {initCount}");
    }

    public override void Update()
    {
        updateCount++;
    }

    public override void FixedUpdate()
    {
        fixedUpdateCount++;
    }

    public override void Terminate()
    {
        terminateCount++;
        canChange = debugCanChange;
        Debug.Log($"{index}:Terminate {terminateCount}");
    }

    public override void Pause()
    {
        pauseCount++;
        canChange = debugCanChange;
        Debug.Log($"{index}:Pause {pauseCount}");
    }

    public override void Resume()
    {
        resumeCount++;
        canChange = debugCanChange;
        Debug.Log($"{index}:Resume {resumeCount}");
    }


}
