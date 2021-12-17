using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TouchSettingScreenCtrl : MonoBehaviour
{
    [SerializeField]
    Slider resolutionX;
    [SerializeField]
    Slider resolutionY;
    [SerializeField]
    InputField markingLength;
    [SerializeField]
    Toggle removeGhostWhen2Cam;
    [SerializeField]
    Dropdown touchDisplayMode;
    [SerializeField]
    InputField minTouchArea;
    [SerializeField]
    TouchDisplay touchDisplay;
    [SerializeField]
    InputField targetFPS;

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
        removeGhostWhen2Cam.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(RemoveGhostWhen2CamChange));
        removeGhostWhen2Cam.isOn = manager.removeGhostWhen2Cam;
        touchDisplayMode.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(TouchDisplayModeChange));
        touchDisplayMode.value = (int)touchDisplay.mode;
        minTouchArea.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(MinTouchAreaChange));
        minTouchArea.text = manager.MinTouchArea.ToString();
        targetFPS.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(TargetFPSChange));
        targetFPS.text = Application.targetFrameRate.ToString();
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
    public void RemoveGhostWhen2CamChange(bool value)
    {
        manager.removeGhostWhen2Cam = value;
        removeGhostWhen2Cam.isOn = manager.removeGhostWhen2Cam;
    }
    public void TouchDisplayModeChange(int value)
    {
        touchDisplay.mode = (TouchDisplayMode)value;
        touchDisplayMode.value = (int)touchDisplay.mode;
    }
    public void MinTouchAreaChange(string i)
    {
        manager.MinTouchArea = int.Parse(i);
        minTouchArea.text = manager.MinTouchArea.ToString();
    }

    public void TargetFPSChange(string i)
    {
        Application.targetFrameRate = int.Parse(i);
        targetFPS.text = Application.targetFrameRate.ToString();
    }

    public void AllCamInputsBaseColorsUpdate()
    {
        int length = manager.camInputs.Length;
        for (int i = 0; i < length; i++)
            manager.camInputs[i].BaseColorsUpdate();
    }
    public void Return2Main()
    {
        SceneManager.LoadScene(0);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            AllCamInputsBaseColorsUpdate();
    }
}
