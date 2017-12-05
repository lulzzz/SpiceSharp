﻿using System.Collections.Generic;
using SpiceSharp.Components;
using SpiceSharp.Circuits;

namespace SpiceSharp.Parser.Readers
{
    /// <summary>
    /// Reads <see cref="VoltageSwitch"/> and <see cref="CurrentSwitch"/> components.
    /// </summary>
    public class SwitchReader : ComponentReader
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SwitchReader() : base("s;w") { }

        /// <summary>
        /// Generate a switch
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="name">Name</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="netlist">Netlist</param>
        /// <returns></returns>
        protected override ICircuitObject Generate(string type, CircuitIdentifier name, List<Token> parameters, Netlist netlist)
        {
            switch (type)
            {
                case "s": return GenerateVSW(name, parameters, netlist);
                case "w": return GenerateCSW(name, parameters, netlist);
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
        protected ICircuitObject GenerateVSW(CircuitIdentifier name, List<Token> parameters, Netlist netlist)
        {
            VoltageSwitch vsw = new VoltageSwitch(name);
            vsw.ReadNodes(netlist.Path, parameters);

            // Read the model
            if (parameters.Count < 5)
                throw new ParseException(parameters[3], "Model expected", false);
            vsw.Model = netlist.FindModel<VoltageSwitchModel>(parameters[4]);

            // Optional ON or OFF
            if (parameters.Count == 6)
            {
                switch (parameters[5].image.ToLower())
                {
                    case "on":
                        vsw.SetOn();
                        break;
                    case "off":
                        vsw.SetOff();
                        break;
                    default:
                        throw new ParseException(parameters[5], "ON or OFF expected");
                }
            }
            return vsw;
        }

        /// <summary>
        /// Generate a current switch
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="netlist">Netlist</param>
        /// <returns></returns>
        protected ICircuitObject GenerateCSW(CircuitIdentifier name, List<Token> parameters, Netlist netlist)
        {
            CurrentSwitch csw = new CurrentSwitch(name);
            csw.ReadNodes(netlist.Path, parameters);
            switch (parameters.Count)
            {
                case 2: throw new ParseException(parameters[1], "Voltage source expected", false);
                case 3: throw new ParseException(parameters[2], "Model expected", false);
            }

            // Get the controlling voltage source
            switch (parameters[2].kind)
            {
                case WORD:
                    csw.CSWcontName = new CircuitIdentifier(parameters[2].image);
                    break;
                default:
                    throw new ParseException(parameters[2], "Voltage source name expected");
            }

            // Get the model
            csw.Model = netlist.FindModel<CurrentSwitchModel>(parameters[3]);

            // Optional on or off
            if (parameters.Count > 4)
            {
                switch (parameters[4].image.ToLower())
                {
                    case "on":
                        csw.SetOn();
                        break;
                    case "off":
                        csw.SetOff();
                        break;
                    default:
                        throw new ParseException(parameters[4], "ON or OFF expected");
                }
            }
            return csw;
        }
    }
}
