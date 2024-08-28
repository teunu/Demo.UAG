using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class SC_Boot : MonoBehaviour
{
    VideoPlayer video;

    private async void Awake()
    {
        Application.targetFrameRate = 30;
        video = GetComponent<VideoPlayer>();

        video.Pause();
        await Globals.GetGlobals();
        video.Play();

        video.loopPointReached += VideoDone;
    }

    void VideoDone(VideoPlayer vp)
    {
        GameSceneHandler.SwitchGameScene("main");
    }

}
