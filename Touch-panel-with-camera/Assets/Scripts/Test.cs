using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public GameObject pointPrefab;

    RawImage rawImage;
    RectTransform rectTransform;
    AspectRatioFitter fitter;

    Image[,] pointObjects;

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();
        fitter = GetComponent<AspectRatioFitter>();
    }


    void Update()
    {
        if (rawImage.texture != null)
        {
            fitter.aspectRatio = (float)rawImage.texture.width / rawImage.texture.height;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            WebCamDevice[] webCamDevices = WebCamTexture.devices;
            //Vector2 bottomLeft = new Vector2(0f,0f);
            //Vector2 topRight = new Vector2(1f,1f);
            Vector2 bottomLeft = new Vector2(0f,0f);
            Vector2 topRight = new Vector2(1f,1f);
            CamInputManager.Instance.camInputs[0].Init(webCamDevices[0].name, new Vector2(bottomLeft.x, topRight.y), topRight, bottomLeft, new Vector2(topRight.x, bottomLeft.y));
            rawImage.texture = CamInputManager.Instance.camInputs[0].Texture;
            print(rectTransform.sizeDelta);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (pointObjects != null)
            {
                for (int i = 0; i < pointObjects.GetLength(0); i++)
                {
                    for (int j = 0; j < pointObjects.GetLength(1); j++)
                    {
                        GameObject.Destroy(pointObjects[i, j].gameObject);
                    }
                }
            }

            pointObjects = new Image[CamInputManager.Instance.Resolution.x, CamInputManager.Instance.Resolution.y];

            for (int i = 0; i < pointObjects.GetLength(0); i++)
            {
                for (int j = 0; j < pointObjects.GetLength(1); j++)
                {
                    pointObjects[i, j] = GameObject.Instantiate(pointPrefab, transform).GetComponent<Image>();

                    Vector2Int pos = CamInputManager.Instance.camInputs[0].markingPositions[i* CamInputManager.Instance.MarkingLength, j * CamInputManager.Instance.MarkingLength];
                    Vector2Int pos1 = CamInputManager.Instance.camInputs[0].markingPositions[i * CamInputManager.Instance.MarkingLength + CamInputManager.Instance.MarkingLength -1, j * CamInputManager.Instance.MarkingLength + CamInputManager.Instance.MarkingLength - 1];
                    float x = (pos.x + pos1.x) * 0.5f / rawImage.texture.width * rectTransform.rect.width;
                    float y = (pos.y+ pos1.y) * 0.5f / rawImage.texture.height * rectTransform.rect.height;
                    pointObjects[i, j].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            CamInputManager.Instance.camInputs[0].BaseColorsUpdate();
        }


        if (pointObjects != null)
        {
            for (int i = 0; i < pointObjects.GetLength(0); i++)
            {
                for (int j = 0; j < pointObjects.GetLength(1); j++)
                {
                    if (CamInputManager.Instance.camInputs[0].markings[i, j])
                    {
                        pointObjects[i, j].color = Color.red;
                    }
                    else
                    {
                        pointObjects[i, j].color = CamInputManager.Instance.camInputs[0].baseColors[i,j];
                    }
                }
            }
        }
    }
}
