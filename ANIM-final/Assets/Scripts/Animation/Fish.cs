using System.Collections;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Transform mesh;

    [Header("Waypoints")]
    [SerializeField]
    Transform inWaterIn;

    [SerializeField]
    Transform inWaterOut;

    [SerializeField]
    Transform OutWater;

    [Header("Settings")]
    [SerializeField]
    float speed = 1;

    bool run = false;

    /// <summary>
    /// Starts the fish loop if it isn't already running.
    /// Returns false if the loop was already active, true if it was successfully started.
    /// </summary>
    public bool LoopStart()
    {
        if (run)
            return false;
        run = true;
        StartCoroutine(Loop());
        return true;
    }

    /// <summary>
    /// Stops the fish loop on the next frame by setting the run flag to false.
    /// </summary>
    public void LoopStop()
    {
        run = false;
    }

    /// <summary>
    /// Instantly snaps the fish mesh to the given transform's position and rotation.
    /// </summary>
    public void SetPosition(Transform transform)
    {
        mesh.position = transform.position;
        mesh.rotation = transform.rotation;
    }

    /// <summary>
    /// Main loop that animates the fish back and forth between the two in-water waypoints.
    /// On the return trip, applies a sine-based arc toward the out-of-water height.
    /// </summary>
    private IEnumerator Loop()
    {
        float targetY = OutWater.position.y; // Y height the fish reaches at the peak of its arc

        Transform start = inWaterIn;
        Transform stop = inWaterOut;
        float distance = (start.position - stop.position).sqrMagnitude;
        float endTime = distance / speed; // Total duration of one pass based on distance and speed

        while (run)
        {
            // Forward pass: straight movement from inWaterIn to inWaterOut
            start = inWaterIn;
            stop = inWaterOut;
            float time = 0;

            mesh.LookAt(stop);

            while (run && time < endTime)
            {
                mesh.position = Vector3.Lerp(start.position, stop.position, time / endTime);
                time += Time.deltaTime;
                yield return null;
            }

            // Return pass: movement from inWaterOut to inWaterIn with a sine arc on Y
            start = inWaterOut;
            stop = inWaterIn;
            time = 0;

            while (run && time < endTime)
            {
                float nextTime = time + Time.deltaTime;

                // Current position with sine-based Y arc
                Vector3 position = Vector3.Lerp(start.position, stop.position, time / endTime);
                float t = Mathf.Sin(Mathf.PI * time / endTime); // Peaks at 1 at the midpoint
                position.y = Mathf.Lerp(position.y, targetY, t);

                // Next frame position used to compute forward direction
                Vector3 nextPosition = Vector3.Lerp(
                    start.position,
                    stop.position,
                    nextTime / endTime
                );
                float nextT = Mathf.Sin(Mathf.PI * nextTime / endTime);
                nextPosition.y = Mathf.Lerp(nextPosition.y, targetY, nextT);

                Vector3 direction = (nextPosition - position).normalized;
                mesh.LookAt(mesh.position + direction, Vector3.up); // Orient mesh along movement direction

                mesh.position = position;
                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}
