using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AspectRatioFitter),typeof(RawImage))]
public class CamInputWebcamDisplay : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    Text nameText;
    [SerializeField]
    CamInputDisplay camInputDisplay;
    [SerializeField]
    Slider focusX;
    [SerializeField]
    Slider focusY;
    [SerializeField]
    GameObject[] ignorePointerEvent;

    public CamInputDisplay CamInputDisplay
    {
        get
        {
            return camInputDisplay;
        }
    }
    WebCamTexture cam;
    int index;

    AspectRatioFitter fitter;
    RawImage image;

    public void Init(int camInputIndex)
    {
        cam = CamInputManager.Instance.webCams[camInputIndex];
        index = camInputIndex;

        if (fitter == null)
            fitter = GetComponent<AspectRatioFitter>();
        image = GetComponent<RawImage>();

        image.color = Color.black;
        image.texture = null;
        fitter.aspectRatio = 3f;

        if(nameText != null)
            nameText.text = CamInputManager.Instance.webCamNames[index];
        if (camInputDisplay != null)
            camInputDisplay.Init(index);
        if (focusX != null)
        {
            focusX.value = CamInputManager.Instance.camInputs[index].FocusX;
            focusX.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(FocusXChange));
        }
        if (focusY != null)
        {
            focusY.value = CamInputManager.Instance.camInputs[index].FocusY;
            focusY.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(FocusYChange));
        }
        prevIsPlaying = false;
    }
    bool prevIsPlaying;
    private void Update()
    {
        if (prevIsPlaying != cam.isPlaying)
        {
            if (cam.isPlaying)
            {
                image.color = Color.white;
                image.texture = cam;
                fitter.aspectRatio = (float)cam.width / cam.height;
            }
            else
            {
                image.color = Color.black;
                image.texture = null;
                fitter.aspectRatio = 3f;
            }

            prevIsPlaying = cam.isPlaying;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        int length = ignorePointerEvent.Length;
        for (int i = 0; i < length; i++)
            if (ignorePointerEvent[i] == eventData.pointerPress)
                return;

        Click?.Invoke(index);
        ClickAndPointerEventData?.Invoke(eventData);
    }
    public WithID Click;
    public WithPointerEvent ClickAndPointerEventData;
    public void FocusXChange(float value)
    {
        CamInputManager.Instance.camInputs[index].FocusX = value;
        if (focusX != null)
            focusX.value = CamInputManager.Instance.camInputs[index].FocusX;
    }
    public void FocusYChange(float value)
    {
        CamInputManager.Instance.camInputs[index].FocusY = value;
        if (focusY != null)
            focusY.value = CamInputManager.Instance.camInputs[index].FocusY;
    }
    public void ToggleCamInputActive()
    {
        CamInputManager.Instance.camInputs[index].Active = !CamInputManager.Instance.camInputs[index].Active;
    }
}
public delegate void WithID(int id);
public delegate void WithPointerEvent(PointerEventData eventData);