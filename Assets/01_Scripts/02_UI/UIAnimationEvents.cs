using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Create standardize animations for UI Elements using Canvas Renderer
/// </summary>
public class UIAnimationEvents : MonoBehaviour
{
    private CanvasRenderer _image; 
    [SerializeField] private float FadeTargetValue;
    [SerializeField] private float animationTime;


    private void Awake()
    {
        if (_image == null) _image = GetComponent<CanvasRenderer>();
        TurnOffObject();
        _image.SetAlpha(0);
    }

    public void TurnOffObject()
    {
        gameObject.SetActive(false);
    }

    public void TurnOnObject()
    {
        gameObject.SetActive(true);
    }

    public void FadeIn()
    {
        TurnOnObject();
        StartCoroutine(Fade(FadeTargetValue, animationTime));
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(0, animationTime));
        }

    IEnumerator Fade(float FadeTarget, float duration)
    {
        float t = 0;
        
        float tempFloat = _image.GetAlpha();
        while (t < duration)
        {
            _image.SetAlpha(Mathf.Lerp(tempFloat, FadeTarget, t / duration));

            t += Time.deltaTime;
            yield return null;
        }

        _image.SetAlpha(FadeTarget);
        if(FadeTarget==0)TurnOffObject();

    }
}
