using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSettingWindow : MonoBehaviour
{
    [SerializeField]
    Image colorDisplay;
    [SerializeField]
    InputField r;
    [SerializeField]
    InputField g;
    [SerializeField]
    InputField b;
    [SerializeField]
    InputField h;
    [SerializeField]
    InputField s;
    [SerializeField]
    InputField v;
    [SerializeField]
    Dropdown markingUpdateMethod;
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
    Toggle negative;

    CamInput camInput;
    CustomColor customColor;
    int index;
    CamInputSettingWindow camInputSettingWindow;

    public void Init(int customColorIndex, CamInputSettingWindow cisw)//생성직후 실행해줌. 
    {
        index = customColorIndex;
        camInputSettingWindow = cisw;

        r.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(RChange));
        g.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(GChange));
        b.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(BChange));
        h.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(HChange));
        s.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(SChange));
        v.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(VChange));
        markingUpdateMethod.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(MethodChange));
        rDelta.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(RDeltaChange));
        gDelta.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(GDeltaChange));
        bDelta.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(BDeltaChange));
        hDelta.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(HDeltaChange));
        sDelta.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(SDeltaChange));
        vDelta.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(VDeltaChange));
        negative.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(NegativeChange));
    }
    public void CamInputChange(CamInput ci)//캠인풋이 변경되거나(즉 캠인풋세팅윈도우의 StartSetting이 실행될 때) 캠인풋의 커스텀컬러의 수가 늘거나 줄어들때도 작동.
    {
        camInput = ci;
        if (index < camInput.customColors.Count)
        {
            customColor = camInput.customColors[index];
            gameObject.SetActive(true);
            //작동시작. 초기화 하는 부분
            ColorTextsAndDisplayUpdate();
            markingUpdateMethod.value = (int)customColor.markingsUpdateMethod;
            rDelta.gameObject.SetActive(customColor.markingsUpdateMethod == MarkingsUpdateMethod.CompareRGB);
            gDelta.gameObject.SetActive(customColor.markingsUpdateMethod == MarkingsUpdateMethod.CompareRGB);
            bDelta.gameObject.SetActive(customColor.markingsUpdateMethod == MarkingsUpdateMethod.CompareRGB);
            hDelta.gameObject.SetActive(customColor.markingsUpdateMethod == MarkingsUpdateMethod.CompareHSV);
            sDelta.gameObject.SetActive(customColor.markingsUpdateMethod == MarkingsUpdateMethod.CompareHSV);
            vDelta.gameObject.SetActive(customColor.markingsUpdateMethod == MarkingsUpdateMethod.CompareHSV);
            rDelta.value = customColor.allowedRedDelta;
            gDelta.value = customColor.allowedGreenDelta;
            bDelta.value = customColor.allowedBlueDelta;
            hDelta.value = customColor.allowedHueDelta;
            sDelta.value = customColor.allowedSaturationDelta;
            vDelta.value = customColor.allowedValueDelta;
            negative.isOn = customColor.negative;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    byte resultByte;
    public void RChange(string value)
    {
        if (byte.TryParse(value, out resultByte))
            customColor.color = new Color32(resultByte, customColor.color.g, customColor.color.b, 255);
        ColorTextsAndDisplayUpdate();
    }
    public void GChange(string value)
    {
        if (byte.TryParse(value, out resultByte))
            customColor.color = new Color32(customColor.color.r, resultByte, customColor.color.b, 255);
        ColorTextsAndDisplayUpdate();
    }
    public void BChange(string value)
    {
        if (byte.TryParse(value, out resultByte))
            customColor.color = new Color32(customColor.color.r, customColor.color.g, resultByte, 255);
        ColorTextsAndDisplayUpdate();
    }
    float resultFloat;
    public void HChange(string value)
    {
        if (float.TryParse(value, out resultFloat))
        {
            Color.RGBToHSV(customColor.color, out tempH, out tempS, out tempV);
            customColor.color = Color.HSVToRGB(resultFloat, tempS, tempV);
        }
        ColorTextsAndDisplayUpdate();
    }
    public void SChange(string value)
    {
        if (float.TryParse(value, out resultFloat))
        {
            Color.RGBToHSV(customColor.color, out tempH, out tempS, out tempV);
            customColor.color = Color.HSVToRGB(tempH, resultFloat, tempV);
        }
        ColorTextsAndDisplayUpdate();
    }
    public void VChange(string value)
    {
        if (float.TryParse(value, out resultFloat))
        {
            Color.RGBToHSV(customColor.color, out tempH, out tempS, out tempV);
            customColor.color = Color.HSVToRGB(tempH, tempS, resultFloat);
        }
        ColorTextsAndDisplayUpdate();
    }
    public void MethodChange(int method)
    {
        customColor.markingsUpdateMethod = (MarkingsUpdateMethod)method;
        markingUpdateMethod.value = (int)customColor.markingsUpdateMethod;
        rDelta.gameObject.SetActive(customColor.markingsUpdateMethod == MarkingsUpdateMethod.CompareRGB);
        gDelta.gameObject.SetActive(customColor.markingsUpdateMethod == MarkingsUpdateMethod.CompareRGB);
        bDelta.gameObject.SetActive(customColor.markingsUpdateMethod == MarkingsUpdateMethod.CompareRGB);
        hDelta.gameObject.SetActive(customColor.markingsUpdateMethod == MarkingsUpdateMethod.CompareHSV);
        sDelta.gameObject.SetActive(customColor.markingsUpdateMethod == MarkingsUpdateMethod.CompareHSV);
        vDelta.gameObject.SetActive(customColor.markingsUpdateMethod == MarkingsUpdateMethod.CompareHSV);
    }
    public void RDeltaChange(float value)
    {
        customColor.allowedRedDelta = (int)value;
        rDelta.value = customColor.allowedRedDelta;
    }
    public void GDeltaChange(float value)
    {
        customColor.allowedGreenDelta = (int)value;
        gDelta.value = customColor.allowedGreenDelta;
    }
    public void BDeltaChange(float value)
    {
        customColor.allowedBlueDelta = (int)value;
        bDelta.value = customColor.allowedBlueDelta;
    }
    public void HDeltaChange(float value)
    {
        customColor.allowedHueDelta = value;
        hDelta.value = customColor.allowedHueDelta;
    }
    public void SDeltaChange(float value)
    {
        customColor.allowedSaturationDelta = value;
        sDelta.value = customColor.allowedSaturationDelta;
    }
    public void VDeltaChange(float value)
    {
        customColor.allowedValueDelta = value;
        vDelta.value = customColor.allowedValueDelta;
    }
    public void NegativeChange(bool value)
    {
        customColor.negative = value;
        negative.isOn = customColor.negative;
    }

    float tempH, tempS, tempV;
    void ColorTextsAndDisplayUpdate()
    {
        r.text = customColor.color.r.ToString();
        g.text = customColor.color.g.ToString();
        b.text = customColor.color.b.ToString();
        Color.RGBToHSV(customColor.color, out tempH, out tempS, out tempV);
        h.text = tempH.ToString();
        s.text = tempS.ToString();
        v.text = tempV.ToString();

        colorDisplay.color = customColor.color;
    }

    public void CustomColorRemove()
    {
        camInputSettingWindow.RemoveCustomColor(index);
    }
}
