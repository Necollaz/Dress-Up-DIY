using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace _Project.Gameplay.Makeup.Interaction
{
    public sealed class MakeupActionSequencer
    {
        private bool _isRunning;

        public bool TryRun(Func<UniTask> action)
        {
            if (_isRunning || action == null)
                return false;

            RunAsync(action).Forget(Debug.LogException);

            return true;
        }

        private async UniTask RunAsync(Func<UniTask> action)
        {
            _isRunning = true;

            try
            {
                await action.Invoke();
            }
            finally
            {
                _isRunning = false;
            }
        }
    }
}