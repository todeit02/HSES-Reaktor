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

    protected readonly string[] taskViewNames = { "TaskView1", "TaskView2" };
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

    public abstract void Awake();

    public abstract void Start();

    public abstract void Update();

    public virtual void FixedUpdate()
    {
        TriggerTaskTimer();
    }

    public bool HasEnded()
    {
        return (remainingWins > 0);
    }

    public void Pause()
    {
        LockTaskTimer();
    }

    public void Resume()
    {
        ReleaseTaskTimer();
    }

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }

    public abstract bool ExpectsReaction();

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

    private void TriggerTaskTimer()
    {
        if(isTaskTimerRunning)
        {
            taskRuntime += Time.fixedDeltaTime;
        }
    }

    protected void decreaseRemainingWins()
    {
        remainingWins--;
    }

    protected abstract bool HasTaskExpired();
}
