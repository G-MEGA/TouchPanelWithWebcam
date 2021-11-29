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
    private void Update()
    {
        activeCaminputCount = manager.ActiveCamInputCount;

        if (activeCaminputCount > 0)
        {
            if (mode == TouchDisplayMode.Default)
            {
                for (int i = 0; i < texture.width; i++)
                {
                    for (int j = 0; j < texture.height; j++)
                    {
                        if (manager.marking[i, j] != -2)
                        {
                            if (manager.touches[manager.marking[i, j]].isGhost)
                            {
                                colors[i + texture.width * j] = Color.cyan;
                            }
                            else
                            {
                                if (manager.touches[manager.marking[i, j]].isDownThisFrame)
                                    colors[i + texture.width * j] = Color.red;
                                else
                                    colors[i + texture.width * j] = Color.white;
                            }
                        }
                        else
                        {
                            colors[i + texture.width * j] = Color.black;
                        }
                    }
                }
            }
            else if(mode == TouchDisplayMode.Each)
            {
                for (int i = 0; i < texture.width; i++)
                {
                    for (int j = 0; j < texture.height; j++)
                    {
                        if (manager.marking[i, j] != -2)
                        {
                            if (manager.touches[manager.marking[i, j]].isGhost)
                            {
                                colors[i + texture.width * j] = Color.cyan;
                            }
                            else
                            {
                                if (manager.touches[manager.marking[i, j]].isDownThisFrame)
                                    colors[i + texture.width * j] = Color.red;
                                else
                                    colors[i + texture.width * j] = Color.white;
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
                                            colors[i + texture.width * j] = Color.red;
                                            break;
                                        case 1:
                                            colors[i + texture.width * j] = Color.green;
                                            break;
                                        case 2:
                                            colors[i + texture.width * j] = Color.blue;
                                            break;
                                        case 3:
                                            colors[i + texture.width * j] = Color.cyan;
                                            break;
                                        case 4:
                                            colors[i + texture.width * j] = Color.magenta;
                                            break;
                                        case 5:
                                            colors[i + texture.width * j] = Color.yellow;
                                            break;
                                    }
                                    if (manager.RemoveGhostActive || manager.camInputs[k].shadow)//꼭짓점추출이 작동중일때
                                    {
                                        if (manager.camInputs[k].vertexOfMarkings[i, j] != 0 && manager.camInputs[k].vertexOfMarkings[i, j] != 1)//꼭짓점이 아니라면(0번이랑 1번 꼭짓점만 인정)
                                        {
                                            colors[i + texture.width * j] = Color.Lerp(colors[i + texture.width * j], Color.black, 0.5f);
                                        }
                                    }
                                    break;
                                }
                            }
                            if (k == manager.camInputs.Length) colors[i + texture.width * j] = Color.black;
                        }
                    }
                }
            }
        }
        else
            for (int i = 0; i < texture.width; i++)
                for (int j = 0; j < texture.height; j++)
                    colors[i + texture.width * j] = Color.black;
        texture.SetPixels(colors);
        texture.Apply();
    }

    private void OnResolutionChanged()
    {
        texture = new Texture2D(manager.Resolution.x, manager.Resolution.y);
        texture.filterMode = FilterMode.Point;
        image.texture = texture;
        fitter.aspectRatio = (float)texture.width / texture.height;
        colors = new Color[manager.Resolution.x * manager.Resolution.y];
    }
}
public enum TouchDisplayMode
{
    Default, Each
}