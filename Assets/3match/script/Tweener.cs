using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tweener : MonoBehaviour
{
    enum UIAnimationTypes
    {
        Move,
        Scale,
        Fade,
        Rotate
    }

    [SerializeField] GameObject objectToAnimate = null;

    [Space()]
    [SerializeField] UIAnimationTypes animationType;
    [SerializeField] LeanTweenType easeType;
    [SerializeField] AnimationCurve easeCurve;
    [SerializeField] float duration;
    [SerializeField] float delay;

    [Space()]
    [SerializeField] bool loop;
    [SerializeField] bool pingpong;
    [SerializeField] int pingpongRepetitions;

    [Space()]
    [SerializeField] bool startPositionOffset;
    [SerializeField] Vector3 from;
    [SerializeField] Vector3 to;
    [Space()]
    [SerializeField] bool startRotationOffset;
    [SerializeField] Quaternion rotationFrom;
    [SerializeField] Vector3 rotationTo;

    [Space()]
    [SerializeField] LTDescr _tweenObject;

    [Space()]
    [SerializeField] bool showOnEnable;
    [SerializeField] bool showOnDisable;
    [Space()]
    [SerializeField] bool ignoreTimescale;

    [SerializeField] bool resetScaleWhenAnimationComplete;


    [Space()]
    [SerializeField] UnityEvent OnStart;
    [SerializeField] UnityEvent OnComplete;

    bool isPlaying;

    private void OnEnable()
    {
        if (showOnEnable)
            HandleTween();
    }

    private void OnDisable()
    {
        if (showOnDisable)
            HandleTween();
    }

    

    void OnStartEvent()
    {
        OnStart.Invoke();
    }


    public void ForceTweenRestart()
    {
        isPlaying = false;
        if (_tweenObject != null)
        {
            //Debug.Log("_tweenObject.id " + _tweenObject.id);
            //if (_tweenObject.id > 0)
                LeanTween.cancel(this.gameObject);
        }
        HandleTween();
    }

    public void HandleTween()
    {

        if (isPlaying)
            return;

        _tweenObject = new LTDescr();

        isPlaying = true;
        CancelInvoke();

        if (objectToAnimate == null)
            objectToAnimate = gameObject;


        switch (animationType)
        {
            case UIAnimationTypes.Fade:
                Fade();
                break;
            case UIAnimationTypes.Move:
                MoveAbsolute();
                break;
            case UIAnimationTypes.Scale:
                Scale();
                break;
            case UIAnimationTypes.Rotate:
                Rotate();
                break;
        }
        
        if (!ignoreTimescale)
        {
            Invoke("EndAnimation", duration);

            if (OnStart != null)
                Invoke("OnStartEvent", delay);
        }
        else
            _tweenObject.setOnComplete(EndAnimation);
  

        _tweenObject.setDelay(delay);
        _tweenObject.setIgnoreTimeScale(ignoreTimescale);

        if (easeType == LeanTweenType.animationCurve)
            _tweenObject.setEase(easeCurve);
        else
            _tweenObject.setEase(easeType);

        if (loop)
            _tweenObject.loopCount = int.MaxValue;

        if (pingpong)
            _tweenObject.setLoopPingPong(pingpongRepetitions);


        //Invoke("EndAnimation", delay + duration);
    }
    
    void EndAnimation()
    {
       
        OnComplete.Invoke();

        isPlaying = false;

        if (resetScaleWhenAnimationComplete)
        {
            if (targetTransform != null)
                targetTransform.localScale = Vector3.one;
            CancelInvoke();
        }


    }


    void Fade()
    {
        if (objectToAnimate.GetComponent<CanvasGroup>() == null)
            objectToAnimate.AddComponent<CanvasGroup>();

        if (startPositionOffset)
            objectToAnimate.GetComponent<CanvasGroup>().alpha = from.x;

        _tweenObject = LeanTween.alphaCanvas(objectToAnimate.GetComponent<CanvasGroup>(), to.x, duration);
    }

    public void ResetPosition()
    {
            objectToAnimate.GetComponent<Transform>().position = from;

    }

    void MoveAbsolute()
    {
        if (startPositionOffset)
            ResetPosition();

        _tweenObject = LeanTween.move(objectToAnimate, to, duration);

    }

    Transform targetTransform;
    void Scale()
    {
        if (startPositionOffset)
        {
            if (targetTransform == null)
            {
                targetTransform = objectToAnimate.GetComponent<Transform>();
            }


            targetTransform.localScale = from;
        }

        _tweenObject = LeanTween.scale(objectToAnimate, to, duration);
    }

    void Rotate()
    {
        if (startRotationOffset)
            objectToAnimate.GetComponent<Transform>().localRotation = rotationFrom;

        _tweenObject = LeanTween.rotateLocal(objectToAnimate, rotationTo, duration);
    }

    public void ActivateMe()
    {
        HandleTween();
    }



    public void DeactivateMe()
    {
  

        if (_tweenObject != null)
            _tweenObject.reset();

        switch (animationType)
        {
            case UIAnimationTypes.Fade:
                objectToAnimate.GetComponent<CanvasGroup>().alpha = from.x;
                break;
            case UIAnimationTypes.Move:
                objectToAnimate.GetComponent<RectTransform>().anchoredPosition = from;
                break;
            case UIAnimationTypes.Scale:
                objectToAnimate.GetComponent<RectTransform>().localScale = from;
                break;
            case UIAnimationTypes.Rotate:
                objectToAnimate.GetComponent<RectTransform>().localRotation = rotationFrom;
                break;
        }

    }
}
