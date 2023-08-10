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
        MoveUnit();
        if (GetLengthOfPath() <= 0)
        {
            startMoving = false;
            ActionComplete();
            onStopMoving?.Invoke(this, EventArgs.Empty);
        }
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
        }
        DisplayMovementPath();
        float length = GetLengthOfPath();
    }

    public override void PerformAction(Action onActionStart, Action onActionComplete)
    {
        lineRenderer.enabled = true;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, movementLayer))
        {
            onStartRotating?.Invoke(this, EventArgs.Empty);
            ActionStart(onActionStart, onActionComplete);
            
            richAi.destination = hitInfo.point;

            DisplayMovementPath();
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

    private void DisplayMovementPath()
    {
        richAi.GetRemainingPath(buffer, out bool stale);
        lineRenderer.positionCount = buffer.Count;
        lineRenderer.SetPositions(buffer.ToArray());
    }

    private float GetLengthOfPath()
    {
        DisplayMovementPath();
        float length = 0f;
        if (buffer.Count >= 2)
        {
            for (int i = 0; i < buffer.Count - 1; i++)
            {
                length += Vector3.Distance(buffer[i], buffer[i + 1]);
            }
        }

        return length;
    }

    public override string GetActionName()
    {
        return "Move";
    }
}
