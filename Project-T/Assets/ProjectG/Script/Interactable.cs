using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Script
{
    public abstract class Interactable : MonoBehaviour
    {
        [Header("Interactable setting")]
        List<(string action_name,Action<Player> action)> interactions = new List<(string,Action<Player>)>();
        SpriteRenderer sprite_renderer;
        

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
        [SerializeField] private float animLength = 0.5f;
        public List<(string action_name,Action<Player> action)> GetList()
        {
            return interactions;
        }

        protected void ChangeSpriteWithAnimation(Sprite sprite)
        {
            sprite_renderer.sprite = sprite;
            var oldScale = transform.localScale;
            transform.DOScale(oldScale * 1.25f, 0f);
            transform.DOScale(oldScale , animLength);
        }
        protected virtual void Start()
        {
            sprite_renderer = GetComponent<SpriteRenderer>();
        }
    }
}