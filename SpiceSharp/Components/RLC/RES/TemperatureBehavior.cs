using SpiceSharp.Diagnostics;
using SpiceSharp.Components;
using SpiceSharp.Parameters;
using SpiceSharp.Circuits;

namespace SpiceSharp.Behaviors.RES
{
    /// <summary>
    /// Temperature behavior for a <see cref="Resistor"/>
    /// </summary>
    public class TemperatureBehavior : Behaviors.TemperatureBehavior
    {
        /// <summary>
        /// Parameters
        /// </summary>
        [SpiceName("temp"), SpiceInfo("Instance operating temperature", Interesting = false)]
        public double RES_TEMP
        {
            get => REStemp - Circuit.CONSTCtoK;
            set => REStemp.Set(value + Circuit.CONSTCtoK);
        }
        public Parameter REStemp { get; } = new Parameter(300.15); // 27 C

        /// <summary>
        /// Get the default conductance for this model
        /// </summary>
        public double RESresist { get; protected set; }
        public double RESconduct { get; protected set; }

        /// <summary>
        /// Name of the component
        /// </summary>
        private Identifier name;
        private Resistor res;
        private ResistorModel model;

        /// <summary>
        /// Setup the behavior
        /// </summary>
        /// <param name="component"></param>
        /// <param name="ckt"></param>
        /// <returns></returns>
        public override void Setup(Entity component, Circuit ckt)
        {
            res = component as Resistor;
            model = res.Model as ResistorModel;
            name = res.Name;
        }

        /// <summary>
        /// Execute behavior
        /// </summary>
        /// <param name="ckt">Circuit</param>
        public override void Temperature(Circuit ckt)
        {
            double factor;
            double difference;

            // Default Value Processing for Resistor Instance
            if (!REStemp.Given)
                REStemp.Value = ckt.State.Temperature;
          
            if (model == null)
                throw new CircuitException("No model specified");

            if ((model.RESsheetRes.Given && model.RESsheetRes != 0) && (res.RESlength != 0)) {
                RESresist = model?.RESsheetRes * (res.RESlength - model.RESnarrow) / (res.RESwidth - ((ResistorModel)res.Model).RESnarrow);
            }
            else
            {
                CircuitWarning.Warning(this, $"{name}: resistance=0, set to 1000");
                RESresist = 1000;
            }

            if (model != null)
            {
                difference = REStemp - model.REStnom;
                factor = 1.0 + (model?.REStempCoeff1) * difference + (model?.REStempCoeff2) * difference * difference;
            }
            else
            {
                difference = REStemp - 300.15;
                factor = 1.0;
            }

            RESconduct = 1.0 / (RESresist * factor);
        }
    }
}
