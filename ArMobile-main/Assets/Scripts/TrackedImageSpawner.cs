using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using TMPro;
using System;

public class TrackedImageSpawner : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject prefabToSpawn; // Assign your content prefab in the Inspector
    public TMPro.TextMeshProUGUI txtDebug;

    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, ARTrackedImage> trackedImages = new Dictionary<string, ARTrackedImage>();


    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        string txt = ""; 

        // Instantiate content for newly detected images
        foreach (var trackedImage in eventArgs.added)
        {
            txt += "Image detected " + trackedImage.name + "\n";
            SpawnContent(trackedImage);
        }

        // Remove content for lost images
        foreach (var trackedImage in eventArgs.removed)
        {
            txt += "Image lost " + trackedImage.name + "\n";
            RemoveContent(trackedImage);
        }

        txtDebug.text = txt;
    }

    private void SpawnContent(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (!spawnedObjects.ContainsKey(imageName))
        {
            GameObject newObject = Instantiate(prefabToSpawn, trackedImage.transform.position, trackedImage.transform.rotation);
            newObject.transform.parent = trackedImage.transform; // Parent to the tracked image
            spawnedObjects.Add(imageName, newObject);
            trackedImages.Add(imageName, trackedImage);
        }
    }

    private void RemoveContent(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        if (spawnedObjects.ContainsKey(imageName))
        {
            Destroy(spawnedObjects[imageName]);
            spawnedObjects.Remove(imageName);
            trackedImages.Remove(imageName);
        }
    }

    private void Update()
    {
        // Manually update the position and rotation of all tracked images every frame
        foreach (var imageName in trackedImages.Keys)
        {
            if (spawnedObjects.ContainsKey(imageName))
            {
                ARTrackedImage trackedImage = trackedImages[imageName];
                GameObject spawnedObject = spawnedObjects[imageName];

                // Update position and rotation every frame
                spawnedObject.transform.position = trackedImage.transform.position;
                spawnedObject.transform.rotation = trackedImage.transform.rotation;
            }
        }
    }
}
