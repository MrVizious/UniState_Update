using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;


public abstract class TickableCompositeState : TickableState<EmptyPayload>
{
}
public abstract class TickableState<TPayload> : StateBase<TPayload>
{
    private bool IsAsyncExecutionOverridden()
    {
        var method = GetType().GetMethod(
            nameof(AsyncExecution),
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.DeclaredOnly
        );
        return method != null;
    }

    private bool IsAnyTickMethodOverridden()
    {
        var flags = System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.DeclaredOnly;

        return GetType().GetMethod(nameof(EarlyTick), flags) != null ||
               GetType().GetMethod(nameof(Tick), flags) != null ||
               GetType().GetMethod(nameof(PreLateTick), flags) != null ||
               GetType().GetMethod(nameof(PostLateTick), flags) != null;
    }

    public sealed override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        bool hasAsyncExecution = IsAsyncExecutionOverridden();
        bool hasTickMethods = IsAnyTickMethodOverridden();

        if (!hasAsyncExecution && !hasTickMethods)
        {
            throw new InvalidOperationException(
                $"State {GetType().Name} must override either AsyncExecution or at least one Tick method");
        }

        using var localCts = CancellationTokenSource.CreateLinkedTokenSource(token);

        try
        {

            UniTask<StateTransitionInfo> loop = hasTickMethods
                ? TickLoop(localCts.Token)
                : UniTask.FromResult<StateTransitionInfo>(null);

            UniTask<StateTransitionInfo> enter = hasAsyncExecution
                ? AsyncExecution(localCts.Token)
                : UniTask.FromResult<StateTransitionInfo>(null);


            var (completed, result1, result2) = await UniTask.WhenAny(loop, enter);

            localCts.Cancel();

            return result1 ?? result2
                ?? throw new Exception($"Something wrong happened in TickableState {GetType().Name}");
        }
        finally
        {
            localCts.Cancel();
        }
    }

    private async UniTask<StateTransitionInfo> TickLoop(CancellationToken token)
    {
        StateTransitionInfo transition = null;

        while (transition == null)
        {
            // Early Tick
            await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);
            if (token.IsCancellationRequested) return null;
            transition = EarlyTick();
            if (transition != null) return transition;

            // Tick
            await UniTask.Yield(PlayerLoopTiming.Update);
            if (token.IsCancellationRequested) return null;
            transition = Tick();
            if (transition != null) return transition;

            // Prelate Tick
            await UniTask.Yield(PlayerLoopTiming.PreLateUpdate);
            if (token.IsCancellationRequested) return null;
            transition = PreLateTick();
            if (transition != null) return transition;

            // Prelate Tick
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
            if (token.IsCancellationRequested) return null;
            transition = PostLateTick();
            if (transition != null) return transition;
        }
        return null;
    }

    public virtual async UniTask<StateTransitionInfo> AsyncExecution(CancellationToken token) => null;

    /// <summary>
    /// Executes on Unity's player loop, before Early Update
    /// </summary>
    /// <returns>Null if not ended, <seealso cref="StateTransitionInfo"/> if not null</returns>
    public virtual StateTransitionInfo EarlyTick() => null;

    /// <summary>
    /// Executes on Unity's player loop, before Update
    /// </summary>
    /// <returns>Null if not ended, <seealso cref="StateTransitionInfo"/> if not null</returns>
    public virtual StateTransitionInfo Tick() => null;

    /// <summary>
    /// Executes on Unity's player loop, before Late Update
    /// </summary>
    /// <returns>Null if not ended, <seealso cref="StateTransitionInfo"/> if not null</returns>
    public virtual StateTransitionInfo PreLateTick() => null;

    /// <summary>
    /// Executes on Unity's player loop, after Late Update
    /// </summary>
    /// <returns>Null if not ended, <seealso cref="StateTransitionInfo"/> if not null</returns>
    public virtual StateTransitionInfo PostLateTick() => null;
}