using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_Finished : MonoBehaviour
{
    public static SC_Finished instance;

    private void Awake()
    {
        instance = this;
    }

    public static async Task Open()
    {
        if (instance != null) { return; }

        SceneManager.LoadSceneAsync("finished", LoadSceneMode.Additive);

        while (instance == null)
        {
            await Task.Yield();
        }
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
