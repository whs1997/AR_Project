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
        // ���� �ִϸ��̼��� �۵������� ����, ������̶�� true
        // Ŭ���� ù �������� 0, ������ �������� 1��, normalizedTime�� 1���� ������ �����
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0;        
    }

    public void Attack()
    {
        int rand = Random.Range(1, 4);

        switch (rand)
        {
            case 1:
                animator.Play("Attack1");
                Debug.Log("���� 1");
                break;
            case 2:
                animator.Play("Attack2");
                Debug.Log("���� 2");
                break;
            case 3:
                animator.Play("Attack3");
                Debug.Log("���� 3");
                break;
        }
    }

    public void Walk()
    {
        animator.Play("Walk");
        Debug.Log("�ȱ�");
    }

    public void Jump()
    {
        animator.Play("Jump");
        Debug.Log("����");
    }

    public void Drag()
    {
        if(Input.touchCount > 0) // ��ġ �Է��� �����
        {
            Touch touch = Input.GetTouch(0); // ù��°�� ��ġ�� ���� touch

            Vector3 touchPos = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.nearClipPlane));
            RaycastHit hit;

            if(touch.phase == TouchPhase.Began)
            {
                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                if(Physics.Raycast(ray, out hit))
                {
                    if(hit.transform == this.transform)
                    {
                        Debug.Log("ĳ���� ��ġ��");
                        isDragging = true;
                        offset = transform.position - mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.WorldToScreenPoint(transform.position).z));
                    }
                }
            }

            if(isDragging && ( touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary ))
            {
                Vector3 newPos = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.WorldToScreenPoint(transform.position).z));
                transform.position = newPos + offset;  // offset�� �����Ͽ� �հ��� ��ġ�� ��Ȯ�� ������Ʈ�� ���󰡵��� ��
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
