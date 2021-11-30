using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage),typeof(AspectRatioFitter))]
public class TouchDisplay : MonoBehaviour
{
    public TouchDisplayMode mode = TouchDisplayMode.Each;

    AspectRatioFitter fitter;
    RawImage image;
    Texture2D texture;
    CamInputManager manager;

    private void Start()
    {

        manager = CamInputManager.Instance;

        fitter = GetComponent<AspectRatioFitter>();

        image = GetComponent<RawImage>();
        image.uvRect = new Rect(0f,1f,1f,-1f);

        OnResolutionChanged();
        manager.ResolutionChanged += OnResolutionChanged;
    }
    private void OnDestroy()
    {
        manager.ResolutionChanged -= OnResolutionChanged;
    }
    int activeCaminputCount;
    Color[] colors;
    int width, height;
    private void Update()
    {
        activeCaminputCount = manager.ActiveCamInputCount;

        if (activeCaminputCount > 0)
        {
            if (mode == TouchDisplayMode.Default)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (manager.marking[i, j] != -2)
                        {
                            if (manager.touches[manager.marking[i, j]].isGhost)
                            {
                                colors[i + width * j] = Color.cyan;
                            }
                            else
                            {
                                colors[i + width * j] = Color.white;
                            }
                        }
                        else
                        {
                            colors[i + width * j] = Color.black;
                        }
                    }
                }
            }
            else if(mode == TouchDisplayMode.Each)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (manager.marking[i, j] != -2)
                        {
                            if (manager.touches[manager.marking[i, j]].isGhost)
                            {
                                colors[i + width * j] = Color.cyan;
                            }
                            else
                            {
                                colors[i + width * j] = Color.white;
                            }
                        }
                        else
                        {
                            int k;
                            for (k = 0; k < manager.camInputs.Length; k++)
                            {
                                if (!manager.camInputs[k].Active)
                                    continue;
                                if (manager.camInputs[k].markings[i, j])
                                {
                                    switch (k)
                                    {
                                        case 0:
                                            colors[i + width * j] = Color.red;
                                            break;
                                        case 1:
                                            colors[i + width * j] = Color.green;
                                            break;
                                        case 2:
                                            colors[i + width * j] = Color.blue;
                                            break;
                                        case 3:
                                            colors[i + width * j] = Color.cyan;
                                            break;
                                        case 4:
                                            colors[i + width * j] = Color.magenta;
                                            break;
                                        case 5:
                                            colors[i + width * j] = Color.yellow;
                                            break;
                                    }
                                    if (manager.RemoveGhostActive || manager.camInputs[k].shadow)//꼭짓점추출이 작동중일때
                                    {
                                        if (manager.camInputs[k].vertexOfMarkings[i, j] != 0 && manager.camInputs[k].vertexOfMarkings[i, j] != 1)//꼭짓점이 아니라면(0번이랑 1번 꼭짓점만 인정)
                                        {
                                            colors[i + width * j] = Color.Lerp(colors[i + width * j], Color.black, 0.5f);
                                        }
                                    }
                                    break;
                                }
                            }
                            if (k == manager.camInputs.Length) colors[i + width * j] = Color.black;
                        }
                    }
                }
            }
        }
        else
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    colors[i + width * j] = Color.black;
        texture.SetPixels(colors);
        texture.Apply();

        for (int i = 0; i <= manager.touchesLastIndex; i++)
        {
            if (!manager.touches[i].isGhost && manager.touches[i].isDownThisFrame)
            {
                StartCoroutine(DownPositionDisplay((int)manager.touches[i].x, (int)manager.touches[i].y));
            }
        }
    }

    const float downPositionDisplayTime = 0.5f;
    Color downPositionDisplayColor = Color.red;
    IEnumerator DownPositionDisplay(int x, int y)
    {
        int widthSave = width, heightSave = height;
        float leftTime = downPositionDisplayTime;
        float timeRatio;

        int i, j, nextX, nextY;

        while (widthSave == width && heightSave == height && leftTime > 0f)
        {
            timeRatio = leftTime / downPositionDisplayTime;
            nextX = x - 1;
            for (i = 0; i < 3; i++)
            {
                nextY = y - 1;
                for (j = 0; j < 3; j++)
                {
                    if (nextX >= 0 && nextY >= 0 && nextX < width && nextY < height)
                    {
                        texture.SetPixel(nextX, nextY, Color.Lerp(Color.black, downPositionDisplayColor, timeRatio));
                    }
                    nextY++;
                }
                nextX++;
            }

            texture.Apply();

            leftTime -= Time.deltaTime;
            yield return null;
        }
    }

    private void OnResolutionChanged()
    {
        texture = new Texture2D(manager.Resolution.x, manager.Resolution.y);
        texture.filterMode = FilterMode.Point;
        image.texture = texture;
        fitter.aspectRatio = (float)texture.width / texture.height;
        colors = new Color[manager.Resolution.x * manager.Resolution.y];
        width = texture.width;
        height = texture.height;
    }
}
public enum TouchDisplayMode
{
    Default, Each
}