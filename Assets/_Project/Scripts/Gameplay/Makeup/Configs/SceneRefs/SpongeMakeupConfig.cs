using System;
using UnityEngine;

namespace _Project.Gameplay.Makeup.Configs.SceneRefs
{
    [Serializable]
    public sealed class SpongeMakeupConfig
    {
        [SerializeField] private Collider2D _spongeTapZone;

        public Collider2D SpongeTapZone => _spongeTapZone;
    }
}