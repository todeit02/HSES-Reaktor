using UnityEngine;
using System.Collections;

public abstract class Subgame : MonoBehaviour
{
    public enum SubgameState
    {
        Active, Terminated, Paused
    }

    public enum InternalTrigger
    {
        Entry, Exit, None
    }

    protected static readonly string[] taskViewNames = { "TaskViewBottom", "TaskViewTop" };

    protected const int taskViewsCount = 2;
    protected bool hasReactionOccured = false;
    private const int requiredWins = 3;
    private int remainingWins = requiredWins;
    private float taskRuntime = 0;
    private bool isTaskTimerRunning = false;

    protected bool AreWinsRemaining
    {
        get
        {
            return (remainingWins > 0);
        }
    }

    protected float TaskRuntime
    {
        get
        {
            return taskRuntime;
        }
    }

    /***********************************************************/
    /********************** Unity Methods **********************/
    /***********************************************************/

    public abstract void Awake();

    public abstract void Start();

    public abstract void Update();

    public virtual void FixedUpdate()
    {
        TriggerTaskTimer();
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

    public void Pause()
    {
        LockTaskTimer();
    }

    public void Resume()
    {
        ReleaseTaskTimer();
    }

    public void DestroyObject()
    {
        Object.Destroy(gameObject);
    }

    public abstract bool ExpectsReaction
    {
        get;
    }

    public abstract SubgameState Run();

    protected virtual void LoadNewTask()
    {
        hasReactionOccured = false;
    }

    protected virtual void TerminateTask()
    {
    }

    protected void InitTaskTimers()
    {
        isTaskTimerRunning = false;
        taskRuntime = 0;
    }

    protected void ReleaseTaskTimer()
    {
        isTaskTimerRunning = true;
    }

    protected void LockTaskTimer()
    {
        isTaskTimerRunning = false;
    }

    protected void DecreaseRemainingWins()
    {
        remainingWins--;
    }

    protected abstract bool HasTaskExpired();

    private void TriggerTaskTimer()
    {
        if (isTaskTimerRunning)
        {
            taskRuntime += Time.fixedDeltaTime;
        }
    }
}
