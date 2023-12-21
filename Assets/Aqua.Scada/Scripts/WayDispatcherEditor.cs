using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;

using static UnityEngine.GraphicsBuffer;

namespace Aqua.Scada
{
#if UNITY_EDITOR

    [CustomEditor(typeof(WayDispatcher))]
    public class WayDispatcherUIEditor : Editor
    {
        private Quaternion _handle;

        private void OnSceneGUI ()
        {
            var tubeBuilder = target as WayDispatcher;
            var points = tubeBuilder.PointsForEditor;
            _handle = Tools.pivotRotation == PivotRotation.Local ? tubeBuilder.transform.rotation : Quaternion.identity;
            var transform = tubeBuilder.transform;

            for (int i = 0, n = points.Count; i < n; i++)
            {
                var point = transform.TransformPoint(points[i]);
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, _handle);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(tubeBuilder, "Point");
                    EditorUtility.SetDirty(tubeBuilder);
                    points[i] = transform.InverseTransformPoint(point);
                }
            }
        }
    }

#endif
}
