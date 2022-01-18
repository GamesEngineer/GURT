using System.Collections.Generic;
using System.Numerics;

namespace GURT
{
    public interface ILight
    {
        Color Sample(Vector3 point, List<ISceneObject> sceneObjects, ISceneObject ignore, out Vector3 delta);
    }
}
