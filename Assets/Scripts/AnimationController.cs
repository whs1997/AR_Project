using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] ImageTracker imageTracker;
    [SerializeField] SpriteRenderer render;

    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;

    private int idleHash = Animator.StringToHash("Idle");
    private bool isJumping;

    private void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Drag();
        Flip();
    }
    
    public bool IsAnimating()
    {
        // 클립의 첫 프레임이 0, 마지막 프레임이 1로, normalizedTime이 1보다 작으면 재생중
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 애니메이션의 재생 상태      
        // idle 애니메이션이 아니고, 다른 애니메이션을 재생중이면 true 반환
        return stateInfo.shortNameHash != idleHash && stateInfo.normalizedTime < 1;
    }
    
    public void Attack()
    {  
        int rand = Random.Range(1, 4);
        switch (rand) // 세가지 공격 애니메이션중 하나를 출력
        {
            case 1:
                animator.Play("Attack1");
                break;
            case 2:
                animator.Play("Attack2");
                break;
            case 3:
                animator.Play("Attack3");
                break;
        }
    }

    public void Walk()
    {
        animator.Play("Walk"); // 걷는 동작
    }

    public void Jump(Transform prefabTransform)
    {
        if (isJumping)
            return;

        isJumping = true;
        StartCoroutine(JumpRoutine(prefabTransform)); // 점프 코루틴
    }

    private IEnumerator JumpRoutine(Transform prefabTransform)
    {
        float jumpTime = 0.25f; // 점프할 시간
        float elapsed = 0; // 경과한 시간

        Vector3 originPos = prefabTransform.position; // 기존 위치 originPos

        animator.Play("Jump"); // Jump 애니메이션 재생

        while (elapsed < jumpTime) // 0.25초간 위로 올라가는 동작
        {
            float yOffset = Mathf.Lerp(0, 0.1f, elapsed / jumpTime); // y로 0.1만큼 서서히 이동시킴
            prefabTransform.position = new Vector3(originPos.x, originPos.y + yOffset, originPos.z); // 트래킹중인 오브젝트를 현재 위치의 y로 0.1만큼 이동
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0; // 경과 시간 0으로 초기화 후 내려오는 동작

        while (elapsed < jumpTime)
        {
            float yOffset = Mathf.Lerp(0.1f, 0, elapsed / jumpTime); // y로 0.1만큼 서서히 이동시킴
            prefabTransform.position = new Vector3(originPos.x, originPos.y + yOffset, originPos.z); // 트래킹중인 오브젝트를 원래 위치로 이동
            elapsed += Time.deltaTime;
            yield return null;
        }
        isJumping = false; // 점프 상태 종료
        imageTracker.JumpEnd(); // ImageTracker의 점프 상태 종료
    }

    public void Drag() // 캐릭터를 드래그
    {
        if (Input.touchCount > 0) // 터치 입력이 생기면
        {
            Touch touch = Input.GetTouch(0); // 첫번째로 터치한 지점 touch
            RaycastHit hit;

            if (touch.phase == TouchPhase.Began) // 터치를 시작하면 
            {
                Ray ray = mainCamera.ScreenPointToRay(touch.position); // 터치한 지점에 Ray 검사

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.CompareTag("Character")) // Character 태그의 오브젝트와 ray 충돌하면
                    {
                        Debug.Log("캐릭터 터치중");
                        isDragging = true;
                        offset = hit.transform.position - hit.point; // 터치한 위치와 오브젝트의 월드 좌표와의 거리 offset
                    }
                }
            }

            if (isDragging && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)) // 터치중일때 이동하면
            {
                Ray ray = mainCamera.ScreenPointToRay(touch.position); // 터치하고 있는곳에 Ray 검사

                if (Physics.Raycast(ray, out hit))
                {
                    transform.position = hit.point + offset; // 터치한곳 + offset 위치로 이동시킴
                    animator.Play("Fly");
                }
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) // 터치가 끝나면
            {
                isDragging = false; // 드래그 종료
                animator.Play("Idle");
            }
        }
    }

    private void Flip() // 화면 좌우를 누르면 캐릭터가 해당 방향을 보게함
    {
        if(Input.touchCount > 0) // 터치 입력이 생기면
        {
            Touch touch = Input.GetTouch(0); // 터치한 지점 touch

            if(touch.phase == TouchPhase.Began) // 터치하면
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return; // 터치한곳에 UI 등 오브젝트가 있으면 무시
                }

                if(touch.position.x < Screen.width / 2) // 화면 절반의 왼쪽을 누르면
                {
                    render.flipX = false; 
                }
                else
                {
                    render.flipX = true; // 화면 절반의 오른쪽을 누르면 좌우반전
                }
            }
        }
    }
}
