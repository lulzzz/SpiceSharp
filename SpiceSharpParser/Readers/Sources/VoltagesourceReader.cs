﻿using System.Collections.Generic;
using SpiceSharp.Components;
using SpiceSharp.Circuits;

namespace SpiceSharp.Parser.Readers
{
    /// <summary>
    /// Reads <see cref="Voltagesource"/>, <see cref="VoltageControlledVoltagesource"/> and <see cref="CurrentControlledVoltagesource"/> components.
    /// </summary>
    public class VoltagesourceReader : ComponentReader
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public VoltagesourceReader() : base("v;h;e") { }

        /// <summary>
        /// Generate a voltage source
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="name">Name</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="netlist">Netlist</param>
        /// <returns></returns>
        protected override Entity Generate(string type, Identifier name, List<Token> parameters, Netlist netlist)
        {
            switch (type)
            {
                case "v": return GenerateVSRC(name, parameters, netlist);
                case "h": return GenerateCCVS(name, parameters, netlist);
                case "e": return GenerateVCVS(name, parameters, netlist);
            }
            return null;
        }

        /// <summary>
        /// Generate
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="netlist">Netlist</param>
        /// <returns></returns>
        protected Entity GenerateVSRC(Identifier name, List<Token> parameters, Netlist netlist)
        {
            Voltagesource vsrc = new Voltagesource(name);
            vsrc.ReadNodes(netlist.Path, parameters);

            // We can have a value or just DC
            for (int i = 2; i < parameters.Count; i++)
            {
                // DC specification
                if (i == 2 && parameters[i].image.ToLower() == "dc")
                {
                    i++;
                    vsrc.Set("dc", netlist.ParseDouble(parameters[i]));
                }
                else if (i == 2 && ReaderExtension.IsValue(parameters[i]))
                    vsrc.Set("dc", netlist.ParseDouble(parameters[i]));

                // AC specification
                else if (parameters[i].image.ToLower() == "ac")
                {
                    i++;
                    vsrc.Set("acmag", netlist.ParseDouble(parameters[i]));

                    // Look forward for one more value
                    if (i + 1 < parameters.Count && ReaderExtension.IsValue(parameters[i + 1]))
                    {
                        i++;
                        vsrc.Set("acphase", netlist.ParseDouble(parameters[i]));
                    }
                }

                // Waveforms
                else if (parameters[i].kind == BRACKET)
                {
                    // Find the reader
                    var bt = parameters[i] as BracketToken;
                    Statement st = new Statement(StatementType.Waveform, bt.Name, bt.Parameters);
                    object w = netlist.Readers.Read(st, netlist);
                    ((SpiceSharp.Behaviors.VSRC.LoadBehavior)vsrc.GetBehavior(typeof(SpiceSharp.Behaviors.VSRC.LoadBehavior))).VSRCwaveform = (Waveform)w;
                }
                else
                    throw new ParseException(parameters[i], "Unrecognized parameter");
            }
            return vsrc;
        }

        /// <summary>
        /// Generate
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="netlist">Netlist</param>
        /// <returns></returns>
        protected Entity GenerateVCVS(Identifier name, List<Token> parameters, Netlist netlist)
        {
            VoltageControlledVoltagesource vcvs = new VoltageControlledVoltagesource(name);
            vcvs.ReadNodes(netlist.Path, parameters);

            if (parameters.Count < 5)
                throw new ParseException(parameters[3], "Value expected");
            vcvs.Set("gain", netlist.ParseDouble(parameters[4]));
            return vcvs;
        }

        /// <summary>
        /// Generate a CCVS
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="netlist">Netlist</param>
        /// <returns></returns>
        protected Entity GenerateCCVS(Identifier name, List<Token> parameters, Netlist netlist)
        {
            CurrentControlledVoltagesource ccvs = new CurrentControlledVoltagesource(name);
            ccvs.ReadNodes(netlist.Path, parameters);
            switch (parameters.Count)
            {
                case 2: throw new ParseException(parameters[1], "Voltage source expected", false);
                case 3: throw new ParseException(parameters[2], "Value expected", false);
            }

            if (!ReaderExtension.IsName(parameters[2]))
                throw new ParseException(parameters[2], "Component name expected");
            ccvs.CCVScontName = new Identifier(parameters[2].image);
            ccvs.Set("gain", netlist.ParseDouble(parameters[3]));
            return ccvs;
        }
    }
}
