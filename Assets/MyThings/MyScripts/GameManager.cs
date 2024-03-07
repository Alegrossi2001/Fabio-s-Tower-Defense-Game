using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameManager
{
    private static GameManager instance = null;
    private bool isGameOver = false;

    private GameManager() { }

    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }
    }

    public bool CheckForGameOver(Transform hq)
    {
        if(hq == null)
        {
            isGameOver = true;
        }
        Debug.Log("GAME IS OVER!");
        return isGameOver;
    }
}
