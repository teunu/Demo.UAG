using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    public static int health;

    public static void GameOverListener()
    {
        if (health == 0)
            SC_GameOver.Open();
    }
}
