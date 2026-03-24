using Unity.Profiling;
using Cysharp.Threading.Tasks;
using _Project.Gameplay.Makeup.Configs.SceneRefs;
using _Project.Gameplay.Makeup.Configs.Settings;
using _Project.Gameplay.Makeup.Data;
using _Project.Gameplay.Makeup.Input;
using _Project.Gameplay.Makeup.State;
using _Project.Gameplay.Makeup.ToolFlow;
using _Project.Gameplay.Makeup.View;

namespace _Project.Gameplay.Makeup.Interaction
{
    public sealed class MakeupTapFlow
    {
        private readonly ProfilerMarker _returnToolMarker = new("Makeup.ReturnTool");
        private readonly ProfilerMarker _spongeClearMarker = new("Makeup.SpongeClear");

        private readonly MakeupRuntimeState _runtimeState;
        private readonly MakeupPointerInput _pointerInput;
        private readonly MakeupActionSequencer _actionSequencer;
        private readonly MakeupBookView _makeupBookView;
        private readonly PlayerFaceStateView _playerFaceStateView;
        private readonly SpongeMakeupConfig _spongeConfig;
        private readonly CreamMakeupSettings _creamSettings;
        private readonly CreamMakeupSceneReferences _creamSceneReferences;
        private readonly ActiveToolReturnSequence _toolReturnSequence;
        private readonly CreamToolFlow _creamToolFlow;

        public MakeupTapFlow(
            MakeupRuntimeState runtimeState,
            MakeupPointerInput pointerInput,
            MakeupActionSequencer actionSequencer,
            MakeupBookView makeupBookView,
            PlayerFaceStateView playerFaceStateView,
            SpongeMakeupConfig spongeConfig,
            CreamMakeupSettings creamSettings,
            CreamMakeupSceneReferences creamSceneReferences,
            ActiveToolReturnSequence toolReturnSequence,
            CreamToolFlow creamToolFlow)
        {
            _runtimeState = runtimeState;
            _pointerInput = pointerInput;
            _actionSequencer = actionSequencer;
            _makeupBookView = makeupBookView;
            _playerFaceStateView = playerFaceStateView;
            _spongeConfig = spongeConfig;
            _creamSettings = creamSettings;
            _creamSceneReferences = creamSceneReferences;
            _toolReturnSequence = toolReturnSequence;
            _creamToolFlow = creamToolFlow;
        }

        public bool ProcessTapInput()
        {
            if (ProcessBookTabInput())
                return true;

            if (ProcessSpongeSelectionInput())
                return true;

            if (ProcessCreamSelectionInput())
                return true;

            return false;
        }

        private bool ProcessBookTabInput()
        {
            if (_pointerInput.TryGetBookTab(out MakeupBookTabView selectedTabView) == false)
                return false;

            if (selectedTabView.PageType == MakeupBookPageType.Blush)
            {
                if (_runtimeState.OpenedBookPageType == MakeupBookPageType.Blush)
                {
                    _makeupBookView.SelectPage(MakeupBookPageType.Blush);
                    return true;
                }

                bool isStarted = _actionSequencer.TryRun(() => OpenPageAsync(MakeupBookPageType.Blush));

                if (isStarted)
                    _makeupBookView.SelectPage(MakeupBookPageType.Blush);

                return isStarted;
            }

            if (selectedTabView.PageType == MakeupBookPageType.Lipstick)
            {
                if (_runtimeState.OpenedBookPageType == MakeupBookPageType.Lipstick)
                {
                    _makeupBookView.SelectPage(MakeupBookPageType.Lipstick);
                    return true;
                }

                bool isStarted = _actionSequencer.TryRun(() => OpenPageAsync(MakeupBookPageType.Lipstick));

                if (isStarted)
                    _makeupBookView.SelectPage(MakeupBookPageType.Lipstick);

                return isStarted;
            }

            if (selectedTabView.PageType == MakeupBookPageType.Eyeshadow)
            {
                if (_runtimeState.OpenedBookPageType == MakeupBookPageType.Eyeshadow)
                {
                    _makeupBookView.SelectPage(MakeupBookPageType.Eyeshadow);
                    return true;
                }

                bool isStarted = _actionSequencer.TryRun(() => OpenPageAsync(MakeupBookPageType.Eyeshadow));

                if (isStarted)
                    _makeupBookView.SelectPage(MakeupBookPageType.Eyeshadow);

                return isStarted;
            }

            bool isCloseStarted = _actionSequencer.TryRun(CloseCurrentPageAsync);

            if (isCloseStarted)
                _makeupBookView.SelectPage(MakeupBookPageType.None);

            return isCloseStarted;
        }

        private bool ProcessSpongeSelectionInput()
        {
            if (_spongeConfig == null || _spongeConfig.SpongeTapZone == null)
                return false;

            if (_pointerInput.ContainsColliderUnderPointer(_spongeConfig.SpongeTapZone) == false)
                return false;

            return _actionSequencer.TryRun(ClearMakeupBySpongeAsync);
        }

        private bool ProcessCreamSelectionInput()
        {
            if (_creamSettings.CanStartFromStage(_runtimeState.ProcessStageType) == false)
                return false;

            if (_pointerInput.ContainsColliderUnderPointer(_creamSceneReferences.CreamTapZone) == false)
                return false;

            return _actionSequencer.TryRun(SwitchToCreamAsync);
        }

        private async UniTask OpenPageAsync(MakeupBookPageType pageType)
        {
            _runtimeState.ProcessStageType = MakeupProcessStageType.ReturningToolBeforeSwitch;

            await ReturnActiveToolToStandAsync();

            _runtimeState.OpenedBookPageType = pageType;
            _toolReturnSequence.ApplyIdleVisualStateForOpenedPage();
            _runtimeState.ProcessStageType = _runtimeState.GetWaitingStageForOpenedPage();
        }

        private async UniTask CloseCurrentPageAsync()
        {
            _runtimeState.ProcessStageType = MakeupProcessStageType.ReturningToolBeforeSwitch;

            await ReturnActiveToolToStandAsync();

            _runtimeState.OpenedBookPageType = MakeupBookPageType.None;
            _toolReturnSequence.ApplyIdleVisualStateForOpenedPage();
            _runtimeState.ProcessStageType = MakeupProcessStageType.Idle;
        }

        private async UniTask SwitchToCreamAsync()
        {
            _runtimeState.ProcessStageType = MakeupProcessStageType.ReturningToolBeforeSwitch;

            if (_runtimeState.OpenedBookPageType != MakeupBookPageType.None)
                _makeupBookView?.SelectPage(MakeupBookPageType.None);

            _runtimeState.OpenedBookPageType = MakeupBookPageType.None;

            await ReturnActiveToolToStandAsync();
            await _creamToolFlow.StartSequenceAsync();
        }

        private async UniTask ReturnActiveToolToStandAsync()
        {
            using (_returnToolMarker.Auto())
                await _toolReturnSequence.ReturnActiveToolAsync();
        }

        private async UniTask ClearMakeupBySpongeAsync()
        {
            using (_spongeClearMarker.Auto())
            {
                _runtimeState.ProcessStageType = MakeupProcessStageType.ReturningToolBeforeSwitch;

                await ReturnActiveToolToStandAsync();

                _playerFaceStateView?.ResetFaceState();

                _runtimeState.ProcessStageType = _runtimeState.GetWaitingStageForOpenedPage();
            }
        }
    }
}