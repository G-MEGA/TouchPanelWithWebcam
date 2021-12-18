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

        leftTime = time - currentTime + GameManager.judgeTimingOffset;//���⿡ ��ġ�� ���ϸ�.... �� �ʰ� ������. ���⿡ ������ ���ϸ�.... �� ���� ������
        if (leftTime < 0f)//Mathf.abs
            absLeftTime = -leftTime;
        else
            absLeftTime = leftTime;

        if (leftTime < -GameManager.goodRange)//���� ���� (-good)-(bad)
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
                //1. �̹� �����ӿ� ��ġ�Ǿ��� - �� �κ��� �÷��̾ �����ؼ� �̸� isAvailableTouch�� �������ش�
                //2. ������ �ʾҰ�
                //3. ��ġ�� �´� - ��ġ�� �ּҰ� ��Ʈ�� �ִ뺸�� �۰�, ��ġ�� �ִ밡 ��Ʈ�� �ּҺ��� ũ�� �ǰڴ�.
                //��ġ�� �߿��� ���� �������� �̿�����.
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

        leftTime = time - currentTime + GameManager.judgeTimingOffset;//���⿡ ��ġ�� ���ϸ�.... �� �ʰ� ������. ���⿡ ������ ���ϸ�.... �� ���� ������
        if (leftTime < 0f)//Mathf.abs
            absLeftTime = -leftTime;
        else
            absLeftTime = leftTime;

        if (leftTime < -GameManager.slideRange)//���� ����
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
                //��ġ�� �°�
                {
                    //�˸��� �������� �����̴� ��ġ�� �ϳ��� �ִٸ� perfect
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

        LeftTime = time - currentTime + GameManager.judgeTimingOffset;//���⿡ ��ġ�� ���ϸ�.... �� �ʰ� ������. ���⿡ ������ ���ϸ�.... �� ���� ������
        if (LeftTime < 0f)//Mathf.abs
            absLeftTime = -LeftTime;
        else
            absLeftTime = LeftTime;

        if (LeftTime < -GameManager.jumpRange)//���� ����
        {
            processed = true;
            return NoteState.Bad;
        }
        else if (LeftTime <= 0f)
        {
            for (int i = 0; i <= manager.touchesLastIndex; i++)//��ġ�� �ϳ��� �ִٸ�(�翬�� ��Ʈ�� ����) ���� �ȵ�
            {
                if (!manager.touches[i].isGhost)
                {
                    return NoteState.BeforeJudge;
                }
            }
            processed = true;//��ġ�� �ϳ��� ���ٸ� perfect�� ����
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

        leftTime = time - currentTime + GameManager.judgeTimingOffset;//���⿡ ��ġ�� ���ϸ�.... �� �ʰ� ������. ���⿡ ������ ���ϸ�.... �� ���� ������
        if (leftTime < 0f)//Mathf.abs
            absLeftTime = -leftTime;
        else
            absLeftTime = leftTime;

        if (leftTime < -GameManager.touchRange)//���� ����
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
                //��ġ�� �´� ��ġ�� �ϳ��� �ִٸ� perfect
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

        LeftTime = time - currentTime + GameManager.judgeTimingOffset;//���⿡ ��ġ�� ���ϸ�.... �� �ʰ� ������. ���⿡ ������ ���ϸ�.... �� ���� ������
        if (LeftTime < 0f)//Mathf.abs
            absLeftTime = -LeftTime;
        else
            absLeftTime = LeftTime;

        if (LeftTime < -GameManager.downRange)//���� ����
        {
            processed = true;
            return NoteState.Bad;
        }
        else if (LeftTime <= GameManager.downRange)
        {
            //Down �Է��� ������ �ȵǾ��⿡ �ϴ� ������ perfect�� ��
            processed = true;
            return NoteState.Perfect;
        }
        else
            return NoteState.BeforeJudge;
    }
}