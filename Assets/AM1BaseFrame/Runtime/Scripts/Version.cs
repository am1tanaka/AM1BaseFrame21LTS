using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace AM1.BaseFrame
{
    public class Version : MonoBehaviour
    {
        void Awake()
        {
            GetComponent<TextMeshProUGUI>().text = $"Ver {Application.version}";
            Destroy(this);
        }
    }
}