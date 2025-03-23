using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImages : MonoBehaviour
{

    [SerializeField]
    ARTrackedImageManager m_TrackedImageManager;
    public GameObject prefabToSpawn; // Assign your content prefab in the Inspector

    void OnEnable() => m_TrackedImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => m_TrackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            // Handle added event
            Debug.Log("Imaged added " + newImage.referenceImage.name);

            GameObject newObject = Instantiate(prefabToSpawn, newImage.transform.position, newImage.transform.rotation);
            newObject.transform.parent = newImage.transform; // Parent to the tracked image
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            // Handle updated event
            Debug.Log("Imaged updated " + updatedImage.referenceImage.name);
        }

        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event
            Debug.Log("Imaged removed " + removedImage.referenceImage.name);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
