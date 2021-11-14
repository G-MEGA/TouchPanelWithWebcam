using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSettingWindow : MonoBehaviour
{
    CamInput camInput;
    int index;

    public void Init(int customColorIndex)
    {
        index = customColorIndex;
    }
    public void CamInputChange(CamInput ci)
    {
        camInput = ci;
        if (index < camInput.customColors.Count)
        {

        }
    }
}
