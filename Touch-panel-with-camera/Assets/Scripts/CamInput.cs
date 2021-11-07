using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamInput
{
    bool active = false;
    public bool Active
    {
        get
        {
            return active;
        }
        set
        {
            if (active != value)
            {
                active = value;
                //active 변경시 실행할 코드
                if (active)
                {
                    if(!cam.isPlaying) cam.Play();
                    camPixels = new Color32[cam.width * cam.height];
                    camWidthCache = cam.width;
                    camHeightCache = cam.height;
                    MarkingPositionsUpdate();
                }
                else
                {
                    if (cam.isPlaying) cam.Stop();
                }
                ActiveChanged?.Invoke();
            }
        }
    }
    public Changed ActiveChanged;
    bool bezier = false;//현재 베지어는 보류... 이게 아닌것같다....
    public bool Bezier
    {
        get
        {
            return bezier;
        }
        set
        {
            if (bezier != value)
            {
                bezier = value;
                //bezier 변경시 실행할 코드
                MarkingPositionsUpdate();
            }
        }
    }
    WebCamTexture cam;//생성된 이후 이게 바뀔 일은 없음
    public WebCamTexture Texture
    {
        get
        {
            return cam;
        }
    }
    
    public bool[,] markings;//CamInputManager.resolution 바뀌었을때 바뀌어야함
    Vector2 frontLeft = new Vector2(0f,1f), frontRight = new Vector2(1f, 1f), backLeft = new Vector2(0f, 0f), backRight = new Vector2(1f, 0f);//0~1사이의 값으로 각 꼭짓점의 위치를 나타냄.
    public Vector2 FL
    {
        get
        {
            return frontLeft;
        }
        set
        {
            Vector2 temp = frontLeft;
            frontLeft.x = Mathf.Clamp01(value.x);
            frontLeft.y = Mathf.Clamp01(value.y);
            if (!temp.Equals(frontLeft)) MarkingPositionsUpdate();
        }
    }
    public Vector2 FR
    {
        get
        {
            return frontRight;
        }
        set
        {
            Vector2 temp = frontRight;
            frontRight.x = Mathf.Clamp01(value.x);
            frontRight.y = Mathf.Clamp01(value.y);
            if (!temp.Equals(frontRight)) MarkingPositionsUpdate();
        }
    }
    public Vector2 BL
    {
        get
        {
            return backLeft;
        }
        set
        {
            Vector2 temp = backLeft;
            backLeft.x = Mathf.Clamp01(value.x);
            backLeft.y = Mathf.Clamp01(value.y);
            if (!temp.Equals(backLeft)) MarkingPositionsUpdate();
        }
    }
    public Vector2 BR
    {
        get
        {
            return backRight;
        }
        set
        {
            Vector2 temp = backRight;
            backRight.x = Mathf.Clamp01(value.x);
            backRight.y = Mathf.Clamp01(value.y);
            if (!temp.Equals(backRight)) MarkingPositionsUpdate();
        }
    }
    Vector2 fl2fr = new Vector2(0.25f, 0f), fl2bl = new Vector2(0f, -0.25f),
        fr2fl = new Vector2(-0.25f, 0f), fr2br = new Vector2(0f, -0.25f),
        bl2br = new Vector2(0.25f, 0f), bl2fl = new Vector2(0f, 0.25f),
        br2bl = new Vector2(-0.25f, 0f), br2fr = new Vector2(0f, 0.25f);//A2B A로부터의 상대적인 위치를 설정
    public Vector2 FL2FR
    {
        get
        {
            return fl2fr;
        }
        set
        {
            if (!fl2fr.Equals(value))
            {
                fl2fr = value;
                if(bezier) MarkingPositionsUpdate();
            }
        }
    }
    public Vector2 FL2BL
    {
        get
        {
            return fl2bl;
        }
        set
        {
            if (!fl2bl.Equals(value))
            {
                fl2bl = value;
                if (bezier) MarkingPositionsUpdate();
            }
        }
    }
    public Vector2 FR2FL
    {
        get
        {
            return fr2fl;
        }
        set
        {
            if (!fr2fl.Equals(value))
            {
                fr2fl = value;
                if (bezier) MarkingPositionsUpdate();
            }
        }
    }
    public Vector2 FR2BR
    {
        get
        {
            return fr2br;
        }
        set
        {
            if (!fr2br.Equals(value))
            {
                fr2br = value;
                if (bezier) MarkingPositionsUpdate();
            }
        }
    }
    public Vector2 BL2BR
    {
        get
        {
            return bl2br;
        }
        set
        {
            if (!bl2br.Equals(value))
            {
                bl2br = value;
                if (bezier) MarkingPositionsUpdate();
            }
        }
    }
    public Vector2 BL2FL
    {
        get
        {
            return bl2fl;
        }
        set
        {
            if (!bl2fl.Equals(value))
            {
                bl2fl = value;
                if (bezier) MarkingPositionsUpdate();
            }
        }
    }
    public Vector2 BR2BL
    {
        get
        {
            return br2bl;
        }
        set
        {
            if (!br2bl.Equals(value))
            {
                br2bl = value;
                if (bezier) MarkingPositionsUpdate();
            }
        }
    }
    public Vector2 BR2FR
    {
        get
        {
            return br2fr;
        }
        set
        {
            if (!br2fr.Equals(value))
            {
                br2fr = value;
                if (bezier) MarkingPositionsUpdate();
            }
        }
    }
    //public Vector2Int[,] markingPositions;//각 요소들은 텍스처상의 위치를 가짐. topLeft,topRight,bottomLeft,bottomRight나 CamInputManager.resolution, markingLength나 active가 바뀌었을때 바뀌어야함
    public int[,] markingPositionXs;//각 요소들은 텍스처상의 위치를 가짐. topLeft,topRight,bottomLeft,bottomRight나 CamInputManager.resolution, markingLength나 active가 바뀌었을때 바뀌어야함
    public int[,] markingPositionYs;//각 요소들은 텍스처상의 위치를 가짐. topLeft,topRight,bottomLeft,bottomRight나 CamInputManager.resolution, markingLength나 active가 바뀌었을때 바뀌어야함
    public Color32[,] baseColors;//바로 위와 같은 조건일때 얘도 바꿔야함. 단, 크기는 markings크기 바꿀때 같이 변경
    Color32[] camPixels;
    int camWidthCache;
    int camHeightCache;

    public MarkingsUpdateMethod markingsUpdateMethod = MarkingsUpdateMethod.CompareHRGB;//언제든지 바뀌어도 됨

    public CamInput(WebCamTexture cam)
    {
        this.cam = cam;
        ResolutionChanged();
    }
    public void MarkingPositionsUpdate()
    {
        if (active)
        {
            int maxX = camWidthCache - 1;
            int maxY = camHeightCache - 1;
            Vector2 frontLeftNotNormal = new Vector2(maxX * frontLeft.x, maxY * frontLeft.y);
            Vector2 frontRightNotNormal = new Vector2(maxX * frontRight.x, maxY * frontRight.y);
            Vector2 backLeftNotNormal = new Vector2(maxX * backLeft.x, maxY * backLeft.y);
            Vector2 backRightNotNormal = new Vector2(maxX * backRight.x, maxY * backRight.y);

            int lengthI = CamInputManager.Instance.Resolution.y * CamInputManager.Instance.MarkingLength;
            int lengthJ = CamInputManager.Instance.Resolution.x * CamInputManager.Instance.MarkingLength;

            //markingPositions = new Vector2Int[lengthJ, lengthI];
            markingPositionXs = new int[lengthJ, lengthI];
            markingPositionYs = new int[lengthJ, lengthI];

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
                    //markingPositions[j, i].x = Mathf.RoundToInt(temp.x);
                    //markingPositions[j, i].y = Mathf.RoundToInt(temp.y);
                    markingPositionXs[j, i] = Mathf.RoundToInt(temp.x);
                    markingPositionYs[j, i] = Mathf.RoundToInt(temp.y);
                }
            }

            BaseColorsUpdate();
            MarkingPositionsChanged?.Invoke();
        }
    }
    public Changed MarkingPositionsChanged;
    public void BaseColorsUpdate()
    {
        int lengthI = CamInputManager.Instance.Resolution.x;
        int lengthJ = CamInputManager.Instance.Resolution.y;
        int markingLength = CamInputManager.Instance.MarkingLength;
        Color32[] colors = new Color32[markingLength * markingLength];
        cam.GetPixels32(camPixels);
        //Vector2Int pos;
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
                        //pos = markingPositions[i * markingLength + k, j * markingLength + n];
                        //colors[m] = camPixels[pos.x + pos.y* camWidthCache];
                        colors[m] = camPixels[markingPositionXs[i * markingLength + k, j * markingLength + n] + markingPositionYs[i * markingLength + k, j * markingLength + n] * camWidthCache];
                        m++;
                    }
                }
                baseColors[i, j] = ColorAverage(colors);
            }
        }
    }
    Color32 ColorAverage(params Color32[] colors)
    {
        int r = 0, g = 0, b = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            r += colors[i].r;
            g += colors[i].g;
            b += colors[i].b;
        }
        r /= colors.Length;
        g /= colors.Length;
        b /= colors.Length;
        
        return new Color32((byte)r, (byte)g, (byte)b, 255);
    }
    Vector2 PointOnBezierCurve3(float t,Vector2 start,Vector2 p1, Vector2 p2, Vector3 dst)
    {
        Vector2 a1 = Vector2.Lerp(start, p1, t);
        Vector2 b1 = Vector2.Lerp(p1, p2, t);
        Vector2 c1 = Vector2.Lerp(p2, dst, t);

        Vector2 a2 = Vector2.Lerp(a1,b1,t);
        Vector2 b2 = Vector2.Lerp(b1,c1,t);

        return Vector2.Lerp(a2, b2, t);
    }
    public void ResolutionChanged()
    {
        markings = new bool[CamInputManager.Instance.Resolution.x, CamInputManager.Instance.Resolution.y];
        baseColors = new Color32[CamInputManager.Instance.Resolution.x, CamInputManager.Instance.Resolution.y];
        MarkingPositionsUpdate();
    }
    public void MarkingLengthChanged()
    {
        MarkingPositionsUpdate();
    }

    public void MarkingsUpdate()//웹캠 텍스쳐가 업데이트 되었을때만 매니저에서 호출
    {
        int width = CamInputManager.Instance.Resolution.x;
        int height = CamInputManager.Instance.Resolution.y;
        int markingLength = CamInputManager.Instance.MarkingLength;
        if (markings.GetLength(0) != width || markings.GetLength(1) != height)
        {
            markings = new bool[width, height];
        }

        Color32[] colors = new Color32[markingLength * markingLength];
        Color32 currentColor;
        //Vector2Int pos;
        int m;
        float h, s, v;
        float currentH, currentS, currentV;
        float deltaH;
        float deltaS;
        float deltaR;
        float deltaG;
        float deltaB;
        float sqrRGBDistance;
        cam.GetPixels32(camPixels);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                m = 0;//현재 색구하기
                for (int k = 0; k < markingLength; k++)
                {
                    for (int n = 0; n < markingLength; n++)
                    {
                        //pos = markingPositions[i * markingLength + k, j * markingLength + n];
                        //colors[m] = camPixels[pos.x + pos.y *camWidthCache];
                        colors[m] = camPixels[markingPositionXs[i * markingLength + k, j * markingLength + n] + markingPositionYs[i * markingLength + k, j * markingLength + n] * camWidthCache];
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

}

public enum MarkingsUpdateMethod
{
    CompareH, CompareRGB, CompareHRGB, CompareS
}