using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTracker : MonoBehaviour
{
    public float timer;

    [SerializeField] ARTrackedImageManager imageManager;

    [SerializeField] List<GameObject> objectList = new List<GameObject>();
    private Dictionary<string, GameObject> prefabDic = new Dictionary<string, GameObject>();

    private List<ARTrackedImage> _trackedImg = new List<ARTrackedImage>();
    private List<float> _trackedTimer = new List<float>();

    private void Awake()
    {
        foreach(GameObject obj in objectList)
        {
            string Name = obj.name;
            prefabDic.Add(Name, obj);
        }
    }

    private void Update()
    {
        if(_trackedImg.Count > 0)
        {
            List<ARTrackedImage> tNumList = new List<ARTrackedImage>();

            for(int i=0; i<_trackedImg.Count; i++)
            {
                if (_trackedImg[i].trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
                {
                    if (_trackedTimer[i] > timer)
                    {
                        string name = _trackedImg[i].referenceImage.name;
                        GameObject tObj = prefabDic[name];
                        tObj.SetActive(false);
                        tNumList.Add(_trackedImg[i]);
                    }
                    else
                    {
                        _trackedTimer[i] += Time.deltaTime;
                    }
                }
            }

            if (tNumList.Count > 0)
            {
                for(int i=0; i<tNumList.Count; i++)
                {
                    int num = _trackedImg.IndexOf(tNumList[i]);
                    _trackedImg.Remove(_trackedImg[num]);
                    _trackedTimer.Remove(_trackedTimer[num]);
                }
            }
        }
    }

    private void OnEnable()
    {
        imageManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        imageManager.trackedImagesChanged -= ImageChanged;
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach(ARTrackedImage trackedImage in args.added)
        {
            if (!_trackedImg.Contains(trackedImage))
            {
                _trackedImg.Add(trackedImage);
                _trackedTimer.Add(0);
            }
        }

        foreach(ARTrackedImage trackedImage in args.updated)
        {
            if (!_trackedImg.Contains(trackedImage))
            {
                _trackedImg.Add(trackedImage);
                _trackedTimer.Add(0);
            }
            else
            {
                int num = _trackedImg.IndexOf(trackedImage);
                _trackedTimer[num] = 0;
            }
            UpdateImage(trackedImage);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        GameObject tObj = prefabDic[name];
        tObj.transform.position = trackedImage.transform.position;
        tObj.transform.rotation = trackedImage.transform.rotation;
        tObj.SetActive(true);
    }
}
