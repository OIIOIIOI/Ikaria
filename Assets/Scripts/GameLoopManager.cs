﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameLoopManager : MonoBehaviour
{

    public Text debug;
    public Image mainProgressBar;
    public Image secondaryProgressBar;
    
    private enum LoopState { Pause, Fall, Repair, Resolve, Prepare };

    private LoopState state = LoopState.Pause;
    private int currentCycle = 0;
    private int cyclesBeforeGameOver = 3;

    private AudioSource audioSource;
    private float audioLoopDuration;
    private int audioLoopsLeft;
    
    [HideInInspector]
    private Dictionary<LoopState, int> statesDurations;

    private bool tmpNeedsRepair;
    private bool tmpNeedsResolve;
    
    private void Awake()
    {
        tmpNeedsRepair = Random.value > 0.5f;
        tmpNeedsResolve = Random.value > 0.5f;
        
        audioSource = GetComponent<AudioSource>();

        statesDurations = new Dictionary<LoopState, int>(){
            {LoopState.Fall, 1},
            {LoopState.Repair, 1},
            {LoopState.Resolve, 1},
            {LoopState.Prepare, 1},
        };
    }

    private void Start()
    {
        state = LoopState.Pause;

        // Wait for a bit or else the first loop will be out of sync
        Invoke(nameof(StartFall), 3.0f);
    }

    private void Update()
    {
        debug.text = "Phase: "+state+"\r\nLoops left: "+audioLoopsLeft+"\r\nLoop duration: "+audioLoopDuration+"\r\nAudio time: "+audioSource.time;
        
        float progress = GetCurrentPhaseProgress();
        mainProgressBar.rectTransform.localScale = new Vector3(progress, 1f, 1f);
        progress = GetCurrentStateProgress();
        secondaryProgressBar.rectTransform.localScale = new Vector3(progress, 1f, 1f);
    }
    
    /* LOOP ============================================ */

    private void StartLoop()
    {
        // Debug.Log("Starting audio loop! Left:" + audioLoopsLeft);
        
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
        // Debug.Log(Time.frameCount + " - Audio loop complete! Left:" + (audioLoopsLeft - 1));
        
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

    private float GetCurrentPhaseLength(bool inSeconds = true)
    {
        int d = 0;
        
        if (state == LoopState.Fall)
            d = statesDurations[LoopState.Fall];
        else
        {
            if (NeedsRepair())
                d += statesDurations[LoopState.Repair];
            if (NeedsResolve())
                d += statesDurations[LoopState.Resolve];
            if (currentCycle < cyclesBeforeGameOver)
                d += statesDurations[LoopState.Prepare];
        }

        return inSeconds ? d * audioLoopDuration : d;
    }

    private float GetCurrentPhaseProgress()
    {
        float total = GetCurrentPhaseLength();
        float secondsLeftInPhase = 0;
        float secondsLeftInState = 0;
        int loopsLeftAfterState = 0;

        switch (state)
        {
            case LoopState.Fall:
                secondsLeftInState = (audioLoopDuration * audioLoopsLeft) - audioSource.time;
                return (total - secondsLeftInState) / total;
            
            case LoopState.Repair:
                secondsLeftInState = (audioLoopDuration * audioLoopsLeft) - audioSource.time;
                if (NeedsResolve())
                    loopsLeftAfterState += statesDurations[LoopState.Resolve];
                if (currentCycle < cyclesBeforeGameOver)
                    loopsLeftAfterState += statesDurations[LoopState.Prepare];
                break;
            
            case LoopState.Resolve:
                secondsLeftInState = (audioLoopDuration * audioLoopsLeft) - audioSource.time;
                if (currentCycle < cyclesBeforeGameOver)
                    loopsLeftAfterState += statesDurations[LoopState.Prepare];
                break;
            
            case LoopState.Prepare:
                secondsLeftInState = (audioLoopDuration * audioLoopsLeft) - audioSource.time;
                break;
            
            default:
                return 0;
        }

        debug.text += "\r\nSeconds left in state: " + secondsLeftInState + "\r\nLoops left after: " + loopsLeftAfterState;
        
        secondsLeftInPhase = secondsLeftInState + loopsLeftAfterState * audioLoopDuration;
        return 1f - ((total - secondsLeftInPhase) / total);
    }

    private float GetCurrentStateProgress()
    {
        if (state == LoopState.Pause)
            return 0;
        
        float total = statesDurations[state] * audioLoopDuration;
        float secondsLeftInState = (audioLoopDuration * audioLoopsLeft) - audioSource.time;
        float p = (total - secondsLeftInState) / total;
        
        return state == LoopState.Fall ? p : 1f - p;
    }
    
    /* FALL ============================================ */

    private void StartFall()
    {
        tmpNeedsRepair = Random.value > 0.5f;
        tmpNeedsResolve = Random.value > 0.5f;
        
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
        // return true;
        return tmpNeedsRepair;
    }
    
    private bool NeedsResolve()
    {
        // TODO Get actual info from somewhere
        // return true;
        return tmpNeedsResolve;
    }

    private void CheckEndGameConditions()
    {
        Debug.Log("END GAME:");
        
        // TODO If enough knowledge, good ending
        // TODO If not, bad ending
    }

}
