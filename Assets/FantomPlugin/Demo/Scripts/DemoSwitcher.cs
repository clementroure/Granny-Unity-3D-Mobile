using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FantomLib;

// Switch pinch and drag functions Demo
public class DemoSwitcher : MonoBehaviour
{

    public Toggle scaleToggle;
    public PinchToScale pinchToScale;
    public SmoothFollow3 smoothFollow;
    public Toggle dragToggle;


    // Use this for initialization
    private void Start()
    {
        if (dragToggle != null)
            OnDraggable(dragToggle.isOn);
    }

    // Update is called once per frame
    //private void Update () {

    //}

    //(*) use PinchInput callback
    //http://fantom1x.blog130.fc2.com/blog-entry-288.html
    //width: distance of two fingers of pinch
    //center: The coordinates of the center of two fingers of pinch
    public void OnPinchStart(float width, Vector2 center)
    {
        if (scaleToggle != null && scaleToggle.isOn && pinchToScale != null)
            pinchToScale.OnPinchStart(width, center);
    }

    //(*) use PinchInput callback
    //http://fantom1x.blog130.fc2.com/blog-entry-288.html
    //width: distance of two fingers of pinch
    //delta: The difference in pinch width just before
    //ratio: Stretch ratio from the start of pinch width (1:At the start of pinch, Expand by 1 or more, lower than 1 (1/2, 1/3, ...)
    public void OnPinch(float width, float delta, float ratio)
    {
        if (scaleToggle != null && scaleToggle.isOn && pinchToScale != null)
            pinchToScale.OnPinch(width, delta, ratio);
        else if (smoothFollow != null)
            smoothFollow.OnPinch(width, delta, ratio);
    }

    public void OnReset()
    {
        if (smoothFollow != null)
            smoothFollow.ResetOperations();

        if (pinchToScale != null)
            pinchToScale.ResetScale();
    }

    public void OnDraggable(bool enable)
    {
        if (smoothFollow != null)
        {
            smoothFollow.angleOperation.dragEnable = enable;
            smoothFollow.heightOperation.dragEnable = enable;
        }
    }
}

