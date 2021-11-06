using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(CanvasRenderer))]
public class CamInputDisplay : MaskableGraphic
{
    [SerializeField] Texture m_Texture;

    protected CamInputDisplay()
    {
        useLegacyMeshGeneration = false;
    }
    public void Init(int camInputIndex)
    {

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
        if (tex != null)
        {
            var r = GetPixelAdjustedRect();
            var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
            {
                var color32 = color;
                vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(0f, 0f));
                vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(0f, 1f));
                vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(1f, 1f));
                vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(1f, 0f));

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
