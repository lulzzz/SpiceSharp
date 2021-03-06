﻿using SpiceSharp.Parameters;

namespace SpiceSharp.Components
{
    /// <summary>
    /// Provides values in function of time.
    /// </summary>
    public interface IWaveform : IParameterized
    {
        /// <summary>
        /// Calculate the waveform at a specific time
        /// </summary>
        /// <param name="time">Time</param>
        /// <returns></returns>
        double At(double time);

        /// <summary>
        /// Setup the waveform
        /// </summary>
        /// <param name="ckt">The circuit</param>
        void Setup(Circuit ckt);

        /// <summary>
        /// Accept the current timepoint
        /// </summary>
        /// <param name="ckt">The circuit</param>
        void Accept(Circuit ckt);
    }
}
