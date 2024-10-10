using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] AnimationController animationController;
    [SerializeField] Button attackButton;
    [SerializeField] Button walkButton;
    [SerializeField] Button jumpButton;

    private void Start()
    {
        attackButton.onClick.AddListener(AttackButton);
        walkButton.onClick.AddListener(WalkButton);
        jumpButton.onClick.AddListener(JumpButton);
    }

    private void AttackButton()
    {
        if (animationController.IsAnimating()) { }
        // 애니메이션 진행 중이면 작업 하지않음
        else
            animationController.Attack();
    }

    private void WalkButton()
    {
        if (animationController.IsAnimating()) { }
        // 애니메이션 진행 중이면 작업 하지않음
        else
            animationController.Walk();
    }

    private void JumpButton()
    {
        if (animationController.IsAnimating()) { }
        // 애니메이션 진행 중이면 작업 하지않음
        else
            animationController.Jump();
    }
}

