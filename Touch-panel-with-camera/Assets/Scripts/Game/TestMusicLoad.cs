using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestMusicLoad : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    AudioClip myClip;

    void Start()
    {
        StartCoroutine(GetAudioClip());
        audioSource.Play();
    }

    IEnumerator GetAudioClip()
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///C:/Users/scw97/Desktop/영상편집/luminous-pajama.wav", AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                myClip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = myClip;
                audioSource.Play();
            }
        }
    }

    private void Update()
    {
        Debug.Log(audioSource.time);
    }
}
