using AM1.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestState1 : TestState
{
    public TestState1(AM1StateStack ins) : base(ins) { }

    public override void OnGUI()
    {
        GUI.color = Color.red;
        if (IsRunning)
        {
            GUI.Label(new Rect(20, 20, 400, 20), $"TestState1 Running {Counter}");
        }
        else if (IsPausing)
        {
            GUI.Label(new Rect(20, 20, 400, 20), $"TestState1 Pausing {Counter}");
        }
    }
}
