using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantWiggle : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float amplitude;

    float z_rotation;

    private void Awake()
    {
        z_rotation = transform.rotation.z;
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D col in cols) { 
            if (col == PlayerController.instance.body_collider) 
            {
                float distance = transform.position.x - col.transform.position.x; 
                transform.rotation = Quaternion.Euler(new Vector3(
                    0,
                    0,
                    z_rotation + (distance * amplitude)
                    ));
            } 
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
