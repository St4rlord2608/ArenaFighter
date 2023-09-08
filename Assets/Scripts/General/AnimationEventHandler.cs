using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public event EventHandler OnFinish;
    public event EventHandler OnShoot;
    public event EventHandler OnMagazineRemoved;
    public event EventHandler OnMagazineAttached;
    public event EventHandler OnLeftFootStep;
    public event EventHandler OnRightFootStep;
    public event EventHandler OnLeftFootStepRun;
    public event EventHandler OnRightFootStepRun;
    public event EventHandler OnPullReloadLever;
    public event EventHandler OnAttachMagazine;

    private void AnimationShoot()
    {
        OnShoot?.Invoke(this, EventArgs.Empty);
    }
    private void AnimationFinishedTrigger()
    {
        OnFinish?.Invoke(this, EventArgs.Empty);
    }

    private void AnimationMagazineRemovedTrigger()
    {
        OnMagazineRemoved?.Invoke(this, EventArgs.Empty);
    }

    private void AnimationMagazineAttachedTrigger()
    {
        OnMagazineAttached?.Invoke(this, EventArgs.Empty);
    }

    private void AnimationAttachMagazine()
    {
        OnAttachMagazine?.Invoke(this, EventArgs.Empty);
    }

    private void AnimationPullReloadLever()
    {
        OnPullReloadLever?.Invoke(this, EventArgs.Empty);
    }

    private void LeftFootStep()
    {
        OnLeftFootStep?.Invoke(this, EventArgs.Empty);
    }

    private void RightFootStep()
    {
        OnRightFootStep?.Invoke(this, EventArgs.Empty);
    }

    private void LeftFootStepRun()
    {
        OnLeftFootStepRun?.Invoke(this, EventArgs.Empty);
    }

    private void RightFootStepRun()
    {
        OnRightFootStepRun?.Invoke(this, EventArgs.Empty);
    }
}
