﻿using System.Collections.Generic;
using SpiceSharp.Components;
using SpiceSharp.Circuits;

namespace SpiceSharp.Parser.Readers
{
    /// <summary>
    /// Reads <see cref="Diode"/> components.
    /// </summary>
    public class DiodeReader : ComponentReader
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DiodeReader() : base("d") { }

        /// <summary>
        /// Generate a diode
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="netlist">Netlist</param>
        /// <returns></returns>
        protected override Entity Generate(string type, Identifier name, List<Token> parameters, Netlist netlist)
        {
            Diode dio = new Diode(name);
            dio.ReadNodes(netlist.Path, parameters);

            if (parameters.Count < 3)
                throw new ParseException(parameters[1], "Model expected", false);
            dio.SetModel(netlist.FindModel<DiodeModel>(parameters[2]));

            var loadBehavior = (SpiceSharp.Behaviors.DIO.LoadBehavior)dio.GetBehavior(typeof(SpiceSharp.Behaviors.DIO.LoadBehavior));
            
            // Read the rest of the parameters
            for (int i = 3; i < parameters.Count; i++)
            {
                switch (parameters[i].kind)
                {
                    case WORD:
                        switch (parameters[i].image.ToLower())
                        {
                            case "on":
                                loadBehavior.DIOoff = false;
                                break;
                            case "off":
                                loadBehavior.DIOoff = true;
                                break;
                            default:
                                throw new ParseException(parameters[i], "ON or OFF expected");
                        }
                        break;
                    case ASSIGNMENT:
                        AssignmentToken at = parameters[i] as AssignmentToken;
                        if (at.Name.image.ToLower() == "ic")
                            dio.Set("ic", netlist.ParseDouble(at.Value));
                        else
                            throw new ParseException(parameters[i], "IC expected");
                        break;
                    case VALUE:
                    case EXPRESSION:
                        if (!dio.Ask("area").Given)
                            dio.Set("area", netlist.ParseDouble(parameters[i]));
                        else if (!dio.Ask("temp").Given)
                            dio.Set("temp", netlist.ParseDouble(parameters[i]));
                        else
                            throw new ParseException(parameters[i], "Invalid parameter");
                        break;
                    default:
                        throw new ParseException(parameters[i], "Unrecognized parameter");
                }
            }

            return dio;
        }
    }
}
