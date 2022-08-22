using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.BaseFrame.General
{
    /// <summary>
    /// SEのボリュームをPlayerPrefsで読み書きする
    /// </summary>
    public class SEVolumeSaverWithPlayerPrefs : VolumeSaverWithPlayerPrefsBase
    {
        protected override string KeyName => $"{prefix}SEVol";
    }
}