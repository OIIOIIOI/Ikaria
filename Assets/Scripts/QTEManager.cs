using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class QTEManager : MonoBehaviour
{
    
    public static QTEManager instance;

    public RectTransform targetCanvas;
    public GameObject QTEUIPrefab;
    
    private List<IEnumerator> activeCoroutines;
    
    private void OnEnable()
    {
        GameLoopManager.StartFallEvent += StartFallHandler;
        GameLoopManager.StartStasisEvent += DeactivateAll;
    }
    
    private void OnDisable()
    {
        GameLoopManager.StartFallEvent -= StartFallHandler;
        GameLoopManager.StartStasisEvent -= DeactivateAll;
    }
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        activeCoroutines = new List<IEnumerator>();
    }

    private void DeactivateAll ()
    {
        foreach (IEnumerator coroutine in activeCoroutines)
            StopCoroutine(coroutine);
        
        activeCoroutines.Clear();
    }

    private void StartFallHandler()
    {
        float p = GameLoopManager.instance.GetCyclesProgress();
        Debug.Log(p);
        Activate(3f);
    }

    private void Activate(float frequency)
    {
        IEnumerator coroutine = TriggerQTE(frequency);
        activeCoroutines.Add(coroutine);
        StartCoroutine(coroutine);
    }

    private IEnumerator TriggerQTE(float frequency)
    {
        while (true)
        {
            // Debug.Log("Trigger QTE");

            GameObject go = Instantiate(QTEUIPrefab, targetCanvas);
            go.GetComponent<QTEScript>().Init();
            
            // TODO delete objects when phase changes
            
            yield return new WaitForSeconds(frequency);
        }
    }

    public void ResolveQTE(QTEScript qte)
    {
        Debug.Log("QTE is over: " + (qte.success ? "SUCCESS!" : "FAIL!"));
    }

}
