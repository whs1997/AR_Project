using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTracker : MonoBehaviour
{

    public ARTrackedImageManager trackedImageManager;

    public List<GameObject> _objectList = new List<GameObject>(); // ���� �ִ� ���ӿ�����Ʈ�� ������ ������Ʈ ����Ʈ 
    private Dictionary<string, GameObject> _prefabDic = new Dictionary<string, GameObject>(); // ������Ʈ�� �̸����� �޾ƿ� ��ųʸ�

    private List<ARTrackedImage> _trackedImg = new List<ARTrackedImage>(); // Ʈ��ŷ�ϰ��ִ� �̹���
    private List<float> _trackedTime = new List<float>(); // Ʈ��ŷ�ϰ� �ִ� �̹����� Ÿ�̸�

    public float timer; // �̹����� Ʈ��ŷ���� ���Ҷ� ������ Ÿ�̸�

    private void Awake()
    {
        foreach (GameObject obj in _objectList)
        {
            string tName = obj.name;

            _prefabDic.Add(tName, obj); // ��ųʸ��� ������Ʈ�� �̸��� Ű������ �޾ƿ�
            Debug.Log($"{tName} �̸��� ������Ʈ �غ� �Ϸ�");
        } 
    }

    private void Update()
    {
        // ImageChanged�� remove ����
        if (_trackedImg.Count > 0) // �̹����� Ʈ��ŷ�ϰ� �ִٸ�
        {
            List<ARTrackedImage> tNumList = new List<ARTrackedImage>(); // Ÿ�̸Ӱ� �Ѿ�� �ӽ÷� ������ ����Ʈ

            for (int i = 0; i < _trackedImg.Count; i++)
            {
                // Ʈ��ŷ ���°� Limited �̸� (�̹����� ��ã��������)
                if (_trackedImg[i].trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
                {
                    if (_trackedTime[i] > timer) // Ʈ��ŷ Ÿ�̸Ӱ� ������ Ÿ�̸Ӻ��� Ŀ���� ����
                    {
                        string name = _trackedImg[i].referenceImage.name; // Ʈ��ŷ�ϰ��ִ� �̹����� �̸��� �޾ƿ�
                        GameObject tObj = _prefabDic[name]; // ������Ʈ�� �����ؼ�
                        tObj.SetActive(false); // ������Ʈ�� ��Ȱ��ȭ
                        tNumList.Add(_trackedImg[i]); // �ӽ� ����Ʈ�� �߰�
                    }
                    else
                    {
                        _trackedTime[i] += Time.deltaTime; // Ʈ��ŷ�ϰ��ִ� �̹����� Ÿ�̸� �ð� ����
                        Debug.Log($"{_trackedImg[i]} �̹����� Ÿ�̸� _trackedTimer {_trackedTime[i]}, timer {timer}�� �Ǹ� ����,");
                    }
                }
            }
            
            if (tNumList.Count > 0)
            {
                for (int i = 0; i < tNumList.Count; i++)
                {
                    int num = _trackedImg.IndexOf(tNumList[i]); // �ӽ� ����Ʈ�� �߰��� trackedImg�� �ε��� �� �޾ƿ�
                    _trackedImg.Remove(_trackedImg[num]); // ������Ʈ�� ��Ȱ��ȭ�ϰ� trackedImg�� ����Ʈ������ ����
                    _trackedTime.Remove(_trackedTime[num]); // ������Ʈ�� ��Ȱ��ȭ�ϰ� trackedTime�� ����Ʈ������ ����
                }
            }
        }
    }
    
    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged; // ImageChanged �Լ��� ������ ���� �����Ǹ� ������
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }
    
    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added) // Ʈ��ŷ �̹��� �߰�
        {
            if (!_trackedImg.Contains(trackedImage)) // Ʈ��ŷ���� �̹����� �߰����� �ʾҴٸ�
            {
                _trackedImg.Add(trackedImage); // �̹����� �߰��ϰ�
                _trackedTime.Add(0); // Ʈ��ŷ Ÿ�̸Ӹ� 0���� �߰�����
            }
        }
        
        foreach (ARTrackedImage trackedImage in eventArgs.updated) // Ʈ��ŷ �̹��� ����
        {
            if (!_trackedImg.Contains(trackedImage))
            {
                _trackedImg.Add(trackedImage);
                _trackedTime.Add(0);
            }
            else
            {
                int num = _trackedImg.IndexOf(trackedImage); // Ʈ��ŷ�ϰ��ִ� �̹����� �ε��� 

                if (_trackedImg[num].trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
                {
                    // Limited �����̸� ������������
                }
                else
                {
                    _trackedTime[num] = 0; // Limited ���°� �ƴҶ� Ʈ��ŷ Ÿ�̸Ӹ� 0���� ����
                }
            }

            UpdateImage(trackedImage); // �̹��� ��ġ ����
        }
    }
    
    private void UpdateImage(ARTrackedImage trackedImage)
    {
        int num = _trackedImg.IndexOf(trackedImage); // Ʈ��ŷ�ϰ��ִ� �̹����� �ε��� 
        string name = trackedImage.referenceImage.name; // ���۷��� �̹��� ���̺귯���� �̸��� �޾ƿ�
        GameObject tObj = _prefabDic[name]; // Ʈ��ŷ�ϰ� �ִ� �̹����� �̸����� �������� ���

        // Limited ���°� �ƴҶ� ��ġ ����, Ȱ��ȭ
        if (_trackedImg[num].trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
        {
            // Limited �����̸� �������� ����
        }
        else
        {
            tObj.transform.position = trackedImage.transform.position;
            tObj.transform.rotation = trackedImage.transform.rotation;
            Debug.Log($"position ({tObj.transform.position.x}, {tObj.transform.position.y})");
            tObj.SetActive(true); // Ʈ��ŷ���� �̹��� ���� ��ġ, ȸ���� �����ϰ� ������Ʈ�� �����
        }
    }
}