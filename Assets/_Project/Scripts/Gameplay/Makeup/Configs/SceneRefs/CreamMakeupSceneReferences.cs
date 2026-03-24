using System;
using UnityEngine;

namespace _Project.Gameplay.Makeup.Configs.SceneRefs
{
    [Serializable]
    public sealed class CreamMakeupSceneReferences
    {
        [SerializeField] private GameObject _creamStandVisual;
        [SerializeField] private Collider2D _creamTapZone;
        [SerializeField] private Collider2D _faceZone;

        [Space(10)]
        [SerializeField] private Transform _creamPickupPoint;
        [SerializeField] private Transform _creamDragStartPoint;
        [SerializeField] private Transform _faceApplyPoint;

        public GameObject CreamStandVisual => _creamStandVisual;
        public Collider2D CreamTapZone => _creamTapZone;
        public Collider2D FaceZone => _faceZone;

        public Transform CreamPickupPoint => _creamPickupPoint;
        public Transform CreamDragStartPoint => _creamDragStartPoint;
        public Transform FaceApplyPoint => _faceApplyPoint;
    }
}