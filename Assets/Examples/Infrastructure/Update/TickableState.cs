using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;

namespace Examples.Infrastructure.Update
{
    public abstract class TickableState : StateBase
    {
        protected StateTransitionInfo _stateTransitionInfo = null;
        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _stateTransitionInfo = null;
            await UniTask.WaitUntil(() => _stateTransitionInfo != null).AttachExternalCancellation(token);
            if (token.IsCancellationRequested) return null;
            return _stateTransitionInfo;
        }

        public virtual UniTask ExecuteCommands(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
        public override abstract void Tick(float deltaTime);
    }
}