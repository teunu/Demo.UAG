using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HookAudioListener : MonoBehaviour
{
    private async void Awake()
    {
        await Task.Delay(200);
        await Globals.SetListener(transform);
    }

    private async void OnDestroy()
    {
        await Globals.SetListener(null);
    }
}
