using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamInputWebcamDisplaysParent : MonoBehaviour
{
    [SerializeField]
    private GameObject CamInputWebcamDisplayPrefab;

    void Start()
    {
        for (int i = 0; i < CamInputManager.Instance.camInputs.Length; i++)
        {
            CamInputWebcamDisplay display = Instantiate(CamInputWebcamDisplayPrefab, transform).GetComponent<CamInputWebcamDisplay>();
            display.Init(i);
        }
    }
    private void Update()
    {
        
    }
}
