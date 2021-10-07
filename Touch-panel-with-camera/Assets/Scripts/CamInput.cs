using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamInput
{
    WebCamTexture camTexture;
    public WebCamTexture Texture
    {
        get
        {
            return camTexture;
        }
    }
    
    public bool[,] markings = new bool[0,0];
    Vector2 frontLeft, frontRight, backLeft,backRight;//0~1사이의 값으로 각 꼭짓점의 위치를 나타냄.
    public Vector2Int[,] markingPositions;//각 요소들은 텍스처상의 위치를 가짐. 따라서 텍스쳐가 바뀌면 얘네도 바뀌어야함.
                               //topLeft,topRight,bottomLeft,bottomRight가 바뀌었을때도 바뀌어야함
    public Color[,] baseColors;

    public MarkingsUpdateMethod markingsUpdateMethod = MarkingsUpdateMethod.CompareHRGB;//언제든지 바뀌어도 됨

    public void Init(string deviceName, Vector2 fL, Vector2 fR, Vector2 bL, Vector2 bR)
    {
        if(camTexture != null)
        {
            camTexture.Stop();
        }

        camTexture = new WebCamTexture(deviceName);
        if(!camTexture.isPlaying)
            camTexture.Play();
        frontLeft = fL;
        frontRight = fR;
        backLeft = bL;
        backRight = bR;

        MarkingPositionsUpdate();
        BaseColorsUpdate();
        markings = new bool[CamInputManager.Instance.Resolution.x, CamInputManager.Instance.Resolution.y];
    }
    public void Init(WebCamTexture webcamTexture, Vector2 fL, Vector2 fR, Vector2 bL, Vector2 bR)
    {
        camTexture = webcamTexture;
        if (!camTexture.isPlaying)
            camTexture.Play();
        frontLeft = fL;
        frontRight = fR;
        backLeft = bL;
        backRight = bR;

        MarkingPositionsUpdate();
        BaseColorsUpdate();
        markings = new bool[CamInputManager.Instance.Resolution.x, CamInputManager.Instance.Resolution.y];
    }

    public void MarkingsUpdate()//아마도 매 프레임마다 호출?
    {
        int width = CamInputManager.Instance.Resolution.x;
        int height = CamInputManager.Instance.Resolution.y;
        int markingLength = CamInputManager.Instance.MarkingLength;
        if (markings.GetLength(0) != width || markings.GetLength(1) != height)
        {
            markings = new bool[width, height];
        }

        Color[] colors = new Color[markingLength * markingLength];
        Color currentColor;
        Vector2Int pos;
        int m;
        float h, s, v;
        float currentH, currentS, currentV;
        float deltaH;
        float deltaS;
        float deltaR;
        float deltaG;
        float deltaB;
        float sqrRGBDistance;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                m = 0;//현재 색구하기
                for (int k = 0; k < markingLength; k++)
                {
                    for (int n = 0; n < markingLength; n++)
                    {
                        pos = markingPositions[i * markingLength + k, j * markingLength + n];
                        colors[m] = camTexture.GetPixel(pos.x, pos.y);
                        m++;
                    }
                }
                currentColor = ColorAverage(colors);

                switch (markingsUpdateMethod)
                {
                    case MarkingsUpdateMethod.CompareH:
                        Color.RGBToHSV(baseColors[i,j], out h, out s, out v);
                        Color.RGBToHSV(currentColor,out currentH, out currentS, out currentV);
                        deltaH = currentH - h;
                        
                        while (deltaH < -0.1f)
                            deltaH += 1.0f;
                        while (deltaH > 0.1f)
                            deltaH -= 1.0f;
                        markings[i, j] = Mathf.Abs(deltaH) > 0.1f;
                        break;
                    case MarkingsUpdateMethod.CompareRGB:
                        deltaR = currentColor.r - baseColors[i, j].r;
                        deltaG = currentColor.g - baseColors[i, j].g;
                        deltaB = currentColor.b - baseColors[i, j].b;
                        sqrRGBDistance = deltaR * deltaR + deltaG * deltaG + deltaB * deltaB;
                        markings[i, j] = sqrRGBDistance > 0.03f;
                        break;
                    case MarkingsUpdateMethod.CompareHRGB:
                        Color.RGBToHSV(baseColors[i, j], out h, out s, out v);
                        Color.RGBToHSV(currentColor, out currentH, out currentS, out currentV);
                        deltaH = currentH - h;

                        while (deltaH < -0.1f)
                            deltaH += 1.0f;
                        while (deltaH > 0.1f)
                            deltaH -= 1.0f;
                        
                        deltaR = currentColor.r - baseColors[i, j].r;
                        deltaG = currentColor.g - baseColors[i, j].g;
                        deltaB = currentColor.b - baseColors[i, j].b;
                        sqrRGBDistance = deltaR * deltaR + deltaG * deltaG + deltaB * deltaB;

                        markings[i, j] = Mathf.Abs(deltaH) > 0.1f && sqrRGBDistance > 0.03f;
                        break;
                    case MarkingsUpdateMethod.CompareS:
                        Color.RGBToHSV(baseColors[i, j], out h, out s, out v);
                        Color.RGBToHSV(currentColor, out currentH, out currentS, out currentV);
                        deltaS = currentS - s;

                        markings[i, j] = Mathf.Abs(deltaS) > 0.1f;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void BaseColorsUpdate()
    {
        baseColors = new Color[CamInputManager.Instance.Resolution.x, CamInputManager.Instance.Resolution.y];

        int lengthI = CamInputManager.Instance.Resolution.x;
        int lengthJ = CamInputManager.Instance.Resolution.y;
        int markingLength = CamInputManager.Instance.MarkingLength;

        Color[] colors = new Color[markingLength * markingLength];
        Vector2Int pos;
        int m;

        for (int i = 0; i < lengthI; i++)
        {
            for (int j = 0; j < lengthJ; j++)
            {
                m = 0;
                for (int k = 0; k < markingLength; k++)
                {
                    for (int n = 0; n < markingLength; n++)
                    {
                        pos = markingPositions[i* markingLength + k,j* markingLength + n];
                        colors[m] = camTexture.GetPixel(pos.x, pos.y);
                        m++;
                    }
                }
                baseColors[i,j] = ColorAverage(colors);
            }
        }
        /*
        int markingLength = CamInputManager.Instance.MarkingLength;
        int lengthI = CamInputManager.Instance.InputResolution.x * markingLength;
        int lengthJ = CamInputManager.Instance.InputResolution.y * markingLength;

        markingPositions = new Vector2Int[lengthI,lengthJ];

        for (int i = 0; i < lengthI; i += markingLength)
        {
            for (int j = 0; j < lengthJ; j += markingLength)
            {
                for (int k = 0; k < markingLength; k++)
                {
                    for (int m = 0; m < markingLength; m++)
                    {

                    }
                }
            }
        }*/
    }
    void MarkingPositionsUpdate()
    {

        int maxX = camTexture.width - 1;
        int maxY = camTexture.height - 1;
        Vector2 frontLeftNotNormal = new Vector2(maxX * frontLeft.x, maxY * frontLeft.y);
        Vector2 frontRightNotNormal = new Vector2(maxX * frontRight.x, maxY * frontRight.y);
        Vector2 backLeftNotNormal = new Vector2(maxX * backLeft.x, maxY * backLeft.y);
        Vector2 backRightNotNormal = new Vector2(maxX * backRight.x, maxY * backRight.y);

        int lengthI = CamInputManager.Instance.Resolution.y * CamInputManager.Instance.MarkingLength;
        int lengthJ = CamInputManager.Instance.Resolution.x * CamInputManager.Instance.MarkingLength;
       
       markingPositions = new Vector2Int[lengthJ, lengthI];

        Vector2 leftSide, rightSide, temp;
        float iRatio;
        for (int i = 0; i < lengthI; i++)
        {
            iRatio = (float)i / (lengthI - 1);
            leftSide = Vector2.Lerp(frontLeftNotNormal, backLeftNotNormal, iRatio);
            rightSide = Vector2.Lerp(frontRightNotNormal, backRightNotNormal, iRatio);
            for (int j = 0; j < lengthJ; j++)
            {
                temp = Vector2.Lerp(leftSide, rightSide, (float)j / (lengthJ - 1));
                markingPositions[j, i].x = Mathf.RoundToInt(temp.x);
                markingPositions[j, i].y = Mathf.RoundToInt(temp.y);
            }
        }
    }
    Color ColorAverage(params Color[] colors)
    {
        float r = 0f, g = 0f, b = 0f;
        for (int i = 0; i < colors.Length; i++)
        {
            r += colors[i].r;
            g += colors[i].g;
            b += colors[i].b;
        }
        float inverseLength = 1f / colors.Length;
        r *= inverseLength;
        g *= inverseLength;
        b *= inverseLength;

        return new Color(r, g, b);
    }
}

public enum MarkingsUpdateMethod
{
    CompareH, CompareRGB, CompareHRGB, CompareS
}