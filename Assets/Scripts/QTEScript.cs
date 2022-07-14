using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QTEScript : MonoBehaviour
{
    
    [HideInInspector]
    public bool success { get; private set; }
    
    public TextMeshProUGUI keyText;
    public SpriteRenderer largeCircle;
    public SpriteMask largeCircleMask;
    public SpriteRenderer smallCircle;
    public SpriteMask smallCircleMask;
    
    private KeyCode key;
    private float neutralTime = 1.2f;
    private float reactionTime = .8f;
    private float elapsedTime;
    private bool ready;
    
    private float minScale;
    private float maxScale;
    
    public void Init()
    {
        key = KeyCode.A;

        SetupGraphics();
        
        success = false;
        elapsedTime = 0f;
        ready = true;
    }

    private void SetupGraphics()
    {
        keyText.text = key.ToString();
        
        minScale = smallCircleMask.transform.localScale.x;
        maxScale = largeCircle.transform.localScale.x;

        float t = reactionTime / (neutralTime + reactionTime);
        float s = Mathf.Lerp(minScale, maxScale, t);
        smallCircle.transform.localScale = new Vector3(s, s, 1f);
    }
    
    private void Update()
    {
        if (!ready)
            return;

        elapsedTime += Time.deltaTime;
        
        // Visual update
        float t = elapsedTime / (neutralTime + reactionTime);
        float s = Mathf.Lerp(maxScale, minScale, t);
        largeCircle.transform.localScale = new Vector3(s, s, 1f);
        largeCircleMask.transform.localScale = new Vector3(s-5f, s-5f, 1f);
        
        // Conditions checks
        if (Input.GetKeyDown(key))
            CheckForSuccess();
        else if (elapsedTime > neutralTime + reactionTime)
            Fail();
    }

    private void OnMouseDown()
    {
        CheckForSuccess();
    }

    private void CheckForSuccess()
    {
        if (elapsedTime > neutralTime && elapsedTime <= neutralTime + reactionTime)
            Success();
        else
            Fail();
    }

    private void Success()
    {
        success = true;
        QTEManager.instance.ResolveQTE(this);
        Destroy(gameObject);
    }

    private void Fail()
    {
        success = false;
        QTEManager.instance.ResolveQTE(this);
        Destroy(gameObject);
    }
    
}
