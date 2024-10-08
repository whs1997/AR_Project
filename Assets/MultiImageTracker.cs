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
        // 새로운 이미지가 추적되었을 때
        foreach (ARTrackedImage trackedImage in args.added)
        {
            // 이미지 라이브러리에서 이미지의 이름을 확인
            string imageName = trackedImage.referenceImage.name;
            GameObject character = null;

            // 새로운 게임오브젝트를 트래킹한 이미지의 자식으로 생성
            switch (imageName)
            {
                case "1":
                    Debug.Log("1번 캐릭터 생성");
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

        // 기존의 이미지가 변경(이동, 회전)되었을 때
        foreach (ARTrackedImage trackedImage in args.updated)
        {
            // 이미지에 변경사항이 있는 경우 자식으로 있던 게임오브젝트의 위치와 회전을 갱신
            trackedImage.transform.GetChild(0).position = trackedImage.transform.position;
            trackedImage.transform.GetChild(0).rotation = trackedImage.transform.rotation;
            
            Debug.Log("캐릭터 위치 변경");
        }

        // 기존의 이미지가 사라졌을 때
        foreach (ARTrackedImage trackedImage in args.removed)
        {
            // 이미지가 사라진 경우 자식으로 있던 게임오브젝트 삭제
            Destroy(trackedImage.transform.GetChild(0).gameObject);
            Debug.Log($"{trackedImage.referenceImage.name} 제거됨");
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
            Debug.Log("공격 애니메이션");
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
