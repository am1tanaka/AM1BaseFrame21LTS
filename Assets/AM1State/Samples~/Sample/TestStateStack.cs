using AM1.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStateStack : AM1StateStack
{
    TestState[] states = new TestState[3];

    private void Awake()
    {
        states[0] = new TestState1(this);
        states[1] = new TestState2(this);
        states[2] = new TestState3(this);
        states[0].SetNext(states[1]);
        states[1].SetNext(states[2]);
        states[2].SetNext(null);
    }

    protected override void Update()
    {
        base.Update();

        if ((CurrentState == null) && Input.GetKeyDown(KeyCode.RightArrow)) {
            this.PushRequest(states[0]);
        }
    }

    private void OnGUI()
    {
        GUI.color = Color.black;
        GUI.Label(new Rect(20, 200, 400, 20), "Left and Right Arrow Key to change state.");

        foreach(var state in states)
        {
            state.OnGUI();
        }
    }
}
