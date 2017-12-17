using System;
using System.Reflection;

namespace SpiceSharp
{
    /// <summary>
    /// This class describes a parameter that is optional. Whether or not it was specified can be
    /// found using the Given variable.
    /// </summary>
    public class PropertyParameter: Parameter
    {
        private PropertyInfo propertyInfo;
        private object obj;

        public override double Value
        {
            get => (double)this.propertyInfo?.GetValue(obj);
            set => this.propertyInfo?.SetValue(obj, value);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PropertyParameter(PropertyInfo propertyInfo, object obj)
        {
            this.propertyInfo = propertyInfo;
            this.obj = obj;
            this.Given = true;
        }

        /// <summary>
        /// Specify the parameter
        /// </summary>
        /// <param name="value"></param>
        public override void Set(double value)
        {
            this.propertyInfo.SetValue(obj, value);
        }
    }
}