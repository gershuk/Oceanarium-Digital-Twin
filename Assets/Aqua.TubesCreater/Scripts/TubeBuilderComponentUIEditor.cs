using UnityEditor;

using UnityEngine;

namespace Aqua.TubesCreater.Builders
{
#if UNITY_EDITOR
    [CustomEditor(typeof(TubeBuilderComponent))]
    public class TubeBuilderComponentUIEditor : Editor
    {
        private Quaternion _handle;

        private void OnSceneGUI ()
        {
            var tubeBuilder = target as TubeBuilderComponent;
            var points = tubeBuilder.Points;
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
                    tubeBuilder.RebuildCurve();
                }
            }
        }
    }
#endif
}