using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public InputField resolutionX;
    public InputField resolutionY;
    public InputField markingLength;
    public void Go()
    {
        CamInputManager.Instance.Resolution = new Vector2Int(int.Parse(resolutionX.text), int.Parse(resolutionY.text));
        CamInputManager.Instance.MarkingLength = int.Parse(markingLength.text);
        SceneManager.LoadScene(1);
    }
}
