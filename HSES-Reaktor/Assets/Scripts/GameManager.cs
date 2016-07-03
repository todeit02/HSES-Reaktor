using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public static class GameManager
{

    public static List<string> subgames;

    private const int maxPlayersCount = 4;
    private static int playersCount = 4;
    private static Subgame runningSubgame;
    private static LinkedList<Player> participants;
    private static LinkedList<PlayerStats> ranking;
    private static int subgameIndex = 0;

    [Flags]
    public enum ResetOption
    {
        subgameList = 0x01,
        subgameIndex = 0x02,
        runningSubgame = 0x04,
        playerList = 0x08,
        playerUIs = 0x10,
        playerCount = 0x20,
        playerScores = 0x40,
        ranking = 0x80
    };

    public static LinkedList<PlayerStats> Ranking
    {
        get
        {
            return ranking;
        }
    }

    public static Subgame RunningSubgame
    {
        set
        {
            runningSubgame = value;
        }
    }

    public static string NextSubgameName
    {
        get
        {
            subgameIndex++;
            if((subgameIndex - 1) >= subgames.Count)
            {
                return null;
            }
            return subgames[subgameIndex - 1];
        }
    }

    // PlayersCount property.
    public static int PlayersCount
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
        // No subgames in list. Don't do anything.
        if ((GameManager.subgames == null) || (GameManager.subgames.Count == 0))
        {
            Debug.Log("Subgame list is empty!");
            return;
        }

        Debug.Log("Starting a new game with subgames:");
        foreach (string usedPrefab in subgames)
        {
            Debug.Log(usedPrefab);
        }

        SceneManager.LoadScene(SubgameManager.sceneName);
    }

    public static void RegisterPlayer(Player registeringPlayer)
    {
        if ((participants != null) && !(participants.Count < maxPlayersCount))
        {
            throw new System.Exception("Maximum registerable player count reached.");
        }

        if (participants == null)
        {
            participants = new LinkedList<Player>();
        }
        participants.AddLast(registeringPlayer);
    }

    public static void UnregisterPlayer(Player unregisteringPlayer)
    {
        if (participants != null)
        {
            participants.Remove(unregisteringPlayer);
        }
    }

    public static void OnPlayerReaction(Player reactor)
    {
        bool wasReactionCorrect = runningSubgame.ExpectsReaction;

        reactor.TakeReactionResult(wasReactionCorrect);
    }

    public static void Reset(ResetOption resetSelection)
    {
        if((resetSelection & ResetOption.subgameList) != 0)
        {
            if (subgames != null)
            {
                subgames.Clear();
            }
            // TO DO: Select ALL instead of none!
        }
        if ((resetSelection & ResetOption.subgameIndex) != 0)
        {
            subgameIndex = 0;
        }
        if ((resetSelection & ResetOption.runningSubgame) != 0)
        {
            runningSubgame = null;
        }
        if ((resetSelection & ResetOption.playerList) != 0)
        {
            participants.Clear();
        }
        if ((resetSelection & ResetOption.playerUIs) != 0)
        {
            if (participants != null)
            {
                foreach (Player resettingPlayer in participants)
                {
                    resettingPlayer.ResetUI();
                }
            }
        }
        if ((resetSelection & ResetOption.playerCount) != 0)
        {
            PlayersCount = maxPlayersCount;
        }
        if ((resetSelection & ResetOption.playerScores) != 0)
        {
            if (participants != null)
            {
                foreach (Player resettingPlayer in participants)
                {
                    resettingPlayer.ResetScore();
                }
            }
        }
        if ((resetSelection & ResetOption.ranking) != 0)
        {
            if (ranking != null)
            {
                ranking.Clear();
            }
        }
    }

    public static void ComputeRanking()
    {
        // TO DO: Find strange bug causing overlong ranking list.
        if (ranking == null)
        {
            ranking = new LinkedList<PlayerStats>();
        }

        foreach (Player rankingPlayer in participants)
        {
            PlayerStats rankingPlayerStats = new PlayerStats(rankingPlayer.Name, rankingPlayer.Score);

            if (ranking.Count != 0)
            {
                bool worseScoreFound = false;

                // Go through the list and add before the player with the poorest score.
                for (LinkedListNode<PlayerStats> rankedPlayerStatsNode = ranking.First; rankedPlayerStatsNode != null; rankedPlayerStatsNode = rankedPlayerStatsNode.Next)
                {
                    if (rankingPlayerStats.score > rankedPlayerStatsNode.Value.score)
                    {
                        ranking.AddBefore(rankedPlayerStatsNode, rankingPlayerStats);
                        worseScoreFound = true;
                    }
                }

                // No poorer score found, so append at the end.
                if (!worseScoreFound)
                {
                    ranking.AddLast(rankingPlayerStats);
                }
            }
            else
            {
                // List is still empty, so just add.
                ranking.AddLast(rankingPlayerStats);
            }
        }
    }
}