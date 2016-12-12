using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    protected ScoreManager() { }

    public int Score = 0;
    public bool RelativeMovement = true;
    public void Reset()
    {
        Score = 0;
    }
}
