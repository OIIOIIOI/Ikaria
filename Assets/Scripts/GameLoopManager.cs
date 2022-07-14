using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameLoopManager : MonoBehaviour
{
    
    private enum LoopState { Pause, Fall, Repair, Resolve, Prepare };

    private LoopState state = LoopState.Pause;
    private int currentCycle = 0;
    private int cyclesBeforeGameOver = 3;

    private AudioSource audioSource;
    private float audioLoopDuration;
    private int audioLoopsLeft;
    
    [HideInInspector]
    private Dictionary<LoopState, int> statesDurations;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        statesDurations = new Dictionary<LoopState, int>(){
            {LoopState.Fall, 2},
            {LoopState.Repair, 1},
            {LoopState.Resolve, 1},
            {LoopState.Prepare, 1},
        };
    }

    private void Start()
    {
        state = LoopState.Pause;
        
        StartFall();
    }

    private void StartLoop()
    {
        // Debug.Log("Starting audio loop");
        
        audioSource.loop = true;
        audioSource.Play();
        audioLoopDuration = audioSource.clip.length;
        StartCoroutine(nameof(LoopAudio));
    }
    
    private IEnumerator LoopAudio()
    {
        yield return new WaitForSeconds(audioLoopDuration);
        AudioLoopComplete();
    }

    private void AudioLoopComplete()
    {
        // Debug.Log("Audio loop complete!");
        
        audioLoopsLeft--;
        if (audioLoopsLeft > 0)
            StartCoroutine(nameof(LoopAudio));
        else
        {
            audioSource.Stop();
            PhaseComplete();
        }
    }

    private void PhaseComplete()
    {
        // Debug.Log("PHASE COMPLETE!");

        switch (state)
        {
            case LoopState.Fall:
                EndFall();
                break;
            
            case LoopState.Repair:
                EndRepair();
                break;
            
            case LoopState.Resolve:
                EndResolve();
                break;
            
            case LoopState.Prepare:
                EndPrepare();
                break;
        }
    }
    
    /* FALL ============================================ */

    private void StartFall()
    {
        Debug.Log("--------------------------------------------");
        Debug.Log("FALLING!");

        state = LoopState.Fall;
        
        audioLoopsLeft = statesDurations[state];
        StartLoop();
    }

    private void EndFall()
    {
        Debug.Log("STABILIZING...");

        state = LoopState.Pause;
        
        if (NeedsRepair())
            StartRepair();
        else if (NeedsResolve())
            StartResolve();
        else if (currentCycle < cyclesBeforeGameOver)
            StartPrepare();
        else
            CheckEndGameConditions();
    }
    
    /* REPAIR ============================================ */

    private void StartRepair()
    {
        Debug.Log("STARTING REPAIR");

        state = LoopState.Repair;

        audioLoopsLeft = statesDurations[state];
        StartLoop();
    }

    private void EndRepair()
    {
        Debug.Log("REPAIR ENDING");
        
        state = LoopState.Pause;
        
        if (NeedsResolve())
            StartResolve();
        else if (currentCycle < cyclesBeforeGameOver)
            StartPrepare();
        else
            CheckEndGameConditions();
    }
    
    /* RESOLVE ============================================ */

    private void StartResolve()
    {
        Debug.Log("STARTING RESOLVE");

        state = LoopState.Resolve;

        audioLoopsLeft = statesDurations[state];
        StartLoop();
    }

    private void EndResolve()
    {
        Debug.Log("RESOLVE ENDING");
        
        state = LoopState.Pause;
        
        if (currentCycle < cyclesBeforeGameOver)
            StartPrepare();
        else
            CheckEndGameConditions();
    }
    
    /* PREPARE ============================================ */

    private void StartPrepare()
    {
        Debug.Log("STARTING PREPARE");

        state = LoopState.Prepare;

        audioLoopsLeft = statesDurations[state];
        StartLoop();
    }

    private void EndPrepare()
    {
        Debug.Log("PREPARE ENDING");
        
        state = LoopState.Pause;
        
        StartFall();
    }
    
    /* CHECKS & END GAME ============================================ */

    private bool NeedsRepair()
    {
        // TODO Get actual info from somewhere
        return Random.value > 0.5f;
    }
    
    private bool NeedsResolve()
    {
        // TODO Get actual info from somewhere
        return Random.value > 0.5f;
    }

    private void CheckEndGameConditions()
    {
        Debug.Log("END GAME:");
        
        // TODO If enough knowledge, good ending
        // TODO If not, bad ending
    }

}
