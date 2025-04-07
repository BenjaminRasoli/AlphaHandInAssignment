using Domain.Models;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Domain.Extensions;

public static class MappingExtensions
{

    public static TDestination MapTo<TDestination>(this object source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        TDestination destination = Activator.CreateInstance<TDestination>()!;

        var sourceProperties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = destination.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var destinationProperty in destinationProperties)
        {
            var sourceProperty = sourceProperties.FirstOrDefault(x => x.Name == destinationProperty.Name && x.PropertyType == destinationProperty.PropertyType);
            if (sourceProperty != null && destinationProperty.CanWrite)
            {
                var value = sourceProperty.GetValue(source);
                destinationProperty.SetValue(destination, value);
            }
        }

        return destination;

    }
}
