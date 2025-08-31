using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;

namespace Examples.States
{
    public class LoopState : StateBase
    {

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            await EarlyTick();
            await Tick();
            await PreLateTick();

            return Transition.GoTo<LoopState>();
        }

        private async UniTask EarlyTick()
        {
            await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);
            Debug.Log($"Early tick at frame {Time.frameCount}");
        }
        private async UniTask Tick()
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
            Debug.Log($"Tick at frame {Time.frameCount}");
        }
        private async UniTask PreLateTick()
        {
            await UniTask.Yield(PlayerLoopTiming.PreLateUpdate);
            Debug.Log($"Late tick at frame {Time.frameCount}");
        }
    }
}