using SpiceSharp.Circuits;
using System;

namespace SpiceSharp.Behaviors
{
    public static class BehaviorsExtensions
    {
        public static void RegisterBehavior(this ICircuitObject @object, Type behaviorType)
        {
            Behaviors.RegisterBehavior(@object.GetType(), behaviorType);
        }
    }
}
