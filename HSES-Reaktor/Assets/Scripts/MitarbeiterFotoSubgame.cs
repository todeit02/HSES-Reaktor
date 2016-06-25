using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MitarbeiterFotoSubgame : TerminableTaskSubgame
{
    private enum SubgameState
    {
        FadingIn, Active, FadingOut, Terminated, Paused
    }

    private SubgameState currentState = SubgameState.FadingIn;

    // paths of contained objects in the prefab
    private readonly string[] taskViewNames = { "TaskView1", "TaskView2" };
    private readonly string[] shownNamePaths = { "TaskView1/Name", "TaskView2/Name" };
    private readonly string[] shownPhotoPaths = { "TaskView1/Photo", "TaskView2/Photo" };
    private readonly string[] blendAnimStateNames = { "AppearFromLeftHalf", "AppearFromRightHalf" };

    // references to displayed name and photo in both TaskViews
    private Text[] shownTexts = new Text[taskViewsCount];
    private Texture2D[] shownPhotos = new Texture2D[taskViewsCount];
    private Animation[] taskStartAnimations = new Animation[taskViewsCount];

    private Mitarbeiter realMitarbeiter;

    private class Mitarbeiter
    {
        public readonly string name;
        public readonly Texture2D photo;
        
        public Mitarbeiter(string name, string photoPath)
        {
            if(null == name)
            {
                throw new System.Exception("Mitarbeiter name was not specified.");
            }
            if(null == photoPath)
            {
                throw new System.Exception("Mitarbeiter photo file path was not specified.");
            }
            if(!File.Exists(photoPath))
            {
                throw new System.Exception("File does not exist: " + photoPath);
            }

            this.name = name;

            byte[] photoData = File.ReadAllBytes(photoPath);
            photo = new Texture2D(0, 0);

            try {
            photo.LoadImage(photoData);
            }
            catch
            {
               throw new System.Exception("Loading failed: " + photoPath);
            }
        }
    }

    public override void Awake()
    {
        base.Awake();

        for(int i = 0; i < taskViewsCount; i++)
        {
            shownTexts[i]           = GameObject.Find(shownNamePaths[i]).GetComponent<Text>();
            shownPhotos[i]          = GameObject.Find(shownPhotoPaths[i]).GetComponent<Image>().sprite.texture;
            taskStartAnimations[i]  = GameObject.Find(taskViewNames[i]).GetComponent<Animation>();
        }
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override bool ExpectsReaction()
    {
        return (realMitarbeiter.name == shownTexts[0].text);
    }

    protected override void StartNewTask()
    {
        base.StartNewTask();

        realMitarbeiter = LoadRandomMitarbeiter();
        shownTexts[0].text = LoadRandomName();
        shownTexts[1].text = shownTexts[0].text;

        for(int i = 0; i < taskViewsCount; i++)
        {
            shownPhotos[i] = realMitarbeiter.photo;
        }
    }

    protected override void TerminateTask()
    {
        base.TerminateTask();
    }

    protected override void PreTaskRunAction()
    {
        // Reset playback direction and starting time of the fading effect.
        for (int i = 0; i < taskViewsCount; i++)
        {
            AnimationState blendAnimState = taskStartAnimations[i][blendAnimStateNames[i]];
            blendAnimState.speed = 1;
            blendAnimState.time = 0;
            taskStartAnimations[i].Play();
            Debug.Log("Insert animation at: " + Time.fixedTime);
        }
    }

    protected override void PostTaskRunAction()
    {
        for (int i = 0; i < taskStartAnimations.Length; i++)
        {
            AnimationState blendAnimState = taskStartAnimations[i][blendAnimStateNames[i]];
            blendAnimState.speed = -1;
            blendAnimState.time = blendAnimState.length;
            //blendAnimState.wrapMode = WrapMode.ClampForever;
            taskStartAnimations[i].Play();
            Debug.Log("Fadeout animation at: " + Time.fixedTime);
        }
    }

    private Mitarbeiter LoadRandomMitarbeiter()
    {
        // Dummy values
        string loadedName = "Echter Name";
        string loadedFilePath = @".\assets\images\mitarbeiter_dummy.png";

        return new Mitarbeiter(loadedName, loadedFilePath);
    }

    private string LoadRandomName()
    {
        return "Zufallsname"; // Dummy value
    }
}