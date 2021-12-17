using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static private string musicPath = "C:/music.wav";
    static public string MusicPath
    {
        set
        {
            musicPath = value.Replace('\\', '/');
            musicPath = musicPath.Trim();
        }
        get
        {
            return musicPath;
        }
    }
    static private string chartPath = "C:\\chart.txt";
    static public string ChartPath
    {
        set
        {
            chartPath = value.Replace('/', '\\');
            chartPath = chartPath.Trim();
        }
        get
        {
            return chartPath;
        }
    }
    static public float perfectRange = 0.062f;
    static public float greatRange = 0.103f;
    static public float goodRange = 0.144f;
    static public float badRange = 0.185f;

    static public float showNotesPositionMax = 50f;
    static public float showNotesPositionMin = -10f;

    static public float noteSpeed = 10f;
}
