using UnityEngine;
using System.Collections;

public abstract class Subgame : MonoBehaviour
{
    public enum SubgameState
    {
        FadingIn, Active, FadingOut, Terminated, Paused
    }

    public enum InternalTrigger
    {
        Entry, Exit, None
    }

    private SubgameState currentSubgameState = SubgameState.FadingIn;
    private SubgameState nextSubgameState = SubgameState.FadingIn;
    private InternalTrigger currentTrigger = InternalTrigger.Entry;
    private Animation[] taskStartAnimations = new Animation[taskViewsCount];

    protected static readonly string[] taskViewNames = { "TaskViewBottom", "TaskViewTop" };
    protected const int taskViewsCount = 2;
    protected bool  hasReactionOccured = false;

    private const int requiredWins = 3;
    private int remainingWins = requiredWins;
    private bool shallPause = false;
    
    public string ResourcesFilePath
    {
        get
        {
            string subgameResFileName = string.Concat(this.GetType(), ".xml");
            string subgameResFilePath = string.Concat(Application.streamingAssetsPath, '/', subgameResFileName);

            return subgameResFilePath;
        }
    }
    public string ResourcesFolder
    {
        get
        {
            string subgameResourcesFolder = string.Concat(Application.streamingAssetsPath, '/', this.GetType(), '/');

            return subgameResourcesFolder;
        }
    }

    public bool ExpectsReaction
    {
        get
        {
            hasReactionOccured = true; // If a reaction is expected will only be checked upon player reaction.
            return OnExpectsReaction();
        }
    }
    public SubgameState State
    {
        get
        {
            return currentSubgameState;
        }
    }
    public bool ChangesState
    {
        get
        {
            return (currentTrigger != InternalTrigger.None);
        }
    }
    public bool HasEnded
    {
        get
        {
            return (remainingWins > 0);
        }
    }
    
    protected Animation[] TaskStartAnimations
    {
        get
        {
            return taskStartAnimations;
        }
    }
    protected bool AreWinsRemaining
    {
        get
        {
            return (remainingWins > 0);
        }
    }

    private bool IsAnimationPlaying
    {
        get
        {
            foreach (Animation checkingAnimation in taskStartAnimations)
            {
                if (checkingAnimation != null && checkingAnimation.isPlaying)
                {
                    return true;
                }
            }
            return false;
        }
    }

    /***********************************************************/
    /********************** Unity Methods **********************/
    /***********************************************************/

    public virtual void Awake()
    {
        for (int i = 0; i < taskViewsCount; i++)
        {
            taskStartAnimations[i] = GameObject.Find(taskViewNames[i]).GetComponent<Animation>();
        }
    }

    public abstract void Start();

    public abstract void Update();

    public virtual void FixedUpdate()
    {
    }

    /***********************************************************/
    /*********************** User Methods **********************/
    /***********************************************************/

    public void Run()
    {
        if (currentTrigger == InternalTrigger.Entry)
        {
            currentTrigger = InternalTrigger.None; // Reset trigger.
            EntryActivity();
        }

        DoActivity();

        bool hasTransitionOccured = TryTransition();

        if (hasTransitionOccured)
        {
            currentTrigger = InternalTrigger.Exit;
        }

        if (currentTrigger == InternalTrigger.Exit)
        {
            currentTrigger = InternalTrigger.Entry; // If a state is left, another one is entered.
            ExitActivity();
        }

        // Switch to next state.
        currentSubgameState = nextSubgameState;
    }

    public void RequestPause()
    {
        shallPause = true;
    }

    public void RequestResume()
    {
        shallPause = false;
    }

    private void EntryActivity()
    {
        switch (currentSubgameState)
        {
            case SubgameState.FadingIn:
                LoadNewTask();
                PlayFadeInAnimations();
                break;

            case SubgameState.Active:
                OnActiveEntry();
                break;

            case SubgameState.FadingOut:
                PlayFadeOutAnimations();
                break;

            case SubgameState.Paused:
                PauseEntry();
                break;
        }
    }

    private void DoActivity()
    {
        switch(currentSubgameState)
        {
            case SubgameState.Active:
                bool isPauseRequested = Input.GetKeyDown(KeyCode.Escape);

                if(isPauseRequested)
                {
                    RequestPause();
                }
                break;
        }
    }

    private void ExitActivity()
    {
        switch (currentSubgameState)
        {
            case SubgameState.Active:
                OnActiveExit();
                break;

            case SubgameState.FadingIn:
                OnFadingInExit();
                break;

            case SubgameState.FadingOut:
                GameManager.Reset(GameManager.ResetOption.playerUIs);
                break;
        }
    }

    private bool TryTransition()
    {
        switch (currentSubgameState)
        {
            case SubgameState.FadingIn:
                if (!IsAnimationPlaying)
                {
                    nextSubgameState = SubgameState.Active;
                    Debug.Log("FadingIn -> Active at " + Time.fixedTime);
                }
                break;

            case SubgameState.Active:
                if (shallPause)
                {
                    nextSubgameState = SubgameState.Paused;
                    Debug.Log("Active -> Paused at " + Time.fixedTime);
                }
                else if (HasTaskExpired() || hasReactionOccured)
                {
                    nextSubgameState = SubgameState.FadingOut;
                    Debug.Log("Active -> FadingOut at " + Time.fixedTime);
                }
                break;

            case SubgameState.FadingOut:
                if (!IsAnimationPlaying)
                {
                    if (AreWinsRemaining)
                    {
                        nextSubgameState = SubgameState.FadingIn;
                        Debug.Log("FadingOut -> FadingIn at " + Time.fixedTime);
                    }
                    else
                    {
                        nextSubgameState = SubgameState.Terminated;
                        Debug.Log("FadingOut -> Terminated at " + Time.fixedTime);
                    }
                }
                break;

            case SubgameState.Paused:
                if (!shallPause)
                {
                    nextSubgameState = SubgameState.Active;
                    Debug.Log("Paused -> Active at " + Time.fixedTime);
                }
                break;
        }

        return (nextSubgameState != currentSubgameState);
    }

    protected abstract void PlayFadeInAnimations();

    protected abstract void PlayFadeOutAnimations();

    public void DestroyObject()
    {
        Object.Destroy(gameObject);
        GameManager.Reset(GameManager.ResetOption.runningSubgame);
    }

    private void LoadNewTask()
    {
        hasReactionOccured = false;
        OnLoadNewTask();
    }

    protected abstract void OnLoadNewTask();

    protected void DecreaseRemainingWins()
    {
        remainingWins--;
    }

    protected abstract bool HasTaskExpired();

    protected abstract bool OnExpectsReaction();

    protected abstract void OnActiveEntry();

    private void PauseEntry()
    {
        Debug.Log("Game has been paused.");
        OnPauseEntry();
    }

    protected abstract void OnPauseEntry();

    protected abstract void OnActiveExit();

    protected abstract void OnFadingInExit();
}
