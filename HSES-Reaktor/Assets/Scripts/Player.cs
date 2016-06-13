using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Button buzzerButton;
    public Text notificationText;

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

    public int Score
    {
        get
        {
            return score;
        }
    }

    public void HandleBuzzerButtonClick()
    {
        GameManager.Instance.OnPlayerReaction(this);
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
        ChangeBuzzerAppearance(resultingState);

        int notificationIndex = random.Next(0, properNotifications.Length);
        ShowNotification(properNotifications[notificationIndex]);
    }

    public void ShowNotification(string notificationText)
    {
        this.notificationText.text = notificationText;
    }

    public void ResetUI()
    {
        ChangeBuzzerAppearance(BuzzerState.idle);
        ShowNotification("");
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
