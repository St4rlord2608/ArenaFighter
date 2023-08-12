using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveAction : BaseAction
{

    [SerializeField] private LayerMask movementLayer;
    [Space]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float rotationOffset = 5f;

    List<Vector3> buffer = new List<Vector3>();

    private RichAI richAi;
    private LineRenderer lineRenderer;

    private bool startMoving;
    private bool isAboveAvailableLength;

    private float activePathMaxDistance;
    private float currentPathLength;

    public event EventHandler onStartRotating;
    public event EventHandler onStopRotating;
    public event EventHandler onStartMoving;
    public event EventHandler onStopMoving;

    protected override void Awake()
    {
        base.Awake();
        richAi = GetComponent<RichAI>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (!isActive)
        {
            lineRenderer.enabled = false;
            return;
        }
        ProcessPath();
        MoveUnit();
        if (currentPathLength <= 0)
        {
            HandleActionComplete();
        }else if(activePathMaxDistance - currentPathLength >= unit.GetAvailableActionPoints(this))
        {
            HandleActionComplete();
            unit.SetAvailableActionPoints(this, 0f);
        }
        SetActionPointsCost(activePathMaxDistance - currentPathLength);
        Debug.Log(actionPointsCost);
        unit.TryToSpendActionPointsToTakeAction(this);
        activePathMaxDistance = activePathMaxDistance - (activePathMaxDistance - currentPathLength);
    }

    private void HandleActionComplete()
    {
        startMoving = false;
        ActionComplete();
        onStopMoving?.Invoke(this, EventArgs.Empty);
    }

    public override void ActionSelectedVisual()
    {
        lineRenderer.enabled = true;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, movementLayer))
        {
            richAi.canMove = false;
            richAi.enableRotation = false;
            richAi.destination = hitInfo.point;
            richAi.SearchPath();
        }
        ProcessPath();
    }

    public override void PerformAction(Action onActionStart, Action onActionComplete)
    {
        lineRenderer.enabled = true;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, movementLayer))
        {
            ProcessPath();
            activePathMaxDistance = currentPathLength;
            if (!(unit.GetAvailableActionPoints(this) <= 0))
            {
                onStartRotating?.Invoke(this, EventArgs.Empty);
                ActionStart(onActionStart, onActionComplete);
            }

        }
    }

    private void MoveUnit()
    {
        if(!startMoving)
        {
            
            var lookingPosition = buffer[1] - transform.position;
            var lookingRotation = Mathf.Atan2(lookingPosition.x, lookingPosition.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(transform.rotation.x, lookingRotation, transform.rotation.z)), rotationSpeed * Time.deltaTime);
            var angleDifference = ((transform.rotation.eulerAngles.y - lookingRotation) + 360f) % 360;
            if (angleDifference <= rotationOffset || angleDifference >= 360 - rotationOffset)
            {
                onStopRotating?.Invoke(this, EventArgs.Empty);
                onStartMoving?.Invoke(this, EventArgs.Empty);
                startMoving = true;
            }
        }
        if (startMoving)
        {
            richAi.enableRotation = true;
            richAi.canMove = true;
        }
    }

    private void ProcessPath()
    {
        richAi.GetRemainingPath(buffer, out bool stale);
        List<Vector3> linePositions = new List<Vector3>();
        currentPathLength = 0f;
        if (buffer.Count >= 2)
        {
            int i = 0;
            for (; i < buffer.Count - 1; i++)
            {
                currentPathLength += Vector3.Distance(buffer[i], buffer[i + 1]);
                if (isAboveAvailableLength)
                {
                    continue;   
                }
                linePositions.Add(buffer[i]);
                float lengthOfCurrentIteration = Vector3.Distance(buffer[i], buffer[i + 1]);
                SetActionPointsCost(currentPathLength);
                if (!unit.CanSpendActionPoints(this))
                {
                    Vector3 dir = (buffer[i + 1] - buffer[i]).normalized;
                    float lengthToAdd = unit.GetAvailableActionPoints(this) - (currentPathLength - lengthOfCurrentIteration);
                    linePositions.Add(buffer[i] + (dir * lengthToAdd));
                    isAboveAvailableLength = true;
                    SetActionPointsCost((currentPathLength - lengthOfCurrentIteration) + lengthToAdd);
                }
            }
            if (!isAboveAvailableLength)
            {
                linePositions.Add(buffer[i]);
            }
            isAboveAvailableLength = false;

        }
        lineRenderer.positionCount = linePositions.Count;
        lineRenderer.SetPositions(linePositions.ToArray());
    }
    public override string GetActionName()
    {
        return Unit.MOVE_ACTION;
    }
}
