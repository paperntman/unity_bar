using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : LivingEntity
{
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private BoxCollider2D _boxCollider2D;

    public float movespeed;
    public float jumpPower;
    bool inputJump = false;

    private static readonly int Moving = Animator.StringToHash("Moving");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Jumping = Animator.StringToHash("jumping");
    private Collider2D _childCollider;

    // Start is called before the first frame update
    void Start()
    {
        _childCollider = transform.GetChild().GetComponent<Collider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var raycastHit = Physics2D.BoxCast(_boxCollider2D.bounds.center, _boxCollider2D.bounds.size, 0f, Vector2.down, 0.02f, LayerMask.GetMask($"Ground"));
        _animator.SetBool(Jumping, raycastHit.collider is null);

        if (Input.GetKeyDown(KeyCode.Space) && !_animator.GetBool(Jumping))
        {
            inputJump = true;
        }

        if (_animator.GetBool(Jumping)) return;
        if(Input.GetKey(KeyCode.Z))
            _animator.SetBool(Attack, true);

    }

    void FixedUpdate()
    {
        if (inputJump && !_childCollider.enabled)
        {
            inputJump = false;
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x , _rigidbody2D.velocity.y+jumpPower);
        }

        if (_animator.GetBool(Attack))
        {
            _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
            return;
        }
        var axisRaw = Input.GetAxisRaw("Horizontal");
        _rigidbody2D.velocity = new Vector2(axisRaw * movespeed, _rigidbody2D.velocity.y);
        var transformLocalScale = transform.localScale;
        transform.localScale = new Vector3(axisRaw.Equals(0f) ? transformLocalScale.x : axisRaw * Math.Abs(transformLocalScale.x), transformLocalScale.y, transformLocalScale.z);
        _animator.SetBool(Moving, !axisRaw.Equals(0f));
    }

    public void AttackTrue()
    {
        transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
    }

    public void AttackFalse()
    {
        transform.GetChild(0).GetComponent<Collider2D>().enabled = false;
        _animator.SetBool(Attack, false);
    }
}
