using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    [SerializeField]
    private InputField judgeTimingOffset;
    [SerializeField]
    private InputField noteSyncOffset;
    [SerializeField]
    private InputField noteSpeed;

    [SerializeField]
    private InputField perfectRange;
    [SerializeField]
    private InputField greatRange;
    [SerializeField]
    private InputField goodRange;
    [SerializeField]
    private InputField badRange;
    [SerializeField]
    private InputField slideRange;
    [SerializeField]
    private InputField jumpRange;
    [SerializeField]
    private InputField touchRange;
    [SerializeField]
    private InputField downRange;

    [SerializeField]
    private Text judgeText;
    [SerializeField]
    private Text comboText;
    [SerializeField]
    private Text allJudgeText;
    private void Start()
    {
        judgeTimingOffset.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(JudgeTimingOffsetChange));
        judgeTimingOffset.text = GameManager.judgeTimingOffset.ToString();
        noteSyncOffset.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(NoteSyncOffsetChange));
        noteSyncOffset.text = GameManager.noteSyncOffset.ToString();
        noteSpeed.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(NoteSpeedChange));
        noteSpeed.text = GameManager.noteSpeed.ToString();

        perfectRange.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(PerfectRangeChange));
        perfectRange.text = GameManager.perfectRange.ToString();
        greatRange.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(GreatRangeChange));
        greatRange.text = GameManager.greatRange.ToString();
        goodRange.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(GoodRangeChange));
        goodRange.text = GameManager.goodRange.ToString();
        badRange.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(BadRangeChange));
        badRange.text = GameManager.badRange.ToString();
        slideRange.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(SlideRangeChange));
        slideRange.text = GameManager.slideRange.ToString();
        jumpRange.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(JumpRangeChange));
        jumpRange.text = GameManager.jumpRange.ToString();
        touchRange.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(TouchRangeChange));
        touchRange.text = GameManager.touchRange.ToString();
        downRange.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(DownRangeChange));
        downRange.text = GameManager.downRange.ToString();
    }
    public void Return2MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void JudgeTimingOffsetChange(string value)
    {
        GameManager.judgeTimingOffset = float.Parse(value);
        judgeTimingOffset.text = GameManager.judgeTimingOffset.ToString();
    }
    public void NoteSyncOffsetChange(string value)
    {
        GameManager.noteSyncOffset = float.Parse(value);
        noteSyncOffset.text = GameManager.noteSyncOffset.ToString();
    }
    public void NoteSpeedChange(string value)
    {
        GameManager.noteSpeed = float.Parse(value);
        noteSpeed.text = GameManager.noteSpeed.ToString();
    }

    public void PerfectRangeChange(string value)
    {
        GameManager.perfectRange = float.Parse(value);
        perfectRange.text = GameManager.perfectRange.ToString();
    }
    public void GreatRangeChange(string value)
    {
        GameManager.greatRange = float.Parse(value);
        greatRange.text = GameManager.greatRange.ToString();
    }
    public void GoodRangeChange(string value)
    {
        GameManager.goodRange = float.Parse(value);
        goodRange.text = GameManager.goodRange.ToString();
    }
    public void BadRangeChange(string value)
    {
        GameManager.badRange = float.Parse(value);
        badRange.text = GameManager.badRange.ToString();
    }
    public void SlideRangeChange(string value)
    {
        GameManager.slideRange = float.Parse(value);
        slideRange.text = GameManager.slideRange.ToString();
    }
    public void JumpRangeChange(string value)
    {
        GameManager.jumpRange = float.Parse(value);
        jumpRange.text = GameManager.jumpRange.ToString();
    }
    public void TouchRangeChange(string value)
    {
        GameManager.touchRange = float.Parse(value);
        touchRange.text = GameManager.touchRange.ToString();
    }
    public void DownRangeChange(string value)
    {
        GameManager.downRange = float.Parse(value);
        downRange.text = GameManager.downRange.ToString();
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
