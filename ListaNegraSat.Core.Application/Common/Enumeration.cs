﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable InconsistentNaming
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace ListaNegraSat.Core.Application.Common;

/// <summary>
///     https://github.com/dotnet-architecture/eShopOnContainers/blob/dev/src/Services/Ordering/Ordering.Domain/SeedWork/Enumeration.cs
/// </summary>
public abstract class Enumeration : IComparable
{
    protected Enumeration(int id, string name)
    {
        (Id, Name) = (id, name);
    }

    public string Name { get; private set; }

    public int Id { get; private set; }

    public int CompareTo(object other)
    {
        return Id.CompareTo(((Enumeration)other).Id);
    }

    public override string ToString()
    {
        return Name;
    }

    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(f => f.GetValue(null))
            .Cast<T>();
    }

    public override bool Equals(object obj)
    {
        if (obj is not Enumeration otherValue) return false;

        bool typeMatches = GetType().Equals(obj.GetType());
        bool valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        int absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
        return absoluteDifference;
    }

    public static T FromValue<T>(int value) where T : Enumeration
    {
        T matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
        return matchingItem;
    }

    public static T FromDisplayName<T>(string displayName) where T : Enumeration
    {
        T matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
        return matchingItem;
    }

    private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
    {
        T matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem == null)
            throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

        return matchingItem;
    }
}