using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameCompletionManager : MonoBehaviour {

    public const string sceneName = "GameCompletion";

    public Text congratulationText;
    public Text rankingText;

    private readonly string[] congratulationPhraseSegments = { "Herzlichen Glückwunsch,\n", "!" };
    private const string nameScoreSeparator = ": ";

    /***********************************************************/
    /********************** Unity Methods **********************/
    /***********************************************************/

    public void Awake ()
    {
        LinkedList<PlayerStats> visualizingRanking = GameManager.Ranking;

        SetCongratulationText(visualizingRanking);
        SetRankingText(visualizingRanking);
    }

    public void OnDestroy()
    {
        GameManager.Reset(GameManager.ResetOption.ranking);
    }

    /***********************************************************/
    /*********************** User Methods **********************/
    /***********************************************************/

    public void HandleMenuMainClick()
    {
        GameManager.Reset(
            GameManager.ResetOption.playerCount |
            GameManager.ResetOption.playerList |
            GameManager.ResetOption.playerScores |
            GameManager.ResetOption.subgameList
            );

        SceneManager.LoadScene(MenuMainManager.sceneName);
    }

    public void HandleRestartGameClick()
    {
        GameManager.StartGame();
    }

    protected override void 

    private void SetCongratulationText(LinkedList<PlayerStats> sourceRanking)
    {
        if (sourceRanking == null)
        {
            return;
        }

        PlayerStats winningPlayerStats = sourceRanking.First.Value;

        string newCongratulation = string.Concat(congratulationPhraseSegments[0], winningPlayerStats.name, congratulationPhraseSegments[1]);
        congratulationText.text = newCongratulation;
    }

    private void SetRankingText(LinkedList<PlayerStats> sourceRanking)
    {
        if (sourceRanking == null)
        {
            return;
        }

        string newRankingText = "";

        foreach(PlayerStats visualizingPlayerStats in sourceRanking)
        {
            newRankingText = string.Concat(newRankingText, visualizingPlayerStats.name);
            newRankingText = string.Concat(newRankingText, nameScoreSeparator);
            newRankingText = string.Concat(newRankingText, visualizingPlayerStats.score);
            newRankingText = string.Concat(newRankingText, '\n');
        }

        rankingText.text = newRankingText;
    }
}