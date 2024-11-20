using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Interpreter : MonoBehaviour
{
    private readonly string[] _validDoorCommands = { "open", "close", "lock", "unlock", "info" };
    private List<string> _response = new List<string>();

    public List<string> Interpret(string input)
    {
        _response.Clear();
        var inputLines = input.Split(" ");

        if (IsValidDoorCommand(inputLines[0], out var doorCommandToken))
        {
            if (!IsDoorCommandLineCorrect(inputLines.Length))
                return _response;

            if (inputLines[1] != null && !IsValidDoorString(inputLines[1]))
            {
                _response.Add("Door id is missing or incorrect");
                return _response;
            }
            
            var doorNumber = ExtractNumber(inputLines[1]);
                
            if (!IsDoorInSystem(doorNumber))
                return _response;

            switch (doorCommandToken)
            {
                case 0:
                    DoorManager.Instance.SetWideOpenedStateById(doorNumber, true);
                    break;
                case 1:
                    DoorManager.Instance.SetWideOpenedStateById(doorNumber, false);
                    break;
                case 2:
                    DoorManager.Instance.SetLockedStateById(doorNumber,true);
                    break;
                case 3:
                    DoorManager.Instance.SetLockedStateById(doorNumber,false);
                    break;
                case 4:
                    break;
            }
            
            _response.Add(DoorManager.Instance.GetDoorStatusStringById(doorNumber));
        }
        else
        {
            _response.Add("Command not recognized");
        }

        return _response;
    }

    private bool IsDoorInSystem(int doorNumber)
    {
        _response.Add(DoorManager.Instance.CheckDoorStatusById(doorNumber, status: out var isDoorInSystem));
        return isDoorInSystem;
    }

    private static bool IsValidDoorString(string input)
    {
        return Regex.IsMatch(input, @"^[dD]\d+$");
    }
    
    private static int ExtractNumber(string input)
    {
        var match = Regex.Match(input, @"^[dD](\d+)$");
        return int.Parse(match.Groups[1].Value);
    }

    private bool IsDoorCommandLineCorrect(int commandLength)
    {
        if (commandLength == 2) 
            return true;
  
        _response.Add("Command line has incorrect length");
        return false;
    }
    
    private bool IsValidDoorCommand(string command, out int commandToken)
    {
        commandToken = Array.FindIndex(_validDoorCommands, validCommand => 
            string.Equals(command, validCommand, StringComparison.OrdinalIgnoreCase));

        return commandToken != -1; 
    }
}
