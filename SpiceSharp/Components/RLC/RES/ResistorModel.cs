﻿using SpiceSharp.Circuits;
using SpiceSharp.Components.RES;

namespace SpiceSharp.Components
{
    /// <summary>
    /// A model for semiconductor <see cref="Resistor"/>
    /// </summary>
    public class ResistorModel : Model
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public ResistorModel(Identifier name) : base(name)
        {
            Parameters.Register(new ModelBaseParameters());
        }
    }
}
