using SpiceSharp.Components;
using SpiceSharp.Circuits;

namespace SpiceSharp.Parser.Readers
{
    /// <summary>
    /// Reads <see cref="BJTModel"/> definitions.
    /// </summary>
    public class BipolarModelReader : ModelReader
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BipolarModelReader()
            : base("npn;pnp")
        {
        }

        /// <summary>
        /// Generate a new model
        /// </summary>
        /// <param name="name">Model name</param>
        /// <param name="type">Model type</param>
        /// <returns></returns>
        protected override Entity GenerateModel(Identifier name, string type)
        {
            BJTModel model = new BJTModel(name);

            var tempBehavior = (SpiceSharp.Behaviors.BJT.ModelTemperatureBehavior)model.GetBehavior(typeof(SpiceSharp.Behaviors.BJT.ModelTemperatureBehavior));

            if (type == "npn")
                tempBehavior.SetNPN(true);
            else if (type == "pnp")
                tempBehavior.SetPNP(true);
            return model;
        }
    }
}
