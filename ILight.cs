using System.Collections.Generic;
using System.Numerics;

namespace GURT
{
    public interface ILight
    {
        Color Sample(Vector3 point, List<ISceneObject> sceneObjects, out Vector3 delta);
    }
}
