using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AspectRatioFitter),typeof(RawImage))]
public class CamInputWebcamDisplay : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    Text nameText;
    [SerializeField]
    CamInputDisplay camInputDisplay;
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
            nameText.text = CamInputManager.Instance.webCamNames[camInputIndex];
        if (camInputDisplay != null)
            camInputDisplay.Init(camInputIndex);
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

    public void OnPointerClick(PointerEventData eventData)
    {
        Click?.Invoke(index);
        ClickAndPointerEventData?.Invoke(eventData);
    }
    public WithID Click;
    public WithPointerEvent ClickAndPointerEventData;
}
public delegate void WithID(int id);
public delegate void WithPointerEvent(PointerEventData eventData);