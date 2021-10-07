using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamInputGenerator : MonoBehaviour
{
    public InputField FL_x;
    public InputField FL_y;
    public InputField FR_x;
    public InputField FR_y;
    public InputField BL_x;
    public InputField BL_y;
    public InputField BR_x;
    public InputField BR_y;
    public RectTransform FL;
    public RectTransform FR;
    public RectTransform BL;
    public RectTransform BR;
    public RawImage rawImage;
    AspectRatioFitter rawImageFitter;

    int index = -1;
    WebCamDevice[] webCamDevices;
    WebCamTexture[] webCamTextures;

    void Start()
    {
        rawImageFitter = rawImage.GetComponent<AspectRatioFitter>();

        webCamDevices = WebCamTexture.devices;
        webCamTextures = new WebCamTexture[webCamDevices.Length];
        for (int i = 0; i < webCamTextures.Length; i++)
            webCamTextures[i] = new WebCamTexture(webCamDevices[i].name);
    }
    private void Update()
    {
        float speed = 0.001f;//한번 입력당 이동속도

        Vector2 vector2 = Vector2.zero;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                vector2 += new Vector2(0f, speed);
            if (Input.GetKeyDown(KeyCode.DownArrow))
                vector2 += new Vector2(0f, -speed);
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                vector2 += new Vector2(-speed, 0f);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                vector2 += new Vector2(speed, 0f);
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            FL_x.text = Clamp01(float.Parse(FL_x.text) + vector2.x).ToString();
            FL_y.text = Clamp01(float.Parse(FL_y.text) + vector2.y).ToString();
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            FR_x.text = Clamp01(float.Parse(FR_x.text) + vector2.x).ToString();
            FR_y.text = Clamp01(float.Parse(FR_y.text) + vector2.y).ToString();
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            BL_x.text = Clamp01(float.Parse(BL_x.text) + vector2.x).ToString();
            BL_y.text = Clamp01(float.Parse(BL_y.text) + vector2.y).ToString();
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            BR_x.text = Clamp01(float.Parse(BR_x.text) + vector2.x).ToString();
            BR_y.text = Clamp01(float.Parse(BR_y.text) + vector2.y).ToString();
        }



        Vector2 newAnchor;

        newAnchor = new Vector2(float.Parse(FL_x.text), float.Parse(FL_y.text));
        FL.anchorMin = newAnchor;
        FL.anchorMax = newAnchor;

        newAnchor = new Vector2(float.Parse(FR_x.text), float.Parse(FR_y.text));
        FR.anchorMin = newAnchor;
        FR.anchorMax = newAnchor;

        newAnchor = new Vector2(float.Parse(BL_x.text), float.Parse(BL_y.text));
        BL.anchorMin = newAnchor;
        BL.anchorMax = newAnchor;

        newAnchor = new Vector2(float.Parse(BR_x.text), float.Parse(BR_y.text));
        BR.anchorMin = newAnchor;
        BR.anchorMax = newAnchor;

        if(rawImage.texture != null)
            rawImageFitter.aspectRatio = (float)rawImage.texture.width / rawImage.texture.height;
    }
    private void FixedUpdate()
    {
        float speed = 0.5f * Time.deltaTime;//초당 이동속도

        Vector2 vector2 = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow))
            vector2 += new Vector2(0f, speed);
        if (Input.GetKey(KeyCode.DownArrow))
            vector2 += new Vector2(0f, -speed);
        if (Input.GetKey(KeyCode.LeftArrow))
            vector2 += new Vector2(-speed, 0f);
        if (Input.GetKey(KeyCode.RightArrow))
            vector2 += new Vector2(speed, 0f);

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                FL_x.text = Clamp01(float.Parse(FL_x.text) + vector2.x).ToString();
                FL_y.text = Clamp01(float.Parse(FL_y.text) + vector2.y).ToString();
            }
            if (Input.GetKey(KeyCode.Alpha2))
            {
                FR_x.text = Clamp01(float.Parse(FR_x.text) + vector2.x).ToString();
                FR_y.text = Clamp01(float.Parse(FR_y.text) + vector2.y).ToString();
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                BL_x.text = Clamp01(float.Parse(BL_x.text) + vector2.x).ToString();
                BL_y.text = Clamp01(float.Parse(BL_y.text) + vector2.y).ToString();
            }
            if (Input.GetKey(KeyCode.Alpha4))
            {
                BR_x.text = Clamp01(float.Parse(BR_x.text) + vector2.x).ToString();
                BR_y.text = Clamp01(float.Parse(BR_y.text) + vector2.y).ToString();
            }
        }
    }

    float Clamp01(float value)
    {
        if (value < 0f)
        {
            return 0f;
        }
        else if (value > 1f)
        {
            return 1f;
        }
        return Mathf.Floor(value * 10000f) * 0.0001f;
    }
    public void Generate()
    {
        if(index != -1)
        {
            int i = CamInputManager.Instance.camInputs.Count;
            CamInputManager.Instance.camInputs.Add(new CamInput());
            CamInputManager.Instance.camInputs[i].Init(webCamTextures[index], 
                new Vector2(float.Parse(FL_x.text), float.Parse(FL_y.text)), 
                new Vector2(float.Parse(FR_x.text), float.Parse(FR_y.text)),
                new Vector2(float.Parse(BL_x.text), float.Parse(BL_y.text)),
                new Vector2(float.Parse(BR_x.text), float.Parse(BR_y.text)));
        }
    }
    public void Next()
    {
        index++;
        if (index >= webCamDevices.Length)
            index = 0;
        if (!webCamTextures[index].isPlaying)
        {
            webCamTextures[index].Play();
            print(webCamTextures[index].isPlaying);
        }
        rawImage.texture = webCamTextures[index];
    }
}
