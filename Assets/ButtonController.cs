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
        animationController.Attack();
    }

    private void WalkButton()
    {
        animationController.Walk();
    }

    private void JumpButton()
    {
        animationController.Jump();
    }
}

