using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Note
{
    public abstract NoteState Process(float prevTime,float currentTime, CamInputManager manager);
}
public enum NoteState
{
    BeforeJudge, Bad, Good, Great, Perfect, Hold, Unhold, Processed
}
public enum Direction
{
    Left,Right
}

public class NormalNote : Note
{
    static public bool[] isAvailableTouch;

    bool processed = false;
    public float position;
    public float scale;
    public float time;
    public Direction whichLeg;

    public NormalNote(float position, float scale,float time, Direction whichLeg)
    {
        this.position = position;
        this.scale = scale;
        this.time = time;
        this.whichLeg = whichLeg;
    }

    float leftTime;
    float absLeftTime;
    public override NoteState Process(float prevTime, float currentTime, CamInputManager manager)
    {
        if (processed)
            return NoteState.Processed;

        leftTime = time - currentTime + GameManager.judgeTimingOffset;//여기에 수치를 더하면.... 더 늦게 판정됨. 여기에 음수를 더하면.... 더 빨리 판정됨
        if (leftTime < 0f)//Mathf.abs
            absLeftTime = -leftTime;
        else
            absLeftTime = leftTime;

        if (leftTime < -GameManager.goodRange)//판정 범위 (-good)-(bad)
        {
            processed = true;
            return NoteState.Bad;
        }
        else if (leftTime <= GameManager.badRange)
        {

            int touchIndex = -1;
            float minDistanceAboutNote2Touch = float.MaxValue;
            float distanceAboutNote2Touch;
            float xMin = (position - scale * 0.5f) * manager.Resolution.x;
            float xMax = (position + scale * 0.5f) * manager.Resolution.x;
            for (int i = 0; i <= manager.touchesLastIndex; i++)
            {
                if (isAvailableTouch[i] && manager.touches[i].minX <= xMax && manager.touches[i].maxX + 1f >= xMin)
                //1. 이번 프레임에 터치되었고 - 이 부분은 플레이어가 판정해서 미리 isAvailableTouch에 적용해준다
                //2. 사용되지 않았고
                //3. 위치가 맞는 - 터치의 최소가 노트의 최대보다 작고, 터치의 최대가 노트의 최소보다 크면 되겠다.
                //터치들 중에서 가장 가까운것을 이용하자.
                {
                    distanceAboutNote2Touch = Mathf.Abs(manager.touches[i].x - position * (manager.Resolution.x - 1));
                    if (minDistanceAboutNote2Touch > distanceAboutNote2Touch)
                    {
                        touchIndex = i;
                        minDistanceAboutNote2Touch = distanceAboutNote2Touch;
                    }
                }
            }
            if (touchIndex != -1)
            {
                isAvailableTouch[touchIndex] = false;
                if (absLeftTime <= GameManager.perfectRange)
                {
                    processed = true;
                    return NoteState.Perfect;
                }
                else if (absLeftTime <= GameManager.greatRange)
                {
                    processed = true;
                    return NoteState.Great;
                }
                else if (absLeftTime <= GameManager.goodRange)
                {
                    processed = true;
                    return NoteState.Good;
                }
                else
                {
                    processed = true;
                    return NoteState.Bad;
                }
            }
            else
            {
                return NoteState.BeforeJudge;
            }
        }
        else
            return NoteState.BeforeJudge;
    }
}
public class SlideNote : Note
{
    bool processed = false;
    public float position;
    public float scale;
    public float time;
    public Direction whichLeg;
    public Direction slideDirection;

    public SlideNote(float position, float scale, float time, Direction whichLeg, Direction slideDirection)
    {
        this.position = position;
        this.scale = scale;
        this.time = time;
        this.whichLeg = whichLeg;
        this.slideDirection = slideDirection;
    }

    float leftTime;
    float absLeftTime;
    public override NoteState Process(float prevTime, float currentTime, CamInputManager manager)
    {
        if (processed)
            return NoteState.Processed;

        leftTime = time - currentTime + GameManager.judgeTimingOffset;//여기에 수치를 더하면.... 더 늦게 판정됨. 여기에 음수를 더하면.... 더 빨리 판정됨
        if (leftTime < 0f)//Mathf.abs
            absLeftTime = -leftTime;
        else
            absLeftTime = leftTime;

        if (leftTime < -GameManager.slideRange)//판정 범위
        {
            processed = true;
            return NoteState.Bad;
        }
        else if (leftTime <= GameManager.slideRange)
        {
            float xMin = (position - scale * 0.5f) * manager.Resolution.x;
            float xMax = (position + scale * 0.5f) * manager.Resolution.x;
            for (int i = 0; i <= manager.touchesLastIndex; i++)
            {
                if (manager.touches[i].minX <= xMax && manager.touches[i].maxX+1f >= xMin)
                //위치가 맞고
                {
                    //알맞은 방향으로 움직이는 터치가 하나라도 있다면 perfect
                    if (slideDirection == Direction.Left)
                    {
                        if (manager.touches[i].moveLeft)
                        {
                            processed = true;
                            return NoteState.Perfect;
                        }
                    }
                    else
                    {
                        if (manager.touches[i].moveRight)
                        {
                            processed = true;
                            return NoteState.Perfect;
                        }
                    }
                }
            }
            return NoteState.BeforeJudge;
        }
        else
            return NoteState.BeforeJudge;
    }
}
public class JumpNote : Note
{
    bool processed = false;
    public float time;

    public JumpNote(float time)
    {
        this.time = time;
    }

    float LeftTime;
    float absLeftTime;
    public override NoteState Process(float prevTime, float currentTime, CamInputManager manager)
    {
        if (processed)
            return NoteState.Processed;

        LeftTime = time - currentTime + GameManager.judgeTimingOffset;//여기에 수치를 더하면.... 더 늦게 판정됨. 여기에 음수를 더하면.... 더 빨리 판정됨
        if (LeftTime < 0f)//Mathf.abs
            absLeftTime = -LeftTime;
        else
            absLeftTime = LeftTime;

        if (LeftTime < -GameManager.jumpRange)//판정 범위
        {
            processed = true;
            return NoteState.Bad;
        }
        else if (LeftTime <= 0f)
        {
            for (int i = 0; i <= manager.touchesLastIndex; i++)//터치가 하나라도 있다면(당연히 고스트는 제외) 판정 안됨
            {
                if (!manager.touches[i].isGhost)
                {
                    return NoteState.BeforeJudge;
                }
            }
            processed = true;//터치가 하나도 없다면 perfect로 판정
            return NoteState.Perfect;
        }
        else
            return NoteState.BeforeJudge;
    }
}
public class TouchNote : Note
{
    bool processed = false;
    public float position;
    public float scale;
    public float time;
    public Direction whichLeg;

    public TouchNote(float position, float scale, float time, Direction whichLeg)
    {
        this.position = position;
        this.scale = scale;
        this.time = time;
        this.whichLeg = whichLeg;
    }

    float leftTime;
    float absLeftTime;
    public override NoteState Process(float prevTime, float currentTime, CamInputManager manager)
    {
        if (processed)
            return NoteState.Processed;

        leftTime = time - currentTime + GameManager.judgeTimingOffset;//여기에 수치를 더하면.... 더 늦게 판정됨. 여기에 음수를 더하면.... 더 빨리 판정됨
        if (leftTime < 0f)//Mathf.abs
            absLeftTime = -leftTime;
        else
            absLeftTime = leftTime;

        if (leftTime < -GameManager.touchRange)//판정 범위
        {
            processed = true;
            return NoteState.Bad;
        }
        else if (leftTime <= 0f)
        {
            float xMin = (position - scale * 0.5f) * manager.Resolution.x;
            float xMax = (position + scale * 0.5f) * manager.Resolution.x;
            for (int i = 0; i <= manager.touchesLastIndex; i++)
            {
                if (manager.touches[i].minX <= xMax && manager.touches[i].maxX + 1f >= xMin)
                //위치가 맞는 터치가 하나라도 있다면 perfect
                {
                    processed = true;
                    return NoteState.Perfect;
                }
            }
            return NoteState.BeforeJudge;
        }
        else
            return NoteState.BeforeJudge;
    }
}
public class DownNote : Note
{
    bool processed = false;
    public float time;

    public DownNote(float time)
    {
        this.time = time;
    }

    float LeftTime;
    float absLeftTime;
    public override NoteState Process(float prevTime, float currentTime, CamInputManager manager)
    {
        if (processed)
            return NoteState.Processed;

        LeftTime = time - currentTime + GameManager.judgeTimingOffset;//여기에 수치를 더하면.... 더 늦게 판정됨. 여기에 음수를 더하면.... 더 빨리 판정됨
        if (LeftTime < 0f)//Mathf.abs
            absLeftTime = -LeftTime;
        else
            absLeftTime = LeftTime;

        if (LeftTime < -GameManager.downRange)//판정 범위
        {
            processed = true;
            return NoteState.Bad;
        }
        else if (LeftTime <= GameManager.downRange)
        {
            //Down 입력이 구현이 안되었기에 일단 무조건 perfect로 둠
            processed = true;
            return NoteState.Perfect;
        }
        else
            return NoteState.BeforeJudge;
    }
}