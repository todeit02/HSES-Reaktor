using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SubgameManager : MonoBehaviour {

    public RectTransform playerCtrlAreaLeft;
    public RectTransform playerCtrlAreaRight;
    public RectTransform taskZone;
    public GameObject playerControlsPrefab;

    private Subgame runningSubgame;

    private const float ctrlsAnchorMaxX = 1.5f;

    // Use this for initialization
    void Start () {
        PlacePlayerControls();
    }
	
	// Update is called once per frame
	void Update () {
        /*
        if (runningSubgame.HasEnded())
        {
            OnSubgameEnd();
        }
        */
	}

    private void PlacePlayerControls()
    {
        byte controlsCount = GameManager.Instance.PlayersCount;

        for (byte ctrlsIndex = 0; ctrlsIndex < controlsCount; ctrlsIndex++)
        {
            GameObject placingControls = Instantiate(playerControlsPrefab);
            RectTransform placingControlsRectTransform = placingControls.GetComponent<RectTransform>();
            
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

            Text buttonLabel = placingControls.transform.Find("Buzzer").transform.Find("Text").GetComponent<Text>();
            buttonLabel.text = string.Concat("Spieler ", ctrlsIndex + 1);
        }
    }

    public void StartSubgame()
    {

    }

    public void OnSubgameEnd()
    {

    }
}
