using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class CamInputManager : MonoBehaviour
{
    Vector2Int resolution = new Vector2Int(30, 30);//최소 (2,2)
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
                markings = new int[resolution.x, resolution.y];

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
    public int[,] markings = new int[0,0];
    public CamInput[] camInputs;
    public WebCamTexture[] webCams;
    public string[] webCamNames;

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
        markings = new int[resolution.x, resolution.y];
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

        for (int i = 0; i < webCamDevices.Length; i++)
        {
            webCams[i] = new WebCamTexture(webCamDevices[i].name);
            webCamNames[i] = webCamDevices[i].name;
            camInputs[i] = new CamInput(webCams[i]);
        }
    }
    private void Update()
    {
        int camInputCount = camInputs.Length;
        for (int i = 0; i < camInputCount; i++)
            if (camInputs[i].Active)
                camInputs[i].MarkingsUpdate();
        int temp;
        for (int i = 0; i < resolution.x; i++)
        {
            for (int j = 0; j < resolution.y; j++)
            {
                temp = 0;
                for (int k = 0; k < camInputCount; k++)
                {
                    if (camInputs[k].markings[i, j])
                        temp++;
                }
                markings[i, j] = temp;
            }
        }
    }
}
public delegate void Changed();