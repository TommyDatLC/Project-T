using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public abstract class Interactable : MonoBehaviour
    {
        List<(string action_name,Action<Player> action)> interactions = new List<(string,Action<Player>)>();
        
        public void Interact(int actionID,Player p)
        {
            interactions[actionID].action?.Invoke(p);
        }
        protected void AddInteraction(string action_name,Action<Player> action)
        {
            interactions.Add((action_name,action));
        }
        protected void EditInteraction(int id,string action_name,Action<Player> action)
        {
            interactions[id] = ((action_name,action));
        }

        public List<(string action_name,Action<Player> action)> GetList()
        {
            return interactions;
        }
        
    }
}