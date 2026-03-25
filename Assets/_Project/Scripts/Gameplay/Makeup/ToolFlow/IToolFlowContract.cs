using UnityEngine;
using Cysharp.Threading.Tasks;
using _Project.Gameplay.Makeup.Data;

namespace _Project.Gameplay.Makeup.ToolFlow
{
    public interface IToolFlowContract
    {
        public Collider2D FaceZone { get; }
        public MakeupProcessStageType DragFallbackStageType { get; }

        public UniTask ApplyAsync();
    }
}