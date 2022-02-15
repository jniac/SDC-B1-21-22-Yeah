using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundDetection), typeof(Rigidbody))]
public class CubeSnapping : MonoBehaviour
{
    public enum SnappingStatus
    {
        MovingTooFast,
        TooFarFromGround,
        Snapping,
    }

    public float velocityThreshold = 1f;
    public float transitionDuration = 0.5f;

    float t = 0;
    SnappingStatus status;
    Rigidbody body;
    GroundDetection groundDetection;

    SnappingStatus GetStatus()
    {
        if (groundDetection.closeToTheGround == false)
            return SnappingStatus.TooFarFromGround;

        if (body.velocity.sqrMagnitude > velocityThreshold * velocityThreshold)
            return SnappingStatus.MovingTooFast;

        return SnappingStatus.Snapping;
    }

    void Start()
    {
        body = GetComponent<Rigidbody>();
        groundDetection = GetComponent<GroundDetection>();
    }

    void Update()
    {
        status = GetStatus();

        if (status != SnappingStatus.Snapping)
        {
            t = 0;
        }
        else
        {
            t = Mathf.Clamp01(t + Time.deltaTime / transitionDuration);

            var position = transform.position;
            var snapPosition = new Vector3(
                Mathf.Round(position.x + 0.5f) - 0.5f,
                position.y,
                Mathf.Round(position.z + 0.5f) - 0.5f);

            var rotation = transform.rotation.eulerAngles;
            var snapRotation = Quaternion.Euler(
                Mathf.Round(rotation.x / 90f) * 90f,
                Mathf.Round(rotation.y / 90f) * 90f,
                Mathf.Round(rotation.z / 90f) * 90f);

            transform.position = Vector3.Lerp(transform.position, snapPosition, t);
            transform.rotation = Quaternion.Slerp(transform.rotation, snapRotation, t);
        }
    }

#if UNITY_EDITOR
    [Tooltip("Show debug info on screen.")]
    public bool debug = false;
    void OnGUI()
    {
        if (debug)
        {
            var style = new GUIStyle();
            style.fontSize = 32;
            style.normal.textColor = status == SnappingStatus.Snapping ? new Color(0.6f, 1.0f, 0.8f) : new Color(1.0f, 0.9f, 0.8f);
            string statusString = (
                status == SnappingStatus.MovingTooFast ? "Moving too fast" :
                status == SnappingStatus.TooFarFromGround ? "Too far from ground" :
                "Snapping");
            GUI.Label(new Rect(10, 10, 150, 100), $"{body.velocity.magnitude:F2} ({statusString})", style);
        }
    }
#endif
}
