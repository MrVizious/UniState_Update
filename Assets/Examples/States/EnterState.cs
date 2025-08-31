using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;

namespace Examples.States
{
    public class EnterState : StateBase
    {
        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            Debug.Log($"Entering at frame {Time.frameCount}");
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
            return Transition.GoTo<LoopState>();
        }
    }
}