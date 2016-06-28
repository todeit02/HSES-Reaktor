using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public static class GameManager {

    public static Queue<string> subgames;

    private const byte maxPlayersCount = 4;
    private static byte playersCount = 4;
    private static Subgame runningSubgame;
    private static List<Player> participants;

    public static Subgame RunningSubgame
    {
        set
        {
            runningSubgame = value;
        }
    }

    // PlayersCount property.
    public static byte PlayersCount
    {
        set
        {
            if ((maxPlayersCount < value) || (1 > value))
            {
                playersCount = maxPlayersCount;
                throw new System.Exception("Tried to set invalid player count.");
            }
            else
            {
                playersCount = value;
            }
        }
        get
        {
            return playersCount;
        }
    }
    
    public static void StartGame()
    {
        Debug.Log("Starting a new game with subgames:");
        foreach (string usedPrefab in subgames.ToArray())
        {
            Debug.Log(usedPrefab);
        }
        SceneManager.LoadScene("Subgame");
    }

    public static void RegisterPlayer(Player registeringPlayer)
    {
        if((participants != null) && !(participants.Count < maxPlayersCount))
        {
            throw new System.Exception("Maximum registerable player count reached.");
        }

        if (participants == null)
        {
            participants = new List<Player>();
        }
        participants.Add(registeringPlayer);
    }

    public static void OnPlayerReaction(Player reactor)
    {
        // TO DO: Ask subgame if reaction was expected.

        bool wasReactionCorrect = runningSubgame.ExpectsReaction();

        reactor.TakeReactionResult(wasReactionCorrect);
    }

    public static void ResetPlayerUIs()
    {
        foreach(Player resettingPlayer in participants)
        {
            resettingPlayer.ResetUI();
        }
    }
}