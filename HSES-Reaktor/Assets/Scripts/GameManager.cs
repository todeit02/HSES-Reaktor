using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager {

    private const byte maxPlayersCount = 4;
    private const string subgameScenePrefix = "Subgame";

    private static GameManager instance = null;

    private List<string> subgames;
    private byte playersCount = 4;

    // GameManager singleton access property.
    public static GameManager Instance
    {
        get
        {
            if (null == instance)
            {
                instance = new GameManager();
            }
            return instance;
        }
    }

    // PlayersCount property.
    public byte PlayersCount
    {
        set
        {
            if ((maxPlayersCount < value) || (1 > value))
            {
                throw new System.Exception("Invalid player count is being set.");
            }
            else
            {
                playersCount = value;
            }
        }
    }

    /*// Use this for initialization
    void Start ()
    {
        // Always use the game created after application start.
        // SetInstance(this);

        if (FindObjectsOfType(typeof(GameManager)).Length != 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            // Pass the Game object to configuration menu or subgame.
            DontDestroyOnLoad(this.gameObject);
        }
    }*/

    public void SetUsedSubgames(List<string> chosenSubgames)
    {
        Debug.Log("Changing used subgames.");

        // TO DO
    }

    public void StartGame()
    {
        Debug.Log("Starting a new game.");
        // Load scene of first subgame.

    }

    // Create list of all the project's subgames' names.
    public static List<string> GetExistingSubgames()
    {
        List<string> SubgameList = new List<string>();
        foreach (UnityEditor.EditorBuildSettingsScene checkingScene in UnityEditor.EditorBuildSettings.scenes)
        {
            if (checkingScene.enabled)
            {                
                string sceneName = checkingScene.path.Substring(checkingScene.path.LastIndexOf('/') + 1);
                sceneName = sceneName.Substring(0, sceneName.Length - 6);

                if (0 == sceneName.IndexOf("Subgame"))
                {
                    string subgameName = sceneName.Substring(subgameScenePrefix.Length, sceneName.Length - subgameScenePrefix.Length); ;
                    SubgameList.Add(subgameName);
                }
            }
        }
        return SubgameList;
    }
}