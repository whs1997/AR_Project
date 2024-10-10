using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;

    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;


    private void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Drag();
    }

    public bool IsAnimating()
    {
        // 현재 애니메이션이 작동중인지 여부, 재생중이라면 true
        // 클립의 첫 프레임이 0, 마지막 프레임이 1로, normalizedTime이 1보다 작으면 재생중
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0;        
    }

    public void Attack()
    {
        int rand = Random.Range(1, 4);

        switch (rand)
        {
            case 1:
                animator.Play("Attack1");
                Debug.Log("공격 1");
                break;
            case 2:
                animator.Play("Attack2");
                Debug.Log("공격 2");
                break;
            case 3:
                animator.Play("Attack3");
                Debug.Log("공격 3");
                break;
        }
    }

    public void Walk()
    {
        animator.Play("Walk");
        Debug.Log("걷기");
    }

    public void Jump()
    {
        animator.Play("Jump");
        Debug.Log("점프");
    }

    public void Drag()
    {
        if(Input.touchCount > 0) // 터치 입력이 생기면
        {
            Touch touch = Input.GetTouch(0); // 첫번째로 터치한 지점 touch

            Vector3 touchPos = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.nearClipPlane));
            RaycastHit hit;

            if(touch.phase == TouchPhase.Began)
            {
                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                if(Physics.Raycast(ray, out hit))
                {
                    if(hit.transform == this.transform)
                    {
                        Debug.Log("캐릭터 터치중");
                        isDragging = true;
                        offset = transform.position - mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.WorldToScreenPoint(transform.position).z));
                    }
                }
            }

            if(isDragging && ( touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary ))
            {
                Vector3 newPos = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.WorldToScreenPoint(transform.position).z));
                transform.position = newPos + offset;  // offset을 적용하여 손가락 위치에 정확히 오브젝트가 따라가도록 함
                animator.Play("Fly");
            }

            if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
                animator.Play("Idle");
            }
        }
    }
}
