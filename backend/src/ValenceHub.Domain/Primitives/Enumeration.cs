using System;
using System.Collections.Generic;
using System.Linq;

namespace ValenceHub.Domain.Primitives
{
    /// <summary>
    /// Base class for strongly-typed enumerations.
    /// </summary>
    public abstract class Enumeration : IComparable
    {
        public string Name { get; }
        public int Id { get; }

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            var fields = typeof(T).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Enumeration other) return false;
            return GetType() == other.GetType() && Id == other.Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public int CompareTo(object? other) => Id.CompareTo(((Enumeration)other!).Id);
    }
}
