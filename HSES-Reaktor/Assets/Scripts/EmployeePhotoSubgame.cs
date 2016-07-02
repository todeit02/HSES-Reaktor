using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class EmployeePhotoSubgame : TerminableTaskSubgame
{

    private enum ParsingState { awaitingStaff, readingStaff, readingEmployee, readingName, readingPhotoPath, done };

    // paths of contained objects in the prefab
    private readonly string[] shownNamePaths = { taskViewNames[0] + "/Name", taskViewNames[1] + "/Name" };
    private readonly string[] shownPhotoPaths = { taskViewNames[0] + "/Photo", taskViewNames[1] + "/Photo" };
    private readonly string[] blendAnimStateNames = { "AppearFromLeftFull", "AppearFromRightFull" };

    // references to displayed name and photo in both TaskViews
    private Text[] shownTexts = new Text[taskViewsCount];
    private Image[] shownPhotos = new Image[taskViewsCount];

    private List<string> loadedEmployeeNames;
    private List<string> loadedPhotoPaths;

    private Employee trueEmployee;

    private class Employee
    {
        public readonly string name;
        public Sprite photo;
        private Texture2D photoTexture;

        public Employee(string name, string photoPath)
        {
            if (null == name)
            {
                throw new System.Exception("Employee name was not specified.");
            }
            if (null == photoPath)
            {
                throw new System.Exception("Employee photo file path was not specified.");
            }
            if (!File.Exists(photoPath))
            {
                throw new System.Exception("File does not exist: " + photoPath);
            }

            this.name = name;

            byte[] photoData = File.ReadAllBytes(photoPath);
            photoTexture = new Texture2D(0, 0);

            try
            {
                photoTexture.LoadImage(photoData);
            }
            catch
            {
                throw new System.Exception("Loading failed: " + photoPath);
            }

            photo = Sprite.Create(photoTexture, new Rect(0, 0, photoTexture.width, photoTexture.height), new Vector2(photoTexture.width / 2, photoTexture.height / 2));
        }
    }

    /***********************************************************/
    /********************** Unity Methods **********************/
    /***********************************************************/

    public override void Awake()
    {
        base.Awake();

        for (int i = 0; i < taskViewsCount; i++)
        {
            shownTexts[i] = GameObject.Find(shownNamePaths[i]).GetComponent<Text>();
            shownPhotos[i] = GameObject.Find(shownPhotoPaths[i]).GetComponent<Image>();
        }

        LoadXML();
    }

    public override void Start()
    {
    }

    public override void Update()
    {
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    /***********************************************************/
    /*********************** User Methods **********************/
    /***********************************************************/

    protected override bool OnExpectsReaction()
    {
        bool isReactionExpected = (trueEmployee.name == shownTexts[0].text);

            if (isReactionExpected)
            {
                DecreaseRemainingWins();
            }

            return isReactionExpected;
    }

    protected override void OnLoadNewTask()
    {
        trueEmployee = LoadRandomEmployee();
        shownTexts[0].text = LoadRandomName();
        shownTexts[1].text = shownTexts[0].text;

        for (int i = 0; i < taskViewsCount; i++)
        {
            shownPhotos[i].sprite = trueEmployee.photo;
        }
    }

    protected override void PlayFadeInAnimations()
    {
        // Reset playback direction and starting time of the fading effect.
        for (int i = 0; i < taskViewsCount; i++)
        {
            AnimationState blendAnimState = TaskStartAnimations[i][blendAnimStateNames[i]];
            blendAnimState.speed = 1;
            blendAnimState.time = 0;
            TaskStartAnimations[i].Play();
        }
    }

    protected override void PlayFadeOutAnimations()
    {
        for (int i = 0; i < TaskStartAnimations.Length; i++)
        {
            AnimationState blendAnimState = TaskStartAnimations[i][blendAnimStateNames[i]];
            blendAnimState.speed = -1;
            blendAnimState.time = blendAnimState.length;
            TaskStartAnimations[i].Play();
        }
    }

    private void LoadXML()
    {
        const string subgameResPath = @".\assets\config\EmployeePhotoSubgame.xml";
        const string elemNameStaff = "staff";
        const string elemNameEmployee = "employee";
        const string elemNameName = "name";
        const string elemNamePhotoPath = "photoPath";
        char[] ignoreChars = { '\t', '\r', '\n' };

        XmlReader subgameResourcesReader = XmlReader.Create(subgameResPath);
        ParsingState state = ParsingState.awaitingStaff;
        bool readSuccess = false;

        string loadedName = null;
        string loadedPhotoPath = null;

        while (ParsingState.done != state)
        {

            // TO DO: Implement DTD-check

            do
            {
                readSuccess = subgameResourcesReader.Read();
            }
            while (-1 != subgameResourcesReader.Name.IndexOfAny(ignoreChars));

            if (!readSuccess)
            {
                state = ParsingState.done;
            }

            switch (state)
            {
                case ParsingState.awaitingStaff:
                    // <staff>
                    if (XmlNodeType.Element == subgameResourcesReader.NodeType && elemNameStaff == subgameResourcesReader.Name)
                    {
                        loadedEmployeeNames = new List<string>();
                        loadedPhotoPaths = new List<string>();

                        state = ParsingState.readingStaff;
                    }
                    break;

                case ParsingState.readingStaff:
                    // <employee>
                    if (XmlNodeType.Element == subgameResourcesReader.NodeType && elemNameEmployee == subgameResourcesReader.Name)
                    {
                        state = ParsingState.readingEmployee;
                    }
                    // </staff>
                    else if (XmlNodeType.EndElement == subgameResourcesReader.NodeType && elemNameStaff == subgameResourcesReader.Name)
                    {
                        state = ParsingState.done;
                    }
                    break;

                case ParsingState.readingEmployee:
                    // </employee>
                    if (XmlNodeType.EndElement == subgameResourcesReader.NodeType && elemNameEmployee == subgameResourcesReader.Name)
                    {
                        state = ParsingState.readingStaff;

                        loadedEmployeeNames.Add(loadedName);
                        loadedPhotoPaths.Add(loadedPhotoPath);

                    }
                    else if (XmlNodeType.Element == subgameResourcesReader.NodeType)
                    {
                        // <name>
                        if (elemNameName == subgameResourcesReader.Name)
                        {
                            state = ParsingState.readingName;
                        }
                        // <photoPath>
                        else if (elemNamePhotoPath == subgameResourcesReader.Name)
                        {
                            state = ParsingState.readingPhotoPath;
                        }
                    }
                    break;

                case ParsingState.readingName:
                    // </name>
                    if (XmlNodeType.EndElement == subgameResourcesReader.NodeType && elemNameName == subgameResourcesReader.Name)
                    {
                        state = ParsingState.readingPhotoPath;
                    }
                    // text inside <name></name>
                    else if (XmlNodeType.Text == subgameResourcesReader.NodeType)
                    {
                        loadedName = subgameResourcesReader.Value;
                    }
                    break;

                case ParsingState.readingPhotoPath:
                    // </photoPath>
                    if (XmlNodeType.EndElement == subgameResourcesReader.NodeType && elemNamePhotoPath == subgameResourcesReader.Name)
                    {
                        state = ParsingState.readingEmployee;
                    }
                    // text inside <photoPath></photoPath>
                    else if (XmlNodeType.Text == subgameResourcesReader.NodeType)
                    {
                        loadedPhotoPath = subgameResourcesReader.Value;
                    }
                    break;
            }
        }
    }

    private Employee LoadRandomEmployee()
    {
        if (loadedEmployeeNames == null || loadedPhotoPaths == null || loadedEmployeeNames.Count != loadedPhotoPaths.Count)
        {
            throw new System.Exception("Employees have not been loaded properly.");
        }

        // Create random index.
        System.Random randomIndexCreator = new System.Random();
        int randomIndex = randomIndexCreator.Next(0, loadedEmployeeNames.Count);

        // Create new Employee instance using the random name and path.
        return new Employee(loadedEmployeeNames[randomIndex], loadedPhotoPaths[randomIndex]);
    }

    private string LoadRandomName()
    {
        if (loadedEmployeeNames == null)
        {
            throw new System.Exception("Employees have not been loaded properly.");
        }

        // Create random index.
        System.Random randomIndexCreator = new System.Random();
        int randomIndex = randomIndexCreator.Next(0, loadedEmployeeNames.Count);

        // Create new Employee instance using the random name and path.
        return loadedEmployeeNames[randomIndex];
    }
}