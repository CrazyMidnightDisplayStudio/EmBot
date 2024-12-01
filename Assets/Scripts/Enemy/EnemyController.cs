using System;
using Interfaces;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour, IRadarTarget
    {
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.enabled = false; // As Radar Target
        }

        public SpriteRenderer GetSpriteRenderer()
        {
            return _spriteRenderer;
        }
    }
}
