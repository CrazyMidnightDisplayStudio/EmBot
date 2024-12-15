using System;
using System.Collections.Generic;
using ActionMenu;
using GlobalUtilities;
using Interfaces;
using Pathfinding;
using UnityEngine;

public class HumanController : MonoBehaviour, IObjectInfo, IRadarTarget, IInteractAction
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
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;

    [Header("States")]
    [SerializeField] private int relaxFrequency = 2;
    private float _timer;

    [SerializeField, Range(1f, 2f)] private float stressLevel = 1.5f;
    [SerializeField, Range(0f, 1f)] private float healthLevel = 0.8f;

    private bool _isInSafePlace = true;
    private Dictionary<string, Action> _actions = new();
    private Transform _pointMove;

    public enum MovementStyle
    {
        Stealth,
        Walk,
        Run
    }
    
    [SerializeField] private MovementStyle currentMovementStyle = MovementStyle.Walk;
    
    public SpriteRenderer GetSpriteRenderer() => _spriteRenderer;
    
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _seeker = GetComponent<Seeker>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
        _spriteRenderer.enabled = false; // as IRadarTarget
        InitActions();
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

    private void UpdatePath()
    {
        if (!target)
            return;

        if (_seeker.IsDone())
            _seeker.StartPath(_rigidbody2D.position, target.position, OnPathComplete);
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

        var direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rigidbody2D.position).normalized;
        var force = direction * (GetMovementSpeed() * stressLevel * speed * Time.deltaTime);

        _rigidbody2D.AddForce(force);

        var distance = Vector2.Distance(_rigidbody2D.position, _path.vectorPath[_currentWaypoint]);

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

    private void InitActions() 
    {
        _actions.Add("Stealth", Stealth);
        _actions.Add("Walk", Walk);
        _actions.Add("Run", Run);
    }

    public void SetWaypoint(Transform targetTransform)
    {
        _pointMove = targetTransform;
    }

    private void Stealth() => Move(MovementStyle.Stealth);
    
    private void Walk() => Move(MovementStyle.Walk);
    
    private void Run() => Move(MovementStyle.Run);
    
    private void Move(MovementStyle movementStyle)
    {
        CurrentMovementStyle = movementStyle;
        SetTarget(_pointMove);
    }
    
    public Dictionary<string, Action> GetActions()
    {
        return _actions;
    }
}
