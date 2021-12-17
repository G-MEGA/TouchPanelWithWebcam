using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class CamInputManager : MonoBehaviour
{
    Vector2Int resolution = new Vector2Int(60, 60);//최소 (2,2)
    int width, height;
    public Vector2Int Resolution
    {
        get
        {
            return resolution;
        }
        set
        {
            Vector2Int temp = resolution;
            resolution = value;
            if (resolution.x < 2)
                resolution.x = 2;
            if (resolution.y < 2)
                resolution.y = 2;
            if(temp != resolution)
            {
                width = resolution.x;
                height = resolution.y;
                markingCount = new int[width, height];
                marking = new int[width, height];
                prevMarking = new int[width, height];
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        prevMarking[i, j] = -2;
                touches = new TouchInfo[width * height];
                prevTouches = new TouchInfo[touches.Length];
                prevTouchesCheck4Update = new bool[touches.Length];
                isUsedPrevTouch = new bool[touches.Length];
                isGrayTouch = new bool[touches.Length];
                touchesLastIndex = -1;
                prevTouchesLastIndex = -1;

                //해상도 변경시 실행
                if (camInputs != null)
                {
                    for (int i = 0; i < camInputs.Length; i++)
                    {
                        camInputs[i].ResolutionChanged();
                    }
                }

                ResolutionChanged?.Invoke();
            }
        }
    }
    public Changed ResolutionChanged;
    int markingLength = 3;//최소 1
    public int MarkingLength
    {
        get
        {
            return markingLength;
        }
        set
        {
            int temp = markingLength;
            if (value < 1)
                markingLength = 1;
            else
                markingLength = value;
            if(temp != markingLength)
            {
                //마킹길이 변경시 실행
                if (camInputs != null)
                {
                    for (int i = 0; i < camInputs.Length; i++)
                    {
                        camInputs[i].MarkingLengthChanged();
                    }
                }
            }
        }
    }
    public bool removeGhostWhen2Cam = true;//활성화된 카메라가 2대밖에 없을 때 고스트 터치 제거를 실행하는가
    public bool RemoveGhostActive
    {
        get
        {
            return removeGhostWhen2Cam && ActiveCamInputCount == 2;
        }
    }
    int minTouchArea = 5;
    public int MinTouchArea
    {
        get
        {
            return minTouchArea;
        }
        set
        {
            minTouchArea = value;
        }
    }

    public int[,] markingCount;
    public int[,] marking;//-2는 공허를 의미. -1은 무주지를 의미. 그 외의 음수가 아닌 자연수는 해당 인덱스의 터치의 영토임을 나타냄.
    int[,] prevMarking;
    public TouchInfo[] touches;
    TouchInfo[] prevTouches;
    public int touchesLastIndex;
    int prevTouchesLastIndex;
    bool[] prevTouchesCheck4Update;
    public CamInput[] camInputs;
    public int ActiveCamInputCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < camInputs.Length; i++)
                if (camInputs[i].Active) count++;
            return count;
        }
    }
    public bool[] camInputsUpdated;
    public WebCamTexture[] webCams;
    public string[] webCamNames;
    bool[] camInputActiveSaveWhenSceneChange;

    private static CamInputManager instance = null;
    public static CamInputManager Instance
    {
        get
        {
            return instance;
        }
    }

    CamInputManager()
    {
        width = resolution.x;
        height = resolution.y;
        markingCount = new int[width, height];
        marking = new int[width, height];
        prevMarking = new int[width, height];
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                prevMarking[i, j] = -2;
        touches = new TouchInfo[width * height];
        prevTouches = new TouchInfo[touches.Length];
        prevTouchesCheck4Update = new bool[touches.Length];
        isUsedPrevTouch = new bool[touches.Length];
        isGrayTouch = new bool[touches.Length];
        touchesLastIndex = -1;
        prevTouchesLastIndex = -1;
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    void Start()
    {
        WebCamDevice[] webCamDevices = WebCamTexture.devices;
        webCams = new WebCamTexture[webCamDevices.Length];
        webCamNames = new string[webCamDevices.Length];
        camInputs = new CamInput[webCamDevices.Length];
        camInputsUpdated = new bool[camInputs.Length];
        camInputActiveSaveWhenSceneChange = new bool[camInputs.Length];

        for (int i = 0; i < webCamDevices.Length; i++)
        {
            webCams[i] = new WebCamTexture(webCamDevices[i].name);
            webCamNames[i] = webCamDevices[i].name;
            camInputs[i] = new CamInput(webCams[i]);

            webCams[i].requestedWidth = 640;//웹캠의 기본 선호 해상도
            webCams[i].requestedHeight = 360;
        }
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        camInputCount = camInputs.Length;
        for (int i = 0; i < camInputCount; i++)
            camInputs[i].Active = camInputActiveSaveWhenSceneChange[i];
    }
    private void SceneManager_sceneUnloaded(Scene arg0)
    {
        camInputCount = camInputs.Length;
        for (int i = 0; i < camInputCount; i++)
        {
            camInputActiveSaveWhenSceneChange[i] = camInputs[i].Active;
            if (camInputs[i].ActiveChanged != null)
                foreach (Changed d in camInputs[i].ActiveChanged.GetInvocationList())
                {
                    camInputs[i].ActiveChanged -= d;
                }
            camInputs[i].Active = false;
        }
    }

    int[,] markingSwap;
    TouchInfo[] touchesSwap;
    int touchesLastIndexSwap;
    int camInputCount;
    int camInputsUpdatedCount = 0;
    bool[] isUsedPrevTouch;
    bool[] isGrayTouch;
    float minDistanceSqr;
    int minDistanceIndex;
    float distanceSqr;
    private void Update()
    {
        camInputCount = camInputs.Length;
        for (int i = 0; i < camInputCount; i++)
            if (camInputs[i].Active && webCams[i].didUpdateThisFrame)
            {
                camInputs[i].MarkingsUpdate();
                if (!camInputsUpdated[i])
                {
                    camInputsUpdated[i] = true;
                    camInputsUpdatedCount++;
                }
            }

        if (camInputsUpdatedCount >= ActiveCamInputCount)//모든 웹캠이 한번이상씩 업데이트 되었을 때 실행
        {
            for (int i = 0; i < camInputCount; i++)
                camInputsUpdated[i] = false;
            camInputsUpdatedCount = 0;

            markingSwap = marking;
            marking = prevMarking;
            prevMarking = markingSwap;

            touchesSwap = touches;
            touches = prevTouches;
            prevTouches = touchesSwap;

            prevTouchesLastIndex = touchesLastIndex;
            touchesLastIndex = -1;

            int caminputMarkingCount;
            int activeCaminputCount = ActiveCamInputCount;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    caminputMarkingCount = 0;
                    for (int k = 0; k < camInputCount; k++)
                        if (camInputs[k].Active&& camInputs[k].markings[i, j])
                            caminputMarkingCount++;
                    markingCount[i, j] = caminputMarkingCount;

                    if (caminputMarkingCount == activeCaminputCount)
                        marking[i, j] = -1;
                    else
                        marking[i, j] = -2;
                }
            }
            int touchArea;

            if (RemoveGhostActive)
            {
                //액티브 상태인 카메라 정리
                CamInput camInput0 = null;
                CamInput camInput1 = null;
                for (int i = 0; i < camInputCount; i++)
                {
                    if (camInputs[i].Active)
                    {
                        if (camInput0 == null)
                        {
                            camInput0 = camInputs[i];
                        }
                        else if (camInput1 == null)
                        {
                            camInput1 = camInputs[i];
                            break;
                        }
                    }
                }
                //터치검출
                for (int i = 0; i <= prevTouchesLastIndex; i++)//이전 터치들 중에서 화이트만 골라서 미사용 상태로, 고스트들은 어차피 안쓸거니까 사용됨 상태로
                {
                    isUsedPrevTouch[i] = prevTouches[i].isGhost;//어차피 이전 터치들은 고스트 아니면 화이트다.
                }
                bool touch0Vertex;
                bool touch1Vertex;
                if (camInput0.VertexCount >= 2 &&  camInput1.VertexCount>=2)
                {
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            if (marking[i, j] == -1)
                            {
                                touchesLastIndex++;
                                for (int k = 0; k <= prevTouchesLastIndex; k++)
                                    prevTouchesCheck4Update[k] = false;

                                //touches[touchesLastIndex].isGhost = false;//어차피 밑에서 확실하게 정해짐
                                touches[touchesLastIndex].isDownThisFrame = true;
                                touches[touchesLastIndex].moveBack = false;
                                touches[touchesLastIndex].moveFront = false;
                                touches[touchesLastIndex].moveLeft = false;
                                touches[touchesLastIndex].moveRight = false;
                                touches[touchesLastIndex].minX = int.MaxValue;//얘네는 FillGray가 처리
                                touches[touchesLastIndex].maxX = int.MinValue;
                                touches[touchesLastIndex].minY = int.MaxValue;
                                touches[touchesLastIndex].maxY = int.MinValue;

                                touch0Vertex = false;
                                touch1Vertex = false;
                                touchArea = 0;
                                FillGray(touchesLastIndex, i, j, camInput0, camInput1, ref touch0Vertex, ref touch1Vertex, ref touchArea);
                                if (touchArea < minTouchArea)//최소 터치 크기 미달로 터치 판정 취소
                                {
                                    FillVoid(touchesLastIndex, i, j);
                                    touchesLastIndex--;
                                    continue;
                                }

                                if (!touch0Vertex || !touch1Vertex)//고스트 확정
                                {
                                    touches[touchesLastIndex].isGhost = true;
                                    isGrayTouch[touchesLastIndex] = false;
                                }
                                else//그레이
                                {
                                    //이전 프레임에서의 고스트 또는 화이트와 겹치는가
                                    bool prevGhost = false, prevWhite = false;
                                    for (int k = 0; k <= prevTouchesLastIndex; k++)
                                    {
                                        if (prevTouchesCheck4Update[k])
                                        {
                                            if (prevTouches[k].isGhost)
                                            {
                                                prevGhost = true;
                                            }
                                            else//화이트와 겹쳤다면
                                            {
                                                prevWhite = true;
                                                isUsedPrevTouch[k] = true;//이번 프레임의 그레이터치와 겹친 이전 화이트 터치는 겹친 터치를 화이트로 만드는데 사용되므로 사용됨 상태로
                                            }
                                        }
                                    }

                                    if (prevWhite)//화이트와 겹친경우 > 화이트 확정
                                    {
                                        isGrayTouch[touchesLastIndex] = false;

                                        touches[touchesLastIndex].x = (touches[touchesLastIndex].maxX + touches[touchesLastIndex].minX) * 0.5f;
                                        touches[touchesLastIndex].y = (touches[touchesLastIndex].maxY + touches[touchesLastIndex].minY) * 0.5f;

                                        touches[touchesLastIndex].isGhost = false;

                                        for (int k = 0; k <= prevTouchesLastIndex; k++)
                                        {
                                            if (prevTouchesCheck4Update[k] && !prevTouches[k].isGhost)
                                            {
                                                touches[touchesLastIndex].isDownThisFrame = false;
                                                if (touches[touchesLastIndex].x > prevTouches[k].x + 0.1f)
                                                    touches[touchesLastIndex].moveRight = true;
                                                if (touches[touchesLastIndex].x < prevTouches[k].x - 0.1f)
                                                    touches[touchesLastIndex].moveLeft = true;
                                                if (touches[touchesLastIndex].y > prevTouches[k].y + 0.1f)
                                                    touches[touchesLastIndex].moveBack = true;
                                                if (touches[touchesLastIndex].y < prevTouches[k].y - 0.1f)
                                                    touches[touchesLastIndex].moveFront = true;
                                            }
                                        }
                                    }
                                    else//겹치지 않은 경우 그레이
                                    {
                                        isGrayTouch[touchesLastIndex] = true;

                                        touches[touchesLastIndex].isGhost = prevGhost;//prevGhost가 false인 경우 사실 화이트 확정이지만 isDownThisFrame을 판정해야함
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            if (marking[i, j] == -1)
                            {
                                touchesLastIndex++;
                                for (int k = 0; k <= prevTouchesLastIndex; k++)
                                    prevTouchesCheck4Update[k] = false;

                                //touches[touchesLastIndex].isGhost = false;//어차피 밑에서 확실하게 정해짐
                                touches[touchesLastIndex].isDownThisFrame = true;
                                touches[touchesLastIndex].moveBack = false;
                                touches[touchesLastIndex].moveFront = false;
                                touches[touchesLastIndex].moveLeft = false;
                                touches[touchesLastIndex].moveRight = false;
                                touches[touchesLastIndex].minX = int.MaxValue;//얘네는 FillGray가 처리
                                touches[touchesLastIndex].maxX = int.MinValue;
                                touches[touchesLastIndex].minY = int.MaxValue;
                                touches[touchesLastIndex].maxY = int.MinValue;

                                touch0Vertex = false;
                                touch1Vertex = false;
                                touchArea = 0;
                                FillGray(touchesLastIndex, i, j, camInput0, camInput1, ref touch0Vertex, ref touch1Vertex,ref touchArea);
                                if (touchArea < minTouchArea)//최소 터치 크기 미달로 터치 판정 취소
                                {
                                    FillVoid(touchesLastIndex, i, j);
                                    touchesLastIndex--;
                                    continue;
                                }

                                if (!touch0Vertex && !touch1Vertex)//고스트 확정
                                {
                                    touches[touchesLastIndex].isGhost = true;
                                    isGrayTouch[touchesLastIndex] = false;
                                }
                                else//그레이
                                {
                                    //이전 프레임에서의 고스트 또는 화이트와 겹치는가
                                    bool prevGhost = false, prevWhite = false;
                                    for (int k = 0; k <= prevTouchesLastIndex; k++)
                                    {
                                        if (prevTouchesCheck4Update[k])
                                        {
                                            if (prevTouches[k].isGhost)
                                            {
                                                prevGhost = true;
                                            }
                                            else//화이트와 겹쳤다면
                                            {
                                                prevWhite = true;
                                                isUsedPrevTouch[k] = true;//이번 프레임의 그레이터치와 겹친 이전 화이트 터치는 겹친 터치를 화이트로 만드는데 사용되므로 사용됨 상태로
                                            }
                                        }
                                    }

                                    touches[touchesLastIndex].x = (touches[touchesLastIndex].maxX + touches[touchesLastIndex].minX) * 0.5f;//화이트인 경우에는 당연하고 그레이인 경우에도 그레이 처리할 때 필요하므로 x,y좌표 계산 해줘야함
                                    touches[touchesLastIndex].y = (touches[touchesLastIndex].maxY + touches[touchesLastIndex].minY) * 0.5f;

                                    if (prevWhite)//화이트와 겹친경우 > 화이트 확정
                                    {
                                        isGrayTouch[touchesLastIndex] = false;

                                        touches[touchesLastIndex].isGhost = false;

                                        for (int k = 0; k <= prevTouchesLastIndex; k++)
                                        {
                                            if (prevTouchesCheck4Update[k] && !prevTouches[k].isGhost)
                                            {
                                                touches[touchesLastIndex].isDownThisFrame = false;
                                                if (touches[touchesLastIndex].x > prevTouches[k].x + 0.1f)
                                                    touches[touchesLastIndex].moveRight = true;
                                                if (touches[touchesLastIndex].x < prevTouches[k].x - 0.1f)
                                                    touches[touchesLastIndex].moveLeft = true;
                                                if (touches[touchesLastIndex].y > prevTouches[k].y + 0.1f)
                                                    touches[touchesLastIndex].moveBack = true;
                                                if (touches[touchesLastIndex].y < prevTouches[k].y - 0.1f)
                                                    touches[touchesLastIndex].moveFront = true;
                                            }
                                        }
                                    }
                                    else//겹치지 않은 경우 그레이
                                    {
                                        isGrayTouch[touchesLastIndex] = true;

                                        if (prevGhost)
                                        {
                                            touches[touchesLastIndex].isGhost = true;//만약 이전 화이트 터치가 지정되면 isGhost가 false로 바뀌는거고 아니면 그냥 이대로 고스트가 되는거고.
                                        }
                                        else
                                        {
                                            touches[touchesLastIndex].isGhost = false;//prevGhost가 false인 경우 사실 화이트 확정이지만 isDownThisFrame을 판정해야함
                                            touches[touchesLastIndex].isDownThisFrame = true;//만약 이전 화이트 터치가 지정되면 isDownThisFrame이 false로 바뀌는거고 아니면 그냥 이대로 true로 가는거고
                                            touches[touchesLastIndex].moveBack = false;//얘네들도 이전 화이트 터치가 지정되면 알아서 바뀔것. 일단 전부 false로 지정해둠
                                            touches[touchesLastIndex].moveFront = false;
                                            touches[touchesLastIndex].moveLeft = false;
                                            touches[touchesLastIndex].moveRight = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //그레이 처리
                for (int i = 0; i <= prevTouchesLastIndex; i++)
                {
                    if (!isUsedPrevTouch[i])//미사용 이전 화이트 터치인 경우
                    {
                        minDistanceSqr = float.MaxValue;//가장 가까이 있는 그레이터치를 선택
                        minDistanceIndex = -1;
                        for (int j = 0; j <= touchesLastIndex; j++)
                        {
                            if (isGrayTouch[j])
                            {
                                distanceSqr = (prevTouches[i].x - touches[j].x) * (prevTouches[i].x - touches[j].x) + (prevTouches[i].y - touches[j].y) * (prevTouches[i].y - touches[j].y);
                                if (minDistanceSqr > distanceSqr)
                                {
                                    minDistanceSqr = distanceSqr;
                                    minDistanceIndex = j;
                                }
                            }
                        }

                        if (minDistanceIndex == -1)//그레이 터치가 하나도 없다는 뜻이므로 더 이상 for문 돌릴 필요 없음
                        {
                            break;
                        }
                        else//화이트 확정
                        {
                            isGrayTouch[minDistanceIndex] = false;

                            touches[minDistanceIndex].isGhost = false;
                            touches[minDistanceIndex].isDownThisFrame = false;

                            if (touches[minDistanceIndex].x > prevTouches[i].x + 0.1f)
                                touches[minDistanceIndex].moveRight = true;
                            if (touches[minDistanceIndex].x < prevTouches[i].x - 0.1f)
                                touches[minDistanceIndex].moveLeft = true;
                            if (touches[minDistanceIndex].y > prevTouches[i].y + 0.1f)
                                touches[minDistanceIndex].moveBack = true;
                            if (touches[minDistanceIndex].y < prevTouches[i].y - 0.1f)
                                touches[minDistanceIndex].moveFront = true;
                        }
                    }
                }
            }
            else//고스트터치검출 비활성화//이때의 그레이는 위의 그레이와 의미가 조금 다름. 얘는 "고스트냐 아니냐"가 아니라 "isDownThisFrame"가 확정되지 않은 상태를 그레이라고 함
            {
                for (int i = 0; i <= prevTouchesLastIndex; i++)//이전 터치들 중에서 화이트만 골라서 미사용 상태로, 고스트들은 어차피 안쓸거니까 사용됨 상태로
                {
                    isUsedPrevTouch[i] = prevTouches[i].isGhost;//어차피 이전 터치들은 고스트 아니면 화이트다.
                }
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (marking[i,j] == -1)
                        {
                            for (int k = 0; k <= prevTouchesLastIndex; k++)
                                prevTouchesCheck4Update[k] = false;

                            touchesLastIndex++;

                            touches[touchesLastIndex].isGhost = false;
                            touches[touchesLastIndex].isDownThisFrame = true;
                            touches[touchesLastIndex].moveBack = false;
                            touches[touchesLastIndex].moveFront = false;
                            touches[touchesLastIndex].moveLeft = false;
                            touches[touchesLastIndex].moveRight = false;
                            touches[touchesLastIndex].minX = int.MaxValue;//얘네는 FillIgnoreGhost가 처리
                            touches[touchesLastIndex].maxX = int.MinValue;
                            touches[touchesLastIndex].minY = int.MaxValue;
                            touches[touchesLastIndex].maxY = int.MinValue;

                            touchArea = 0;
                            FillIgnoreGhost(touchesLastIndex, i, j,ref touchArea);
                            if (touchArea < minTouchArea)//최소 터치 크기 미달
                            {
                                FillVoid(touchesLastIndex, i, j);
                                touchesLastIndex--;
                                continue;
                            }

                            touches[touchesLastIndex].x = (touches[touchesLastIndex].maxX + touches[touchesLastIndex].minX) * 0.5f;
                            touches[touchesLastIndex].y = (touches[touchesLastIndex].maxY + touches[touchesLastIndex].minY) * 0.5f;

                            isGrayTouch[touchesLastIndex] = true;//기본적으로 전부 그레이.

                            for (int k = 0; k <= prevTouchesLastIndex; k++)
                            {
                                if (prevTouchesCheck4Update[k] && !prevTouches[k].isGhost)
                                {
                                    isGrayTouch[touchesLastIndex] = false;
                                    isUsedPrevTouch[k] = true;//이번 프레임의 그레이터치와 겹친 이전 화이트 터치는 겹친 터치를 화이트로 만드는데 사용되므로 사용됨 상태로

                                    touches[touchesLastIndex].isDownThisFrame = false;
                                    if (touches[touchesLastIndex].x > prevTouches[k].x + 0.1f)
                                        touches[touchesLastIndex].moveRight = true;
                                    if (touches[touchesLastIndex].x < prevTouches[k].x - 0.1f)
                                        touches[touchesLastIndex].moveLeft = true;
                                    if (touches[touchesLastIndex].y > prevTouches[k].y + 0.1f)
                                        touches[touchesLastIndex].moveBack = true;
                                    if (touches[touchesLastIndex].y < prevTouches[k].y - 0.1f)
                                        touches[touchesLastIndex].moveFront = true;
                                }
                            }
                        }
                    }
                }
                //그레이 처리
                for (int i = 0; i <= prevTouchesLastIndex; i++)
                {
                    if (!isUsedPrevTouch[i])//미사용 이전 화이트 터치인 경우
                    {
                        minDistanceSqr = float.MaxValue;//가장 가까이 있는 그레이터치를 선택
                        minDistanceIndex = -1;
                        for (int j = 0; j <= touchesLastIndex; j++)
                        {
                            if (isGrayTouch[j])
                            {
                                distanceSqr = (prevTouches[i].x - touches[j].x) * (prevTouches[i].x - touches[j].x) + (prevTouches[i].y - touches[j].y) * (prevTouches[i].y - touches[j].y);
                                if (minDistanceSqr > distanceSqr)
                                {
                                    minDistanceSqr = distanceSqr;
                                    minDistanceIndex = j;
                                }
                            }
                        }

                        if (minDistanceIndex == -1)//그레이 터치가 하나도 없다는 뜻이므로 더 이상 for문 돌릴 필요 없음
                        {
                            break;
                        }
                        else//지정된 이전 화이트 터치가 있으므로 얘는 isDownThisFrame이 false가 되어야함
                        {
                            isGrayTouch[minDistanceIndex] = false;

                            touches[minDistanceIndex].isDownThisFrame = false;

                            if (touches[minDistanceIndex].x > prevTouches[i].x + 0.1f)
                                touches[minDistanceIndex].moveRight = true;
                            if (touches[minDistanceIndex].x < prevTouches[i].x - 0.1f)
                                touches[minDistanceIndex].moveLeft = true;
                            if (touches[minDistanceIndex].y > prevTouches[i].y + 0.1f)
                                touches[minDistanceIndex].moveBack = true;
                            if (touches[minDistanceIndex].y < prevTouches[i].y - 0.1f)
                                touches[minDistanceIndex].moveFront = true;
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i <= touchesLastIndex; i++)
            {
                touches[i].isDownThisFrame = false;
                touches[i].moveLeft = false;
                touches[i].moveRight = false;
                touches[i].moveFront = false;
                touches[i].moveBack = false;
            }
        }
    }
    int[][] add4Fill =
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
    void FillIgnoreGhost(int touchIndex,int x, int y,ref int area)
    {
        area++;
        marking[x, y] = touchIndex;
        if (touches[touchIndex].maxX < x)
            touches[touchIndex].maxX = x;
        if (touches[touchIndex].maxY < y)
            touches[touchIndex].maxY = y;
        if (touches[touchIndex].minX > x)
            touches[touchIndex].minX = x;
        if (touches[touchIndex].minY > y)
            touches[touchIndex].minY = y;

        if (prevMarking[x,y] != -2)
            prevTouchesCheck4Update[prevMarking[x, y]] = true;

        int nextX, nextY;
        for (int i = 0; i < 8; i++)
        {
            nextX = x + add4Fill[i][0];
            nextY = y + add4Fill[i][1];
            if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height)//배열 밖으로 나가면 안됨
                continue;

            if (marking[nextX, nextY] == -1)
                FillIgnoreGhost(touchIndex,nextX, nextY, ref area);
        }
    }
    void FillGray(int touchIndex, int x, int y, CamInput camInput0, CamInput camInput1, ref bool touch0Vertex, ref bool touch1Vertex, ref int area)
    {
        area++;
        marking[x, y] = touchIndex;
        if (touches[touchIndex].maxX < x)
            touches[touchIndex].maxX = x;
        if (touches[touchIndex].maxY < y)
            touches[touchIndex].maxY = y;
        if (touches[touchIndex].minX > x)
            touches[touchIndex].minX = x;
        if (touches[touchIndex].minY > y)
            touches[touchIndex].minY = y;

        if (prevMarking[x, y] != -2)
            prevTouchesCheck4Update[prevMarking[x, y]] = true;

        if (camInput0.vertexOfMarkings[x, y] == 0 || camInput0.vertexOfMarkings[x, y] == 1)
        {
            touch0Vertex = true;
        }
        if (camInput1.vertexOfMarkings[x, y] == 0 || camInput1.vertexOfMarkings[x, y] == 1)
        {
            touch1Vertex = true;
        }

        int nextX, nextY;
        for (int i = 0; i < 8; i++)
        {
            nextX = x + add4Fill[i][0];
            nextY = y + add4Fill[i][1];
            if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height)//배열 밖으로 나가면 안됨
                continue;

            if (marking[nextX, nextY] == -1)
                FillGray(touchIndex, nextX, nextY, camInput0, camInput1, ref touch0Vertex, ref touch1Vertex, ref area);
        }
    }
    void FillVoid(int touchIndex, int x, int y)
    {
        marking[x, y] = -2;

        int nextX, nextY;
        for (int i = 0; i < 8; i++)
        {
            nextX = x + add4Fill[i][0];
            nextY = y + add4Fill[i][1];
            if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height)//배열 밖으로 나가면 안됨
                continue;

            if (marking[nextX, nextY] == touchIndex)
                FillVoid(touchIndex, nextX, nextY);
        }
    }
}
public delegate void Changed();
public struct TouchInfo
{
    public bool isGhost;
    public bool isDownThisFrame;

    public bool moveLeft;
    public bool moveRight;
    public bool moveFront;
    public bool moveBack;

    public int minX;
    public int maxX;
    public int minY;
    public int maxY;

    public float x;
    public float y;
}