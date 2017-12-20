using SpiceSharp.Components;
using SpiceSharp.Circuits;

namespace SpiceSharp.Behaviors.CAP
{
    /// <summary>
    /// Temperature behavior for a <see cref="Capacitor"/>
    /// </summary>
    public class TemperatureBehavior : Behaviors.TemperatureBehavior
    {
        private Capacitor cap;
        private CapacitorModel model;

        /// <summary>
        /// The calculated capacitance
        /// </summary>
        public double CAPcapac { get; private set; }

        /// <summary>
        /// Setup the behavior
        /// </summary>
        /// <param name="component">Component</param>
        /// <param name="ckt">Circuit</param>
        public override void Setup(Entity component, Circuit ckt)
        {
            cap = component as Capacitor;
            model = cap.Model as CapacitorModel;
        }

        /// <summary>
        /// Execute the behavior
        /// </summary>
        /// <param name="ckt">Circuit</param>
        public override void Temperature(Circuit ckt)
        {
            // Default Value Processing for Capacitor Instance
            if (model != null)
            {
                CAPcapac = model.CAPcj *
                                (cap.CAPwidth - model.CAPnarrow) *
                                (cap.CAPlength - model.CAPnarrow) +
                            model.CAPcjsw * 2 * (
                                (cap.CAPlength - model.CAPnarrow) +
                                (cap.CAPwidth - model.CAPnarrow));
            }
        }
    }
}
