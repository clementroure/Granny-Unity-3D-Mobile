using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;
using FantomLib.Example;

//Music player example (Mainly callback handlers)
public class MusicPlayerTest : MonoBehaviour {

    // Use this for initialization
    private void Start () {
            
    }
        
    // Update is called once per frame
    //private void Update () {
            
    //}


    public void ReceiveSongChanged(EasyMusicPlayer.PlayItem item)
    {
        XDebug.Log("Song changed : " + item.title + " / " + item.artist);
    }

    public void ReceivePlaybackModeChanged(EasyMusicPlayer.Mode mode)
    {
        XDebug.Log("PlaybackMode changed : " + mode);
    }

    public void ReceiveAddSong(string path)
    {
        if (!string.IsNullOrEmpty(path))
            XDebug.Log("Add song : path = " + path);
        else
            XDebug.Log("Add song : path is empty.");
    }

    public void ReceiveAddSongInfo(AudioInfo info)
    {
        XDebug.Log("Add song info : ");
        XDebug.Log(info);
    }

}
