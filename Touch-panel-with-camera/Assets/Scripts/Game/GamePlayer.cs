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

    AudioClip music;
    /*
     * chartData����
     * 
     * �� ��Ʈ�� �ٹٲ����� ������
     * 
     * �Ϲ� ��Ʈ
     *     N,[��ġ],[������],[�ð�],[����(L�Ǵ�R)]
     * �����̵� ��Ʈ
     *     S,[��ġ],[������],[�ð�],[����(L�Ǵ�R)],[�����̵����(L�Ǵ�R)]
     * ���� ��Ʈ
     *     J,[�ð�]
     *     
     * ����
     *     N,0.5,0.2,5,L
     *     S,0.1,0.3,2,L,R
     *     J,7
     */
    string chartData = "";
    CamInputManager manager;
    float prevTime = 0f;
    public Note[] notes;

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
            for (int i = 0; i < length; i++)
            {
                noteState = notes[i].Process(prevTime, currentTime, manager);
                //�Ƹ� ���⿡ �׷����̶� ����?����? �۾� ������ �ɰ���
                if (notes[i] is JumpNote)
                {
                    jumpNote = (JumpNote)notes[i];
                    switch (noteState)
                    {
                        case NoteState.BeforeJudge:
                            laneDisplay.JumpNoteDisplay(jumpNote.time - currentTime);
                            break;
                        case NoteState.Bad:
                            break;
                        case NoteState.Perfect:
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
                                laneDisplay.NormalNoteLDisplay(normalNote.position, normalNote.scale, normalNote.time - currentTime);
                            else
                                laneDisplay.NormalNoteRDisplay(normalNote.position, normalNote.scale, normalNote.time - currentTime);
                            break;
                        case NoteState.Bad:
                            break;
                        case NoteState.Good:
                            break;
                        case NoteState.Great:
                            break;
                        case NoteState.Perfect:
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
                                    laneDisplay.SlideNoteLLDisplay(slideNote.position, slideNote.scale, slideNote.time - currentTime);
                                else
                                    laneDisplay.SlideNoteLRDisplay(slideNote.position, slideNote.scale, slideNote.time - currentTime);
                            }
                            else
                            {
                                if (slideNote.slideDirection == Direction.Left)
                                    laneDisplay.SlideNoteRLDisplay(slideNote.position, slideNote.scale, slideNote.time - currentTime);
                                else
                                    laneDisplay.SlideNoteRRDisplay(slideNote.position, slideNote.scale, slideNote.time - currentTime);
                            }
                            break;
                        case NoteState.Bad:
                            break;
                        case NoteState.Perfect:
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
