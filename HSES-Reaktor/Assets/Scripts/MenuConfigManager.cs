using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Xml;

public class MenuConfigManager : MonoBehaviour
{
    private class SubgameToggle
    {
        public readonly string prefabName;
        public readonly GameObject toggleObject;

        public SubgameToggle(string prefabName, string menuName, GameObject togglePrefab)
        {
            this.prefabName = prefabName;

            toggleObject = Instantiate(togglePrefab);
            toggleObject.name = string.Concat(prefabName, "Toggle");

            Toggle gameObjectToggle = toggleObject.transform.Find("Toggle").GetComponent<Toggle>();
            gameObjectToggle.isOn = true;

            Text gameObjectText = toggleObject.transform.Find("Text").GetComponent<Text>();
            gameObjectText.text = menuName;
        }
    }

    private enum ParsingState { awaitingList, readingList, readingSubgame, readingPrefab, readingMenuName, done };

    public RectTransform toggleGrid;
    public GameObject subgameTogglePrefab;

    private List<SubgameToggle> toggles;
    private const short visibleToggles = 8;
    private float toggleGridRectMinX = 0.0f;


    // Use this for initialization
    void Start()
    {
        BuildSubgameToggleList();
        PlaceSubgameToggles();
        SetSelectedToggles();
    }

    public void HandleGameStartClick()
    {
        SaveSelectedToggles();
        GameManager.Instance.StartGame();
    }

    public void HandleMenuMainClick()
    {
        SaveSelectedToggles();
        SceneManager.LoadScene("MenuMain");
    }

    private void BuildSubgameToggleList()
    {
        const string subgameListPath = ".\\assets\\config\\subgame_list.xml";
        const string elemNameSubgameList = "subgameList";
        const string elemNameSubgame = "subgame";
        const string elemNamePrefab = "prefab";
        const string elemNameMenuName = "menuName";
        char[] ignoreChars = { '\t', '\r', '\n' };

        XmlReader subgameReader = XmlReader.Create(subgameListPath);
        ParsingState state = ParsingState.awaitingList;
        bool readSuccess = false;

        string prefab = null;
        string menuName = null;

        while (ParsingState.done != state)
        {

            // TO DO: Implement DTD-check

            do
            {
                readSuccess = subgameReader.Read();
            }
            while (-1 != subgameReader.Name.IndexOfAny(ignoreChars));

            if (!readSuccess)
            {
                state = ParsingState.done;
            }

            switch (state)
            {
                case ParsingState.awaitingList:
                    // <subgameList>
                    if (XmlNodeType.Element == subgameReader.NodeType && elemNameSubgameList == subgameReader.Name)
                    {
                        toggles = new List<SubgameToggle>();

                        state = ParsingState.readingList;
                    }
                    break;

                case ParsingState.readingList:
                    // <subgame>
                    if (XmlNodeType.Element == subgameReader.NodeType && elemNameSubgame == subgameReader.Name)
                    {
                        state = ParsingState.readingSubgame;
                    }
                    // </subgameList>
                    else if (XmlNodeType.EndElement == subgameReader.NodeType && elemNameSubgameList == subgameReader.Name)
                    {
                        state = ParsingState.done;
                    }
                    break;

                case ParsingState.readingSubgame:
                    // </subgame>
                    if (XmlNodeType.EndElement == subgameReader.NodeType && elemNameSubgame == subgameReader.Name)
                    {
                        state = ParsingState.readingList;

                        toggles.Add(new SubgameToggle(prefab, menuName, subgameTogglePrefab));

                    }
                    else if (XmlNodeType.Element == subgameReader.NodeType)
                    {
                        // <prefab>
                        if (elemNamePrefab == subgameReader.Name)
                        {
                            state = ParsingState.readingPrefab;
                        }
                        // <menuName>
                        else if (elemNameMenuName == subgameReader.Name)
                        {
                            state = ParsingState.readingMenuName;
                        }
                    }
                    break;

                case ParsingState.readingPrefab:
                    // </prefab>
                    if (XmlNodeType.EndElement == subgameReader.NodeType && elemNamePrefab == subgameReader.Name)
                    {
                        state = ParsingState.readingMenuName;
                    }
                    // text inside <prefab></prefab>
                    else if (XmlNodeType.Text == subgameReader.NodeType)
                    {
                        prefab = subgameReader.Value;
                    }
                    break;

                case ParsingState.readingMenuName:
                    // </menuName>
                    if (XmlNodeType.EndElement == subgameReader.NodeType && elemNameMenuName == subgameReader.Name)
                    {
                        state = ParsingState.readingSubgame;
                    }
                    // text inside <menuName></menuName>
                    else if (XmlNodeType.Text == subgameReader.NodeType)
                    {
                        menuName = subgameReader.Value;
                    }
                    break;
            }
        }
    }

    private void PlaceSubgameToggles()
    {

        if (toggles.Count > 0)
        {
            float toggleAnchorSpanX;

            if (visibleToggles < toggles.Count)
            {
                // Not all toggles fit into ScrollView matching ToggleGrid, so expand it beyond the ScrollView.
                toggleGridRectMinX = toggleGridRectMinX - (1.0f / visibleToggles) * (toggles.Count - visibleToggles);
                toggleGrid.anchorMin = new Vector2(toggleGridRectMinX, 0);
                toggleAnchorSpanX = 1.0f / toggles.Count;
            }
            else
            {
                // ToggleGrid fits into ScrollView.
                toggleAnchorSpanX = 1.0f / visibleToggles;
            }

            for (int i = 0; i < toggles.Count; i++)
            {
                GameObject placingToggle = toggles[i].toggleObject;

                // Make subgamesScrollRect the parent.
                placingToggle.transform.SetParent(toggleGrid.transform, false);

                // Adjust anchors.
                RectTransform placingToggleRectTransform = placingToggle.GetComponent<RectTransform>();
                float placingAnchorMinX = 1.0f - (i + 1) * toggleAnchorSpanX;
                float placingAnchorMaxX = 1.0f - i * toggleAnchorSpanX;
                placingToggleRectTransform.anchorMin = new Vector2(placingAnchorMinX, 0);
                placingToggleRectTransform.anchorMax = new Vector2(placingAnchorMaxX, 1);
                placingToggleRectTransform.offsetMin = new Vector2(0, 0);
                placingToggleRectTransform.offsetMax = new Vector2(0, 0);
            }
        }
    }

    private void SetSelectedToggles()
    {
        List<string> selections = GameManager.Instance.Subgames;

        if (null != selections)
        {
            // A selection has already been saved, so 
            foreach (SubgameToggle configuringToggle in toggles)
            {
                if (!selections.Contains(configuringToggle.prefabName))
                {
                    configuringToggle.toggleObject.transform.Find("Toggle").GetComponent<Toggle>().isOn = false;
                }
            }
        }
    }

    private void SaveSelectedToggles()
    {
        List<string> selectedSubgames = null;

        bool isSubgameSelected = false;

        // Add selected subgames to the game.
        foreach (SubgameToggle checkingToggle in toggles)
        {
            isSubgameSelected = checkingToggle.toggleObject.transform.Find("Toggle").GetComponent<Toggle>().isOn;

            if (isSubgameSelected)
            {
                if (null == selectedSubgames)
                {
                    selectedSubgames = new List<string>();
                }
                selectedSubgames.Add(checkingToggle.prefabName);
            }
        }
        GameManager.Instance.Subgames = selectedSubgames;
    }
}