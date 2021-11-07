using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CamInputSettingWindow : MonoBehaviour
{
    [SerializeField]
    CamInputWebcamDisplay webcamDisplay;
    

    RectTransform webcamDisplayRectTransform;
    CamInput camInput;

    private void Awake()
    {
        webcamDisplayRectTransform = webcamDisplay.GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }
    public void StartSetting(int index)
    {
        camInput = CamInputManager.Instance.camInputs[index];
        webcamDisplay.Init(index);
        webcamDisplay.ClickAndPointerEventData += OnClickWebcamDisplay;
        gameObject.SetActive(true);
    }
    public void FinishSetting()
    {
        webcamDisplay.ClickAndPointerEventData -= OnClickWebcamDisplay;
        gameObject.SetActive(false);
    }
    public void ToggleCamInputActive()
    {
        camInput.Active = !camInput.Active;
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
    }

    private void Update()
    {
        //test
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FinishSetting();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleCamInputActive();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            camInput.BaseColorsUpdate();
        }
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
    }
}
