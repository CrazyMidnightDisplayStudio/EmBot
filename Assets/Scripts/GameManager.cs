using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    
    [Header("Cursor")]
    [SerializeField] private Sprite cursorSprite;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Terminal terminal;
    private GameObject _cursorObject;
    
    [Header("Game Feel")]
    [SerializeField] private GameObject videoGlitches;

    private void Start()
    {
        var meshRenderer = videoGlitches.GetComponent<MeshRenderer>();
        meshRenderer.enabled = true;
        meshRenderer.sortingOrder = 130;
        Cursor.visible = false;
        CreateCursorObject();
    }

    private void Update()
    {
        DrawCursor();

        if (!player.IsAlive())
            return;
        
        player.SetInput(!terminal.IsFocused());
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!terminal.IsFocused())
                terminal.FocusInputField();
            else
                terminal.UnfocusInputField();
        }
    }

    private void CreateCursorObject()
    {
        _cursorObject = new GameObject("Cursor")
        {
            transform =
            {
                localPosition = Vector3.zero
            }
        };
        _cursorObject.transform.SetParent(transform);
        _cursorObject.layer = LayerMask.NameToLayer("UI");
        var spriteRenderer = _cursorObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = cursorSprite;
        spriteRenderer.sortingOrder = 102;
    }

    private void DrawCursor()
    {
        Vector2 cursorPos = uiCamera.ScreenToWorldPoint(Input.mousePosition);

        _cursorObject.transform.position = cursorPos;
    }
}