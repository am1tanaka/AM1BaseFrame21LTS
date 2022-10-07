using AM1.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestState3 : TestState
{
    public TestState3(AM1StateStack ins) : base(ins) { }

    public override void OnGUI()
    {
        GUI.color = Color.yellow;
        if (IsRunning)
        {
            GUI.Label(new Rect(20, 80, 400, 20), $"TestState3 Running {Counter}");
        }
        else if (IsPausing)
        {
            GUI.Label(new Rect(20, 80, 400, 20), $"TestState3 Pausing {Counter}");
        }
    }
}
