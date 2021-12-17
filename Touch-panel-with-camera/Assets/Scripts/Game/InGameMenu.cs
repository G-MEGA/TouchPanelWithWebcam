using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    [SerializeField]
    private Slider noteSpeed;
    [SerializeField]
    private Text judgeText;
    [SerializeField]
    private Text comboText;
    [SerializeField]
    private Text allJudgeText;
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

    public void JudgeAndComboUpdate(string judge, string combo, int perfect, int great, int good, int bad)
    {
        judgeText.text = judge;
        comboText.text = combo;
        allJudgeText.text =
            "<color=cyan>PERFECT</color>    " + perfect.ToString() + "\n" +
            "<color=lime>GREAT</color>    " + great.ToString() + "\n" +
            "<color=yellow>GOOD</color>    " + good.ToString() + "\n" +
            "<color=red>BAD</color>    " + bad.ToString();
    }
}
