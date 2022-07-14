using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEvents : MonoBehaviour
{
    
    private void OnEnable()
    {
        GameLoopManager.startFallEvent += StartFallHandler;
        GameLoopManager.startStasisEvent += StartStasisHandler;
    }
    
    private void OnDisable()
    {
        GameLoopManager.startFallEvent -= StartFallHandler;
        GameLoopManager.startStasisEvent -= StartStasisHandler;
    }

    private void StartFallHandler()
    {
        Debug.Log("START FALL EVENT");
    }

    private void StartStasisHandler()
    {
        Debug.Log("START STASIS EVENT");
    }
    
}
