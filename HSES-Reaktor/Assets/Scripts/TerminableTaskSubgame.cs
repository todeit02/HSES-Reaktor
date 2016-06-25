using UnityEngine;
public abstract class TerminableTaskSubgame : Subgame
{
    private const float taskDurationS = 4.0f;

    public override void Awake()
    {
        base.Awake();
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

    protected override bool HasTaskExpired()
    {
        // Is the gap between the current time and the start time (considering a possible pause) longer than the same task should be displayed?
        return ((Time.fixedTime - TaskStartingTime) > (taskDurationS + PauseDuration));
    }

    protected override void StartNewTask()
    {
        base.StartNewTask();
        PreTaskRunAction();
        ReleaseTaskTimer();
    }

    protected override void TerminateTask()
    {
        LockTaskTimer();
        PostTaskRunAction();
        base.TerminateTask();
    }

    protected abstract void PreTaskRunAction();

    protected abstract void PostTaskRunAction();
}