using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Button buzzerButton;
    public Text playerNameText;
    public Text notificationText;
    public Text scoreText;

    public const int maxNameLength = 20;

    private const int standardPoints = 1;
    private readonly string[] successNotifications =
    {
        "Gut so!",
        "Prima!",
        "Juhu!"
    };
    private readonly string[] failureNotifications =
    {
        "Das war nix.",
        "Das geht besser.",
        "Nope."
    };
    private enum BuzzerState
    {
        idle,
        showingSuccess,
        showingFailure
    }

    private int score = 0;

    public string Name
    {
        get
        {
            return playerNameText.text;
        }
    }

    public int Score
    {
        get
        {
            return score;
        }
    }

    public void Awake()
    {
        GameManager.RegisterPlayer(this);
    }

    public void OnDestroy()
    {
        GameManager.UnregisterPlayer(this);
    }

    public void HandleBuzzerButtonClick()
    {
        GameManager.OnPlayerReaction(this);
    }

    public void TakeReactionResult(bool wasCorrect)
    {
        System.Random random = new System.Random();
        string[] properNotifications;
        int signedScoreDelta;
        BuzzerState resultingState;

        if (wasCorrect)
        {
            properNotifications = successNotifications;
            signedScoreDelta = standardPoints;
            resultingState = BuzzerState.showingSuccess;
        }
        else
        {
            properNotifications = failureNotifications;
            signedScoreDelta = -standardPoints;
            resultingState = BuzzerState.showingFailure;
        }

        ChangeScore(signedScoreDelta);
        UpdateScoreView();
        ChangeBuzzerAppearance(resultingState);

        int notificationIndex = random.Next(0, properNotifications.Length);
        ShowNotification(properNotifications[notificationIndex]);
    }

    public void ShowNotification(string notificationText)
    {
        this.notificationText.text = notificationText;
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void ResetUI()
    {
        ChangeBuzzerAppearance(BuzzerState.idle);
        ShowNotification("");
    }

    public void InitUI(string initPlayerName)
    {
        if(score != 0)
        {
            throw new System.Exception("Tried to init UI of non-new player.");
        }

        // Truncate the name if too long.
        if (initPlayerName.Length > maxNameLength)
        {
            initPlayerName = initPlayerName.Substring(0, maxNameLength);
        }

        playerNameText.text = initPlayerName;
        scoreText.text = "0";
    }

    private void UpdateScoreView()
    {
        scoreText.text = score.ToString();
    }

    private void ChangeScore(int signedDelta)
    {
        score += signedDelta;
    }

    private void ChangeBuzzerAppearance(BuzzerState state)
    {
        Image buzzerBackground = buzzerButton.GetComponent<Image>();

        switch (state)
        {
            case BuzzerState.idle:
                buzzerBackground.color = Color.white;
                break;

            case BuzzerState.showingSuccess:
                buzzerBackground.color = Color.green;
                break;

            case BuzzerState.showingFailure:
                buzzerBackground.color = Color.red;
                break;
        }
    }
}
