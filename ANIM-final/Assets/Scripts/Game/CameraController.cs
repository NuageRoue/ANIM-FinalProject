using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -40f);
    [SerializeField] private float trackingSpeed = 5f;

    private Transform _trackedTarget;

    private void Update()
    {
        if (_trackedTarget == null) return;

        Vector3 targetPos = new Vector3(
            _trackedTarget.position.x + offset.x,
            transform.position.y,
            _trackedTarget.position.z + offset.z
        );

        transform.position = Vector3.Lerp(transform.position, targetPos, trackingSpeed * Time.deltaTime);
    }

    public void SnapTo(Vector3 worldPosition)
    {
        _trackedTarget = null;
        transform.position = new Vector3(
            worldPosition.x + offset.x,
            transform.position.y,
            worldPosition.z + offset.z
        );
    }

    public void Track(Transform target)
    {
        _trackedTarget = target;
    }

    public void StopTracking()
    {
        _trackedTarget = null;
    }
}