using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class CamInputDisplay : MonoBehaviour
{
    public GameObject pointPrefab;
    public int camInputIndex = 0;

    RawImage rawImage;
    RectTransform rectTransform;
    AspectRatioFitter fitter;

    Image[,] pointObjects;
    bool on = false;

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();
        fitter = GetComponent<AspectRatioFitter>();
    }
    private void Update()
    {
        if (on)
        {
            print(CamInputManager.Instance.camInputs[camInputIndex].markingsUpdateMethod);
            if (fitter != null && rawImage.texture != null)
                fitter.aspectRatio = (float)rawImage.texture.width / rawImage.texture.height;
            if (Input.GetKeyDown(KeyCode.Return))
                CamInputManager.Instance.camInputs[camInputIndex].BaseColorsUpdate();
            for (int i = 0; i < pointObjects.GetLength(0); i++)
            {
                for (int j = 0; j < pointObjects.GetLength(1); j++)
                {
                    if (CamInputManager.Instance.camInputs[camInputIndex].markings[i, j])
                    {
                        switch (camInputIndex)
                        {
                            case 0:
                                pointObjects[i, j].color = Color.red;
                                break;
                            case 1:
                                pointObjects[i, j].color = Color.green;
                                break;
                            case 2:
                                pointObjects[i, j].color = Color.blue;
                                break;
                            case 3:
                                pointObjects[i, j].color = Color.cyan;
                                break;
                            case 4:
                                pointObjects[i, j].color = Color.magenta;
                                break;
                            case 5:
                                pointObjects[i, j].color = Color.yellow;
                                break;
                        }
                    }
                    else
                    {
                        pointObjects[i, j].color = CamInputManager.Instance.camInputs[camInputIndex].baseColors[i, j];
                    }
                }
            }
        }
        else
        {
            if (CamInputManager.Instance.camInputs.Count > camInputIndex)
            {
                on = true;
                rawImage.texture = CamInputManager.Instance.camInputs[camInputIndex].Texture;

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

                int markingLength = CamInputManager.Instance.MarkingLength;
                for (int i = 0; i < pointObjects.GetLength(0); i++)
                {
                    for (int j = 0; j < pointObjects.GetLength(1); j++)
                    {
                        pointObjects[i, j] = GameObject.Instantiate(pointPrefab, transform).GetComponent<Image>();

                        Vector2Int pos = CamInputManager.Instance.camInputs[camInputIndex].markingPositions[i * markingLength, j * markingLength];
                        Vector2Int pos1 = CamInputManager.Instance.camInputs[camInputIndex].markingPositions[i * markingLength + markingLength - 1, j * markingLength + markingLength - 1];
                        float x = (pos.x + pos1.x) * 0.5f / rawImage.texture.width;
                        float y = (pos.y + pos1.y) * 0.5f / rawImage.texture.height;
                        pointObjects[i, j].GetComponent<RectTransform>().anchorMin = new Vector2(x, y);
                        pointObjects[i, j].GetComponent<RectTransform>().anchorMax = new Vector2(x, y);
                        pointObjects[i, j].transform.SetAsFirstSibling();
                    }
                }
            }
        }
    }

    public void OnMarkingsUpdateMethodChanged(int value)
    {
        if (on)
        {
            CamInputManager.Instance.camInputs[camInputIndex].markingsUpdateMethod = (MarkingsUpdateMethod)value;
        }
    }
}
