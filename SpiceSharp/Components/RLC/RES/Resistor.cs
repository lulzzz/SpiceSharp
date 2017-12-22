using SpiceSharp.Circuits;
using SpiceSharp.Components.RES;
using SpiceSharp.Behaviors.RES;
using SpiceSharp.Attributes;

namespace SpiceSharp.Components
{
    /// <summary>
    /// A resistor
    /// </summary>
    [SpicePins("R+", "R-")]
    public class Resistor : Component
    {
        /// <summary>
        /// Parameters
        /// </summary>
        [SpiceName("resistance"), SpiceInfo("Resistance", IsPrincipal = true)]
        public Parameter RESresist { get; } = new Parameter();
        [SpiceName("w"), SpiceInfo("Width", Interesting = false)]
        public Parameter RESwidth { get; } = new Parameter();
        [SpiceName("l"), SpiceInfo("Length", Interesting = false)]
        public Parameter RESlength { get; } = new Parameter();

        /// <summary>
        /// Set the model for the resistor
        /// </summary>
        /// <param name="model"></param>
        public void SetModel(ResistorModel model)
        {
            this.Model = model;
        }

        /// <summary>
        /// Nodes
        /// </summary>
        public int RESposNode { get; private set; }
        public int RESnegNode { get; private set; }
        
        /// <summary>
        /// Constants
        /// </summary>
        public const int RESpinCount = 2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the resistor</param>
        public Resistor(Identifier name) 
            : base(name, RESpinCount)
        {
            // Register parameters
            Parameters.Register(new BaseParameters());

            // Register factories
            AddFactory(typeof(LoadBehavior), () => new LoadBehavior());
            AddFactory(typeof(AcBehavior), () => new AcBehavior());
            AddFactory(typeof(NoiseBehavior), () => new NoiseBehavior());
            AddFactory(typeof(TemperatureBehavior), () => new TemperatureBehavior());
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the resistor</param>
        /// <param name="pos">The positive node</param>
        /// <param name="neg">The negative node</param>
        /// <param name="res">The resistance</param>
        public Resistor(Identifier name, Identifier pos, Identifier neg, double res) 
            : base(name, RESpinCount)
        {
            // Register parameters
            Parameters.Register(new BaseParameters(res));

            // Register factories
            AddFactory(typeof(LoadBehavior), () => new LoadBehavior());
            AddFactory(typeof(AcBehavior), () => new AcBehavior());
            AddFactory(typeof(NoiseBehavior), () => new NoiseBehavior());
            AddFactory(typeof(TemperatureBehavior), () => new TemperatureBehavior());

            // Connect
            Connect(pos, neg);
        }

        /// <summary>
        /// Setup the resistor
        /// </summary>
        /// <param name="ckt"></param>
        public override void Setup(Circuit ckt)
        {
            var nodes = BindNodes(ckt);
            RESposNode = nodes[0].Index;
            RESnegNode = nodes[1].Index;
        }
    }
}
