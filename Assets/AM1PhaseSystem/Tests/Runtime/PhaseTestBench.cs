using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.PhaseSystem;

public class PhaseTestBench : PhaseBase
{
    public int initCount;
    public int updateCount;
    public int fixedUpdateCount;
    public int terminateCount;
    public int pauseCount;
    public int resumeCount;

    public override bool CanChange => canChange;
    public override bool IsTerminated => isTerminated;

    /// <summary>
    /// テスト用に公開。任意のタイミングでテストから設定する
    /// </summary>
    public bool canChange;

    /// <summary>
    /// テスト用に公開。任意のタイミングでテストから設定する
    /// </summary>
    public bool isTerminated;

    /// <summary>
    /// テスト用に公開。
    /// </summary>
    public bool isPaused;

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
        canChange = false;
        isTerminated = false;
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
    }

    public override void Pause()
    {
        pauseCount++;
    }

    public override void Resume()
    {
        resumeCount++;
    }


}
