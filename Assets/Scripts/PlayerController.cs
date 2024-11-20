using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
    
    private CharacterController2D _characterController;

    private bool _isInputOn = true;

    private Vector2 _moveDirection;
    private float _turnInput;
    private bool _isAlive = true;

    private void Start()
    {
        _characterController = GetComponent<CharacterController2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            _isAlive = false;
            SetInput(false);
            _characterController.DeadAnimation();
        }
    }

    private void Update()
    {
        if (_isInputOn)
            HandleInput();
    }

    private void FixedUpdate()
    {
        _characterController.Rotate(_turnInput);
        _characterController.Move(_moveDirection, speed);
        
        if(!_isInputOn)
            _characterController.Stop();
        
        _characterController.SetAnimations();
    }

    private void HandleInput()
    {
        var moveInput = -Input.GetAxis("Vertical");

        _turnInput = Input.GetAxis("Horizontal");
        _moveDirection = transform.up * moveInput;
    }

    public void SetInput(bool state)
    {
        _isInputOn = state;
    }
    
    public bool IsAlive() => _isAlive;
}
