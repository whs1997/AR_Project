using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTracker : MonoBehaviour
{
    public float _timer;
    public ARTrackedImageManager trackedImageManager;
    public List<GameObject> _objectList = new List<GameObject>();
    private Dictionary<string, GameObject> _prefabDic = new Dictionary<string, GameObject>();
    private List<ARTrackedImage> _trackedImg = new List<ARTrackedImage>();
    private List<float> _trackedTimer = new List<float>();

    private void Awake()
    {
        foreach (GameObject obj in _objectList)
        {
            string tName = obj.name;

            _prefabDic.Add(tName, obj);
        } 
    }

    private void Update()
    {
        if (_trackedImg.Count > 0)
        {
            List<ARTrackedImage> tNumList = new List<ARTrackedImage>();
            for (var i = 0; i < _trackedImg.Count; i++)
            {
                if (_trackedImg[i].trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
                {
                    if (_trackedTimer[i] > _timer)
                    {
                        string name = _trackedImg[i].referenceImage.name;
                        GameObject tObj = _prefabDic[name];
                        tObj.SetActive(false);
                        tNumList.Add(_trackedImg[i]);
                    }
                    else
                    {
                        _trackedTimer[i] += Time.deltaTime;
                        Debug.Log($"timer {_timer}, _trackedTimer {_trackedTimer[i]}");
                    }
                }
            }
            
            if (tNumList.Count > 0)
            {
                for (var i = 0; i < tNumList.Count; i++)
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
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }
    
    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            if (!_trackedImg.Contains(trackedImage))
            {
                _trackedImg.Add(trackedImage);
                _trackedTimer.Add(0);
            }
        }
        
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (!_trackedImg.Contains(trackedImage))
            {
                _trackedImg.Add(trackedImage);
                _trackedTimer.Add(0);
            }
            else
            {
                int num = _trackedImg.IndexOf(trackedImage);

                // Limited 상태가 아닐때만 타이머를 0으로 만듬
                if (_trackedImg[num].trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
                {
                }
                else
                {
                    _trackedTimer[num] = 0;
                }
            }
            UpdateImage(trackedImage);
        }
    }
    
    private void UpdateImage(ARTrackedImage trackedImage)
    {
        int num = _trackedImg.IndexOf(trackedImage);
        string name = trackedImage.referenceImage.name;
        GameObject tObj = _prefabDic[name];

        // Limited 상태가 아닐때 위치 갱신, 활성화
        if (_trackedImg[num].trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
        {
        }
        else
        {
            tObj.transform.position = trackedImage.transform.position;
            tObj.transform.rotation = trackedImage.transform.rotation;
            tObj.SetActive(true);
        }
    }
}