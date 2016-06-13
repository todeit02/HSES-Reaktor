using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager {

    private const byte maxPlayersCount = 4;

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

    public List<string> Subgames
    {
        get
        {
            return subgames;
        }
        set
        {
            subgames = value;
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
    
    public void StartGame()
    {
        Debug.Log("Starting a new game with subgames:");
        foreach (string usedPrefab in subgames)
        {
            Debug.Log(usedPrefab);
        }
    }

    public void OnPlayerReaction(Player reactor)
    {
        // TO DO: Ask subgame if reaction was expected.

        bool wasReactionCorrect = false; // Dummy value

        reactor.TakeReactionResult(wasReactionCorrect);
    }
}