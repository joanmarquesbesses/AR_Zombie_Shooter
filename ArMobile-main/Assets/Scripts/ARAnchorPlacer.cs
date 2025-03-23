using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ARAnchorPlacer : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public ARAnchorManager anchorManager;
    public GameObject prefabToPlace;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        // Using New Input System for click/tap detection
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 screenPosition = Pointer.current.position.ReadValue();

            if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                // Get the hit pose and the plane hit
                Pose hitPose = hits[0].pose;
                ARPlane hitPlane = hits[0].trackable as ARPlane;

                ARAnchor anchor;

                // Optional: Attach the anchor to the plane for better stability
                if (hitPlane != null)
                {
                    anchor = anchorManager.AttachAnchor(hitPlane, hitPose);
                }
                else
                {
                    // Updated way: Manually create an ARAnchor GameObject
                    GameObject anchorObject = new GameObject("ARAnchor");
                    anchorObject.transform.position = hitPose.position;
                    anchorObject.transform.rotation = hitPose.rotation;
                    anchor = anchorObject.AddComponent<ARAnchor>();
                }

                if (anchor != null)
                {
                    Instantiate(prefabToPlace, anchor.transform);
                    Debug.Log("Anchor created and prefab placed.");
                }
                else
                {
                    Debug.LogWarning("Failed to create anchor.");
                }
            }
        }
    }
}

