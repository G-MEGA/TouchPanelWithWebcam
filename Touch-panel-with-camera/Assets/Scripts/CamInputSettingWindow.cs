using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CamInputSettingWindow : MonoBehaviour
{
    [SerializeField]
    CamInputWebcamDisplay webcamDisplay;
    [SerializeField]
    Text log;
    [SerializeField]
    Dropdown markingUpdateMethod;
    [SerializeField]
    Dropdown camInputDisplayMode;
    [SerializeField]
    Slider rDelta;
    [SerializeField]
    Slider gDelta;
    [SerializeField]
    Slider bDelta;
    [SerializeField]
    Slider hDelta;
    [SerializeField]
    Slider sDelta;
    [SerializeField]
    Slider vDelta;
    [SerializeField]
    GameObject colorSettingWindowPrefab;
    [SerializeField]
    RectTransform colorSettingWindowsParent;
    [SerializeField]
    InputField webcamResolutionX;
    [SerializeField]
    InputField webcamResolutionY;
    [SerializeField]
    InputField vertexArea;
    [SerializeField]
    InputField minGroupArea;
    [SerializeField]
    Toggle shadow;

    RectTransform webcamDisplayRectTransform;
    CamInput camInput;
    List<ColorSettingWindow> colorSettingWindows = new List<ColorSettingWindow>();

    private void Awake()
    {
        webcamDisplayRectTransform = webcamDisplay.GetComponent<RectTransform>();
        gameObject.SetActive(false);

        camInputDisplayMode.value = (int)webcamDisplay.CamInputDisplay.mode;
        camInputDisplayMode.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(CamInputDisplayModeChange));

        markingUpdateMethod.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(MethodChange));
        rDelta.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(RDeltaChange));
        gDelta.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(GDeltaChange));
        bDelta.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(BDeltaChange));
        hDelta.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(HDeltaChange));
        sDelta.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(SDeltaChange));
        vDelta.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(VDeltaChange));
        webcamResolutionX.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<string>(WebcamResolutionXChange));
        webcamResolutionY.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<string>(WebcamResolutionYChange));
        vertexArea.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<string>(VertexAreaChange));
        minGroupArea.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<string>(MinGroupAreaChange));
        shadow.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(ShadowChange));
    }
    public void StartSetting(int index)
    {
        camInput = CamInputManager.Instance.camInputs[index];

        ColorSettingWindowsUpdate();

        markingUpdateMethod.value = (int)camInput.markingsUpdateMethod;
        rDelta.gameObject.SetActive(camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareRGB);
        gDelta.gameObject.SetActive(camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareRGB);
        bDelta.gameObject.SetActive(camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareRGB);
        hDelta.gameObject.SetActive(camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareHSV || camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareH_SV);
        sDelta.gameObject.SetActive(camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareHSV || camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareH_SV);
        vDelta.gameObject.SetActive(camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareHSV);
        rDelta.value = camInput.allowedRedDeltaWithBaseColor;
        gDelta.value = camInput.allowedGreenDeltaWithBaseColor;
        bDelta.value = camInput.allowedBlueDeltaWithBaseColor;
        hDelta.value = camInput.allowedHueDeltaWithBaseColor;
        sDelta.value = camInput.allowedSaturationDeltaWithBaseColor;
        vDelta.value = camInput.allowedValueDeltaWithBaseColor;
        webcamResolutionX.text = camInput.Texture.requestedWidth.ToString();
        webcamResolutionY.text = camInput.Texture.requestedHeight.ToString();
        vertexArea.text = camInput.vertexArea.ToString();
        minGroupArea.text = camInput.minGroupArea.ToString();
        shadow.isOn = camInput.shadow;

        webcamDisplay.Init(index);
        webcamDisplay.ClickAndPointerEventData += OnClickWebcamDisplay;
        gameObject.SetActive(true);
    }
    public void FinishSetting()
    {
        webcamDisplay.ClickAndPointerEventData -= OnClickWebcamDisplay;
        gameObject.SetActive(false);
    }

    public void MethodChange(int method)
    {
        camInput.markingsUpdateMethod = (MarkingsUpdateMethod)method;
        markingUpdateMethod.value = (int)camInput.markingsUpdateMethod;
        rDelta.gameObject.SetActive(camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareRGB);
        gDelta.gameObject.SetActive(camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareRGB);
        bDelta.gameObject.SetActive(camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareRGB);
        hDelta.gameObject.SetActive(camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareHSV || camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareH_SV);
        sDelta.gameObject.SetActive(camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareHSV || camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareH_SV);
        vDelta.gameObject.SetActive(camInput.markingsUpdateMethod == MarkingsUpdateMethod.CompareHSV);
    }
    public void CamInputDisplayModeChange(int mode)
    {
        webcamDisplay.CamInputDisplay.mode = (CamInputDisplayMode)mode;
        camInputDisplayMode.value = (int)webcamDisplay.CamInputDisplay.mode;
    }
    public void RDeltaChange(float value)
    {
        camInput.allowedRedDeltaWithBaseColor = (int)value;
        rDelta.value = camInput.allowedRedDeltaWithBaseColor;
    }
    public void GDeltaChange(float value)
    {
        camInput.allowedGreenDeltaWithBaseColor = (int)value;
        gDelta.value = camInput.allowedGreenDeltaWithBaseColor;
    }
    public void BDeltaChange(float value)
    {
        camInput.allowedBlueDeltaWithBaseColor = (int)value;
        bDelta.value = camInput.allowedBlueDeltaWithBaseColor;
    }
    public void HDeltaChange(float value)
    {
        camInput.allowedHueDeltaWithBaseColor = value;
        hDelta.value = camInput.allowedHueDeltaWithBaseColor;
    }
    public void SDeltaChange(float value)
    {
        camInput.allowedSaturationDeltaWithBaseColor = value;
        sDelta.value = camInput.allowedSaturationDeltaWithBaseColor;
    }
    public void VDeltaChange(float value)
    {
        camInput.allowedValueDeltaWithBaseColor = value;
        vDelta.value = camInput.allowedValueDeltaWithBaseColor;
    }
    public void WebcamResolutionXChange(string value)
    {
        camInput.Texture.requestedWidth = int.Parse(value);
        webcamResolutionX.text = camInput.Texture.requestedWidth.ToString();
    }
    public void WebcamResolutionYChange(string value)
    {
        camInput.Texture.requestedHeight = int.Parse(value);
        webcamResolutionY.text = camInput.Texture.requestedHeight.ToString();
    }
    public void VertexAreaChange(string value)
    {
        camInput.vertexArea = int.Parse(value);
        vertexArea.text = camInput.vertexArea.ToString();
    }
    public void MinGroupAreaChange(string value)
    {
        camInput.minGroupArea = int.Parse(value);
        minGroupArea.text = camInput.minGroupArea.ToString();
    }
    public void ShadowChange(bool value)
    {
        camInput.shadow = value;
        shadow.isOn = camInput.shadow;
    }

    public void ToggleCamInputActive()
    {
        camInput.Active = !camInput.Active;
    }
    public void ToggleCamInputNegative()
    {
        camInput.negativeWithBaseColor = !camInput.negativeWithBaseColor;
    }
    public void BaseColorsUpdate()
    {
        camInput.BaseColorsUpdate();
    }

    void OnClickWebcamDisplay(PointerEventData data)
    {
        Vector2 vector2 = new Vector2(-0.1f, 0f);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(webcamDisplayRectTransform, data.position, null, out vector2);
        vector2.x = vector2.x / webcamDisplayRectTransform.rect.width + 0.5f;
        vector2.y = vector2.y / webcamDisplayRectTransform.rect.height + 0.5f;

        if (Input.GetKey(KeyCode.Alpha1))
            camInput.FL = vector2;
        else if (Input.GetKey(KeyCode.Alpha2))
            camInput.FR = vector2;
        else if (Input.GetKey(KeyCode.Alpha3))
            camInput.BL = vector2;
        else if (Input.GetKey(KeyCode.Alpha4))
            camInput.BR = vector2;
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!camInput.Texture.isPlaying)
                return;
            AddCustomColor(camInput.Texture.GetPixel((int)(vector2.x * camInput.Texture.width), (int)(vector2.y * camInput.Texture.height)));
        }
    }

    void AddCustomColor(Color32 color32)
    {
        CustomColor temp = new CustomColor();
        temp.color = color32;
        camInput.customColors.Add(temp);
        ColorSettingWindowsUpdate();
    }
    public void RemoveCustomColor(int index)
    {
        camInput.customColors.RemoveAt(index);
        ColorSettingWindowsUpdate();
    }
    void ColorSettingWindowsUpdate()
    {
        while (camInput.customColors.Count > colorSettingWindows.Count)
        {
            ColorSettingWindow temp = Instantiate(colorSettingWindowPrefab, colorSettingWindowsParent).GetComponent<ColorSettingWindow>();
            temp.Init(colorSettingWindows.Count, this);
            colorSettingWindows.Add(temp);
        }

        int length = colorSettingWindows.Count;
        for (int i = 0; i < length; i++)
            colorSettingWindows[i].CamInputChange(camInput);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {/*
            if (webcamDisplay.CamInputDisplay.mode == CamInputDisplayMode.Default)
            {
                webcamDisplay.CamInputDisplay.mode = CamInputDisplayMode.SolidColor;
            }
            else if (webcamDisplay.CamInputDisplay.mode == CamInputDisplayMode.SolidColor)
            {
                webcamDisplay.CamInputDisplay.mode = CamInputDisplayMode.NoBaseColor;
            }
            else if (webcamDisplay.CamInputDisplay.mode == CamInputDisplayMode.NoBaseColor)
            {
                webcamDisplay.CamInputDisplay.mode = CamInputDisplayMode.Default;
            }*/
            if (webcamDisplay.CamInputDisplay.mode == CamInputDisplayMode.NoBaseColor)
                webcamDisplay.CamInputDisplay.mode = 0;
            else webcamDisplay.CamInputDisplay.mode++;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            BaseColorsUpdate();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleCamInputActive();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FinishSetting();
        }

        if (Input.GetKey(KeyCode.Alpha1)) {
            log.text = "현재 Front-Left 꼭짓점의 위치" + camInput.FL.ToString();
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            log.text = "현재 Front-Right 꼭짓점의 위치" + camInput.FR.ToString();
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            log.text = "현재 Back-Left 꼭짓점의 위치" + camInput.BL.ToString();
        }
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            log.text = "현재 Back-Right 꼭짓점의 위치" + camInput.BR.ToString();
        }
        else
        {
            log.text = "-단축키- \nM - 색상인식영역 표시 방식 변경 \nEnter - 기준 색상 갱신 \nSpace bar -카메라 활성화 토글 \n1,2,3,4 + 좌클릭 - 색상 인식 영역 지정";
        }

        //test
        /*if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                camInput.FocusY += 0.05f;
            if (Input.GetKeyDown(KeyCode.DownArrow))
                camInput.FocusY += -0.05f;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                camInput.FocusX += -0.05f;
            if (Input.GetKeyDown(KeyCode.RightArrow))
                camInput.FocusX += 0.05f;
        }*/
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            camInput.FocusPower += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            camInput.FocusPower += -0.1f;
        }

        //focusXText.text = camInput.FocusX.ToString();
        //focusYText.text = camInput.FocusY.ToString();
        //focusPowerText.text = camInput.FocusPower.ToString();
    }
}
