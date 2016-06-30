using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SubgameManager : MonoBehaviour
{
    public const string sceneName = "Subgame";
    public RectTransform playerCtrlAreaBottom;
    public RectTransform playerCtrlAreaTop;
    public RectTransform taskZone;
    public GameObject playerControlsPrefab;

    private Subgame runningSubgame;

    private const float ctrlsAnchorMaxY = 1.5f;

    private enum BuzzerPosition
    {
        bottomRighthand = 0, topRighthand, bottomLefthand, topLefthand
    }

    /***********************************************************/
    /********************** Unity Methods **********************/
    /***********************************************************/

    void Start ()
    {
        if (!((GameManager.subgames == null) || (GameManager.subgames.Count == 0)))
        {
            // Trial of prefab placement may result in return to main menu, so don't place controls until this is done.
            PlaceNextSubgamePrefab();
            PlacePlayerControls();
        }
        else
        {
            GameManager.Reset();
            SceneManager.LoadScene(MenuMainManager.sceneName);
        }
    }

    void FixedUpdate()
    {
        if (runningSubgame != null)
        {
            Subgame.SubgameState currentSubgameState = runningSubgame.Run();

            if (currentSubgameState == Subgame.SubgameState.Terminated)
            {
                runningSubgame.DestroyObject();
                runningSubgame = null;
                PlaceNextSubgamePrefab();
            }
        }
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
        byte controlsCount = GameManager.PlayersCount;

        for (BuzzerPosition placingPosition = (BuzzerPosition)0; placingPosition < (BuzzerPosition)controlsCount; placingPosition++)
        {
            GameObject placingControls = Instantiate(playerControlsPrefab);
            RectTransform placingControlsRectTransform = placingControls.GetComponent<RectTransform>();

            // Let the GameManager know about the new participating Player.
            Player placingPlayer = placingControls.GetComponent<Player>();
            GameManager.RegisterPlayer(placingPlayer);
            
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
            Text buttonLabel = placingControls.transform.Find("Buzzer").transform.Find("Text").GetComponent<Text>();
            buttonLabel.text = string.Concat("Spieler ", (int)placingPosition + 1);
        }
    }

    private void PlaceNextSubgamePrefab()
    {
        while ((runningSubgame == null) && (GameManager.subgames.Count > 0)) // Jump all non-existent subgames.
        {
            runningSubgame = PlaceSubgamePrefab(GameManager.subgames.Dequeue());
        }

        if (runningSubgame == null) // No more subgames availabe.
        {
            GameManager.Reset();
            SceneManager.LoadScene(MenuMainManager.sceneName);
        }
    }
}
