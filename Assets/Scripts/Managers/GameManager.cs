using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Action<bool> onPaused;
    public bool isPaused = false;

    public void TogglePause()
    {
        isPaused = !isPaused;
        onPaused?.Invoke(isPaused);
    }
}
