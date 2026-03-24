using _Project.Gameplay.Makeup.Configs.SceneRefs;

namespace _Project.Gameplay.Makeup.Hand
{
    public sealed class HandDefaultPosePlacement
    {
        private readonly MakeupHandConfig _handConfig;

        public HandDefaultPosePlacement(MakeupHandConfig handConfig)
        {
            _handConfig = handConfig;
        }

        public void MoveHandToDefaultPointImmediately()
        {
            if (_handConfig.HandRoot == null || _handConfig.HandDefaultPoint == null)
                return;

            if (_handConfig.HandRoot.position == _handConfig.HandDefaultPoint.position)
                return;

            _handConfig.HandRoot.position = _handConfig.HandDefaultPoint.position;
        }
    }
}