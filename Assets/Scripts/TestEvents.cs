using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEvents : MonoBehaviour
{
    
    private void OnEnable()
    {
        GameLoopManager.StartFallEvent += StartFallHandler;
        GameLoopManager.StartStasisEvent += StartStasisHandler;
    }
    
    private void OnDisable()
    {
        GameLoopManager.StartFallEvent -= StartFallHandler;
        GameLoopManager.StartStasisEvent -= StartStasisHandler;
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
