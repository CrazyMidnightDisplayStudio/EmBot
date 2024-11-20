using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

public class DoorManager : MonoBehaviour
{
    public static DoorManager Instance { get; set; }
    
    private readonly List<Door> _doors = new List<Door>();

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _doors.AddRange(FindObjectsOfType<Door>());
    }

    private Door FindDoorById(int id)
    {
        foreach (var door in _doors.Where(door => door.ID == id))
        {
            return door;
        }
        
        return null;
    }

    public void SetWideOpenedStateById(int id, bool state)
    {
        var door = FindDoorById(id);
        if (door != null)
        {
            door.SetWideOpenedState(state);
        }
    }

    public void SetJammedStateById(int id, bool state)
    {
        var door = FindDoorById(id);
        if (door != null)
        {
            door.SetJammedState(state);
        }
    }

    public void SetLockedStateById(int id, bool state)
    {
        var door = FindDoorById(id);
        if (door != null)
        {
            door.SetLockedState(state);
        }
    }

    public string GetDoorStatusStringById(int id)
    {
        var door = FindDoorById(id);
        return door == null ? "Door not found" : door.GetStatusString();
    }

    public string CheckDoorStatusById(int id, out bool status)
    {
        var door = FindDoorById(id);
        status = door != null;

        return door == null ? $"Door {id} not found" : $"Door {id} is in the system array";
    }
}