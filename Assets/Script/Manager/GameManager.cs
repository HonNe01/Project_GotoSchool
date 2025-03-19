using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [Header(" # Game Infos")]
    public int playerId;
    public int curMap;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Application.targetFrameRate = 60;
    }

    public void GameEnd()
    {
        Application.Quit();
    }
}
