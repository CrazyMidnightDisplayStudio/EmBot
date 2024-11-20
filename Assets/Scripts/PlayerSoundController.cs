using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip turnSound;
    
    private AudioSource _source;
    private CharacterController2D _controller;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        _controller = GetComponent<CharacterController2D>();
    }

    public void OnMovePlaySound()
    {
        if (_controller.IsTurning())
        {
            _source.PlayOneShot(turnSound);
        }
        
        if (_controller.IsMoving())
        {
            _source.PlayOneShot(moveSound);
        }
    }


        
}
