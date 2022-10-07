using AM1.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestState2 : TestState
{
    public TestState2(AM1StateStack ins) : base (ins) { }

    public override void OnGUI()
    {
        GUI.color = Color.green;
        if (IsRunning)
        {
            GUI.Label(new Rect(20, 50, 400, 20), $"TestState2 Running {Counter}");
        }
        else if (IsPausing)
        {
            GUI.Label(new Rect(20, 50, 400, 20), $"TestState2 Pausing {Counter}");
        }
    }
}
