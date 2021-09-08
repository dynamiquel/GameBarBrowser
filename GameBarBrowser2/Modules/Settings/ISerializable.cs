using System;

namespace GameBarBrowser2
{
    public interface ISerializable
    {
        event Action<ISerializable> Modified;
    }
}
