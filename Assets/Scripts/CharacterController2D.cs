using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsDead = Animator.StringToHash("IsDead");

    [SerializeField] private float rotationSpeed = 150f;

    private Animator _animator;
    private Rigidbody2D _rb;
    private float _turnInputValue;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 direction, float moveSpeed)
    {
        _rb.velocity = moveSpeed * Time.fixedDeltaTime * direction;
    }
    
    public void Rotate(float turnInput)
    {
        if (turnInput != 0)
        {
            var turnAmount = turnInput * rotationSpeed * Time.deltaTime;
            _rb.MoveRotation(_rb.rotation - turnAmount);
        }
        _turnInputValue = turnInput;
    }
    
    public void SetAnimations()
    {
        var speed = Mathf.Abs(_rb.velocity.magnitude) > 0.01f ? _rb.velocity.magnitude : _turnInputValue;
        
        _animator.SetFloat(Speed, Mathf.Abs(speed));
    }

    public void DeadAnimation()
    {
        _animator.SetBool(IsDead, true);
    }

    public void Stop()
    {
        _rb.velocity = Vector2.zero;
    }

    public bool IsTurning() => Mathf.Abs(_turnInputValue) > 0.01f;
    public bool IsMoving() => Mathf.Abs(_rb.velocity.magnitude) > 0.01f;
}