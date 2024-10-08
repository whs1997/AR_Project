using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MultiImageTracker : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager imageManager;
    [SerializeField] GameObject Prefab1;
    [SerializeField] GameObject Prefab2;
    [SerializeField] GameObject Prefab3;
    [SerializeField] GameObject Prefab4;
    [SerializeField] GameObject Prefab5;

    [SerializeField] AnimationController animator;

    private void OnEnable()
    {
        imageManager.trackedImagesChanged += OnImageChange;
    }

    private void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnImageChange;
    }

    private void OnImageChange(ARTrackedImagesChangedEventArgs args)
    {
        // ���ο� �̹����� �����Ǿ��� ��
        foreach (ARTrackedImage trackedImage in args.added)
        {
            // �̹��� ���̺귯������ �̹����� �̸��� Ȯ��
            string imageName = trackedImage.referenceImage.name;
            GameObject character = null;

            // ���ο� ���ӿ�����Ʈ�� Ʈ��ŷ�� �̹����� �ڽ����� ����
            switch (imageName)
            {
                case "1":
                    Debug.Log("1�� ĳ���� ����");
                    character = Instantiate(Prefab1, trackedImage.transform.position, trackedImage.transform.rotation);
                    break;
                case "2":
                    character = Instantiate(Prefab2, trackedImage.transform.position, trackedImage.transform.rotation);
                    break;
                case "3":
                    character = Instantiate(Prefab3, trackedImage.transform.position, trackedImage.transform.rotation);
                    break;
                case "4":
                    character = Instantiate(Prefab4, trackedImage.transform.position, trackedImage.transform.rotation);
                    break;
                case "5":
                    character = Instantiate(Prefab5, trackedImage.transform.position, trackedImage.transform.rotation);
                    break;
            }

            if(character != null)
            {
                character.transform.parent = trackedImage.transform;
                animator = character.GetComponent<AnimationController>();
            }
        }

        // ������ �̹����� ����(�̵�, ȸ��)�Ǿ��� ��
        foreach (ARTrackedImage trackedImage in args.updated)
        {
            // �̹����� ��������� �ִ� ��� �ڽ����� �ִ� ���ӿ�����Ʈ�� ��ġ�� ȸ���� ����
            trackedImage.transform.GetChild(0).position = trackedImage.transform.position;
            trackedImage.transform.GetChild(0).rotation = trackedImage.transform.rotation;
            
            Debug.Log("ĳ���� ��ġ ����");
        }

        // ������ �̹����� ������� ��
        foreach (ARTrackedImage trackedImage in args.removed)
        {
            // �̹����� ����� ��� �ڽ����� �ִ� ���ӿ�����Ʈ ����
            Destroy(trackedImage.transform.GetChild(0).gameObject);
            Debug.Log($"{trackedImage.referenceImage.name} ���ŵ�");
            if(trackedImage.transform.childCount > 0)
            {
                GameObject character = trackedImage.transform.GetChild(0).gameObject;
                Destroy(character);                
            }
        }
    }

    public void AttackButtonPressed()
    {
        if(animator != null)
        {
            animator.Attack();
            Debug.Log("���� �ִϸ��̼�");
        }
    }

    public void WalkButtonPressed()
    {
        if (animator != null)
        {
            animator.Walk();
        }
    }

    public void JumpButtonPressed()
    {
        if (animator != null)
        {
            animator.Jump();
        }
    }
}
