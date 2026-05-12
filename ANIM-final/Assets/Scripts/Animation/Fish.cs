using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Fish : MonoBehaviour
{
    [SerializeField]
    Transform mesh;

    [SerializeField]
    Transform inWaterIn;

    [SerializeField]
    Transform inWaterOut;

    [SerializeField]
    Transform OutWater;

    [SerializeField]
    float speed = 1;

    bool run = false;

    public bool LoopStart()
    {
        if (run)
            return false;
        run = true;
        StartCoroutine(Loop());
        return true;
    }

    public void LoopStop()
    {
        run = false;
    }

    public void SetPosition(Transform transform)
    {
        mesh.position = transform.position;
        mesh.rotation = transform.rotation;
    }

    private IEnumerator Loop()
    {
        float targetY = OutWater.position.y;

        Transform start = inWaterIn;
        Transform stop = inWaterOut;
        float distance = (start.position - stop.position).sqrMagnitude;
        float endTime = distance / speed;

        while (run)
        {
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

            start = inWaterOut;
            stop = inWaterIn;
            time = 0;

            while (run && time < endTime)
            {
                float nextTime = time + Time.deltaTime;

                Vector3 position = Vector3.Lerp(start.position, stop.position, time / endTime);
                float t = Mathf.Sin(Mathf.PI * time / endTime);
                position.y = Mathf.Lerp(position.y, targetY, t);

                Vector3 nextPosition = Vector3.Lerp(
                    start.position,
                    stop.position,
                    nextTime / endTime
                );
                float nextT = Mathf.Sin(Mathf.PI * nextTime / endTime);
                nextPosition.y = Mathf.Lerp(nextPosition.y, targetY, nextT);

                Vector3 direction = (nextPosition - position).normalized;
                mesh.LookAt(mesh.position + direction, Vector3.up);

                mesh.position = position;
                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}
