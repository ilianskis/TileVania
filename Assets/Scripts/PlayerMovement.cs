using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    [SerializeField] private float speed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float climbSpeed;
    private CapsuleCollider2D _capsule;
    private float _gravityScaleOnStart;

    private Animator _ar;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _ar = GetComponent<Animator>();
        _capsule = GetComponent<CapsuleCollider2D>();
        _gravityScaleOnStart = _rb.gravityScale;
    }
    void Update()
    {
        Run();
        FlipSprite();
        ClimbLadder();
    }

    private void ClimbLadder()
    {
        if (!_capsule.IsTouchingLayers(LayerMask.GetMask("Climb")))
        {
            _ar.SetBool("isClimbing", false);
            _rb.gravityScale = _gravityScaleOnStart;
            return;
        }
        bool isMovingVertically = Mathf.Abs(_rb.velocity.y) > Mathf.Epsilon;
        _ar.SetBool("isClimbing",  isMovingVertically);
        
        _rb.gravityScale = 0;
        Vector2 climbVelocity = new Vector2(_rb.velocity.x, _moveInput.y * climbSpeed);
        _rb.velocity = climbVelocity;
    }

    private void FlipSprite()
    {
        bool isMovingHorizontally = Mathf.Abs(_rb.velocity.x) > Mathf.Epsilon;
        
        if(isMovingHorizontally) transform.localScale = new Vector2(Mathf.Sign(_rb.velocity.x), 1f);
        else _ar.SetBool("isRunning", false);
    }

    private void Run()
    {
        Vector2 playerVelocity = new Vector2(_moveInput.x * speed, _rb.velocity.y);
        _rb.velocity = playerVelocity;
        
        _ar.SetBool("isRunning", true);
    }

    void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }
    
    void OnJump(InputValue value)
    {
        if (!_capsule.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;
        
        if (value.isPressed) _rb.velocity += new Vector2(0f, jumpSpeed);
    }
}
