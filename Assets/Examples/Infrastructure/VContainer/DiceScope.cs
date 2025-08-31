using Examples.States;
using UniState;
using VContainer;
using VContainer.Unity;

namespace Examples.Infrastructure.VContainer
{
    public class DiceScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<DiceEntryPoint>();

            builder.RegisterStateMachine<IStateMachine, StateMachine>();

            builder.RegisterState<ExampleTickableState>();

            builder.RegisterState<EnterState>();
            builder.RegisterState<LoopState>();

            builder.RegisterState<StartGameState>();
            builder.RegisterState<RollDiceState>();
            builder.RegisterState<LostState>();
            builder.RegisterState<WinState>();
        }
    }
}