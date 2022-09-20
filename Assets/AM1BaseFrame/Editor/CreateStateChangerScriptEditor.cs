using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateStateChangerScriptEditor
{
    [MenuItem("Assets/Create/AM1/Create StateChanger Script")]
    static void CreateStateChangerScript(MenuCommand menuCommand)
    {
        Debug.Log($"{(menuCommand.context != null ? menuCommand.context.ToString() : "null")}");
   }
}
