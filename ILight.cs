using System;
using System.Drawing;
using System.Numerics;

namespace GURT
{
    public interface ILight
    {
        Color Sample(Vector3 point, out Vector3 delta);
    }
}
