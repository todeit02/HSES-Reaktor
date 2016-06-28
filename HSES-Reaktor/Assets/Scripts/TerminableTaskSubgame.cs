using UnityEngine;
public abstract class TerminableTaskSubgame : Subgame
{
    public enum TerminableTaskSubgameState
    {
        FadingIn, Active, FadingOut, Terminated, Paused
    }

    private TerminableTaskSubgameState currentTerminableTaskSubgameState = TerminableTaskSubgameState.FadingIn;
    private TerminableTaskSubgameState nextTerminableTaskSubgameState = TerminableTaskSubgameState.FadingIn;
    private InternalTrigger currentTrigger = InternalTrigger.Entry;
    private Animation[] taskStartAnimations = new Animation[taskViewsCount];

    private const float taskDuration = 4.0f;

    public override void Awake()
    {
        for (int i = 0; i < taskViewsCount; i++)
        {
            taskStartAnimations[i] = GameObject.Find(taskViewNames[i]).GetComponent<Animation>();
        }
    }

    public abstract override void Start();

    public abstract override void Update();

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected Animation[] TaskStartAnimations
    {
        get
        {
            return taskStartAnimations;
        }
    }

    protected override bool HasTaskExpired()
    {
        // Is the gap between the current time and the start time (considering a possible pause) longer than the same task should be displayed?
        return (TaskRuntime > taskDuration);
    }

    protected override void LoadNewTask()
    {
        base.LoadNewTask();
    }

    protected override void TerminateTask()
    {
        LockTaskTimer();
        base.TerminateTask();
    }

    public static SubgameState ToSubgameState(TerminableTaskSubgameState convertingState)
    {
        switch (convertingState)
        {
            case TerminableTaskSubgameState.Active:
            case TerminableTaskSubgameState.FadingIn:
            case TerminableTaskSubgameState.FadingOut:
                return SubgameState.Active;

            case TerminableTaskSubgameState.Paused:
                return SubgameState.Paused;

            case TerminableTaskSubgameState.Terminated:
                return SubgameState.Terminated;
        }
        throw new System.Exception("Illegal conversion.");
    }

    public override SubgameState Run()
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
        currentTerminableTaskSubgameState = nextTerminableTaskSubgameState;

        return ToSubgameState(currentTerminableTaskSubgameState);
    }

    private void EntryActivity()
    {
        switch (currentTerminableTaskSubgameState)
        {
            case TerminableTaskSubgameState.FadingIn:
                LoadNewTask();
                PlayFadeInAnimations();
                break;

            case TerminableTaskSubgameState.Active:
                InitTaskTimers();
                ReleaseTaskTimer();
                break;

            case TerminableTaskSubgameState.FadingOut:
                PlayFadeOutAnimations();
                break;

            case TerminableTaskSubgameState.Paused:
                LockTaskTimer();
                break;
        }
    }

    private void ExitActivity()
    {
        switch (currentTerminableTaskSubgameState)
        {
            case TerminableTaskSubgameState.Active:
                LockTaskTimer();
                break;

            case TerminableTaskSubgameState.FadingOut:
                GameManager.ResetPlayerUIs();
                break;
        }
    }

    private bool TryTransition()
    {
        switch (currentTerminableTaskSubgameState)
        {
            case TerminableTaskSubgameState.FadingIn:
                if (!IsAnimationPlaying)
                {
                    nextTerminableTaskSubgameState = TerminableTaskSubgameState.Active;
                    Debug.Log("FadingIn -> Active at " + Time.fixedTime);
                }
                break;

            case TerminableTaskSubgameState.Active:
                if (false /*Dummy value - Pause button pressed*/)
                {
                    nextTerminableTaskSubgameState = TerminableTaskSubgameState.Paused;
                }
                else if (HasTaskExpired() || hasReactionOccured)
                {
                    nextTerminableTaskSubgameState = TerminableTaskSubgameState.FadingOut;
                    Debug.Log("Active -> FadingOut at " + Time.fixedTime);
                }
                break;

            case TerminableTaskSubgameState.FadingOut:
                if (!IsAnimationPlaying)
                {
                    if (AreWinsRemaining)
                    {
                        nextTerminableTaskSubgameState = TerminableTaskSubgameState.FadingIn;
                        Debug.Log("FadingOut -> FadingIn at " + Time.fixedTime);
                    }
                    else
                    {
                        nextTerminableTaskSubgameState = TerminableTaskSubgameState.Terminated;
                        Debug.Log("FadingOut -> Terminated at " + Time.fixedTime);
                    }
                }
                break;

            case TerminableTaskSubgameState.Paused:
                if (false /*Dummy value - Resume button pressed*/)
                {
                    nextTerminableTaskSubgameState = TerminableTaskSubgameState.Active;
                }
                break;
        }

        return (nextTerminableTaskSubgameState != currentTerminableTaskSubgameState);
    }

    protected abstract void PlayFadeInAnimations();

    protected abstract void PlayFadeOutAnimations();

    private bool IsAnimationPlaying
    {
        get
        {
            foreach (Animation checkingAnimation in taskStartAnimations)
            {
                if (checkingAnimation.isPlaying)
                {
                    return true;
                }
            }
            return false;
        }
    }
}