using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImageToAnchor : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private Transform sceneRoot;

    private ARTrackedImageManager trackedImageManager;
    private ARAnchorManager anchorManager;
    private ARAnchor anchor;

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        anchorManager = GetComponent<ARAnchorManager>();
    }

    private void OnEnable()
    {
        trackedImageManager.trackablesChanged.AddListener(OnTrackablesChanged);
    }

    private void OnDisable()
    {
        trackedImageManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
    }

    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var added in args.added)
        {
            TryCreateAnchorFrom(added);
        }

        foreach (var updated in args.updated)
        {
            TryCreateAnchorFrom(updated);
        }
    }

    private async void TryCreateAnchorFrom(ARTrackedImage trackedImage)
    {
        if (anchor != null) return;

        if (trackedImage.trackingState != TrackingState.Tracking) return;

        Pose pose = new Pose(trackedImage.transform.position, trackedImage.transform.rotation);

        Result<ARAnchor> result = await anchorManager.TryAddAnchorAsync(pose);

        if (result.status.IsSuccess())
        {
            anchor = result.value;

            sceneRoot.SetPositionAndRotation(anchor.transform.position, anchor.transform.rotation);
            sceneRoot.gameObject.SetActive(true);

            trackedImageManager.enabled = false;
            Debug.Log("[ImageToAnchor] Anchor created & image tracking disabled.");
        }
    }

    public void ResetAndRescan()
    {
        if (anchor == null) return;

        anchorManager.TryRemoveAnchor(anchor);
        anchor = null;

        trackedImageManager.enabled = true;
        Debug.Log("[ImageToAnchor] Reset - image tracking re-enabled.");
    }
}
