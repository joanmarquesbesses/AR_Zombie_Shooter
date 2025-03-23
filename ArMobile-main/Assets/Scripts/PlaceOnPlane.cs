using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : MonoBehaviour
{
    public GameObject m_PlacedPrefab;
    public GameObject spawnedObject;
    ARRaycastManager m_RaycastManager;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.isPressed)
        {
            Vector2 screenPosition = Pointer.current.position.ReadValue();

            if (m_RaycastManager.Raycast(screenPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = s_Hits[0].pose;

                if (spawnedObject == null)
                {
                    spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                }
                else
                {
                    spawnedObject.transform.position = hitPose.position;
                }
            }
        }
    }
}
