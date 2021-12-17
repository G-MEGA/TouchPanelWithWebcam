using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCtrl : MonoBehaviour
{
    [SerializeField]
    private InputField musicPath;
    [SerializeField]
    private InputField chartPath;
    private void Awake()
    {
        musicPath.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(MusicPathChanged));
        musicPath.text = GameManager.MusicPath;
        chartPath.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(ChartPath));
        chartPath.text = GameManager.ChartPath;
    }
    public void MusicPathChanged(string path)
    {
        GameManager.MusicPath = path;
        musicPath.text = GameManager.MusicPath;
    }
    public void ChartPath(string path)
    {
        GameManager.ChartPath = path;
        chartPath.text = GameManager.ChartPath;
    }

    public void GoToSettingScene()
    {
        SceneManager.LoadScene(1);
    }
    public void GameStart()
    {
        SceneManager.LoadScene(2);
    }
}
