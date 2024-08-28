using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible : MonoBehaviour
{
    [SerializeField] float shine_range;
    [SerializeField] float interact_range;

    Animator animator;
    [SerializeField] Animator interact_panel;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, shine_range);
        foreach (Collider2D col in cols)
        {
            if (col == PlayerController.instance.body_collider)
            {
                InRange();
                animator?.SetBool("InRange", true);
                return;
            }
        }
        animator?.SetBool("InRange", false);
    }

    private void InRange()
    {
        if (Vector2.Distance(PlayerController.instance.transform.position, transform.position) < interact_range)
        {
            //Code for displaying the interact message and reading interaction itself go here!
            interact_panel?.SetBool("InRange", true);
        }
        else
            interact_panel?.SetBool("InRange", false);
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.DrawWireSphere(transform.position, shine_range);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (transform.position, interact_range);
    }
}
