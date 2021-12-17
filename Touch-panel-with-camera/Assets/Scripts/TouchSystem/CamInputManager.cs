using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class CamInputManager : MonoBehaviour
{
    Vector2Int resolution = new Vector2Int(60, 60);//�ּ� (2,2)
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

                //�ػ� ����� ����
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
    int markingLength = 3;//�ּ� 1
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
                //��ŷ���� ����� ����
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
    public bool removeGhostWhen2Cam = true;//Ȱ��ȭ�� ī�޶� 2��ۿ� ���� �� ��Ʈ ��ġ ���Ÿ� �����ϴ°�
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
    public int[,] marking;//-2�� ���㸦 �ǹ�. -1�� �������� �ǹ�. �� ���� ������ �ƴ� �ڿ����� �ش� �ε����� ��ġ�� �������� ��Ÿ��.
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

            webCams[i].requestedWidth = 640;//��ķ�� �⺻ ��ȣ �ػ�
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

        if (camInputsUpdatedCount >= ActiveCamInputCount)//��� ��ķ�� �ѹ��̻� ������Ʈ �Ǿ��� �� ����
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
                //��Ƽ�� ������ ī�޶� ����
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
                //��ġ����
                for (int i = 0; i <= prevTouchesLastIndex; i++)//���� ��ġ�� �߿��� ȭ��Ʈ�� ��� �̻�� ���·�, ��Ʈ���� ������ �Ⱦ��Ŵϱ� ���� ���·�
                {
                    isUsedPrevTouch[i] = prevTouches[i].isGhost;//������ ���� ��ġ���� ��Ʈ �ƴϸ� ȭ��Ʈ��.
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

                                //touches[touchesLastIndex].isGhost = false;//������ �ؿ��� Ȯ���ϰ� ������
                                touches[touchesLastIndex].isDownThisFrame = true;
                                touches[touchesLastIndex].moveBack = false;
                                touches[touchesLastIndex].moveFront = false;
                                touches[touchesLastIndex].moveLeft = false;
                                touches[touchesLastIndex].moveRight = false;
                                touches[touchesLastIndex].minX = int.MaxValue;//��״� FillGray�� ó��
                                touches[touchesLastIndex].maxX = int.MinValue;
                                touches[touchesLastIndex].minY = int.MaxValue;
                                touches[touchesLastIndex].maxY = int.MinValue;

                                touch0Vertex = false;
                                touch1Vertex = false;
                                touchArea = 0;
                                FillGray(touchesLastIndex, i, j, camInput0, camInput1, ref touch0Vertex, ref touch1Vertex, ref touchArea);
                                if (touchArea < minTouchArea)//�ּ� ��ġ ũ�� �̴޷� ��ġ ���� ���
                                {
                                    FillVoid(touchesLastIndex, i, j);
                                    touchesLastIndex--;
                                    continue;
                                }

                                if (!touch0Vertex || !touch1Vertex)//��Ʈ Ȯ��
                                {
                                    touches[touchesLastIndex].isGhost = true;
                                    isGrayTouch[touchesLastIndex] = false;
                                }
                                else//�׷���
                                {
                                    //���� �����ӿ����� ��Ʈ �Ǵ� ȭ��Ʈ�� ��ġ�°�
                                    bool prevGhost = false, prevWhite = false;
                                    for (int k = 0; k <= prevTouchesLastIndex; k++)
                                    {
                                        if (prevTouchesCheck4Update[k])
                                        {
                                            if (prevTouches[k].isGhost)
                                            {
                                                prevGhost = true;
                                            }
                                            else//ȭ��Ʈ�� ���ƴٸ�
                                            {
                                                prevWhite = true;
                                                isUsedPrevTouch[k] = true;//�̹� �������� �׷�����ġ�� ��ģ ���� ȭ��Ʈ ��ġ�� ��ģ ��ġ�� ȭ��Ʈ�� ����µ� ���ǹǷ� ���� ���·�
                                            }
                                        }
                                    }

                                    if (prevWhite)//ȭ��Ʈ�� ��ģ��� > ȭ��Ʈ Ȯ��
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
                                    else//��ġ�� ���� ��� �׷���
                                    {
                                        isGrayTouch[touchesLastIndex] = true;

                                        touches[touchesLastIndex].isGhost = prevGhost;//prevGhost�� false�� ��� ��� ȭ��Ʈ Ȯ�������� isDownThisFrame�� �����ؾ���
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

                                //touches[touchesLastIndex].isGhost = false;//������ �ؿ��� Ȯ���ϰ� ������
                                touches[touchesLastIndex].isDownThisFrame = true;
                                touches[touchesLastIndex].moveBack = false;
                                touches[touchesLastIndex].moveFront = false;
                                touches[touchesLastIndex].moveLeft = false;
                                touches[touchesLastIndex].moveRight = false;
                                touches[touchesLastIndex].minX = int.MaxValue;//��״� FillGray�� ó��
                                touches[touchesLastIndex].maxX = int.MinValue;
                                touches[touchesLastIndex].minY = int.MaxValue;
                                touches[touchesLastIndex].maxY = int.MinValue;

                                touch0Vertex = false;
                                touch1Vertex = false;
                                touchArea = 0;
                                FillGray(touchesLastIndex, i, j, camInput0, camInput1, ref touch0Vertex, ref touch1Vertex,ref touchArea);
                                if (touchArea < minTouchArea)//�ּ� ��ġ ũ�� �̴޷� ��ġ ���� ���
                                {
                                    FillVoid(touchesLastIndex, i, j);
                                    touchesLastIndex--;
                                    continue;
                                }

                                if (!touch0Vertex && !touch1Vertex)//��Ʈ Ȯ��
                                {
                                    touches[touchesLastIndex].isGhost = true;
                                    isGrayTouch[touchesLastIndex] = false;
                                }
                                else//�׷���
                                {
                                    //���� �����ӿ����� ��Ʈ �Ǵ� ȭ��Ʈ�� ��ġ�°�
                                    bool prevGhost = false, prevWhite = false;
                                    for (int k = 0; k <= prevTouchesLastIndex; k++)
                                    {
                                        if (prevTouchesCheck4Update[k])
                                        {
                                            if (prevTouches[k].isGhost)
                                            {
                                                prevGhost = true;
                                            }
                                            else//ȭ��Ʈ�� ���ƴٸ�
                                            {
                                                prevWhite = true;
                                                isUsedPrevTouch[k] = true;//�̹� �������� �׷�����ġ�� ��ģ ���� ȭ��Ʈ ��ġ�� ��ģ ��ġ�� ȭ��Ʈ�� ����µ� ���ǹǷ� ���� ���·�
                                            }
                                        }
                                    }

                                    touches[touchesLastIndex].x = (touches[touchesLastIndex].maxX + touches[touchesLastIndex].minX) * 0.5f;//ȭ��Ʈ�� ��쿡�� �翬�ϰ� �׷����� ��쿡�� �׷��� ó���� �� �ʿ��ϹǷ� x,y��ǥ ��� �������
                                    touches[touchesLastIndex].y = (touches[touchesLastIndex].maxY + touches[touchesLastIndex].minY) * 0.5f;

                                    if (prevWhite)//ȭ��Ʈ�� ��ģ��� > ȭ��Ʈ Ȯ��
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
                                    else//��ġ�� ���� ��� �׷���
                                    {
                                        isGrayTouch[touchesLastIndex] = true;

                                        if (prevGhost)
                                        {
                                            touches[touchesLastIndex].isGhost = true;//���� ���� ȭ��Ʈ ��ġ�� �����Ǹ� isGhost�� false�� �ٲ�°Ű� �ƴϸ� �׳� �̴�� ��Ʈ�� �Ǵ°Ű�.
                                        }
                                        else
                                        {
                                            touches[touchesLastIndex].isGhost = false;//prevGhost�� false�� ��� ��� ȭ��Ʈ Ȯ�������� isDownThisFrame�� �����ؾ���
                                            touches[touchesLastIndex].isDownThisFrame = true;//���� ���� ȭ��Ʈ ��ġ�� �����Ǹ� isDownThisFrame�� false�� �ٲ�°Ű� �ƴϸ� �׳� �̴�� true�� ���°Ű�
                                            touches[touchesLastIndex].moveBack = false;//��׵鵵 ���� ȭ��Ʈ ��ġ�� �����Ǹ� �˾Ƽ� �ٲ��. �ϴ� ���� false�� �����ص�
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

                //�׷��� ó��
                for (int i = 0; i <= prevTouchesLastIndex; i++)
                {
                    if (!isUsedPrevTouch[i])//�̻�� ���� ȭ��Ʈ ��ġ�� ���
                    {
                        minDistanceSqr = float.MaxValue;//���� ������ �ִ� �׷�����ġ�� ����
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

                        if (minDistanceIndex == -1)//�׷��� ��ġ�� �ϳ��� ���ٴ� ���̹Ƿ� �� �̻� for�� ���� �ʿ� ����
                        {
                            break;
                        }
                        else//ȭ��Ʈ Ȯ��
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
            else//��Ʈ��ġ���� ��Ȱ��ȭ//�̶��� �׷��̴� ���� �׷��̿� �ǹ̰� ���� �ٸ�. ��� "��Ʈ�� �ƴϳ�"�� �ƴ϶� "isDownThisFrame"�� Ȯ������ ���� ���¸� �׷��̶�� ��
            {
                for (int i = 0; i <= prevTouchesLastIndex; i++)//���� ��ġ�� �߿��� ȭ��Ʈ�� ��� �̻�� ���·�, ��Ʈ���� ������ �Ⱦ��Ŵϱ� ���� ���·�
                {
                    isUsedPrevTouch[i] = prevTouches[i].isGhost;//������ ���� ��ġ���� ��Ʈ �ƴϸ� ȭ��Ʈ��.
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
                            touches[touchesLastIndex].minX = int.MaxValue;//��״� FillIgnoreGhost�� ó��
                            touches[touchesLastIndex].maxX = int.MinValue;
                            touches[touchesLastIndex].minY = int.MaxValue;
                            touches[touchesLastIndex].maxY = int.MinValue;

                            touchArea = 0;
                            FillIgnoreGhost(touchesLastIndex, i, j,ref touchArea);
                            if (touchArea < minTouchArea)//�ּ� ��ġ ũ�� �̴�
                            {
                                FillVoid(touchesLastIndex, i, j);
                                touchesLastIndex--;
                                continue;
                            }

                            touches[touchesLastIndex].x = (touches[touchesLastIndex].maxX + touches[touchesLastIndex].minX) * 0.5f;
                            touches[touchesLastIndex].y = (touches[touchesLastIndex].maxY + touches[touchesLastIndex].minY) * 0.5f;

                            isGrayTouch[touchesLastIndex] = true;//�⺻������ ���� �׷���.

                            for (int k = 0; k <= prevTouchesLastIndex; k++)
                            {
                                if (prevTouchesCheck4Update[k] && !prevTouches[k].isGhost)
                                {
                                    isGrayTouch[touchesLastIndex] = false;
                                    isUsedPrevTouch[k] = true;//�̹� �������� �׷�����ġ�� ��ģ ���� ȭ��Ʈ ��ġ�� ��ģ ��ġ�� ȭ��Ʈ�� ����µ� ���ǹǷ� ���� ���·�

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
                //�׷��� ó��
                for (int i = 0; i <= prevTouchesLastIndex; i++)
                {
                    if (!isUsedPrevTouch[i])//�̻�� ���� ȭ��Ʈ ��ġ�� ���
                    {
                        minDistanceSqr = float.MaxValue;//���� ������ �ִ� �׷�����ġ�� ����
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

                        if (minDistanceIndex == -1)//�׷��� ��ġ�� �ϳ��� ���ٴ� ���̹Ƿ� �� �̻� for�� ���� �ʿ� ����
                        {
                            break;
                        }
                        else//������ ���� ȭ��Ʈ ��ġ�� �����Ƿ� ��� isDownThisFrame�� false�� �Ǿ����
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
            if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height)//�迭 ������ ������ �ȵ�
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
            if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height)//�迭 ������ ������ �ȵ�
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
            if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height)//�迭 ������ ������ �ȵ�
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