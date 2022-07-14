using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameLoopManager : MonoBehaviour
{
    
    private enum LoopState { Pause, Fall, Stasis };

    private LoopState state = LoopState.Pause;
    private int currentCycle = 0;

    private AudioSource audioSource;
    private float audioLoopDuration;
    private int fallDurationInAudioLoops = 1;
    private int stasisDurationInAudioLoops = 2;
    private int audioLoopsLeft;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        state = LoopState.Pause;
        
        StartFall();
    }

    private void StartLoop()
    {
        Debug.Log("Starting audio loop");
        
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
        Debug.Log("Audio loop complete!");
        
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
        Debug.Log("PHASE COMPLETE!");

        switch (state)
        {
            case LoopState.Fall:
                EndFall();
                break;
            
            case LoopState.Stasis:
                EndStasis();
                break;
        }
    }

    private void StartFall()
    {
        Debug.Log("FALLING!");

        state = LoopState.Fall;
        
        audioLoopsLeft = fallDurationInAudioLoops;
        StartLoop();
    }

    private void EndFall()
    {
        Debug.Log("STABILIZING...");

        state = LoopState.Pause;
        
        StartStasis();
    }

    private void StartStasis()
    {
        Debug.Log("STASIS ACTIVE!");

        state = LoopState.Stasis;

        audioLoopsLeft = stasisDurationInAudioLoops;
        StartLoop();
    }

    private void EndStasis()
    {
        Debug.Log("STASIS ENDING...");
        
        state = LoopState.Pause;

        currentCycle++;
        
        if (currentCycle < 2)
            StartFall();
        else
            CheckEndGameConditions();
    }

    private void CheckEndGameConditions()
    {
        Debug.Log("END GAME:");
        
        // TODO If enough knowledge, good ending
        // TODO If not, bad ending
    }

    private void Update()
    {
        
    }

}
