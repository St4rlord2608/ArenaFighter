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

    List<Vector3> buffer = new List<Vector3>();

    private RichAI richAi;
    private LineRenderer lineRenderer;

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
            return;
        }
        if (GetLengthOfPath() <= 0)
        {
            ActionComplete();
        }
    }

    public override void ActionSelectedVisual()
    {
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, movementLayer))
        {
            ActionStart(onActionStart, onActionComplete);
            richAi.canMove = true;
            richAi.enableRotation = true;
            richAi.destination = hitInfo.point;

            DisplayMovementPath();
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
}
