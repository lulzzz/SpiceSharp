using SpiceSharp.Circuits;

namespace SpiceSharp.Components
{
    public interface ICircuitComponent : ICircuitObject
    {
        int PinCount { get; }
        CircuitModel Model { get; }

        int GetNodeIndex(int i);
        CircuitIdentifier GetNode(int i);
        void Connect(CircuitIdentifier[] nodes);
    }
}
