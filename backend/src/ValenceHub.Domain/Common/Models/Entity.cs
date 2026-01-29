using System;
using System.Collections.Generic;
using System.Text;

namespace ValenceHub.Domain.Common.Models;

public abstract class Entity<T>
{
    public T Id { get; protected set; }

    protected Entity() { }

    protected Entity(T id)
    {
        Id = id;
    }
}
