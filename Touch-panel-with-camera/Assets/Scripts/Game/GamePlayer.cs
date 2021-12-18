using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class GamePlayer : MonoBehaviour
{
    [SerializeField]
    LaneDisplay laneDisplay;
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    InGameMenu menu;

    AudioClip music;
    /*
     * chartData구조
     * 
     * 각 노트는 줄바꿈으로 구분함
     * 
     * 일반 노트
     *     N,[위치],[스케일],[시간],[지시(L또는R)]
     * 슬라이드 노트
     *     S,[위치],[스케일],[시간],[지시(L또는R)],[슬라이드방향(L또는R)]
     * 점프 노트
     *     J,[시간]
     * 다운 노트
     *     D,[시간]
     * 터치노트
     *     T,[위치],[스케일],[시간],[지시(L또는R)]
     *     
     * 예시
     *     N,0.5,0.2,5,L
     *     S,0.1,0.3,2,L,R
     *     J,7
     *     D,3.5
     *     T,0.5,0.2,5.5,L
     */
    string chartData = "";
    CamInputManager manager;
    float prevTime = 0f;
    public Note[] notes;

    public int combo=0;
    public int perfect=0;
    public int great=0;
    public int good=0;
    public int bad=0;
    const string perfectText = "<color=cyan>PERFECT</color>";
    const string greatText = "<color=lime>GREAT</color>";
    const string goodText = "<color=yellow>GOOD</color>";
    const string badText = "<color=red>BAD</color>";

    void Start()
    {
        using (StreamReader streamReader = new StreamReader(GameManager.ChartPath))
        {
            chartData = streamReader.ReadToEnd();
        }
        chartData = chartData.Trim();
        string[] noteData = chartData.Split('\n');
        length = noteData.Length;
        notes = new Note[length];
        Direction whichLeg;
        Direction slideDirection;
        for (int i = 0; i < length; i++)
        {
            string[] noteInfo = noteData[i].Trim().Split(',');
            switch (noteInfo[0])
            {
                case "N":
                    if (noteInfo[4] == "L")
                        whichLeg = Direction.Left;
                    else
                        whichLeg = Direction.Right;
                    notes[i] = new NormalNote(float.Parse(noteInfo[1]), float.Parse(noteInfo[2]), float.Parse(noteInfo[3]), whichLeg);
                    break;
                case "S":
                    if (noteInfo[4] == "R")
                        whichLeg = Direction.Right;
                    else
                        whichLeg = Direction.Left;
                    if (noteInfo[5] == "R")
                        slideDirection = Direction.Right;
                    else
                        slideDirection = Direction.Left;
                    notes[i] = new SlideNote(float.Parse(noteInfo[1]), float.Parse(noteInfo[2]), float.Parse(noteInfo[3]), whichLeg, slideDirection);
                    break;
                case "J":
                    notes[i] = new JumpNote(float.Parse(noteInfo[1]));
                    break;
                case "D":
                    notes[i] = new DownNote(float.Parse(noteInfo[1]));
                    break;
                case "T":
                    if (noteInfo[4] == "L")
                        whichLeg = Direction.Left;
                    else
                        whichLeg = Direction.Right;
                    notes[i] = new TouchNote(float.Parse(noteInfo[1]), float.Parse(noteInfo[2]), float.Parse(noteInfo[3]), whichLeg);
                    break;
                default:
                    break;
            }
        }

        StartCoroutine(GetAudioClip());

        manager = CamInputManager.Instance;
    }

    IEnumerator GetAudioClip()
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///"+GameManager.MusicPath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                music = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = music;
                audioSource.Play();
            }
        }
    }

    int length;
    float inverseResolutionX;
    float currentTime;
    NoteState noteState;
    private void Update()
    {
        currentTime = audioSource.time;
        if (prevTime != currentTime)
        {
            laneDisplay.Clear();

            inverseResolutionX = 1f / manager.Resolution.x;
            length = manager.touchesLastIndex + 1;
            for (int i = 0; i < length; i++)
            {
                if (!manager.touches[i].isGhost)
                {
                    laneDisplay.FootDisplay(manager.touches[i].minX * inverseResolutionX, (manager.touches[i].maxX - manager.touches[i].minX + 1) * inverseResolutionX);
                }
            }

            if (NormalNote.isAvailableTouch == null || NormalNote.isAvailableTouch.Length != manager.touches.Length)
                NormalNote.isAvailableTouch = new bool[manager.touches.Length];

            length = manager.touchesLastIndex + 1;
            for (int i = 0; i < length; i++)
                NormalNote.isAvailableTouch[i] = !manager.touches[i].isGhost && manager.touches[i].isDownThisFrame;

            length = notes.Length;
            JumpNote jumpNote;
            NormalNote normalNote;
            SlideNote slideNote;
            DownNote downNote;
            TouchNote touchNote;
            for (int i = 0; i < length; i++)
            {
                noteState = notes[i].Process(prevTime, currentTime, manager);
                //아마 여기에 그래픽이랑 판정?점수? 작업 넣으면 될거임
                if (notes[i] is JumpNote)
                {
                    jumpNote = (JumpNote)notes[i];
                    switch (noteState)
                    {
                        case NoteState.BeforeJudge:
                            laneDisplay.JumpNoteDisplay(jumpNote.time - currentTime + GameManager.noteSyncOffset);
                            break;
                        case NoteState.Bad:
                            bad++;
                            combo=0;
                            menu.JudgeAndComboUpdate(badText, combo.ToString(),perfect,great,good,bad);
                            break;
                        case NoteState.Perfect:
                            perfect++;
                            combo++;
                            menu.JudgeAndComboUpdate(perfectText, combo.ToString(), perfect, great, good, bad);
                            break;
                        case NoteState.Processed:
                            break;
                    }
                }
                else if (notes[i] is NormalNote)
                {
                    normalNote = (NormalNote)notes[i];
                    switch (noteState)
                    {
                        case NoteState.BeforeJudge:
                            if (normalNote.whichLeg == Direction.Left)
                                laneDisplay.NormalNoteLDisplay(normalNote.position, normalNote.scale, normalNote.time - currentTime + GameManager.noteSyncOffset);
                            else
                                laneDisplay.NormalNoteRDisplay(normalNote.position, normalNote.scale, normalNote.time - currentTime + GameManager.noteSyncOffset);
                            break;
                        case NoteState.Bad:
                            bad++;
                            combo = 0;
                            menu.JudgeAndComboUpdate(badText,combo.ToString(), perfect, great, good, bad);
                            break;
                        case NoteState.Good:
                            good++;
                            combo++;
                            menu.JudgeAndComboUpdate(goodText, combo.ToString(), perfect, great, good, bad);
                            break;
                        case NoteState.Great:
                            great++;
                            combo++;
                            menu.JudgeAndComboUpdate(greatText, combo.ToString(), perfect, great, good, bad);
                            break;
                        case NoteState.Perfect:
                            perfect++;
                            combo++;
                            menu.JudgeAndComboUpdate(perfectText, combo.ToString(), perfect, great, good, bad);
                            break;
                        case NoteState.Processed:
                            break;
                    }
                }
                else if (notes[i] is SlideNote)
                {
                    slideNote = (SlideNote)notes[i];
                    switch (noteState)
                    {
                        case NoteState.BeforeJudge:
                            if (slideNote.whichLeg == Direction.Left)
                            {
                                if (slideNote.slideDirection == Direction.Left)
                                    laneDisplay.SlideNoteLLDisplay(slideNote.position, slideNote.scale, slideNote.time - currentTime + GameManager.noteSyncOffset);
                                else
                                    laneDisplay.SlideNoteLRDisplay(slideNote.position, slideNote.scale, slideNote.time - currentTime + GameManager.noteSyncOffset);
                            }
                            else
                            {
                                if (slideNote.slideDirection == Direction.Left)
                                    laneDisplay.SlideNoteRLDisplay(slideNote.position, slideNote.scale, slideNote.time - currentTime + GameManager.noteSyncOffset);
                                else
                                    laneDisplay.SlideNoteRRDisplay(slideNote.position, slideNote.scale, slideNote.time - currentTime + GameManager.noteSyncOffset);
                            }
                            break;
                        case NoteState.Bad:
                            bad++;
                            combo = 0;
                            menu.JudgeAndComboUpdate(badText, combo.ToString(), perfect, great, good, bad);
                            break;
                        case NoteState.Perfect:
                            perfect++;
                            combo++;
                            menu.JudgeAndComboUpdate(perfectText, combo.ToString(), perfect, great, good, bad);
                            break;
                        case NoteState.Processed:
                            break;
                    }
                }
                else if (notes[i] is DownNote)
                {
                    downNote = (DownNote)notes[i];
                    switch (noteState)
                    {
                        case NoteState.BeforeJudge:
                            laneDisplay.DownNoteDisplay(downNote.time - currentTime + GameManager.noteSyncOffset);
                            break;
                        case NoteState.Bad:
                            bad++;
                            combo = 0;
                            menu.JudgeAndComboUpdate(badText, combo.ToString(), perfect, great, good, bad);
                            break;
                        case NoteState.Perfect:
                            perfect++;
                            combo++;
                            menu.JudgeAndComboUpdate(perfectText, combo.ToString(), perfect, great, good, bad);
                            break;
                        case NoteState.Processed:
                            break;
                    }
                }
                else if (notes[i] is TouchNote)
                {
                    touchNote = (TouchNote)notes[i];
                    switch (noteState)
                    {
                        case NoteState.BeforeJudge:
                            if (touchNote.whichLeg == Direction.Left)
                                laneDisplay.TouchNoteLDisplay(touchNote.position, touchNote.scale, touchNote.time - currentTime + GameManager.noteSyncOffset);
                            else
                                laneDisplay.TouchNoteRDisplay(touchNote.position, touchNote.scale, touchNote.time - currentTime + GameManager.noteSyncOffset);
                            break;
                        case NoteState.Bad:
                            bad++;
                            combo = 0;
                            menu.JudgeAndComboUpdate(badText, combo.ToString(), perfect, great, good, bad);
                            break;
                        case NoteState.Perfect:
                            perfect++;
                            combo++;
                            menu.JudgeAndComboUpdate(perfectText, combo.ToString(), perfect, great, good, bad);
                            break;
                        case NoteState.Processed:
                            break;
                    }
                }
            }
            prevTime = currentTime;
        }
    }


}
