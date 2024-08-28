using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SC_Main : MonoBehaviour
{
    //TODO: Move the opening of GLOBALS to an even earlier stage.
    private async void Awake()
    {
        //When returning from further into the game's flowchart, reset important stuff.
        //SceneSerializationHandler.instance = null;
        //SaveManager.current = null;


        await Globals.GetGlobals();
        clean = true;

        //ClearSave.interactable = SaveManager.exists;
    }


    public void GoWebsite()
    {
        Application.OpenURL(Globals.website);
    }

    public void GoWebsite(string url)
    {
        Application.OpenURL(url);
    }

    bool clean;

    public void RequestPlay(string pointer)
    {
        PlayerData.health = 5;

        if (clean) //Want to make sure Globals are fully load
            SceneManager.LoadSceneAsync(pointer);
    }

    public void RequestQuit()
    {
        Application.Quit();
    }
}
