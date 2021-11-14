using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchSettingScreenCtrl : MonoBehaviour
{
    [SerializeField]
    Slider resolutionX;
    [SerializeField]
    Slider resolutionY;
    [SerializeField]
    InputField markingLength;

    CamInputManager manager;
    
    private void Start()
    {
        manager = CamInputManager.Instance;
        resolutionX.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(ResolutionXChange));
        resolutionX.value = manager.Resolution.x;
        resolutionY.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(ResolutionYChange));
        resolutionY.value = manager.Resolution.y;
        markingLength.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(MarkingLengthChange));
        markingLength.text = manager.MarkingLength.ToString();
    }

    public void ResolutionXChange(float x)
    {
        manager.Resolution = new Vector2Int((int)x, manager.Resolution.y);
        resolutionX.value = manager.Resolution.x;
    }
    public void ResolutionYChange(float y)
    {
        manager.Resolution = new Vector2Int(manager.Resolution.x, (int)y);
        resolutionY.value = manager.Resolution.y;
    }
    public void MarkingLengthChange(string i)
    {
        manager.MarkingLength = int.Parse(i);
        markingLength.text = manager.MarkingLength.ToString();
    }

    public void AllCamInputsBaseColorsUpdate()
    {
        int length = manager.camInputs.Length;
        for (int i = 0; i < length; i++)
            manager.camInputs[i].BaseColorsUpdate();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            AllCamInputsBaseColorsUpdate();
    }
}
