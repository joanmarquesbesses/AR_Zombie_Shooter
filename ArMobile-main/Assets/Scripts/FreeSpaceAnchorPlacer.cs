using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class FreeSpaceAnchorPlacer : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public ARAnchorManager anchorManager;
    public GameObject prefabToPlace;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Optional: Store anchors if you plan to persist or re-use them
    private List<ARAnchor> placedAnchors = new List<ARAnchor>();

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 screenPosition = Pointer.current.position.ReadValue();

            // Optional: Raycast to get the world position relative to real-world depth
            if (raycastManager.Raycast(screenPosition, hits, TrackableType.FeaturePoint | TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                // Create a free-floating anchor (not attached to any plane)
                GameObject anchorObject = new GameObject("FreeAnchor");
                anchorObject.transform.position = hitPose.position;
                anchorObject.transform.rotation = hitPose.rotation;

                ARAnchor anchor = anchorObject.AddComponent<ARAnchor>();

                if (anchor != null)
                {
                    // Store it if you want persistence or later access
                    placedAnchors.Add(anchor);

                    // Instantiate your content parented to the anchor
                    Instantiate(prefabToPlace, anchor.transform);

                    Debug.Log("Free-space Anchor created.");
                }
                else
                {
                    Debug.LogWarning("Failed to create ARAnchor.");
                }
            }
        }
    }

    // Optional: Example function to clear anchors (cleanup / reset)
    public void ClearAnchors()
    {
        foreach (var anchor in placedAnchors)
        {
            if (anchor != null)
                Destroy(anchor.gameObject);
        }
        placedAnchors.Clear();
    }
}
