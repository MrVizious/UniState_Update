using System.Threading;
using Cysharp.Threading.Tasks;
using Examples.States;
using UniState;
using UnityEngine;

namespace Examples.Infrastructure.Update
{
    public class StateMachineStarter : MonoBehaviour
    {
        private readonly IStateMachine _stateMachine;
        private readonly ITypeResolver _resolver;

        public StateMachineStarter()
        {
            // Create a simple resolver that can create your states
            _resolver = new SimpleResolver();
            _stateMachine = new StateMachine();
            _stateMachine.SetResolver(_resolver);
        }

        private async void Start()
        {
            try
            {
                await _stateMachine.Execute<LoopUpdateGameState>(this.GetCancellationTokenOnDestroy());
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"State machine execution failed: {ex}");
            }
        }
        void Update()
        {
            _stateMachine.Tick(Time.deltaTime);
        }

        // Simple resolver implementation
        private class SimpleResolver : ITypeResolver
        {
            public object Resolve(System.Type type)
            {
                return System.Activator.CreateInstance(type);
            }
        }
    }

}