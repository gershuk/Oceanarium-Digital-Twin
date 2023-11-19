#nullable enable

using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    /// <summary>
    /// Class that encapsulates <see cref="UnityEngine.UI"/> object creation functions.
    /// </summary>
    public static class GUIFactory
    {
        public readonly struct ButtonParameters
        {
            public Image? Image { get; }
            public string Name { get; }
            public UnityAction OnClickFunction { get; }
            public RectTransformParameters RectTransformParameters { get; }

            public ButtonParameters (UnityAction onClickFunction,
                                     string? name = default,
                                     in RectTransformParameters rectTransformParameters = default,
                                     Image? image = default)
            {
                Name = name ?? string.Empty;
                RectTransformParameters = rectTransformParameters;
                Image = image;
                OnClickFunction = onClickFunction;
            }
        }

        public readonly struct CanvasParameters
        {
            public Camera Camera { get; }

            public RectTransformParameters RectTransformParameters { get; }

            public RenderMode RenderMode { get; }

            public CanvasParameters (RectTransformParameters rectTransformParameters, RenderMode renderMode, Camera camera)
            {
                RectTransformParameters = rectTransformParameters;
                RenderMode = renderMode;
                Camera = camera != null ? camera : throw new ArgumentNullException(nameof(camera));
            }
        }

        public readonly struct RectTransformParameters
        {
            public Vector2 AnchoredPosition { get; }
            public Vector3? AnchoredPosition3D { get; }
            public Vector2 AnchorMax { get; }
            public Vector2 AnchorMin { get; }
            public Vector3? EulerAngles { get; }
            public Transform Parent { get; }
            public Vector2 SizeDelta { get; }

            public RectTransformParameters (Transform parent,
                                            Vector2 anchorMin,
                                            Vector2 anchorMax,
                                            Vector2 sizeDelta,
                                            Vector2 anchoredPosition,
                                            Vector3? anchoredPosition3D = default,
                                            Vector3? eulerAngles = default)
            {
                Parent = parent;
                AnchorMin = anchorMin;
                AnchorMax = anchorMax;
                SizeDelta = sizeDelta;
                AnchoredPosition = anchoredPosition;
                AnchoredPosition3D = anchoredPosition3D;
                EulerAngles = eulerAngles;
            }
        }

        public readonly struct TextParameters
        {
            public TextAnchor Anchor { get; }
            public Color Color { get; }
            public Font Font { get; }
            public RectTransformParameters RectTransformParameters { get; }
            public int Size { get; }
            public string Text { get; }

            public TextParameters (string text,
                                   Color color,
                                   Font font,
                                   TextAnchor anchor,
                                   int size,
                                   in RectTransformParameters rectTransformParameters)
            {
                Text = text ?? string.Empty;
                Color = color;
                Font = font != null ? font : throw new ArgumentNullException(nameof(font));
                Anchor = anchor;
                Size = size;
                RectTransformParameters = rectTransformParameters;
            }
        }

        public readonly struct Vector3ViewParameters
        {
            public string Name { get; }
            public RectTransformParameters RectTransformParameters { get; }
            public Vector3 Vector { get; }

            public Vector3ViewParameters (Vector3 vector, string name, RectTransformParameters rectTransformParameters) =>
                (Vector, Name, RectTransformParameters) = (vector, name, rectTransformParameters);
        }

        private static GameObject? _vector3ViewPrefab;

        public static GameObject CreateButton (in ButtonParameters parameters)
        {
            GameObject newButton = new($"{parameters.Name}Button", typeof(Image), typeof(Button), typeof(LayoutElement));

            var buttonComponent = newButton.GetComponent<Button>();
            if (parameters.OnClickFunction != null)
                buttonComponent.onClick.AddListener(parameters.OnClickFunction);
            if (parameters.Image != null)
                buttonComponent.image = parameters.Image;

            newButton.GetComponent<RectTransform>().SetUpRectTransform(parameters.RectTransformParameters);
            return newButton;
        }

        public static GameObject CreateCanvas (in CanvasParameters parameters)
        {
            GameObject newCanvas = new("Canvas", typeof(Canvas), typeof(GraphicRaycaster));
            newCanvas.GetComponent<RectTransform>().SetUpRectTransform(parameters.RectTransformParameters);
            var canvas = newCanvas.GetComponent<Canvas>();
            canvas.renderMode = parameters.RenderMode;
            canvas.worldCamera = parameters.Camera;
            return newCanvas;
        }

        public static GameObject CreateText (in TextParameters parameters)
        {
            GameObject newText = new($"{parameters.Text}Text", typeof(Text));
            newText.GetComponent<RectTransform>().SetUpRectTransform(parameters.RectTransformParameters);
            var textComponent = newText.GetComponent<Text>();
            textComponent.text = $"{parameters.Text}";
            textComponent.color = parameters.Color;
            textComponent.font = parameters.Font;
            textComponent.alignment = parameters.Anchor;
            textComponent.fontSize = parameters.Size;
            return newText;
        }

        public static GameObject CreateVector3View (in Vector3ViewParameters parameters)
        {
            _vector3ViewPrefab ??= (Resources.Load<GameObject>(@"Prefabs\Vector3View"));
            var vector3View = UnityEngine.Object.Instantiate(_vector3ViewPrefab);

            var vector3ViewComponent = vector3View.GetComponent<Vector3View>();
            vector3ViewComponent.Name = parameters.Name;
            vector3ViewComponent.Vector = parameters.Vector;

            vector3View.GetComponent<RectTransform>().SetUpRectTransform(parameters.RectTransformParameters);

            return vector3View;
        }

        public static void SetUpRectTransform (this RectTransform rectTransform, in RectTransformParameters parameters)
        {
            rectTransform.SetParent(parameters.Parent);
            rectTransform.anchorMin = parameters.AnchorMin;
            rectTransform.anchorMax = parameters.AnchorMax;
            rectTransform.sizeDelta = parameters.SizeDelta;
            if (parameters.AnchoredPosition3D.HasValue)
                rectTransform.anchoredPosition3D = parameters.AnchoredPosition3D.Value;
            else
                rectTransform.anchoredPosition = parameters.AnchoredPosition;

            if (parameters.EulerAngles.HasValue)
                rectTransform.eulerAngles = parameters.EulerAngles.Value;
        }
    }
}