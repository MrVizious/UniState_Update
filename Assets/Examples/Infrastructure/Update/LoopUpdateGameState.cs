using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;

namespace Examples.Infrastructure.Update
{
    public class LoopUpdateGameState : TickableState
    {
        private float elapsedTime = 0f;

        public override async UniTask ExecuteCommands(CancellationToken token)
        {
            elapsedTime = 0f;
        }

        public override void Tick(float deltaTime)
        {
            elapsedTime += deltaTime;
            Debug.Log($"Elapsed time: {elapsedTime}");
            if (elapsedTime > 3f) _stateTransitionInfo = Transition.GoToExit();
        }
    }
}