using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.State;

public abstract class TestState : AM1StateBase
{
    AM1StateBase next;

    protected int Counter { get; private set; }
    protected bool IsPausing { get; private set; }

    AM1StateStack stateStack;

    public TestState(AM1StateStack ins)
    {
        stateStack = ins;
    }

    /// <summary>
    /// 次の状態を設定。
    /// </summary>
    /// <param name="nx">次の状態</param>
    public void SetNext(AM1StateBase nx)
    {
        next = nx;
    }

    public override void Init()
    {
        base.Init();
        Counter = 0;
    }

    public override void Update()
    {
        base.Update();

        if (IsRunning)
        {
            Counter++;

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (next != null)
                {
                    stateStack?.PushRequest(next);
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                stateStack?.PopRequest();
            }
        }
    }

    public override void Pause()
    {
        base.Pause();
        IsPausing = true;
    }

    public override void Resume()
    {
        base.Resume();
        IsPausing = false;
    }

    public abstract void OnGUI();
}
