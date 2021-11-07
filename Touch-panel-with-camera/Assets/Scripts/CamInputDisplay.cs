using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(CanvasRenderer))]
public class CamInputDisplay : MaskableGraphic
{
    public CamInputDisplayMode mode;
    Texture m_Texture;
    CamInput camInput;
    Texture2D markingTexture;
    int index;

    Color[] colors;

    protected CamInputDisplay()
    {
        useLegacyMeshGeneration = false;
    }
    public void Init(int camInputIndex)
    {
        camInput = CamInputManager.Instance.camInputs[camInputIndex];
        index = camInputIndex;

        OnActiveChanged();
        camInput.ActiveChanged += OnActiveChanged;
        OnMarkingPositionsChanged();
        camInput.MarkingPositionsChanged += OnMarkingPositionsChanged;
        OnResolutionChanged();
        CamInputManager.Instance.ResolutionChanged += OnResolutionChanged;
    }
    protected override void OnDestroy()
    {
        if (camInput == null) return;
        camInput.ActiveChanged -= OnActiveChanged;
        camInput.MarkingPositionsChanged -= OnMarkingPositionsChanged;
        if (CamInputManager.Instance == null) return;
        CamInputManager.Instance.ResolutionChanged -= OnResolutionChanged;
    }
    void OnActiveChanged()
    {
        if (camInput.Active)
        {
            color = Color.white;
        }
        else
        {
            color = Color.clear;
        }
    }
    void OnResolutionChanged()
    {
        markingTexture = new Texture2D(CamInputManager.Instance.Resolution.x, CamInputManager.Instance.Resolution.y);
        texture = markingTexture;
        markingTexture.filterMode = FilterMode.Point;
        colors = new Color[CamInputManager.Instance.Resolution.x* CamInputManager.Instance.Resolution.y];
    }
    void OnMarkingPositionsChanged()
    {
        SetMaterialDirty();
        SetVerticesDirty();
    }

    private void Update()
    {
        if (camInput != null&& camInput.Active)
        {
            for (int i = 0; i < texture.width; i++)
            {
                for (int j = 0; j < texture.height; j++)
                {
                    if (mode == CamInputDisplayMode.SolidColor)
                    {
                        switch (index)
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
                    }
                    else
                    {
                        if (camInput.markings[i, j])
                        {
                            switch (index)
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
                        }
                        else
                        {
                            if (mode == CamInputDisplayMode.Default)
                            {
                                colors[i + texture.width * j] = camInput.baseColors[i, j];//기준 색 설정
                            }
                            else
                            {
                                colors[i + texture.width * j] = Color.clear;//기준 색 설정
                            }
                        }
                    }
                }
            }
            markingTexture.SetPixels(colors);
            markingTexture.Apply();
        }
    }
    /// <summary>
    /// Returns the texture used to draw this Graphic.
    /// </summary>
    public override Texture mainTexture
    {
        get
        {
            if (m_Texture == null)
            {
                if (material != null && material.mainTexture != null)
                {
                    return material.mainTexture;
                }
                return s_WhiteTexture;
            }

            return m_Texture;
        }
    }
    public Texture texture
    {
        get
        {
            return m_Texture;
        }
        set
        {
            if (m_Texture == value)
                return;

            m_Texture = value;
            SetVerticesDirty();
            SetMaterialDirty();
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        Texture tex = mainTexture;
        vh.Clear();
        if (tex != null && camInput != null)
        {
            var r = GetPixelAdjustedRect();
            {
                var color32 = color;
                vh.AddVert(new Vector3(r.x + r.width * camInput.FL.x, r.y + r.height * camInput.FL.y), color32, new Vector2(0f, 0f));//LF
                vh.AddVert(new Vector3(r.x + r.width * camInput.BL.x, r.y + r.height * camInput.BL.y), color32, new Vector2(0f, 1f));//LB
                vh.AddVert(new Vector3(r.x + r.width * camInput.BR.x, r.y + r.height * camInput.BR.y), color32, new Vector2(1f, 1f));//RB
                vh.AddVert(new Vector3(r.x + r.width * camInput.FR.x, r.y + r.height * camInput.FR.y), color32, new Vector2(1f, 0f));//RF

                Vector3 v0 = new Vector3(r.x + r.width * camInput.FL.x, r.y + r.height * camInput.FL.y);
                Vector3 v1 = new Vector3(r.x + r.width * camInput.BL.x, r.y + r.height * camInput.BL.y);
                Vector3 v2 = new Vector3(r.x + r.width * camInput.BR.x, r.y + r.height * camInput.BR.y);
                Vector3 v3 = new Vector3(r.x + r.width * camInput.FR.x, r.y + r.height * camInput.FR.y);

                vh.AddTriangle(0, 1, 2);
                vh.AddTriangle(2, 3, 0);
            }
        }
    }
    

    protected override void OnDidApplyAnimationProperties()
    {
        SetMaterialDirty();
        SetVerticesDirty();
    }
}

/*
public class CamInputDisplay : MonoBehaviour
{
    public int camInputIndex = 0;

    RawImage rawImage;
    RectTransform rectTransform;
    AspectRatioFitter fitter;

    Image[,] pointObjects;
    bool on = false;

    public void Init()
    {
        
    }
    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();
        fitter = GetComponent<AspectRatioFitter>();
    }
    private void Update()
    {
        if (!CamInputManager.Instance.camInputs[camInputIndex].Active) return;
        if (on)
        {
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
            if (CamInputManager.Instance.camInputs.Length > camInputIndex)
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
}*/
public enum CamInputDisplayMode {Default, SolidColor, NoBaseColor}