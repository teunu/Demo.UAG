using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.XR.GoogleVr;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Globals : MonoBehaviour
{
    public static Globals instance;
    public static bool initialized = false;

    static EventSystem event_system;
    static AudioListener audio_listener;

    public const string website = "https://teunu.com";

    private void Awake()
    {
        instance = this;    
        initialized = true;

        DontDestroyOnLoad(gameObject);

        event_system = EventSystem.current;
        audio_listener = GetComponentInChildren<AudioListener>();

        Application.targetFrameRate = 30;

        PlayerData.health = 5;
    }

    private void OnDestroy()
    {
        Destroy(audio_listener.gameObject);
        initialized = false;
    }

    public static async Task GetGlobals()
    {
        if (initialized == false)
        {
            SceneManager.LoadSceneAsync("globals", LoadSceneMode.Additive);

            while (initialized == false)
            {
                await Task.Yield();
            }
        }
    }

    public static async Task SetListener(Transform parent) {
        if (parent == null && instance != null) { audio_listener.transform.parent = instance.transform; }
        //if (instance == null) { await GetGlobals(); }
        
        audio_listener.transform.parent = parent;
        audio_listener.transform.localPosition = Vector3.zero;

        await Task.Yield();
    }

}
