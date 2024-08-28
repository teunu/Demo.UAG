using Cinemachine;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    PlayerController controller;
    Rigidbody2D rb;
    
    SpriteRenderer renderer;
    Animator animator;
    [SerializeField] CinemachineImpulseSource impulse;

    private void Awake()
    {
        renderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        controller = GetComponentInParent<PlayerController>();
        rb = GetComponentInParent<Rigidbody2D>();

        controller.OnHurt += GetHurt;

        //Make sure the impulse manager ignores time scale
        CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
    }

    private void OnDestroy()
    {
        controller.OnHurt -= GetHurt;
    }

    private void Update()
    {
        if (hurt) { return; }

        if (rb.velocity.x < -0.1) { transform.localScale = new Vector2(-1, 1); }
        else if (rb.velocity.x > 0.1) { transform.localScale = new Vector2(1, 1); }

        animator.SetBool("freefall", controller.freefall);
        animator.SetBool("grounded", controller.grounded);
        animator.SetBool("crouching", controller.state == PlayerController.LocomotionStates.crouch);
        animator.SetFloat("horizontal", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("vertical", rb.velocity.y);
    }

    void GetHurt() { TriggerHurt(); }

    bool hurt;
    private async UniTask TriggerHurt()
    {
        impulse.GenerateImpulse(new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f)));
        animator.SetTrigger("hurt");

        animator.SetFloat("vertical", 0);   //Hurt conflicts with the falling animation in some cases. Set the vertical to 0 for this frame.

        #region Hurt Animation Suspension
        hurt = true;

        await UniTask.DelayFrame(10);

        hurt = false;
        #endregion
    }
}
