using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamInputManager : MonoBehaviour
{
    Vector2Int resolution = new Vector2Int(30, 30);//置社 (2,2)
    public Vector2Int Resolution
    {
        get
        {
            return resolution;
        }
        set
        {
            resolution = value;
            if (resolution.x < 2)
                resolution.x = 2;
            if (resolution.y < 2)
                resolution.y = 2;
        }
    }
    int markingLength = 3;//置社 1
    public int MarkingLength
    {
        get
        {
            return markingLength;
        }
        set
        {
            if (value < 1)
                markingLength = 1;
            else
                markingLength = value;
        }
    }
    public int[,] markings = new int[0,0];
    public List<CamInput> camInputs = new List<CamInput>();

    private static CamInputManager instance = null;
    public static CamInputManager Instance
    {
        get
        {
            return instance;
        }
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

        for (int i = 0; i < webCamDevices.Length; i++)
        {
            print(webCamDevices[i].name);
        }

        //camInputs.Add(new CamInput());
        //camInputs[0].Init(webCamDevices[0].name,new Vector2(), new Vector2(), new Vector2(), new Vector2());
        StartCoroutine(MarkingsUpdate());
    }
    IEnumerator MarkingsUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            if (markings.GetLength(0) != resolution.x || markings.GetLength(1) != resolution.y)
                markings = new int[resolution.x, resolution.y];
            int camInputCount = camInputs.Count;
            for (int i = 0; i < camInputCount; i++)
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

}
