using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTracker : MonoBehaviour
{

    public ARTrackedImageManager trackedImageManager;

    public List<GameObject> _objectList = new List<GameObject>(); // 씬에 있는 게임오브젝트를 가져올 오브젝트 리스트 
    private Dictionary<string, GameObject> _prefabDic = new Dictionary<string, GameObject>(); // 오브젝트를 이름으로 받아올 딕셔너리

    private List<ARTrackedImage> _trackedImg = new List<ARTrackedImage>(); // 트래킹하고있는 이미지
    private List<float> _trackedTime = new List<float>(); // 트래킹하고 있는 이미지의 타이머

    public float timer; // 이미지를 트래킹하지 못할때 증가할 타이머

    private void Awake()
    {
        foreach (GameObject obj in _objectList)
        {
            string tName = obj.name;

            _prefabDic.Add(tName, obj); // 딕셔너리에 오브젝트의 이름을 키값으로 받아옴
            Debug.Log($"{tName} 이름의 오브젝트 준비 완료");
        } 
    }

    private void Update()
    {
        // ImageChanged의 remove 상태
        if (_trackedImg.Count > 0) // 이미지를 트래킹하고 있다면
        {
            List<ARTrackedImage> tNumList = new List<ARTrackedImage>(); // 타이머가 넘어가면 임시로 저장할 리스트

            for (int i = 0; i < _trackedImg.Count; i++)
            {
                // 트래킹 상태가 Limited 이면 (이미지를 못찾고있으면)
                if (_trackedImg[i].trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
                {
                    if (_trackedTime[i] > timer) // 트래킹 타이머가 지정된 타이머보다 커지면 제거
                    {
                        string name = _trackedImg[i].referenceImage.name; // 트래킹하고있는 이미지의 이름을 받아옴
                        GameObject tObj = _prefabDic[name]; // 오브젝트와 연동해서
                        tObj.SetActive(false); // 오브젝트를 비활성화
                        tNumList.Add(_trackedImg[i]); // 임시 리스트에 추가
                    }
                    else
                    {
                        _trackedTime[i] += Time.deltaTime; // 트래킹하고있는 이미지의 타이머 시간 증가
                        Debug.Log($"{_trackedImg[i]} 이미지의 타이머 _trackedTimer {_trackedTime[i]}, timer {timer}가 되면 제거,");
                    }
                }
            }
            
            if (tNumList.Count > 0)
            {
                for (int i = 0; i < tNumList.Count; i++)
                {
                    int num = _trackedImg.IndexOf(tNumList[i]); // 임시 리스트에 추가된 trackedImg의 인덱스 값 받아옴
                    _trackedImg.Remove(_trackedImg[num]); // 오브젝트를 비활성화하고 trackedImg의 리스트에서도 삭제
                    _trackedTime.Remove(_trackedTime[num]); // 오브젝트를 비활성화하고 trackedTime의 리스트에서도 삭제
                }
            }
        }
    }
    
    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged; // ImageChanged 함수와 연동해 값이 변동되면 갱신함
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }
    
    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added) // 트래킹 이미지 추가
        {
            if (!_trackedImg.Contains(trackedImage)) // 트래킹중인 이미지가 추가되지 않았다면
            {
                _trackedImg.Add(trackedImage); // 이미지를 추가하고
                _trackedTime.Add(0); // 트래킹 타이머를 0으로 추가해줌
            }
        }
        
        foreach (ARTrackedImage trackedImage in eventArgs.updated) // 트래킹 이미지 갱신
        {
            if (!_trackedImg.Contains(trackedImage))
            {
                _trackedImg.Add(trackedImage);
                _trackedTime.Add(0);
            }
            else
            {
                int num = _trackedImg.IndexOf(trackedImage); // 트래킹하고있는 이미지의 인덱스 

                if (_trackedImg[num].trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
                {
                    // Limited 상태이면 갱신하지않음
                }
                else
                {
                    _trackedTime[num] = 0; // Limited 상태가 아닐때 트래킹 타이머를 0으로 만듬
                }
            }

            UpdateImage(trackedImage); // 이미지 위치 갱신
        }
    }
    
    private void UpdateImage(ARTrackedImage trackedImage)
    {
        int num = _trackedImg.IndexOf(trackedImage); // 트래킹하고있는 이미지의 인덱스 
        string name = trackedImage.referenceImage.name; // 레퍼런스 이미지 라이브러리의 이름을 받아옴
        GameObject tObj = _prefabDic[name]; // 트래킹하고 있는 이미지의 이름으로 프리팹을 띄움

        // Limited 상태가 아닐때 위치 갱신, 활성화
        if (_trackedImg[num].trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
        {
            // Limited 상태이면 갱신하지 않음
        }
        else
        {
            tObj.transform.position = trackedImage.transform.position;
            tObj.transform.rotation = trackedImage.transform.rotation;
            Debug.Log($"position ({tObj.transform.position.x}, {tObj.transform.position.y})");
            tObj.SetActive(true); // 트래킹중인 이미지 위에 위치, 회전을 갱신하고 오브젝트를 띄워줌
        }
    }
}