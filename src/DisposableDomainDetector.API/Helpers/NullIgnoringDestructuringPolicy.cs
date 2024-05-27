using Serilog.Core;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DisposableDomainDetector.API.Helpers
{
    public sealed class NullIgnoringDestructuringPolicy : IDestructuringPolicy
    {
        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, [NotNullWhen(true)] out LogEventPropertyValue? result)
        {
            try
            {
                // Special handling of a few types that Serilog already has destructuring policies for
                if (value is Delegate or Type or MemberInfo)
                {
                    result = null;
                    return false;
                }

                var properties = value.GetType().GetProperties();

                if (properties.Length == 0)
                {
                    result = null;
                    return false;
                }

                var logEventProperties = properties
                    .Select(propertyInfo => new { propertyInfo, value = propertyInfo.GetValue(value), })
                    .Where(t => t.value != null)
                    .Select(t =>
                        new LogEventProperty(t.propertyInfo.Name, propertyValueFactory.CreatePropertyValue(t.value, true)));

                result = new StructureValue(logEventProperties);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
