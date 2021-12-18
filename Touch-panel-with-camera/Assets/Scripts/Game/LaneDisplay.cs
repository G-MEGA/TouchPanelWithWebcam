using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneDisplay : MonoBehaviour
{
    [SerializeField]
    float width = 10f;
    [SerializeField]
    GameObject footObjectPrefab;
    [SerializeField]
    GameObject jumpNotePrefab;
    [SerializeField]
    GameObject normalNoteLPrefab;
    [SerializeField]
    GameObject normalNoteRPrefab;
    [SerializeField]
    GameObject slideNoteLLPrefab;
    [SerializeField]
    GameObject slideNoteLRPrefab;
    [SerializeField]
    GameObject slideNoteRLPrefab;
    [SerializeField]
    GameObject slideNoteRRPrefab;
    [SerializeField]
    GameObject downNotePrefab;
    [SerializeField]
    GameObject touchNoteLPrefab;
    [SerializeField]
    GameObject touchNoteRPrefab;

    List<Transform> footObjectPool = new List<Transform>();
    int nextFootObjectPoolIndex = 0;

    List<Transform> jumpNotePool = new List<Transform>();
    int nextjumpNotePoolIndex = 0;

    List<Transform> normalNoteLPool = new List<Transform>();
    int nextNormalNoteLPoolIndex = 0;

    List<Transform> normalNoteRPool = new List<Transform>();
    int nextNormalNoteRPoolIndex = 0;

    List<Transform> slideNoteLLPool = new List<Transform>();
    int nextSlideNoteLLPoolIndex = 0;

    List<Transform> slideNoteLRPool = new List<Transform>();
    int nextSlideNoteLRPoolIndex = 0;

    List<Transform> slideNoteRLPool = new List<Transform>();
    int nextSlideNoteRLPoolIndex = 0;

    List<Transform> slideNoteRRPool = new List<Transform>();
    int nextSlideNoteRRPoolIndex = 0;

    List<Transform> downNotePool = new List<Transform>();
    int nextDownNotePoolIndex = 0;

    List<Transform> touchNoteLPool = new List<Transform>();
    int nextTouchNoteLPoolIndex = 0;

    List<Transform> touchNoteRPool = new List<Transform>();
    int nextTouchNoteRPoolIndex = 0;

    int length;
    public void Clear()
    {
        length = footObjectPool.Count;
        for (int i = 0; i < length; i++)
            footObjectPool[i].gameObject.SetActive(false);

        nextFootObjectPoolIndex = 0;

        length = jumpNotePool.Count;
        for (int i = 0; i < length; i++)
            jumpNotePool[i].gameObject.SetActive(false);
        nextjumpNotePoolIndex = 0;

        length = normalNoteLPool.Count;
        for (int i = 0; i < length; i++)
            normalNoteLPool[i].gameObject.SetActive(false);
        nextNormalNoteLPoolIndex = 0;

        length = normalNoteRPool.Count;
        for (int i = 0; i < length; i++)
            normalNoteRPool[i].gameObject.SetActive(false);
        nextNormalNoteRPoolIndex = 0;

        length = slideNoteLLPool.Count;
        for (int i = 0; i < length; i++)
            slideNoteLLPool[i].gameObject.SetActive(false);
        nextSlideNoteLLPoolIndex = 0;

        length = slideNoteLRPool.Count;
        for (int i = 0; i < length; i++)
            slideNoteLRPool[i].gameObject.SetActive(false);
        nextSlideNoteLRPoolIndex = 0;

        length = slideNoteRLPool.Count;
        for (int i = 0; i < length; i++)
            slideNoteRLPool[i].gameObject.SetActive(false);
        nextSlideNoteRLPoolIndex = 0;

        length = slideNoteRRPool.Count;
        for (int i = 0; i < length; i++)
            slideNoteRRPool[i].gameObject.SetActive(false);
        nextSlideNoteRRPoolIndex = 0;

        length = downNotePool.Count;
        for (int i = 0; i < length; i++)
            downNotePool[i].gameObject.SetActive(false);
        nextDownNotePoolIndex = 0;

        length = touchNoteLPool.Count;
        for (int i = 0; i < length; i++)
            touchNoteLPool[i].gameObject.SetActive(false);
        nextTouchNoteLPoolIndex = 0;

        length = touchNoteRPool.Count;
        for (int i = 0; i < length; i++)
            touchNoteRPool[i].gameObject.SetActive(false);
        nextTouchNoteRPoolIndex = 0;
    }

    public void FootDisplay(float minX,float scale)
    {
        if (nextFootObjectPoolIndex == footObjectPool.Count)
            footObjectPool.Add(Instantiate(footObjectPrefab,transform).transform);

        footObjectPool[nextFootObjectPoolIndex].gameObject.SetActive(true);
        footObjectPool[nextFootObjectPoolIndex].localPosition = new Vector3(minX * width, 0f, 0f);
        footObjectPool[nextFootObjectPoolIndex].localScale = new Vector3(scale, 1f, 1f);

        nextFootObjectPoolIndex++;
    }
    float notePosition;
    public void JumpNoteDisplay(float leftTime)
    {
        notePosition = GameManager.noteSpeed * leftTime;
        if (notePosition > GameManager.showNotesPositionMax || notePosition < GameManager.showNotesPositionMin)
            return;

        if (nextjumpNotePoolIndex == jumpNotePool.Count)
            jumpNotePool.Add(Instantiate(jumpNotePrefab, transform).transform);
        jumpNotePool[nextjumpNotePoolIndex].gameObject.SetActive(true);
        jumpNotePool[nextjumpNotePoolIndex].localPosition = new Vector3(0f,0f, notePosition);

        nextjumpNotePoolIndex++;
    }
    public void NormalNoteLDisplay(float position, float scale,float leftTime)
    {
        notePosition = GameManager.noteSpeed * leftTime;
        if (notePosition > GameManager.showNotesPositionMax || notePosition < GameManager.showNotesPositionMin)
            return;

        if (nextNormalNoteLPoolIndex == normalNoteLPool.Count)
            normalNoteLPool.Add(Instantiate(normalNoteLPrefab, transform).transform);
        normalNoteLPool[nextNormalNoteLPoolIndex].gameObject.SetActive(true);
        normalNoteLPool[nextNormalNoteLPoolIndex].localPosition = new Vector3(position * width, 0f, notePosition);
        normalNoteLPool[nextNormalNoteLPoolIndex].localScale = new Vector3(scale, 1f, 1f);

        nextNormalNoteLPoolIndex++;
    }
    public void NormalNoteRDisplay(float position, float scale, float leftTime)
    {
        notePosition = GameManager.noteSpeed * leftTime;
        if (notePosition > GameManager.showNotesPositionMax || notePosition < GameManager.showNotesPositionMin)
            return;

        if (nextNormalNoteRPoolIndex == normalNoteRPool.Count)
            normalNoteRPool.Add(Instantiate(normalNoteRPrefab, transform).transform);
        normalNoteRPool[nextNormalNoteRPoolIndex].gameObject.SetActive(true);
        normalNoteRPool[nextNormalNoteRPoolIndex].localPosition = new Vector3(position * width, 0f, notePosition);
        normalNoteRPool[nextNormalNoteRPoolIndex].localScale = new Vector3(scale, 1f, 1f);

        nextNormalNoteRPoolIndex++;
    }
    public void SlideNoteLLDisplay(float position, float scale, float leftTime)
    {
        notePosition = GameManager.noteSpeed * leftTime;
        if (notePosition > GameManager.showNotesPositionMax || notePosition < GameManager.showNotesPositionMin)
            return;

        if (nextSlideNoteLLPoolIndex == slideNoteLLPool.Count)
            slideNoteLLPool.Add(Instantiate(slideNoteLLPrefab, transform).transform);
        slideNoteLLPool[nextSlideNoteLLPoolIndex].gameObject.SetActive(true);
        slideNoteLLPool[nextSlideNoteLLPoolIndex].localPosition = new Vector3(position * width, 0f, notePosition);
        slideNoteLLPool[nextSlideNoteLLPoolIndex].localScale = new Vector3(scale, 1f, 1f);

        nextSlideNoteLLPoolIndex++;
    }
    public void SlideNoteLRDisplay(float position, float scale, float leftTime)
    {
        notePosition = GameManager.noteSpeed * leftTime;
        if (notePosition > GameManager.showNotesPositionMax || notePosition < GameManager.showNotesPositionMin)
            return;

        if (nextSlideNoteLRPoolIndex == slideNoteLRPool.Count)
            slideNoteLRPool.Add(Instantiate(slideNoteLRPrefab, transform).transform);
        slideNoteLRPool[nextSlideNoteLRPoolIndex].gameObject.SetActive(true);
        slideNoteLRPool[nextSlideNoteLRPoolIndex].localPosition = new Vector3(position * width, 0f, notePosition);
        slideNoteLRPool[nextSlideNoteLRPoolIndex].localScale = new Vector3(scale, 1f, 1f);

        nextSlideNoteLRPoolIndex++;
    }
    public void SlideNoteRLDisplay(float position, float scale, float leftTime)
    {
        notePosition = GameManager.noteSpeed * leftTime;
        if (notePosition > GameManager.showNotesPositionMax || notePosition < GameManager.showNotesPositionMin)
            return;

        if (nextSlideNoteRLPoolIndex == slideNoteRLPool.Count)
            slideNoteRLPool.Add(Instantiate(slideNoteRLPrefab, transform).transform);
        slideNoteRLPool[nextSlideNoteRLPoolIndex].gameObject.SetActive(true);
        slideNoteRLPool[nextSlideNoteRLPoolIndex].localPosition = new Vector3(position * width, 0f, notePosition);
        slideNoteRLPool[nextSlideNoteRLPoolIndex].localScale = new Vector3(scale, 1f, 1f);

        nextSlideNoteRLPoolIndex++;
    }
    public void SlideNoteRRDisplay(float position, float scale, float leftTime)
    {
        notePosition = GameManager.noteSpeed * leftTime;
        if (notePosition > GameManager.showNotesPositionMax || notePosition < GameManager.showNotesPositionMin)
            return;

        if (nextSlideNoteRRPoolIndex == slideNoteRRPool.Count)
            slideNoteRRPool.Add(Instantiate(slideNoteRRPrefab, transform).transform);
        slideNoteRRPool[nextSlideNoteRRPoolIndex].gameObject.SetActive(true);
        slideNoteRRPool[nextSlideNoteRRPoolIndex].localPosition = new Vector3(position * width, 0f, notePosition);
        slideNoteRRPool[nextSlideNoteRRPoolIndex].localScale = new Vector3(scale, 1f, 1f);

        nextSlideNoteRRPoolIndex++;
    }
    public void DownNoteDisplay(float leftTime)
    {
        notePosition = GameManager.noteSpeed * leftTime;
        if (notePosition > GameManager.showNotesPositionMax || notePosition < GameManager.showNotesPositionMin)
            return;

        if (nextDownNotePoolIndex == downNotePool.Count)
            downNotePool.Add(Instantiate(downNotePrefab, transform).transform);
        downNotePool[nextDownNotePoolIndex].gameObject.SetActive(true);
        downNotePool[nextDownNotePoolIndex].localPosition = new Vector3(0f, 0f, notePosition);

        nextDownNotePoolIndex++;
    }
    public void TouchNoteLDisplay(float position, float scale, float leftTime)
    {
        notePosition = GameManager.noteSpeed * leftTime;
        if (notePosition > GameManager.showNotesPositionMax || notePosition < GameManager.showNotesPositionMin)
            return;

        if (nextTouchNoteLPoolIndex == touchNoteLPool.Count)
            touchNoteLPool.Add(Instantiate(touchNoteLPrefab, transform).transform);
        touchNoteLPool[nextTouchNoteLPoolIndex].gameObject.SetActive(true);
        touchNoteLPool[nextTouchNoteLPoolIndex].localPosition = new Vector3(position * width, 0f, notePosition);
        touchNoteLPool[nextTouchNoteLPoolIndex].localScale = new Vector3(scale, 1f, 1f);

        nextTouchNoteLPoolIndex++;
    }
    public void TouchNoteRDisplay(float position, float scale, float leftTime)
    {
        notePosition = GameManager.noteSpeed * leftTime;
        if (notePosition > GameManager.showNotesPositionMax || notePosition < GameManager.showNotesPositionMin)
            return;

        if (nextTouchNoteRPoolIndex == touchNoteRPool.Count)
            touchNoteRPool.Add(Instantiate(touchNoteRPrefab, transform).transform);
        touchNoteRPool[nextTouchNoteRPoolIndex].gameObject.SetActive(true);
        touchNoteRPool[nextTouchNoteRPoolIndex].localPosition = new Vector3(position * width, 0f, notePosition);
        touchNoteRPool[nextTouchNoteRPoolIndex].localScale = new Vector3(scale, 1f, 1f);

        nextTouchNoteRPoolIndex++;
    }
}
