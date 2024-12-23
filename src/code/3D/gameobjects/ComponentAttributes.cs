#pragma warning disable CS8602
#pragma warning disable CS8601

using System.Reflection;

namespace Uniray
{
    /// <summary>Defines whether a field or a property can be used as a component field.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ComponentField : Attribute 
    {
        public string Group;
        public ComponentField() { Group = ""; }

        /// <summary>Sets the property value through function and updates UI components linked to it (Property needs the <see cref="ComponentField"/> attribute.)</summary>
        /// <typeparam name="T">Type of property value.</typeparam>
        /// <param name="value">Value to update.</param>
        /// <param name="valuePtr">Property value passed by reference.</param>
        public static unsafe void SetProperty<T>(T value, ref T valuePtr, string propName)
        {
            valuePtr = value;
            // Set UI Component value
            if (UData.Selection.Count == 1) // if 1 Game Object selected (UI Components active)
            {
                ((RayGUI_cs.Textbox)Uniray.UI.Components.Where(x => x.Key == $"{propName}{UI.COMPONENT_IDENTIFIER}").ToList().First().Value).Text = value.ToString();
            }
        }
    }

    /// <summary>Represents an instance of the validation class for <see cref="ComponentField"/>.</summary>
    public static class ComponentFieldValidator
    {
        /// <summary>Runs the <see cref="ComponentField"/> tests for the given type.</summary>
        /// <typeparam name="T">Type to test (must inherit from <see cref="GameObject3D"/>.)</typeparam>
        public static void Validate<T>() where T : GameObject3D
        {
            Type type = typeof(T);
            // Get properties using ComponentField attribute
            PropertyInfo[] props = type.GetProperties().Where(prop => prop.IsDefined(typeof(ComponentField), false)).ToArray();
            // Check if implements SetProperty<T> in setter

            for (int i = 0; i < props.Length; i++) 
            {
                UDebugger.Send($"Property {props[i].Name} from {type.Name} respects [ComponentField] conventions", ConsoleColor.Green);
            }
        }
    }
}