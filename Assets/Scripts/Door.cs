using Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

public class Door : MonoBehaviour, IObjectInfo
{
    private static readonly int IsClose = Animator.StringToHash("IsClose");
    private static readonly int IsOpened = Animator.StringToHash("IsOpened");
    private static readonly int IsUnavailable = Animator.StringToHash("IsUnavailable");

    public int ID
    {
        get => id;
        private set => id = value;
    }

    private Animator _animator;
    private Collider2D _collider2D;
    private AudioSource _audioSource;
    private GameObject _openDoorCollider;
    
    
    private bool _wasWideOpened = false;
    private bool _wasJammed = false;
    private bool _wasLocked = false;
    

    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private GameObject jammedIndicator;
    [SerializeField] private GameObject lockedIndicator;

    [SerializeField] private bool isJammed = false;
    [SerializeField] private bool isLocked = false;
    [SerializeField] private bool isWideOpened = false;
    [SerializeField] private bool isAvailable = true;
    [SerializeField] private int id;
    
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    
    private SpriteRenderer _spriteRenderer;
    
    public bool IsLocked => isLocked;
    public bool IsWideOpened => isWideOpened;
    public bool IsJammed => isJammed;
    public bool IsAvailable => isAvailable;
    public bool IsOpen => !isClose;
    
    [Header("For Debug")]
    [SerializeField] private bool isClose = true;
    [SerializeField] private int _objectCount = 0;
    
    private void Start()
    {
        _openDoorCollider = transform.Find("OpenCollider").gameObject;
        _animator = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        ID = GetIDFromName(gameObject.name);
    }
    
    private static int GetIDFromName(string objectName)
    {
        var numberString = string.Empty;

        for (var i = objectName.Length - 1; i >= 0; i--)
        {
            if (char.IsDigit(objectName[i]))
            {
                numberString = objectName[i] + numberString;
            }
            else
            {
                if (!string.IsNullOrEmpty(numberString))
                {
                    break;
                }
            }
        }

        if (int.TryParse(numberString, out var parsedId))
        {
            return parsedId;
        }

        Debug.LogWarning("ID не найден в имени объекта: " + objectName);
        return -1;
    }

    private void Update()
    {
        _animator.SetBool(IsUnavailable, !IsAvailable);

        if (isJammed)
        {
            jammedIndicator.SetActive(_objectCount > 0);
            lockedIndicator.SetActive(false);
            
            isWideOpened = false;
            isLocked = false;
            
            if (_wasJammed)
                return;
            
            CloseDoor();
            _wasJammed = true;
            return;
        }
        
        if (_wasJammed)
            _wasJammed = false;

        if (isLocked)
        {
            lockedIndicator.SetActive(_objectCount > 0);
            jammedIndicator.SetActive(false);
            
            isWideOpened = false;
            isJammed = false;
            
            if(_wasLocked)
                return;
            
            CloseDoor();
            _wasLocked = true;
            return;
        }
        
        if (_wasLocked)
            _wasLocked = false;

        if (isWideOpened)
        {
            isJammed = false;
            isLocked = false;
            lockedIndicator.SetActive(false);
            jammedIndicator.SetActive(false);
            
            if(_wasWideOpened)
                return;
            
            OpenDoor();
            _wasWideOpened = true;
            return;
        }

        if (_wasWideOpened)
        {
            _wasWideOpened = false;
            // if (_objectCount == 0)
            //     CloseDoor();
        }
        
        lockedIndicator.SetActive(false);
        jammedIndicator.SetActive(false);
        
        if (_objectCount > 0 && isClose) // Open
        {
            OpenDoor();
        }
        else if (_objectCount == 0 && !isClose) // Close
        {
            CloseDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((interactableLayers & (1 << other.gameObject.layer)) == 0) 
            return;
        
        _objectCount++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((interactableLayers & (1 << other.gameObject.layer)) == 0) 
            return;
        
        _objectCount--;
    }

    private void OpenDoor()
    {
        if(!isClose)
            return;
        
        _audioSource.PlayOneShot(openSound);
        _animator.SetBool(IsClose, false);
        isClose = false;
        
        _openDoorCollider.SetActive(true);
    }

    private void CloseDoor()
    {
        if (isClose)
            return;
        
        _audioSource.PlayOneShot(closeSound);
        _animator.SetBool(IsClose, true);
        isClose = true;
        
        _openDoorCollider.SetActive(false);
    }

    public void OpeningEnd()
    {
        _animator.SetBool(IsOpened, true);
        _collider2D.enabled = false;
    }

    public void ClosingEnd()
    {
        _animator.SetBool(IsOpened, false);
        _collider2D.enabled = true;
    }
    
    public void SetJammedState(bool state)
    {
        isJammed = state;
    }

    public void SetLockedState(bool state)
    {
        isLocked = state;
    }

    public void SetWideOpenedState(bool state)
    {
        isWideOpened = state;
    }

    public void SetAvailableState(bool state)
    {
        isAvailable = state;
    }

    public string GetStatusString()
    {
        var statusString = $"Door d{ID} ";
        
        if (!isAvailable)
            statusString += "is not available";
        else if (isJammed)
            statusString += $"is jammed";
        else if (isLocked)
            statusString += $"is locked";
        else if (isWideOpened)
            statusString += $"is open";
        else
            statusString += $"in sensor mode";
        
        return statusString;
    }

    public string GetInfo()
    {
        var info = string.Empty;
        info += "Object: Door ";
        info += $"ID: d{ID}\n";
        info += GetStatusString() + "\n";
        
        if (isAvailable && !IsJammed)
            info += "Supported commands: info, open, close, lock, unlock\nPattern: command ID\nExamples: lock d1, UNLOCK D12";
            
        return info;
    }
}
