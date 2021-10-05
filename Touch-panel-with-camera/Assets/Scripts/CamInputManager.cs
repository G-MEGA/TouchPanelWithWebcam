using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamInputManager : MonoBehaviour
{
    Vector2Int resolution = new Vector2Int(10, 10);//置社 (2,2)
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
    int[,] markings;
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

        camInputs.Add(new CamInput());
        camInputs[0].Init(webCamDevices[0].name,new Vector2(), new Vector2(), new Vector2(), new Vector2());
        StartCoroutine(MarkingsUpdate());
    }
    IEnumerator MarkingsUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            camInputs[0].MarkingsUpdate();
        }
    }

    /*
    public Vector2Int pixel;
    public RawImage pixelColorDisplay;

    public RawImage test0;
    public RawImage test1;
    WebCamTexture webCamTexture;
    WebCamTexture webCamTexture1;

    void Start()
    {
        WebCamDevice[] webCamDevices = WebCamTexture.devices;

        for (int i = 0; i < webCamDevices.Length; i++)
        {
            print(webCamDevices[i].name);
        }

        webCamTexture = new WebCamTexture(webCamDevices[0].name);
        webCamTexture.requestedFPS = 30;
        test0.texture = webCamTexture;
        webCamTexture.Play();
        test0.GetComponent<AspectRatioFitter>().aspectRatio = (float)webCamTexture.width / webCamTexture.height;


        webCamTexture1 = new WebCamTexture(webCamDevices[1].name);
        webCamTexture1.requestedFPS = 30;
        test1.texture = webCamTexture1;
        webCamTexture1.Play();
        test1.GetComponent<AspectRatioFitter>().aspectRatio = (float)webCamTexture1.width / webCamTexture1.height;
    }
    private void Update()
    {
        int offset = 1;
        pixelColorDisplay.color = ColorAverage(
            webCamTexture.GetPixel(pixel.x, pixel.y),
            webCamTexture.GetPixel(pixel.x, pixel.y + offset),
            webCamTexture.GetPixel(pixel.x, pixel.y + offset + 1),
            webCamTexture.GetPixel(pixel.x, pixel.y - offset),
            webCamTexture.GetPixel(pixel.x, pixel.y - offset - 1),

            webCamTexture.GetPixel(pixel.x+offset, pixel.y),
            webCamTexture.GetPixel(pixel.x + offset, pixel.y + offset),
            webCamTexture.GetPixel(pixel.x + offset, pixel.y + offset + 1),
            webCamTexture.GetPixel(pixel.x + offset, pixel.y - offset),
            webCamTexture.GetPixel(pixel.x + offset, pixel.y - offset - 1),

            webCamTexture.GetPixel(pixel.x + offset + 1, pixel.y),
            webCamTexture.GetPixel(pixel.x + offset + 1, pixel.y + offset),
            webCamTexture.GetPixel(pixel.x + offset + 1, pixel.y + offset + 1),
            webCamTexture.GetPixel(pixel.x + offset + 1, pixel.y - offset),
            webCamTexture.GetPixel(pixel.x + offset + 1, pixel.y - offset - 1),

            webCamTexture.GetPixel(pixel.x - offset, pixel.y),
            webCamTexture.GetPixel(pixel.x - offset, pixel.y + offset),
            webCamTexture.GetPixel(pixel.x - offset, pixel.y + offset + 1),
            webCamTexture.GetPixel(pixel.x - offset, pixel.y - offset),
            webCamTexture.GetPixel(pixel.x - offset, pixel.y - offset - 1),

            webCamTexture.GetPixel(pixel.x - offset -1, pixel.y),
            webCamTexture.GetPixel(pixel.x - offset - 1, pixel.y + offset),
            webCamTexture.GetPixel(pixel.x - offset - 1, pixel.y + offset + 1),
            webCamTexture.GetPixel(pixel.x - offset - 1, pixel.y - offset),
            webCamTexture.GetPixel(pixel.x - offset - 1, pixel.y - offset - 1)
            );
        pixelColorDisplay.color = ColorAverage(
            webCamTexture.GetPixel(pixel.x, pixel.y),
            webCamTexture.GetPixel(pixel.x, pixel.y + offset),
            webCamTexture.GetPixel(pixel.x, pixel.y - offset),

            webCamTexture.GetPixel(pixel.x + offset, pixel.y),
            webCamTexture.GetPixel(pixel.x + offset, pixel.y + offset),
            webCamTexture.GetPixel(pixel.x + offset, pixel.y - offset),

            webCamTexture.GetPixel(pixel.x + offset + 1, pixel.y),
            webCamTexture.GetPixel(pixel.x + offset + 1, pixel.y + offset),
            webCamTexture.GetPixel(pixel.x + offset + 1, pixel.y - offset),

            webCamTexture.GetPixel(pixel.x - offset, pixel.y),
            webCamTexture.GetPixel(pixel.x - offset, pixel.y + offset),
            webCamTexture.GetPixel(pixel.x - offset, pixel.y - offset),

            webCamTexture.GetPixel(pixel.x - offset - 1, pixel.y),
            webCamTexture.GetPixel(pixel.x - offset - 1, pixel.y + offset),
            webCamTexture.GetPixel(pixel.x - offset - 1, pixel.y - offset)
            );

    int length = 8;
        int half_length = (int)(length * 0.5f);
        Color[] colors = new Color[length* length];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = webCamTexture1.GetPixel(pixel.x - half_length + i % length, pixel.y - half_length + i / length);

        pixelColorDisplay.color = ColorAverage(colors);
        StartCoroutine(analysis());
    }
    
    int minR = 255;
    int minG = 255;
    int minB = 255;
    int maxR = 0;
    int maxG = 0;
    int maxB = 0;
    int minH = 255;
    int minS = 255;
    int minV = 255;
    int maxH = 0;
    int maxS = 0;
    int maxV = 0;
    IEnumerator analysis()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        maxR = (int)Mathf.Max(maxR, pixelColorDisplay.color.r * 256);
        maxG = (int)Mathf.Max(maxG, pixelColorDisplay.color.g * 256);
        maxB = (int)Mathf.Max(maxB, pixelColorDisplay.color.b * 256);

        minR = (int)Mathf.Min(minR, pixelColorDisplay.color.r * 256);
        minG = (int)Mathf.Min(minG, pixelColorDisplay.color.g * 256);
        minB = (int)Mathf.Min(minB, pixelColorDisplay.color.b * 256);

        float h;
        float s;
        float v;
        Color.RGBToHSV(pixelColorDisplay.color, out h, out s, out v);

        maxH = (int)Mathf.Max(maxH, h * 256);
        maxS = (int)Mathf.Max(maxS, s * 256);
        maxV = (int)Mathf.Max(maxV, v * 256);

        minH = (int)Mathf.Min(minH, h * 256);
        minS = (int)Mathf.Min(minS, s * 256);
        minV = (int)Mathf.Min(minV, v * 256);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //print((maxR - minR) + " / " + (maxG - minG) + " / " + (maxB - minB));
            print((maxH - minH) + " / " + (maxS - minS) + " / " + (maxV - minV));
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            minR = 255;
            minG = 255;
            minB = 255;
            maxR = 0;
            maxG = 0;
            maxB = 0;
            minH = 255;
            minS = 255;
            minV = 255;
            maxH = 0;
            maxS = 0;
            maxV = 0;
        }
    }
    */
}
