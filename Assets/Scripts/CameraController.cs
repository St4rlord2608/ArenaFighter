using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [Space]
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float aimTargetSpeed = 4f;
    [SerializeField] private float rotationSpeed = 100f;
    [Space]
    [SerializeField] private float minFollowYOffset = 2f;
    [SerializeField] private float maxFollowYOffset = 12f;

    private CinemachineTransposer cinemachineTransposer;
    private Vector3 targetFollowOffset;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        cinemachineTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 movementVector = transform.forward * inputVector.y + transform.right * inputVector.x;
        transform.position += movementVector * Time.deltaTime * movementSpeed;
    }

    public bool MoveToPosition(Vector3 pos)
    {
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * aimTargetSpeed);
        return transform.position == pos;
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0f, 0f, 0f);

        if(GameInput.Instance.GetRotationFloat() > 0f)
        {
            rotationVector.y = 1f;
        }else if(GameInput.Instance.GetRotationFloat() < 0f)
        {
            rotationVector.y = -1f;
        }

        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float zoomAmount = 1f;
        if(GameInput.Instance.GetZoom() > 0f)
        {
            targetFollowOffset.y -= zoomAmount;
        }else if (GameInput.Instance.GetZoom() < 0f)
        {
            targetFollowOffset.y += zoomAmount;
        }
        float zoomSpeed = 5f;
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, minFollowYOffset, maxFollowYOffset);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, zoomSpeed * Time.deltaTime);
    }
}
