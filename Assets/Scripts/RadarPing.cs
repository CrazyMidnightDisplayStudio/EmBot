
using UnityEngine;

public class RadarPing : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
   
    private float _disappearTimer = 0f;
    [SerializeField] private float disappearTime = 2f;
    
    private SpriteRenderer _targetSpriteRenderer;
    private Color _color;

    public void Initialize(SpriteRenderer targetSpriteRenderer)
    {
        _targetSpriteRenderer = targetSpriteRenderer;
    }

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _color = new Color(1, 1, 1, 1f);
    }

    private void Update()
    {
        _disappearTimer += Time.deltaTime;

        _spriteRenderer.sprite = _targetSpriteRenderer.sprite;
        _color.a = Mathf.Lerp(disappearTime, 0f, _disappearTimer / disappearTime);
        _spriteRenderer.color = _color;
        
        if (_disappearTimer >= disappearTime)
            Destroy(gameObject);
    }

    public void SetDisappearTime(float newDisappearTime)
    {
        disappearTime = newDisappearTime;
    }
    
}
