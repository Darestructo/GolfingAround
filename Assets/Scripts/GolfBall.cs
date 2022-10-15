using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBall : MonoBehaviour
{
    [SerializeField] float _stopVelocity = .05f;

    [SerializeField] float _modifiedPower = 1f;

    bool _bIsIdle;
    bool _bIsAiming;

    LineRenderer _lineRenderer;

    Rigidbody _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;

        _bIsAiming = false;

        _lineRenderer = transform.GetChild(0).GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
    }

    void Update()
    {
        HandleAimer();
    }

    void FixedUpdate()
    {
        if (_rigidbody.velocity.magnitude < _stopVelocity)
        {
            StopBall();
        }
    }

    void OnMouseDown()
    {
        if (_bIsIdle)
        {
            _bIsAiming = true;
        }
    }

    void HandleAimer()
    {
        if (!_bIsAiming || !_bIsIdle ||PowerBar.Instance.BPowerBarFrozen)
            return;

        Vector3? worldPoint = CastMouseClickRay();

        if (!worldPoint.HasValue)
            return;

        DrawLine(worldPoint.Value);

        if (Input.GetMouseButtonUp(0))
        {
            Shoot(worldPoint.Value);
        }
    }

    void Shoot(Vector3 worldPoint)
    {
        _bIsAiming = false;
        _lineRenderer.enabled = false;

        Vector3 horizontalWorldPoint = new Vector3(worldPoint.x, transform.position.y, worldPoint.z);

        Vector3 direction = (horizontalWorldPoint - transform.position).normalized;

        float flatPower = Vector3.Distance(transform.position, horizontalWorldPoint);

        SnapshotPower();

        _rigidbody.AddForce(direction * flatPower * _modifiedPower);

        _bIsIdle = false;
    }

    void SnapshotPower()
    {
        _modifiedPower = PowerBar.Instance.CurrentPower;
        PowerBar.Instance.FreezePowerBar();
    }

    void StopBall()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _bIsIdle = true;
    }

    void DrawLine(Vector3 worldPoint)
    {
        Vector3[] positions = { transform.position, worldPoint };
        _lineRenderer.SetPositions(positions);
        _lineRenderer.enabled = true;
    }

    Vector3? CastMouseClickRay()
    {
        Vector3 screenMousePosFar = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane);

        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);

        RaycastHit hit;

        if (Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit, float.PositiveInfinity))
            return hit.point;
        else
            return null;
    }
}