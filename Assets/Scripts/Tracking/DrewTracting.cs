using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class DrewTracting : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager imageManager;
    [SerializeField] private GameObject virtualImagePrefab;

    private Dictionary<string, GameObject> spawnedImageObjects = new Dictionary<string, GameObject>();

    void OnEnable()
    {
        if (imageManager == null)
        {
            Debug.LogError("ARTrackedImageManager is not assigned!");
            return;
        }

        imageManager.trackedImagesChanged += OnTrackedImagesChanged;
        Debug.Log("DrewTracting Enabled");
    }

    void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        Debug.Log("DrewTracting Disabled");
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            Debug.Log("Image Added: " + trackedImage.referenceImage.name);
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            Debug.Log("Image Updated: " + trackedImage.referenceImage.name);
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            Debug.Log("Image Removed: " + trackedImage.referenceImage.name);
            if (spawnedImageObjects.TryGetValue(trackedImage.referenceImage.name, out GameObject obj))
            {
                obj.SetActive(false);
                Debug.Log("Disabled object for: " + trackedImage.referenceImage.name);
            }
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        Debug.Log("Updating Image: " + imageName);

        if (imageName == "Wheatfield")
        {
            if (!spawnedImageObjects.ContainsKey(imageName))
            {
                GameObject newImage = Instantiate(virtualImagePrefab, trackedImage.transform.position, trackedImage.transform.rotation);
                newImage.transform.SetParent(trackedImage.transform);
                spawnedImageObjects[imageName] = newImage;
                Debug.Log("Created new virtual image for: " + imageName);
            }

            GameObject imageObj = spawnedImageObjects[imageName];
            imageObj.SetActive(true);

            // Update position and rotation
            imageObj.transform.SetParent(trackedImage.transform);
            imageObj.transform.localPosition = Vector3.zero;
            imageObj.transform.localRotation = Quaternion.identity;
            Debug.Log("Updated position and rotation for: " + imageName);
        }
        else
        {
            Debug.LogWarning("No action taken for unrecognized image: " + imageName);
        }
    }
}
