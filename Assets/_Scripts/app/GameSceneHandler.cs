using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneHandler : MonoBehaviour
{
    static Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    const int minimal_loading_time = 1000;

    public static async Task SwitchGameScene(string scene)
    {
        #region Start Loading Screen
        animator.SetTrigger("in");                                          
        await Task.Delay(200);          //Grace for 1/5th of a second as the load fade plays
        Time.timeScale = 0;             //Pause any time-related things while loading
        #endregion

        Stopwatch timer = new Stopwatch(); timer.Start(); //Keep track of elapsed time during loading.
        AsyncOperation loading = SceneManager.LoadSceneAsync(scene);

        //Wait until the scene is properly done loading
        while (loading.isDone) { await Task.Yield(); }

        //If the scene loads particularly fast, make sure that we at least make sure all animations can happen.
        while (timer.ElapsedMilliseconds < minimal_loading_time) { await Task.Yield();  } 

        #region End Loading
        Time.timeScale = 1;             //Unpause before removing the fade
        await Task.Delay(200);          //Grace for 1/5th of a second for things to settle now time's playing again
        animator.SetTrigger("out");     //Start the load screen unload animation 
        #endregion
    }
}
