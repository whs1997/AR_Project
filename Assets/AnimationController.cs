using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }


    public void Attack()
    {
        int rand = Random.Range(1, 4);

        switch (rand)
        {
            case 1:
                animator.Play("Swing1");
                Debug.Log("���� 1");
                break;
            case 2:
                animator.Play("Swing2");
                Debug.Log("���� 2");
                break;
            case 3:
                animator.Play("Swing2");
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
}
