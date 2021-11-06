using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage),typeof(AspectRatioFitter))]
public class TouchDisplay : MonoBehaviour
{
    AspectRatioFitter fitter;
    RawImage image;
    Texture2D texture;
    private void Start()
    {

        fitter = GetComponent<AspectRatioFitter>();

        image = GetComponent<RawImage>();
        image.uvRect = new Rect(0f,1f,1f,-1f);

        OnResolutionChanged();
        CamInputManager.Instance.ResolutionChanged += OnResolutionChanged;
    }
    private void OnDestroy()
    {
        CamInputManager.Instance.ResolutionChanged -= OnResolutionChanged;
    }
    int activeCaminputCount;
    Color[] colors;
    private void Update()
    {
        activeCaminputCount = 0;
        for (int i = 0; i < CamInputManager.Instance.camInputs.Length; i++)
            if (CamInputManager.Instance.camInputs[i].Active) activeCaminputCount++;

        if (activeCaminputCount > 0)
        {
            for (int i = 0; i < texture.width; i++)
            {
                for (int j = 0; j < texture.height; j++)
                {
                    if (CamInputManager.Instance.markings[i, j] == activeCaminputCount)
                        colors[i + texture.width * j] = Color.white;
                    else
                    {
                        int k;
                        for (k = 0; k < CamInputManager.Instance.camInputs.Length; k++)
                        {
                            if (!CamInputManager.Instance.camInputs[k].Active)
                                continue;
                            if (CamInputManager.Instance.camInputs[k].markings[i, j])
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
                                break;
                            }
                        }
                        if (k == CamInputManager.Instance.camInputs.Length) colors[i + texture.width * j] = Color.black;
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
        texture = new Texture2D(CamInputManager.Instance.Resolution.x, CamInputManager.Instance.Resolution.y);
        texture.filterMode = FilterMode.Point;
        image.texture = texture;
        fitter.aspectRatio = (float)texture.width / texture.height;
        colors = new Color[CamInputManager.Instance.Resolution.x * CamInputManager.Instance.Resolution.y];
    }
}
