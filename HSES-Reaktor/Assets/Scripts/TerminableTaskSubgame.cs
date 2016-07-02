using System;
using UnityEngine;
public abstract class TerminableTaskSubgame : Subgame
{
    private const float taskDuration = 4.0f;

    private float taskRuntime = 0;
    private bool isTaskTimerRunning = false;

    /***********************************************************/
    /********************** Unity Methods **********************/
    /***********************************************************/

    public override void Awake()
    {
        base.Awake();
    }

    public abstract override void Start();

    public abstract override void Update();

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        TriggerTaskTimer();
    }

    /***********************************************************/
    /*********************** User Methods **********************/
    /***********************************************************/

    private float TaskRuntime
    {
        get
        {
            return taskRuntime;
        }
    }

    protected override bool HasTaskExpired()
    {
        // Is the gap between the current time and the start time (considering a possible pause) longer than the same task should be displayed?
        return (TaskRuntime > taskDuration);
    }

    protected abstract override void OnLoadNewTask();

    protected abstract override void PlayFadeInAnimations();

    protected abstract override void PlayFadeOutAnimations();

    protected override void OnActiveEntry()
    {
        ReleaseTaskTimer();
    }

    protected override void OnPauseEntry()
    {        
    }

    protected override void OnActiveExit()
    {
        LockTaskTimer();
    }

    protected override void OnFadingInExit()
    {
        InitTaskTimers();
    }

    private void InitTaskTimers()
    {
        isTaskTimerRunning = false;
        taskRuntime = 0;
    }

    private void ReleaseTaskTimer()
    {
        isTaskTimerRunning = true;
    }

    private void LockTaskTimer()
    {
        isTaskTimerRunning = false;
    }

    private void TriggerTaskTimer()
    {
        if (isTaskTimerRunning)
        {
            taskRuntime += Time.fixedDeltaTime;
        }
    }
}