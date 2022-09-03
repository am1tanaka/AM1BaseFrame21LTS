using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AM1.BaseFrame.Editor
{
    public class AM1BaseFrameMenu : EditorWindow
    {
        [MenuItem("Tools/AM1/New BaseFrame Scene", false, 2)]
        static void NewBaseFrameScene()
        {

        }

        [MenuItem("Tools/AM1/Import BaseFrame Assets", false, 20)]
        static bool ImportBaseFrameAssets()
        {
            return true;
        }


    }
}