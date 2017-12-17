﻿using System;
using System.Numerics;
using SpiceSharp.Circuits;
using SpiceSharp.Simulations;
using SpiceSharp.Components;

namespace SpiceSharp.Parser.Readers.Exports
{
    /// <summary>
    /// Reads a current export (I, IR, II, IDB, IP).
    /// </summary>
    public class CurrentReader : Reader
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CurrentReader() : base(StatementType.Export)
        {
            Identifier = "i;ir;ii;idb;ip";
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="netlist">Netlist</param>
        /// <returns></returns>
        public override bool Read(string type, Statement st, Netlist netlist)
        {
            // Get the source name
            Identifier source;
            switch (st.Parameters.Count)
            {
                case 0:
                    throw new ParseException(st.Name, "Voltage source expected", false);
                case 1:
                    if (!ReaderExtension.IsName(st.Parameters[0]))
                        throw new ParseException(st.Parameters[0], "Component name expected");
                    source = new Identifier(st.Parameters[0].image);
                    break;
                default:
                    throw new ParseException(st.Name, "Too many nodes specified", false);
            }

            // Add to the exports
            Export ce = null;
            switch (type)
            {
                case "i": ce = new CurrentExport(source); break;
                case "ir": ce = new CurrentRealExport(source); break;
                case "ii": ce = new CurrentImaginaryExport(source); break;
                case "im": ce = new CurrentMagnitudeExport(source); break;
                case "ip": ce = new CurrentPhaseExport(source); break;
                case "idb": ce = new CurrentDecibelExport(source); break;
            }
            if (ce != null)
                netlist.Exports.Add(ce);
            Generated = ce;
            return true;
        }
    }

    /// <summary>
    /// Current export.
    /// </summary>
    public class CurrentExport : Export
    {
        /// <summary>
        /// The main node
        /// </summary>
        public Identifier Source { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node"></param>
        /// <param name="reference"></param>
        public CurrentExport(Identifier source)
        {
            Source = source;
        }

        /// <summary>
        /// Get the type name
        /// </summary>
        public override string TypeName => "current";

        /// <summary>
        /// Get the name based on the properties
        /// </summary>
        public override string Name => "i(" + Source + ")";

        /// <summary>
        /// Read the voltage and write to the output
        /// </summary>
        /// <param name="data">Simulation data</param>
        public override double Extract(SimulationData data)
        {
            Voltagesource vsrc = (Voltagesource)data.GetObject(Source);
            var loadBehavior = (SpiceSharp.Behaviors.VSRC.LoadBehavior)vsrc.GetBehavior(typeof(SpiceSharp.Behaviors.VSRC.LoadBehavior));

            if (data.Circuit.State.Domain == State.DomainTypes.Frequency || data.Circuit.State.Domain == State.DomainTypes.Laplace)
                return data.Circuit.State.Solution[loadBehavior.VSRCbranch];
            else
                return loadBehavior.GetCurrent(data.Circuit);
        }
    }

    /// <summary>
    /// Real part of a complex current export.
    /// </summary>
    public class CurrentRealExport : Export
    {
        /// <summary>
        /// The main node
        /// </summary>
        public Identifier Source { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node">Positive node</param>
        /// <param name="reference">Negative reference node</param>
        public CurrentRealExport(Identifier source)
        {
            Source = source;
        }

        /// <summary>
        /// Get the type name
        /// </summary>
        public override string TypeName => "current";

        /// <summary>
        /// Get the name
        /// </summary>
        public override string Name => "ir(" + Source + ")";

        /// <summary>
        /// Extract
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override double Extract(SimulationData data)
        {
            Voltagesource vsrc = (Voltagesource)data.GetObject(Source);
            var loadBehavior = (SpiceSharp.Behaviors.VSRC.LoadBehavior)vsrc.GetBehavior(typeof(SpiceSharp.Behaviors.VSRC.LoadBehavior));

            switch (data.Circuit.State.Domain)
            {
                case State.DomainTypes.Frequency:
                case State.DomainTypes.Laplace:
                    return data.Circuit.State.Solution[loadBehavior.VSRCbranch];
                default:
                    return loadBehavior.GetCurrent(data.Circuit);
            }
        }
    }

    /// <summary>
    /// Imaginary part of a complex current export.
    /// </summary>
    public class CurrentImaginaryExport : Export
    {
        /// <summary>
        /// The main node
        /// </summary>
        public Identifier Source { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node">Positive node</param>
        /// <param name="reference">Negative reference node</param>
        public CurrentImaginaryExport(Identifier source)
        {
            Source = source;
        }

        /// <summary>
        /// Get the type name
        /// </summary>
        public override string TypeName => "current";

        /// <summary>
        /// Get the name
        /// </summary>
        public override string Name => "ii(" + Source + ")";

        /// <summary>
        /// Extract
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override double Extract(SimulationData data)
        {
            Voltagesource vsrc = (Voltagesource)data.GetObject(Source);
            var loadBehavior = (SpiceSharp.Behaviors.VSRC.LoadBehavior)vsrc.GetBehavior(typeof(SpiceSharp.Behaviors.VSRC.LoadBehavior));

            switch (data.Circuit.State.Domain)
            {
                case State.DomainTypes.Frequency:
                case State.DomainTypes.Laplace:
                    return data.Circuit.State.iSolution[loadBehavior.VSRCbranch];
                default:
                    return 0.0;
            }
        }
    }

    /// <summary>
    /// Magnitude of a complex current export.
    /// </summary>
    public class CurrentMagnitudeExport : Export
    {
        /// <summary>
        /// The main node
        /// </summary>
        public Identifier Source { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node">Positive node</param>
        /// <param name="reference">Negative reference node</param>
        public CurrentMagnitudeExport(Identifier source)
        {
            Source = source;
        }

        /// <summary>
        /// Get the type name
        /// </summary>
        public override string TypeName => "current";

        /// <summary>
        /// Get the name
        /// </summary>
        public override string Name => "im(" + Source + ")";

        /// <summary>
        /// Extract
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override double Extract(SimulationData data)
        {
            Voltagesource vsrc = (Voltagesource)data.GetObject(Source);
            var loadBehavior = (SpiceSharp.Behaviors.VSRC.LoadBehavior)vsrc.GetBehavior(typeof(SpiceSharp.Behaviors.VSRC.LoadBehavior));

            switch (data.Circuit.State.Domain)
            {
                case State.DomainTypes.Frequency:
                case State.DomainTypes.Laplace:
                    double r = data.Circuit.State.Solution[loadBehavior.VSRCbranch];
                    double i = data.Circuit.State.iSolution[loadBehavior.VSRCbranch];
                    return Math.Sqrt(r * r + i * i);
                default:
                    return loadBehavior.GetCurrent(data.Circuit);
            }
        }
    }

    /// <summary>
    /// Phase of a complex current export.
    /// </summary>
    public class CurrentPhaseExport : Export
    {
        /// <summary>
        /// The main node
        /// </summary>
        public Identifier Source { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node">Positive node</param>
        /// <param name="reference">Negative reference node</param>
        public CurrentPhaseExport(Identifier source)
        {
            Source = source;
        }

        /// <summary>
        /// Get the type name
        /// </summary>
        public override string TypeName => "current";

        /// <summary>
        /// Get the name
        /// </summary>
        public override string Name => "ip(" + Source + ")";

        /// <summary>
        /// Extract
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override double Extract(SimulationData data)
        {
            Voltagesource vsrc = (Voltagesource)data.GetObject(Source);
            var loadBehavior = (SpiceSharp.Behaviors.VSRC.LoadBehavior)vsrc.GetBehavior(typeof(SpiceSharp.Behaviors.VSRC.LoadBehavior));

            switch (data.Circuit.State.Domain)
            {
                case State.DomainTypes.Frequency:
                case State.DomainTypes.Laplace:
                    double r = data.Circuit.State.Solution[loadBehavior.VSRCbranch];
                    double i = data.Circuit.State.iSolution[loadBehavior.VSRCbranch];
                    return 180.0 / Math.PI * Math.Atan2(i, r);
                default:
                    return loadBehavior.GetCurrent(data.Circuit);
            }
        }
    }

    /// <summary>
    /// Magnitude in decibels of a complex current export.
    /// </summary>
    public class CurrentDecibelExport : Export
    {
        /// <summary>
        /// The main node
        /// </summary>
        public Identifier Source { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node">Positive node</param>
        /// <param name="reference">Negative reference node</param>
        public CurrentDecibelExport(Identifier source)
        {
            Source = source;
        }

        /// <summary>
        /// Get the type name
        /// </summary>
        public override string TypeName => "none";

        /// <summary>
        /// Get the name
        /// </summary>
        public override string Name => "idb(" + Source + ")";

        /// <summary>
        /// Extract
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override double Extract(SimulationData data)
        {
            Voltagesource vsrc = (Voltagesource)data.GetObject(Source);
            var loadBehavior = (SpiceSharp.Behaviors.VSRC.LoadBehavior)vsrc.GetBehavior(typeof(SpiceSharp.Behaviors.VSRC.LoadBehavior));

            switch (data.Circuit.State.Domain)
            {
                case State.DomainTypes.Frequency:
                case State.DomainTypes.Laplace:
                    double r = data.Circuit.State.Solution[loadBehavior.VSRCbranch];
                    double i = data.Circuit.State.iSolution[loadBehavior.VSRCbranch];
                    return 10.0 * Math.Log10(r * r + i * i);
                default:
                    return 20.0 * Math.Log10(loadBehavior.GetCurrent(data.Circuit));
            }
        }
    }
}
