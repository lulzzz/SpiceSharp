﻿using System;
using System.Diagnostics;

namespace SpiceSharp.Circuits
{
    /// <summary>
    /// Keeps track of simulation statistics.
    /// </summary>
    public class CircuitStatistics
    {
        /// <summary>
        /// Get the total number of iterations
        /// </summary>
        public int NumIter { get; set; } = 0;

        /// <summary>
        /// The total time spent solving equations
        /// </summary>
        public Stopwatch SolveTime { get; } = new Stopwatch();

        /// <summary>
        /// The total time spent loading the equation matrix
        /// </summary>
        public Stopwatch LoadTime { get; } = new Stopwatch();

        /// <summary>
        /// Gets or sets the number of timepoints calculated
        /// </summary>
        public int TimePoints { get; set; } = 0;

        /// <summary>
        /// Get or sets the total number of transient iterations
        /// </summary>
        public int TranIter { get; set; } = 0;

        /// <summary>
        /// The total time spent executing transient simulations
        /// </summary>
        public Stopwatch TransientTime { get; } = new Stopwatch();

        /// <summary>
        /// Get or sets the total solving time during transient simulations
        /// </summary>
        public TimeSpan TransientSolveTime { get; set; } = new TimeSpan();

        /// <summary>
        /// Get the number of accepted timepoints
        /// </summary>
        public int Accepted { get; set; } = 0;

        /// <summary>
        /// Get the number of rejected timepoints
        /// </summary>
        public int Rejected { get; set; } = 0;

        /// <summary>
        /// Clear the statistics
        /// </summary>
        public void Clear()
        {
            NumIter = 0;
            SolveTime.Reset();
            LoadTime.Reset();
            TimePoints = 0;
            TranIter = 0;
            TransientTime.Reset();
            TransientSolveTime = TimeSpan.Zero;
            Accepted = 0;
            Rejected = 0;
        }
    }
}
