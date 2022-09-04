using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AM1.BaseFrame.General.Editor;

namespace AM1.BaseFrame.Editor
{
    public class AM1BaseFrameMenu : EditorWindow
    {
        [MenuItem("Tools/AM1/Import BaseFrame Assets", false, 20)]
        static bool ImportBaseFrameAssets()
        {
            string baseFramePackagePath = AM1BaseFrameUtils.packageFullPath + "/Package Resources/BaseFrame.unitypackage";
            AssetDatabase.ImportPackage(baseFramePackagePath, true);

            return true;
        }


    }
}