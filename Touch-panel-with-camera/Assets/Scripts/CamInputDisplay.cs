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
    int textureWidthCache;
    int textureHeightCache;
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
        textureWidthCache = markingTexture.width;
        textureHeightCache = markingTexture.height;
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
            for (int i = 0; i < textureWidthCache; i++)
            {
                for (int j = 0; j < textureHeightCache; j++)
                {
                    if (mode == CamInputDisplayMode.SolidColor)
                    {
                        switch (index)
                        {
                            case 0:
                                colors[i + textureWidthCache * j] = Color.red;
                                break;
                            case 1:
                                colors[i + textureWidthCache * j] = Color.green;
                                break;
                            case 2:
                                colors[i + textureWidthCache * j] = Color.blue;
                                break;
                            case 3:
                                colors[i + textureWidthCache * j] = Color.cyan;
                                break;
                            case 4:
                                colors[i + textureWidthCache * j] = Color.magenta;
                                break;
                            case 5:
                                colors[i + textureWidthCache * j] = Color.yellow;
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
                                    colors[i + textureWidthCache * j] = Color.red;
                                    break;
                                case 1:
                                    colors[i + textureWidthCache * j] = Color.green;
                                    break;
                                case 2:
                                    colors[i + textureWidthCache * j] = Color.blue;
                                    break;
                                case 3:
                                    colors[i + textureWidthCache * j] = Color.cyan;
                                    break;
                                case 4:
                                    colors[i + textureWidthCache * j] = Color.magenta;
                                    break;
                                case 5:
                                    colors[i + textureWidthCache * j] = Color.yellow;
                                    break;
                            }
                        }
                        else
                        {
                            if (mode == CamInputDisplayMode.Default)
                            {
                                colors[i + textureWidthCache * j] = camInput.baseColors[i, j];//기준 색 설정
                            }
                            else
                            {
                                colors[i + textureWidthCache * j] = Color.clear;//기준 색 설정
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
public enum CamInputDisplayMode {Default, SolidColor, NoBaseColor}