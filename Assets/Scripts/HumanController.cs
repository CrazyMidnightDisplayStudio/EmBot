using GlobalUtilities;
using Interfaces;
using Pathfinding;
using UnityEngine;

public class HumanController : MonoBehaviour, IObjectInfo
{
    [Header("Parameters")]
    [SerializeField] private string realName;
    [SerializeField] private string profession;
    
    [Header("AI Behaviour")]
    [SerializeField] Transform target = null;
    [SerializeField] float speed = 200f;
    [SerializeField] float nextWaypointDistance = 1f;

    private Path _path;
    private int _currentWaypoint = 0;
    private Seeker _seeker;
    private Rigidbody2D _rb;

    [Header("States")]
    [SerializeField] private int relaxFrequency = 2;
    private float _timer;

    [SerializeField, Range(1f, 2f)] private float stressLevel = 1.5f;
    [SerializeField, Range(0f, 1f)] private float healthLevel = 0.8f;

    private bool _isInSafePlace = true;

    public enum MovementStyle
    {
        Stealth,
        Walk,
        Run
    }
    
    [SerializeField] private MovementStyle currentMovementStyle = MovementStyle.Walk;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _seeker = GetComponent<Seeker>();
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }
    
    private float GetMovementSpeed()
    {
        return currentMovementStyle switch
        {
            MovementStyle.Stealth => 0.5f,
            MovementStyle.Walk => 1f,
            MovementStyle.Run => 2f,
            _ => 1f
        };
    }
    
    void UpdatePath()
    {
        if (!target)
            return;

        if (_seeker.IsDone())
            _seeker.StartPath(_rb.position, target.position, OnPathComplete);
    }
    
    void FixedUpdate()
    {
        MoveNPC();

        if (_isInSafePlace)
        {
            _timer += Time.deltaTime;
            if (_timer >= 1f / relaxFrequency)
            {
                Relax(0.01f);
                _timer = 0f;
            }
        }
    }
    
    private void MoveNPC()
    {
        if (_path == null)
            return;

        if (_currentWaypoint >= _path.vectorPath.Count)
            return;

        var direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rb.position).normalized;
        var force = direction * (GetMovementSpeed() * stressLevel * speed * Time.deltaTime);

        _rb.AddForce(force);

        var distance = Vector2.Distance(_rb.position, _path.vectorPath[_currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            _currentWaypoint++;
        }
    }
    
    private void OnPathComplete(Path p)
    {
        if (target.CompareTag("Waypoint"))
        {
            Destroy(target.gameObject);
            target = null;
        }

        if (!p.error)
        {
            _path = p;
            _currentWaypoint = 0;
        }
    }

    public void Scare(float terrorAmount)
    {
        stressLevel += terrorAmount;
        if (stressLevel > 2)
        {
            stressLevel = 2;
        }
    }

    public void Relax(float relaxAmount)
    {
        stressLevel -= relaxAmount;
        if (stressLevel < 1)
        {
            stressLevel = 1;
        }
    }

    public MovementStyle CurrentMovementStyle
    {
        get => currentMovementStyle;
        set => currentMovementStyle = value;
    }
    
    public void SetTarget(Transform newTarget) => target = newTarget;

    public void SetSafeState(bool isSafeNow) => _isInSafePlace = isSafeNow;
    
    public string GetInfo()
    {
        string info = $"Human\nName: {realName}\nSpeciality: {profession}";

        info += $"\nStress: {Bar.Create(stressLevel - 1)}";
        info += $"\nHealth: {Bar.Create(healthLevel)}";
        
        return info;
    }
}
