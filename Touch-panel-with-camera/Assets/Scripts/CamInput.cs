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
    
    public bool[,] markings;//CamInputManager.resolution 바뀌었을때 크기가 바뀌어야함
    public int[,] vertexOfMarkings;//CamInputManager.resolution 바뀌었을때 크기가 바뀌어야함. -2는 공허를 의미. -1은 무주지를 의미. 그 외의 음수가 아닌 자연수는 해당 인덱스의 그룹의 영토임을 나타냄.
    public int vertexCount = 0;
    int[] groupArea;//길이는... CamInputManager.resolution 바뀌었을때 바뀌어야함. vertexOfMarkings구할때 사용함. List로 안하는 이유는 메모리용량을 희생해서 메모리 할당 오버헤드를 최소화 하려고
    public int lastGroupIndex;
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
    public Vector2Int[] markingPositionIndexsSortByY;//길이는... CamInputManager.resolution 바뀌었을때 바뀌어야함.  요소 정렬은....MarkingPositionsUpdate 실행될 때 실행.
    public int[,] markingPositionXs;//각 요소들은 텍스처상의 위치를 가짐. topLeft,topRight,bottomLeft,bottomRight나 CamInputManager.resolution, markingLength나 active, FocusX, FocusY, FocusPower가 바뀌었을때 바뀌어야함
    public int[,] markingPositionYs;//각 요소들은 텍스처상의 위치를 가짐. topLeft,topRight,bottomLeft,bottomRight나 CamInputManager.resolution, markingLength나 active, FocusX, FocusY, FocusPower가 바뀌었을때 바뀌어야함
    public Color32[,] baseColors;//바로 위와 같은 조건일때 얘도 바꿔야함. 단, 크기는 markings크기 바꿀때 같이 변경
    Color32[] camPixels;
    int camWidthCache;
    int camHeightCache;
    int markingLengthCache;
    public bool negativeWithBaseColor = false;
    public int allowedRedDeltaWithBaseColor = 25;//0~255
    public int allowedGreenDeltaWithBaseColor = 25;//0~255
    public int allowedBlueDeltaWithBaseColor = 25;//0~255
    public float allowedHueDeltaWithBaseColor = 0.1f;//0~0.5
    public float allowedSaturationDeltaWithBaseColor = 1f;//0~1
    public float allowedValueDeltaWithBaseColor = 1f;//0~1
    float focusX = 0f;
    public float FocusX
    {
        get
        {
            return focusX;
        }
        set
        {
            value = Mathf.Clamp(value, -0.5f, 0.5f);
            if (focusX != value)
            {
                focusX = value;
                MarkingPositionsUpdate();
            }
        }
    }
    float focusY = 0f;
    public float FocusY
    {
        get
        {
            return focusY;
        }
        set
        {
            value = Mathf.Clamp(value, -0.5f, 0.5f);
            if (focusY != value) 
            { 
                focusY = value;
                MarkingPositionsUpdate();
            }
        }
    }
    float focusPower = 1f;
    public float FocusPower
    {
        get
        {
            return focusPower;
        }
        set
        {
            if (focusPower!=value && value > 0f)
            {
                focusPower = value;
                MarkingPositionsUpdate();
            }
        }
    }
    public List<CustomColor> customColors = new List<CustomColor>();

    public MarkingsUpdateMethod markingsUpdateMethod = MarkingsUpdateMethod.CompareHSV;//언제든지 바뀌어도 됨

    public int resolutionRequestToCamWidth = 1;
    public int resolutionRequestToCamHeight = 1;
    public int removeSmallArea = 4;//0일시 비활성화
    public bool shadow = true;
    public int vertexArea = 5;

    public CamInput(WebCamTexture cam)
    {
        this.cam = cam;
        ResolutionChanged();
        MarkingLengthChanged();
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

            float[] xRatios = new float[lengthJ];
            for (int j = 0; j < lengthJ; j++)
            {
                xRatios[j] = (float)j / (lengthJ - 1);

                xRatios[j] = xRatios[j] * 2f - 1f;
                if (xRatios[j] > 0f)
                    xRatios[j] = Mathf.Pow(xRatios[j], focusPower);
                else if (xRatios[j] < 0f)
                    xRatios[j] = -(Mathf.Pow(-xRatios[j],focusPower));
                xRatios[j] = 0.5f + xRatios[j] * 0.5f;

                xRatios[j] += focusX * (1f - Mathf.Abs(0.5f - xRatios[j]) * 2f);
            }
            float[] yRatios = new float[lengthI];
            for (int i = 0; i < lengthI; i++)
            {
                yRatios[i] = (float)i / (lengthI - 1);

                yRatios[i] = yRatios[i] * 2f - 1f;
                if (yRatios[i] > 0f)
                    yRatios[i] = Mathf.Pow(yRatios[i], focusPower);
                else if (yRatios[i] < 0f)
                    yRatios[i] = -(Mathf.Pow(-yRatios[i], focusPower));
                yRatios[i] = 0.5f + yRatios[i] * 0.5f;

                yRatios[i] += focusY * (1f - Mathf.Abs(0.5f - yRatios[i]) * 2f);
            }

            //markingPositions = new Vector2Int[lengthJ, lengthI];
            markingPositionXs = new int[lengthJ, lengthI];
            markingPositionYs = new int[lengthJ, lengthI];

            Vector2 leftSide, rightSide, temp;
            for (int i = 0; i < lengthI; i++)
            {
                leftSide = Vector2.Lerp(frontLeftNotNormal, backLeftNotNormal, yRatios[i]);
                rightSide = Vector2.Lerp(frontRightNotNormal, backRightNotNormal, yRatios[i]);
                for (int j = 0; j < lengthJ; j++)
                {
                    temp = Vector2.Lerp(leftSide, rightSide, xRatios[j]);
                    //markingPositions[j, i].x = Mathf.RoundToInt(temp.x);
                    //markingPositions[j, i].y = Mathf.RoundToInt(temp.y);
                    markingPositionXs[j, i] = Mathf.RoundToInt(temp.x);
                    markingPositionYs[j, i] = Mathf.RoundToInt(temp.y);
                }
            }

            BaseColorsUpdate();
            MarkingPositionsChanged?.Invoke();

            int width = CamInputManager.Instance.Resolution.x;
            int height = CamInputManager.Instance.Resolution.y;
            System.Array.Sort<Vector2Int>(markingPositionIndexsSortByY, (a, b) => (markingPositionYs[a.x * markingLengthCache, a.y * markingLengthCache] < markingPositionYs[b.x * markingLengthCache, b.y * markingLengthCache]) ? -1 : 1);
        }
    }
    public Changed MarkingPositionsChanged;
    public void BaseColorsUpdate()
    {
        if (!Active)
            return;

        int lengthI = CamInputManager.Instance.Resolution.x;
        int lengthJ = CamInputManager.Instance.Resolution.y;
        Color32[] colors = new Color32[markingLengthCache * markingLengthCache];
        cam.GetPixels32(camPixels);
        //Vector2Int pos;
        int m;

        for (int i = 0; i < lengthI; i++)
        {
            for (int j = 0; j < lengthJ; j++)
            {
                m = 0;
                for (int k = 0; k < markingLengthCache; k++)
                {
                    for (int n = 0; n < markingLengthCache; n++)
                    {
                        //pos = markingPositions[i * markingLengthCache + k, j * markingLengthCache + n];
                        //colors[m] = camPixels[pos.x + pos.y* camWidthCache];
                        colors[m] = camPixels[markingPositionXs[i * markingLengthCache + k, j * markingLengthCache + n] + markingPositionYs[i * markingLengthCache + k, j * markingLengthCache + n] * camWidthCache];
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
        vertexOfMarkings = new int[CamInputManager.Instance.Resolution.x, CamInputManager.Instance.Resolution.y];
        baseColors = new Color32[CamInputManager.Instance.Resolution.x, CamInputManager.Instance.Resolution.y];

        int length = CamInputManager.Instance.Resolution.x * CamInputManager.Instance.Resolution.y;
        markingPositionIndexsSortByY = new Vector2Int[length];
        int lengthI = CamInputManager.Instance.Resolution.x;
        int lengthJ = CamInputManager.Instance.Resolution.y;
        int index = 0;
        for (int j = 0; j < lengthJ; j++)
            for (int i = 0; i < lengthI; i++)
            {
                markingPositionIndexsSortByY[index].x = i;
                markingPositionIndexsSortByY[index].y = j;
                index++;
            }
        groupArea = new int[length];
        MarkingPositionsUpdate();
    }
    public void MarkingLengthChanged()
    {
        markingLengthCache = CamInputManager.Instance.MarkingLength;
        MarkingPositionsUpdate();
    }

    public void MarkingsUpdate()//웹캠 텍스쳐가 업데이트 되었을때만 매니저에서 호출
    {
        int width = CamInputManager.Instance.Resolution.x;
        int height = CamInputManager.Instance.Resolution.y;

        Color32[] colors = new Color32[markingLengthCache * markingLengthCache];
        Color32 currentColor;
        //Vector2Int pos;
        int m;
        int customColorsLength = customColors.Count;
        CustomColor customColor;
        float h, s, v;
        float currentH, currentS, currentV;
        float deltaH;
        float deltaS;
        float deltaV;
        float deltaR;
        float deltaG;
        float deltaB;
        cam.GetPixels32(camPixels);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                m = 0;//현재 색구하기
                for (int k = 0; k < markingLengthCache; k++)
                {
                    for (int n = 0; n < markingLengthCache; n++)
                    {
                        //pos = markingPositions[i * markingLengthCache + k, j * markingLengthCache + n];
                        //colors[m] = camPixels[pos.x + pos.y *camWidthCache];
                        colors[m] = camPixels[markingPositionXs[i * markingLengthCache + k, j * markingLengthCache + n] + markingPositionYs[i * markingLengthCache + k, j * markingLengthCache + n] * camWidthCache];
                        m++;
                    }
                }
                currentColor = ColorAverage(colors);

                switch (markingsUpdateMethod)
                {
                    case MarkingsUpdateMethod.CompareHSV:
                        Color.RGBToHSV(baseColors[i,j], out h, out s, out v);
                        Color.RGBToHSV(currentColor,out currentH, out currentS, out currentV);
                        deltaH = currentH - h;

                        while (deltaH <= -0.5f)
                            deltaH += 1.0f;
                        while (deltaH > 0.5f)
                            deltaH -= 1.0f;

                        deltaS = currentS - s;

                        deltaV = currentV - v;

                        markings[i, j] = Mathf.Abs(deltaH) > allowedHueDeltaWithBaseColor || Mathf.Abs(deltaS) > allowedSaturationDeltaWithBaseColor || Mathf.Abs(deltaV) > allowedValueDeltaWithBaseColor;
                        break;
                    case MarkingsUpdateMethod.CompareRGB:
                        //deltaR = currentColor.r - baseColors[i, j].r;
                        //deltaG = currentColor.g - baseColors[i, j].g;
                        //deltaB = currentColor.b - baseColors[i, j].b;
                        //sqrRGBDistance = deltaR * deltaR + deltaG * deltaG + deltaB * deltaB;
                        //markings[i, j] = sqrRGBDistance > 0.03f;
                        deltaR = Mathf.Abs(currentColor.r - baseColors[i, j].r);
                        deltaG = Mathf.Abs(currentColor.g - baseColors[i, j].g);
                        deltaB = Mathf.Abs(currentColor.b - baseColors[i, j].b);
                        markings[i, j] = (deltaR > allowedRedDeltaWithBaseColor) || (deltaG > allowedGreenDeltaWithBaseColor) || (deltaB > allowedBlueDeltaWithBaseColor);
                        break;
                    case MarkingsUpdateMethod.None:
                        markings[i, j] = false;
                        break;
                    default:
                        break;
                }
                if (negativeWithBaseColor)
                    markings[i, j] = !markings[i, j];

                //baseColor를 이용한 마킹판정과는 or연산이므로 markings[i, j]가 false면 실행
                if (!markings[i, j])
                {
                    //각 색들도 서로 or이므로 true가 나오는 즉시 break
                    for (int k = 0; k < customColorsLength; k++)
                    {
                        customColor = customColors[k]; 

                        switch (customColor.markingsUpdateMethod)
                        {
                            case MarkingsUpdateMethod.CompareHSV:
                                Color.RGBToHSV(customColor.color, out h, out s, out v);
                                Color.RGBToHSV(currentColor, out currentH, out currentS, out currentV);
                                deltaH = currentH - h;

                                while (deltaH <= -0.5f)
                                    deltaH += 1.0f;
                                while (deltaH > 0.5f)
                                    deltaH -= 1.0f;

                                deltaS = currentS - s;

                                deltaV = currentV - v;

                                markings[i, j] = Mathf.Abs(deltaH) <= customColor.allowedHueDelta && Mathf.Abs(deltaS) <= customColor.allowedSaturationDelta && Mathf.Abs(deltaV) <= customColor.allowedValueDelta;
                                break;
                            case MarkingsUpdateMethod.CompareRGB:
                                deltaR = Mathf.Abs(currentColor.r - customColor.color.r);
                                deltaG = Mathf.Abs(currentColor.g - customColor.color.g);
                                deltaB = Mathf.Abs(currentColor.b - customColor.color.b);
                                markings[i, j] = (deltaR <= customColor.allowedRedDelta) && (deltaG <= customColor.allowedGreenDelta) && (deltaB <= customColor.allowedBlueDelta);
                                break;
                            case MarkingsUpdateMethod.None:
                                markings[i, j] = false;
                                break;
                            default:
                                break;
                        }
                        if (customColor.negative)
                            markings[i, j] = !markings[i, j];
                        if (markings[i, j])
                            break;
                    }
                }
            }
        }

        //꼭짓점 추출. 고스트터치 선별이 작동중이거나, 그림자 옵션이 켜져있을때 실행함.
        if (true)//Test //(CamInputManager.Instance.RemoveGhostActive || shadow)
        {
            //공허와 무주지 설정
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (markings[i,j])
                        vertexOfMarkings[i, j] = -1;//무주지
                    else
                        vertexOfMarkings[i, j] = -2;//공허
                }
            }

            int length = markingPositionIndexsSortByY.Length;
            lastGroupIndex = -1;
            //그룹별 영토 표시(이 시점에서 무주지(-1)는 완전히 사라지게됨)
            for (int i = 0; i < length; i++)
            {
                if (vertexOfMarkings[markingPositionIndexsSortByY[i].x, markingPositionIndexsSortByY[i].y] == -1)//무주지일경우 HalfFill 실행. 이때 각 영역별 넓이도 구함.
                {
                    lastGroupIndex++;
                    groupArea[lastGroupIndex] = 0;
                    HalfFillInVertexOfMarkings(lastGroupIndex, markingPositionIndexsSortByY[i].x, markingPositionIndexsSortByY[i].y,width,height, markingPositionYs[markingPositionIndexsSortByY[i].x * markingLengthCache, markingPositionIndexsSortByY[i].y * markingLengthCache]);
                }
            }
            //기준넓이에 미달하는 그룹들(단, 모서리에 닿은 경우 예외) 제거 또는 주변 그룹에 병합....을 구현할 자리지만 일단 다음 동작이 더 중요하므로 보류해둠

            //가장 큰 그룹 두 개 선정
            if (lastGroupIndex == -1)//그룹이 하나도 없는 경우
            {
                vertexCount = 0;
            }
            else if (lastGroupIndex == 0)//그룹이 단 하나인 경우 (이 경우 가장 큰 그룹의 인덱스는 반드시 0)
            {
                vertexCount = 1;
                int leftVertexArea = vertexArea;
                for (int i = 0; i < length; i++)
                {
                    if (vertexOfMarkings[markingPositionIndexsSortByY[i].x, markingPositionIndexsSortByY[i].y] == 0)//그룹의 영역중 y값이 작은 순서대로 leftVertexArea만큼 남기고 나머지 영역은 공허로.
                    {
                        if(leftVertexArea > 0)
                            leftVertexArea--;
                        else
                            vertexOfMarkings[markingPositionIndexsSortByY[i].x, markingPositionIndexsSortByY[i].y] = -2;
                    }
                }
            }
            else//그룹이 둘 이상인 경우
            {
                vertexCount = 2;
            }
        }

        //그림자옵션구현해야함
    }
    int[][] add4HalfFill =
    {
        new int[]{-1,-1 },
        new int[]{0,-1 },
        new int[]{1,-1 },
        new int[]{-1,0 },
        new int[]{1,0 },
        new int[]{-1,1 },
        new int[]{0,1 },
        new int[]{1,1 }
    };
    void HalfFillInVertexOfMarkings(int groupIndex,int x,int y,int width ,int height,float limitY)//무주지에만 실행해라..
    {
        vertexOfMarkings[x, y] = groupIndex;//이 곳을 주어진 그룹의 영토로.
        groupArea[groupIndex]++;

        int nextX, nextY;//주변의 픽셀들을 y값이 낮은 순서대로 정렬
        float aY, bY;
        System.Array.Sort<int[]>(add4HalfFill,delegate(int[] a,int[] b)
        {
            nextX = x + a[0];
            nextY = y + a[1];
            if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height)//배열 밖으로 나가면 무조건 얘가 앞
            {
                return -1;
            }
            aY = markingPositionYs[nextX * markingLengthCache, nextY * markingLengthCache];

            nextX = x + b[0];
            nextY = y + b[1];
            if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height)//배열 밖으로 나가면 무조건 얘가 앞
            {
                return 1;
            }
            bY = markingPositionYs[nextX * markingLengthCache, nextY * markingLengthCache];

            return (aY < bY) ? -1 : 1;
        });

        for (int i = 0; i < 8; i++)
        {
            nextX = x + add4HalfFill[i][0];
            nextY = y + add4HalfFill[i][1];
            if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height)//배열 밖으로 나가면 안됨
            {
                limitY = markingPositionYs[x*markingLengthCache, y * markingLengthCache];
                continue;
            }

            if (markingPositionYs[nextX * markingLengthCache, nextY * markingLengthCache] > limitY-1f)//높이 제한에 안걸림.
            {
                if (vertexOfMarkings[nextX, nextY] == -1)//무주지라면 HalfFill 실행
                {
                    HalfFillInVertexOfMarkings(groupIndex, nextX, nextY, width, height, limitY);
                }
                else//높이제한에 안걸리는데....무주지도 아니고(즉 장애물이면) limitY를 갱신
                {
                    limitY = markingPositionYs[nextX * markingLengthCache, nextY * markingLengthCache];
                }
            }
        }
    }
}
public class CustomColor
{
    public Color32 color;

    public bool negative = false;
    public int allowedRedDelta = 25;//0~255
    public int allowedGreenDelta = 25;//0~255
    public int allowedBlueDelta = 25;//0~255
    public float allowedHueDelta = 0.1f;//0~0.5
    public float allowedSaturationDelta = 0.8f;//0~1
    public float allowedValueDelta = 1f;//0~1
    public MarkingsUpdateMethod markingsUpdateMethod = MarkingsUpdateMethod.CompareHSV;//언제든지 바뀌어도 됨

}
public enum MarkingsUpdateMethod
{
    CompareHSV, CompareRGB, None
}