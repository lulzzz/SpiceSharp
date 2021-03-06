﻿using SpiceSharp.Circuits;
using SpiceSharp.Parameters;
using SpiceSharp.Diagnostics;

namespace SpiceSharp.Components
{
    /// <summary>
    /// A resistor
    /// </summary>
    [SpicePins("R+", "R-")]
    public class Resistor : CircuitComponent<Resistor>
    {
        /// <summary>
        /// Set the model for the resistor
        /// </summary>
        /// <param name="model"></param>
        public void SetModel(ResistorModel model) => Model = model;

        /// <summary>
        /// Parameters
        /// </summary>
        [SpiceName("temp"), SpiceInfo("Instance operating temperature", Interesting = false)]
        public double RES_TEMP
        {
            get => REStemp - Circuit.CONSTCtoK;
            set => REStemp.Set(value + Circuit.CONSTCtoK);
        }
        public Parameter REStemp { get; } = new Parameter(300.15);
        [SpiceName("resistance"), SpiceInfo("Resistance", IsPrincipal = true)]
        public Parameter RESresist { get; } = new Parameter();
        [SpiceName("w"), SpiceInfo("Width", Interesting = false)]
        public Parameter RESwidth { get; } = new Parameter();
        [SpiceName("l"), SpiceInfo("Length", Interesting = false)]
        public Parameter RESlength { get; } = new Parameter();
        [SpiceName("i"), SpiceInfo("Current")]
        public double GetCurrent(Circuit ckt) => (ckt.State.Real.Solution[RESposNode] - ckt.State.Real.Solution[RESnegNode]) * RESconduct;
        [SpiceName("p"), SpiceInfo("Power")]
        public double GetPower(Circuit ckt) => (ckt.State.Real.Solution[RESposNode] - ckt.State.Real.Solution[RESnegNode]) *
            (ckt.State.Real.Solution[RESposNode] - ckt.State.Real.Solution[RESnegNode]) * RESconduct;

        /// <summary>
        /// Nodes
        /// </summary>
        public int RESposNode { get; private set; }
        public int RESnegNode { get; private set; }
        public ComponentNoise RESnoise { get; private set; } = new ComponentNoise(new Noise.NoiseThermal("thermal", 0, 1));

        /// <summary>
        /// Private variables
        /// </summary>
        private double RESconduct = 0.0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the resistor</param>
        public Resistor(CircuitIdentifier name) : base(name) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the resistor</param>
        /// <param name="pos">The positive node</param>
        /// <param name="neg">The negative node</param>
        /// <param name="res">The resistance</param>
        public Resistor(CircuitIdentifier name, CircuitIdentifier pos, CircuitIdentifier neg, double res) : base(name)
        {
            Connect(pos, neg);
            RESresist.Set(res);
        }

        /// <summary>
        /// Load the resistor
        /// </summary>
        /// <param name="ckt">The circuit</param>
        public override void Load(Circuit ckt)
        {
            var rstate = ckt.State.Real;
            rstate.Matrix[RESposNode, RESposNode] += RESconduct;
            rstate.Matrix[RESnegNode, RESnegNode] += RESconduct;
            rstate.Matrix[RESposNode, RESnegNode] -= RESconduct;
            rstate.Matrix[RESnegNode, RESposNode] -= RESconduct;
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
            RESnoise.Setup(ckt, RESposNode, RESnegNode);
        }

        /// <summary>
        /// Do temperature-dependent calculations
        /// </summary>
        /// <param name="ckt">The circuit</param>
        public override void Temperature(Circuit ckt)
        {
            double factor;
            double difference;
            ResistorModel model = Model as ResistorModel;

            // Default Value Processing for Resistor Instance
            if (!REStemp.Given)
                REStemp.Value = ckt.State.Temperature;
            if (!RESwidth.Given) RESwidth.Value = model?.RESdefWidth ?? 0.0;
            if (!RESresist.Given)
            {
                if (model == null)
                    throw new CircuitException("No model specified");
                if (model.RESsheetRes.Given && (model.RESsheetRes != 0) && (RESlength != 0))
                    RESresist.Value = model.RESsheetRes * (RESlength - model.RESnarrow) / (RESwidth - model.RESnarrow);
                else
                {
                    CircuitWarning.Warning(this, $"{Name}: resistance=0, set to 1000");
                    RESresist.Value = 1000;
                }
            }

            if (model != null)
            {
                difference = REStemp - model.REStnom;
                factor = 1.0 + (model.REStempCoeff1) * difference + (model.REStempCoeff2) * difference * difference;
            }
            else
            {
                difference = REStemp - 300.15;
                factor = 1.0;
            }

            RESconduct = 1.0 / (RESresist * factor);
        }

        /// <summary>
        /// Load the resistor for AC anlalysis
        /// </summary>
        /// <param name="ckt"></param>
        public override void AcLoad(Circuit ckt)
        {
            var cstate = ckt.State.Complex;
            cstate.Matrix[RESposNode, RESposNode] += RESconduct;
            cstate.Matrix[RESposNode, RESnegNode] -= RESconduct;
            cstate.Matrix[RESnegNode, RESposNode] -= RESconduct;
            cstate.Matrix[RESnegNode, RESnegNode] += RESconduct;
        }

        /// <summary>
        /// Noise calculations
        /// </summary>
        /// <param name="ckt">Circuit</param>
        public override void Noise(Circuit ckt)
        {
            // Evaluate the noise sources
            RESnoise.Evaluate(ckt, RESconduct);
        }
    }
}
