using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    [SerializeField]
    private Slider noteSpeed;
    private void Start()
    {
        noteSpeed.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(NoteSpeedChange));
        noteSpeed.value = GameManager.noteSpeed;
    }
    public void Return2MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void NoteSpeedChange(float value)
    {
        GameManager.noteSpeed = value;
        noteSpeed.value = GameManager.noteSpeed;
    }
}
