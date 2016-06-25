using UnityEngine;
using System.Collections;

public abstract class Subgame : MonoBehaviour
{
    protected const int taskViewsCount = 2;
    private const int requiredWins = 3;
    private int remainingWins = requiredWins;
    private float taskStartingTime;
    private float pauseDuration = 0;
    private bool isTaskTimerRunning = false;
    private bool isPaused = false;

    protected float TaskStartingTime
    {
        get
        {
            return taskStartingTime;
        }
    }

    protected float PauseDuration
    {
        get
        {
            return pauseDuration;
        }
    }

    public virtual void Awake()
    {

    }

    public virtual void Start()
    {
        StartNewTask();
    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {
        if(isPaused)
        {
            pauseDuration += Time.fixedDeltaTime;
        }

        if (HasTaskExpired())
        {
            TerminateTask();
            StartNewTask();
        }
    }

    public bool HasEnded()
    {
        return (remainingWins > 0);
    }

    public void Pause()
    {
        isPaused = true;
        LockTaskTimer();
    }

    public void Resume()
    {
        isPaused = false;
        ReleaseTaskTimer();
    }

    public abstract bool ExpectsReaction();

    protected virtual void StartNewTask()
    {
        InitTaskTimers();
    }

    protected virtual void TerminateTask()
    {
    }

    protected void InitTaskTimers()
    {
        isTaskTimerRunning = false;
        taskStartingTime = Time.fixedTime;
        pauseDuration = 0;
    }

    protected void ReleaseTaskTimer()
    {
        isTaskTimerRunning = true;
    }

    protected void LockTaskTimer()
    {
        isTaskTimerRunning = false;
    }

    protected abstract bool HasTaskExpired();
}
