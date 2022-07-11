using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FantomLib;

// Display Hardware Volume on the UI
public class VolumeUI : MonoBehaviour {

    public Text displayText;
    public HardVolumeController hardVolumeController;


    // Use this for initialization
    private void Start () {
        if (hardVolumeController == null)
            hardVolumeController = FindObjectOfType<HardVolumeController>();

        if (hardVolumeController != null)
            OnDisplay(hardVolumeController.volume);
    }

    // Update is called once per frame
    private void Update () {
        
    }

    public void OnDisplay(int volume)
    {
        if (displayText != null && hardVolumeController != null)
            displayText.text = "Hard Volume : " + volume + " / " + hardVolumeController.maxVolume;
    }

}
