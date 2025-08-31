using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;


public class ExampleTickableState : TickableState<EmptyPayload>
{
    public override async UniTask<StateTransitionInfo> AsyncExecution(CancellationToken token)
    {
        Debug.Log($"Starting AsyncExecution on {this.GetType().Name}");
        await UniTask.WaitForSeconds(3f).AttachExternalCancellation(token);
        if (token.IsCancellationRequested) return null;
        Debug.Log($"Finished AsyncExecution on {this.GetType().Name}");
        return Transition.GoToExit();
    }

    public override StateTransitionInfo EarlyTick()
    {
        Debug.Log($"EarlyTick at frame {Time.frameCount}");
        return null;
    }
    public override StateTransitionInfo Tick()
    {
        Debug.Log($"Tick at frame {Time.frameCount}");
        if (Time.frameCount > 150)
        {
            Debug.Log($"Finishing Tick at frame {Time.frameCount}");
            return Transition.GoToExit();
        }
        return null;
    }
    public override StateTransitionInfo PostLateTick()
    {
        Debug.Log($"PostLateTick at frame {Time.frameCount}");
        return null;
    }
}
