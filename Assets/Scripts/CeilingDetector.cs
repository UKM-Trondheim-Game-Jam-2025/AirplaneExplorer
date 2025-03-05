using UnityEngine;
using System;

public class CeilingDetector : MonoBehaviour
{
    public event Action<bool> OnCeilingStateChanged;

    [Header("Detection Settings")]
    [SerializeField] bool ignoreLayerMask = true; // New option to ignore layer mask checks
    [SerializeField] LayerMask ceilingLayerMask;
    [SerializeField] bool showDebugGizmos = true;
    [SerializeField] Color gizmoColor = Color.yellow;

    bool m_CeilingDetected = false;
    BoxCollider m_TriggerCollider;

    void Awake()
    {
        // Make sure we have a trigger collider
        m_TriggerCollider = GetComponent<BoxCollider>();
        if (m_TriggerCollider is null)
        {
            m_TriggerCollider = gameObject.AddComponent<BoxCollider>();
            m_TriggerCollider.size = new Vector3(0.9f, 0.2f, 0.9f); // Default size - adjust in inspector
            m_TriggerCollider.center = Vector3.zero;
        }
        m_TriggerCollider.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // If ignoreLayerMask is true, detect any object. Otherwise, check the layer mask.
        if (ignoreLayerMask || ((1 << other.gameObject.layer) & ceilingLayerMask) != 0)
        {
            if (!m_CeilingDetected)
            {
                m_CeilingDetected = true;
                OnCeilingStateChanged?.Invoke(true);
                if (Debug.isDebugBuild)
                    Debug.Log($"Ceiling detected: {other.gameObject.name}");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // If ignoreLayerMask is true, detect any object. Otherwise, check the layer mask.
        if (ignoreLayerMask || ((1 << other.gameObject.layer) & ceilingLayerMask) != 0)
        {
            // Only count as "no ceiling" if we've exited all ceiling objects
            if (m_CeilingDetected && !IsAnyOverlappingCeiling())
            {
                m_CeilingDetected = false;
                OnCeilingStateChanged?.Invoke(false);
                if (Debug.isDebugBuild)
                    Debug.Log("Ceiling no longer detected");
            }
        }
    }

    // Check if we're still overlapping any ceiling objects
    bool IsAnyOverlappingCeiling()
    {
        if (ignoreLayerMask)
        {
            // If we're ignoring layer mask, check for any collider
            Collider[] overlapping = Physics.OverlapBox(
                m_TriggerCollider.bounds.center,
                m_TriggerCollider.bounds.extents * 0.9f,
                transform.rotation
            );

            // Filter out self and parent player colliders
            int count = 0;
            foreach (var collider in overlapping)
            {
                // Skip colliders on our own GameObject or parent
                if (collider.transform == transform ||
                    (transform.parent != null && collider.transform == transform.parent) ||
                    collider.isTrigger)
                {
                    continue;
                }
                count++;
            }
            return count > 0;
        }
        else
        {
            // Use the original layer mask check
            Collider[] overlapping = Physics.OverlapBox(
                m_TriggerCollider.bounds.center,
                m_TriggerCollider.bounds.extents * 0.9f,
                transform.rotation,
                ceilingLayerMask
            );

            return overlapping.Length > 0;
        }
    }

    // Public accessor for the current ceiling detection state
    public bool IsCeilingDetected()
    {
        return m_CeilingDetected;
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos || m_TriggerCollider == null)
            return;

        Gizmos.color = m_CeilingDetected ? Color.red : gizmoColor;
        
        // Draw the trigger box
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(
            m_TriggerCollider.bounds.center,
            transform.rotation,
            Vector3.one
        );
        Gizmos.DrawWireCube(Vector3.zero, m_TriggerCollider.bounds.size);
        Gizmos.matrix = oldMatrix;
    }
}