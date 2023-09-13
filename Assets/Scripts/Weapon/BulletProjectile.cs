using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVfxPrefab;
    private Vector3 targetPosition;
    private Vector3 moveDir;
    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        moveDir = (targetPosition - transform.position).normalized;
    }

    private void Update()
    {
        

        //float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);
        float moveSpeed = 30f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        //float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        //if (distanceBeforeMoving < distanceAfterMoving)
        //{
        //    transform.position = targetPosition;
        //    trailRenderer.transform.parent = null;
        //    Destroy(gameObject);

        //    Instantiate(bulletHitVfxPrefab, targetPosition, Quaternion.identity);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        transform.position = targetPosition;
        trailRenderer.transform.parent = null;
        Destroy(gameObject);

        Instantiate(bulletHitVfxPrefab, targetPosition, Quaternion.identity);
    }
}
