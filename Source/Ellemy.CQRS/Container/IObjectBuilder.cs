using System;
using System.Collections.Generic;

namespace Ellemy.CQRS.Container
{
    public interface IObjectBuilder
    {
        IEnumerable<object> BuildAll(Type type);
        IEnumerable<T> BuildAll<T>();
    }
}