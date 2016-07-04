using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SubgameManager : MonoBehaviour
{
    public const string sceneName = "Subgame";
    public RectTransform pauseDialogueBottom;
    public RectTransform pauseDialogueTop;
    public RectTransform playerCtrlAreaBottom;
    public RectTransform playerCtrlAreaTop;
    public RectTransform taskZone;
    public GameObject playerControlsPrefab;

    private bool wasValidSubgameInstantiated = false;
    private Subgame runningSubgame;

    private const float ctrlsAnchorMaxY = 1.5f;
    private const float pauseDialogueAlpha = 1.0f;

    private enum BuzzerPosition
    {
        bottomRighthand = 0, topRighthand, bottomLefthand, topLefthand
    }

    /***********************************************************/
    /********************** Unity Methods **********************/
    /***********************************************************/

    public void Start ()
    {
        HidePauseDialogues();

        wasValidSubgameInstantiated = false;
        
        // Trial of prefab placement may result in return to main menu, so don't place controls until this is done.
        PlaceNextSubgamePrefab();
        PlacePlayerControls();
    }

    public void FixedUpdate()
    {
        
        if (runningSubgame == null)
        {
            return; // Nothing to check if there is no running subgame.
        }

        // Prompt the subgame to continue.
        runningSubgame.Run();

        // Perform actions that have to be done upon subgame state's change.
        if (runningSubgame.ChangesState)
        {
            switch (runningSubgame.State)
            {
                case Subgame.SubgameState.Active:
                    HidePauseDialogues();
                    break;

                case Subgame.SubgameState.Paused:
                    ShowPauseDialogues();
                    break;

                case Subgame.SubgameState.Terminated:
                    runningSubgame.DestroyObject();
                    PlaceNextSubgamePrefab();
                    break;
            }
        }
    }

    public void OnDestroy()
    {
        GameManager.Reset(GameManager.ResetOption.subgameIndex);
    }

    /***********************************************************/
    /*********************** User Methods **********************/
    /***********************************************************/

    public Subgame PlaceSubgamePrefab(string subgamePrefabName)
    {
        GameObject placingSubgamePrefab = Resources.Load(subgamePrefabName) as GameObject;

        if (placingSubgamePrefab == null) // Return upon wrong prefab name.
        {
            return null;
        }

        wasValidSubgameInstantiated = true;

        GameObject placingSubgameInstance = Instantiate(placingSubgamePrefab);

        placingSubgameInstance.transform.SetParent(taskZone.transform, false);

        // Make sure prefab instance is stretched all over the task zone.
        RectTransform placingSgInstanceRectTrans = placingSubgameInstance.GetComponent<RectTransform>();
        placingSgInstanceRectTrans.anchorMin = new Vector2(0, 0);
        placingSgInstanceRectTrans.anchorMax = new Vector2(1, 1);
        placingSgInstanceRectTrans.offsetMin = new Vector2(0, 0);
        placingSgInstanceRectTrans.offsetMax = new Vector2(0, 0);

        Subgame placedSubgame = placingSubgameInstance.GetComponent<Subgame>();

        GameManager.RunningSubgame = placedSubgame;

        return placedSubgame;
    }

    private void PlacePlayerControls()
    {
        int controlsCount = GameManager.PlayersCount;

        for (BuzzerPosition placingPosition = (BuzzerPosition)0; placingPosition < (BuzzerPosition)controlsCount; placingPosition++)
        {
            GameObject placingControls = Instantiate(playerControlsPrefab);
            RectTransform placingControlsRectTransform = placingControls.GetComponent<RectTransform>();

            // Place controls depending on order.
            switch (placingPosition)
            {
                case BuzzerPosition.bottomRighthand:
                    placingControls.transform.SetParent(playerCtrlAreaBottom.transform, false);
                    placingControlsRectTransform.anchorMin = new Vector2(0.5f, 0);
                    placingControlsRectTransform.anchorMax = new Vector2(1, ctrlsAnchorMaxY);
                    break;

                case BuzzerPosition.topRighthand:
                    placingControls.transform.SetParent(playerCtrlAreaTop.transform, false);
                    placingControlsRectTransform.anchorMin = new Vector2(0.5f, 0);
                    placingControlsRectTransform.anchorMax = new Vector2(1, ctrlsAnchorMaxY);
                    break;

                case BuzzerPosition.bottomLefthand:
                    placingControls.transform.SetParent(playerCtrlAreaBottom.transform, false);
                    placingControlsRectTransform.anchorMin = new Vector2(0, 0);
                    placingControlsRectTransform.anchorMax = new Vector2(0.5f, ctrlsAnchorMaxY);
                    break;

                case BuzzerPosition.topLefthand:
                    placingControls.transform.SetParent(playerCtrlAreaTop.transform, false);
                    placingControlsRectTransform.anchorMin = new Vector2(0, 0);
                    placingControlsRectTransform.anchorMax = new Vector2(0.5f, ctrlsAnchorMaxY);
                    break;
            }

            placingControlsRectTransform.offsetMin = new Vector2(0, 0);
            placingControlsRectTransform.offsetMax = new Vector2(0, 0);

            // Create player control's button label.
            string placingPlayerName = string.Concat("Spieler ", (int)placingPosition + 1);

            Player placingPlayer = placingControls.GetComponent<Player>();
            placingPlayer.InitUI(placingPlayerName);
        }
    }

    private void PlaceNextSubgamePrefab()
    {
        string placingSubgameName;

        do
        {
            placingSubgameName = GameManager.NextSubgameName;
            runningSubgame = PlaceSubgamePrefab(placingSubgameName);
        }
        while ((placingSubgameName != null) && (runningSubgame == null)); // Subgame not loaded but more to follow.

        // Subgame is now running or no more subgames are available.

        if ((runningSubgame == null)) 
        {
            // No more subgames available.
            if (wasValidSubgameInstantiated)
            {
                GameManager.ComputeRanking(); // Compute ranking before players are gone.
                SceneManager.LoadScene(GameCompletionManager.sceneName);
            }
            else
            {
                // No subgame could be started, so no ranking can be done.            
                GameManager.Reset(GameManager.ResetOption.playerCount | GameManager.ResetOption.subgameList);
                SceneManager.LoadScene(MenuMainManager.sceneName);
            }
        }
    }

    private void ShowPauseDialogues()
    {
        pauseDialogueBottom.GetComponent<CanvasRenderer>().SetAlpha(pauseDialogueAlpha);
        pauseDialogueBottom.Find("PauseText").GetComponent<CanvasRenderer>().SetAlpha(pauseDialogueAlpha);

        pauseDialogueTop.GetComponent<CanvasRenderer>().SetAlpha(pauseDialogueAlpha);
        pauseDialogueTop.Find("PauseText").GetComponent<CanvasRenderer>().SetAlpha(pauseDialogueAlpha);
    }

    private void HidePauseDialogues()
    {
        pauseDialogueBottom.GetComponent<CanvasRenderer>().SetAlpha(0);
        pauseDialogueBottom.Find("PauseText").GetComponent<CanvasRenderer>().SetAlpha(0);
        pauseDialogueTop.GetComponent<CanvasRenderer>().SetAlpha(0);
        pauseDialogueTop.Find("PauseText").GetComponent<CanvasRenderer>().SetAlpha(0);
    }
}