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

        // 1,2,3キー - 現在の状態から押したキーの状態に切り替え
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PopAndPushRequest(states[0]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PopAndPushRequest(states[1]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PopAndPushRequest(states[2]);
        }

        // Q,W,Eキー - 現在の状態を中断して、対応する状態(Q = TestState1, W = TestState2, E = TestState3)に切り替える
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PushRequest(states[0]);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            PushRequest(states[1]);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            PushRequest(states[2]);
        }

        // A,S,Dキー - 押したキーがスタックにあったら、その状態(A = TestState1, S = TestState2, D = TestState3)まで戻す
        if (Input.GetKeyDown(KeyCode.A))
        {
            PopRequest(states[0]);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            PopRequest(states[1]);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            PopRequest(states[2]);
        }

        // Rキー - 最初の状態まで戻します
        if (Input.GetKeyDown(KeyCode.R))
        {
            PopToRootRequest();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            // 0キー - 全ての状態を戻して状態なしにします
            PopAllRequest();
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
