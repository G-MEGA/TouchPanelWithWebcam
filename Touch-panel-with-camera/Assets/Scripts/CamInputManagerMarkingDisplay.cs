using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamInputManagerMarkingDisplay : MonoBehaviour
{
    public GameObject pointPrefab;
    Image[,] pointObjects = new Image[0,0];

    void Start()
    {
        
    }

    void Update()
    {
        int width = CamInputManager.Instance.Resolution.x;
        int height = CamInputManager.Instance.Resolution.y;
        if (pointObjects.GetLength(0) != width || pointObjects.GetLength(1) != height)
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

            pointObjects = new Image[width, height]; 

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    pointObjects[i, j] = GameObject.Instantiate(pointPrefab, transform).GetComponent<Image>();

                    float x = (float)i / (width - 1);
                    float y = (float)j / (height - 1);
                    pointObjects[i, j].GetComponent<RectTransform>().anchorMin = new Vector2(x, y);
                    pointObjects[i, j].GetComponent<RectTransform>().anchorMax = new Vector2(x, y);
                }
            }
        }
        else
        {
            int canInputsCount = CamInputManager.Instance.camInputs.Count;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    pointObjects[i, j].color = Color.black;
                    for (int k = 0; k < canInputsCount; k++)
                    {
                        if (CamInputManager.Instance.camInputs[k].markings[i,j])
                        {
                            switch (k)
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
                            break;
                        }
                    }
                    if(CamInputManager.Instance.markings[i,j] == canInputsCount)
                    {
                        pointObjects[i, j].color = Color.white;
                    }
                }
            }
        }
    }
}
