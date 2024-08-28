using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_Pause : MonoBehaviour
{
    public static SC_Pause instance;

    private void Awake()
    {
        instance = this;
    }

    public static async Task Open()
    {
        if (instance != null) { return; }

        SceneManager.LoadSceneAsync("pause", LoadSceneMode.Additive);

        while (instance == null)
        {
            await Task.Yield();
        }
    }

    public void Continue()
    {
        SceneManager.UnloadSceneAsync("pause");
        instance = null;

        PlayerInput.CursorInquiry();
    }

    public async void GoMainMenu()
    {
        await Globals.SetListener(null);
        GameSceneHandler.SwitchGameScene("main");
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
