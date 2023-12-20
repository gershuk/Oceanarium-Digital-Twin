using UnityEngine;

namespace Aqua.Utils
{
    public static class EulerUtils
    {
        public static Vector3 SmoothDampEuler (Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime) =>
            Time.deltaTime == 0
                    ? current
                    : smoothTime == 0
                        ? target
                        : new(Mathf.SmoothDampAngle(current.x, target.x, ref currentVelocity.x, smoothTime),
                              Mathf.SmoothDampAngle(current.y, target.y, ref currentVelocity.y, smoothTime),
                              Mathf.SmoothDampAngle(current.z, target.z, ref currentVelocity.z, smoothTime));
    }
}