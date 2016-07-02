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

    protected bool AreWinsRemaining
    {
        get
        {
            return (remainingWins > 0);
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

    public bool HasEnded
    {
        get
        {
            return (remainingWins > 0);
        }
    }

    public SubgameState Run()
    {
        if (currentTrigger == InternalTrigger.Entry)
        {
            currentTrigger = InternalTrigger.None; // Reset trigger.
            EntryActivity();
        }

        // "Do" activities (None available.)

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

        return currentSubgameState;
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
                GameManager.ResetPlayerUIs();
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
                if (false /*Dummy value - Pause button pressed*/)
                {
                    nextSubgameState = SubgameState.Paused;
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
                if (false /*Dummy value - Resume button pressed*/)
                {
                    nextSubgameState = SubgameState.Active;
                }
                break;
        }

        return (nextSubgameState != currentSubgameState);
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

    protected Animation[] TaskStartAnimations
    {
        get
        {
            return taskStartAnimations;
        }
    }

    protected abstract void PlayFadeInAnimations();

    protected abstract void PlayFadeOutAnimations();

    public void DestroyObject()
    {
        Object.Destroy(gameObject);
    }

    public bool ExpectsReaction
    {
        get
        {
            hasReactionOccured = true; // If a reaction is expected will only be checked upon player reaction.
            return OnExpectsReaction();
        }
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
        // TO DO: Show pause menu.
        Debug.Log("Game has been paused.");
        OnPauseEntry();
    }

    protected abstract void OnPauseEntry();

    protected abstract void OnActiveExit();

    protected abstract void OnFadingInExit();
}
