namespace Uniray
{
    /// <summary>Defines whether a field or a property can be used as a component field.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ComponentField : Attribute 
    {
        public string Group;
        public ComponentField() { Group = ""; }
    }
}