using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformColliderFitter : MonoBehaviour
{

    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
    }


    BoxCollider2D _collider;
    SpriteRenderer _sprite;

    void Update()
    {
        _collider.offset = new Vector2(0, 0);
        _collider.size = new Vector2(_sprite.size.x / transform.lossyScale.x,
                                     _sprite.size.y / transform.lossyScale.y);
    }
}
