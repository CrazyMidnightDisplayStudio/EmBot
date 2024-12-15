using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActionMenu
{
    public interface IInteractAction
    {
        private void InitActions(){}
        
        public void SetWaypoint(Transform wayPoint);

        public Dictionary<string, Action> GetActions();
        
    }
}
