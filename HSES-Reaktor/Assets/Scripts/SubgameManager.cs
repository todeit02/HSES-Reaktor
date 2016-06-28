using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SubgameManager : MonoBehaviour
{
    public RectTransform playerCtrlAreaLeft;
    public RectTransform playerCtrlAreaRight;
    public RectTransform taskZone;
    public GameObject playerControlsPrefab;

    private Subgame runningSubgame;

    private const float ctrlsAnchorMaxX = 1.5f;

    void Start () {
        PlacePlayerControls();
        runningSubgame = StartSubgame(GameManager.subgames.Dequeue());
    }
	
	void Update () {
        if(runningSubgame.Run() == Subgame.SubgameState.Terminated)
        {
            runningSubgame.Destroy();
            StartSubgame(GameManager.subgames.Dequeue());
        }
	}

    private void PlacePlayerControls()
    {
        byte controlsCount = GameManager.PlayersCount;

        for (byte ctrlsIndex = 0; ctrlsIndex < controlsCount; ctrlsIndex++)
        {
            GameObject placingControls = Instantiate(playerControlsPrefab);
            RectTransform placingControlsRectTransform = placingControls.GetComponent<RectTransform>();

            // Let the GameManager know about the new participating Player.
            Player placingPlayer = placingControls.GetComponent<Player>();
            GameManager.RegisterPlayer(placingPlayer);
            
            // Place controls depending on order.
            switch (ctrlsIndex)
            {
                case 0:
                    placingControls.transform.SetParent(playerCtrlAreaLeft.transform, false);
                    placingControlsRectTransform.anchorMin = new Vector2(0, 0.5f);
                    placingControlsRectTransform.anchorMax = new Vector2(ctrlsAnchorMaxX, 1);
                    break;

                case 1:
                    placingControls.transform.SetParent(playerCtrlAreaRight.transform, false);
                    placingControlsRectTransform.anchorMin = new Vector2(0, 0);
                    placingControlsRectTransform.anchorMax = new Vector2(ctrlsAnchorMaxX, 0.5f);
                    break;

                case 2:
                    placingControls.transform.SetParent(playerCtrlAreaRight.transform, false);
                    placingControlsRectTransform.anchorMin = new Vector2(0, 0.5f);
                    placingControlsRectTransform.anchorMax = new Vector2(ctrlsAnchorMaxX, 1);
                    break;

                case 3:
                    placingControls.transform.SetParent(playerCtrlAreaLeft.transform, false);
                    placingControlsRectTransform.anchorMin = new Vector2(0, 0);
                    placingControlsRectTransform.anchorMax = new Vector2(ctrlsAnchorMaxX, 0.5f);
                    break;
            }

            placingControlsRectTransform.offsetMin = new Vector2(0, 0);
            placingControlsRectTransform.offsetMax = new Vector2(0, 0);

            // Create player control's button label.
            Text buttonLabel = placingControls.transform.Find("Buzzer").transform.Find("Text").GetComponent<Text>();
            buttonLabel.text = string.Concat("Spieler ", ctrlsIndex + 1);
        }
    }

    public Subgame StartSubgame(string subgamePrefabName)
    {
        GameObject startingSubgamePrefab = Resources.Load(subgamePrefabName) as GameObject;
        GameObject startingSubgameInstance = Instantiate(startingSubgamePrefab);

        startingSubgameInstance.transform.SetParent(taskZone.transform, false);

        // Make sure prefab instance is stretched all over the task zone.
        RectTransform startingSgInstanceRectTrans = startingSubgameInstance.GetComponent<RectTransform>();
        startingSgInstanceRectTrans.anchorMin = new Vector2(0, 0);
        startingSgInstanceRectTrans.anchorMax = new Vector2(1, 1);
        startingSgInstanceRectTrans.offsetMin = new Vector2(0, 0);
        startingSgInstanceRectTrans.offsetMax = new Vector2(0, 0);

        Subgame startedSubgame = startingSubgameInstance.GetComponent<Subgame>();

        GameManager.RunningSubgame = startedSubgame;

        return startedSubgame;
    }
}
