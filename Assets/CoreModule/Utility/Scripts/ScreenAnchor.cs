#pragma warning disable IDE0130

namespace Texell.Utility
{
    using UnityEngine;

    [ExecuteInEditMode]
    public class ScreenAnchor : MonoBehaviour
    {
        [SerializeField] private AnchorType _anchorType = AnchorType.TopLeft;
        [Header("Offset")]
        [SerializeField] private float _offsetX;
        [SerializeField] private OffsetUnit _offsetUnitX = OffsetUnit.Pixels;

        [SerializeField] private float _offsetY;
        [SerializeField] private OffsetUnit _offsetUnitY = OffsetUnit.Pixels;

        private Camera _camera;

#if UNITY_EDITOR
        private Vector2 _editorWindow;
#endif

        public enum AnchorType
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            Center
        }

        public enum OffsetUnit
        {
            [InspectorName("px")]
            Pixels,
            [InspectorName("%")]
            Percentage
        }

        void Start()
        {
            _camera = Camera.main;
            SetAnchor();

#if UNITY_EDITOR
            _editorWindow = _camera.pixelRect.size;
#endif

        }

        private void SetAnchor()
        {
            Vector2 offset = Vector2.zero;
            Vector3 startPoint = _camera.ViewportToWorldPoint(new Vector2(0, 0));
            Vector3 worldPoint;

            if (_offsetUnitX == OffsetUnit.Pixels)
            {
                worldPoint = _camera.ViewportToWorldPoint(new Vector2(_offsetX / _camera.pixelWidth, 0));
                offset.x = Mathf.Abs(startPoint.x - worldPoint.x) * Mathf.Sign(_offsetX);
            }
            else if (_offsetUnitX == OffsetUnit.Percentage)
            {
                worldPoint = _camera.ViewportToWorldPoint(new Vector2(_offsetX / 100f, 0));
                offset.x = Mathf.Abs(startPoint.x - worldPoint.x) * Mathf.Sign(_offsetX);
            }

            if (_offsetUnitY == OffsetUnit.Pixels)
            {
                worldPoint = _camera.ViewportToWorldPoint(new Vector2(0, _offsetY / _camera.pixelHeight));
                offset.y = -Mathf.Abs(startPoint.y - worldPoint.y) * Mathf.Sign(_offsetY);
            }
            else if (_offsetUnitY == OffsetUnit.Percentage)
            {
                worldPoint = _camera.ViewportToWorldPoint(new Vector2(0, _offsetY / 100f));
                offset.y = -Mathf.Abs(startPoint.y - worldPoint.y) * Mathf.Sign(_offsetY);
            }

            switch (_anchorType)
            {
                case AnchorType.TopLeft:
                    Vector3 topLeft = _camera.ViewportToWorldPoint(new Vector2(0, 1));
                    transform.position = new Vector3(topLeft.x + offset.x, topLeft.y + offset.y, transform.position.z);
                    break;
                case AnchorType.TopRight:
                    Vector3 topRight = _camera.ViewportToWorldPoint(new Vector2(1, 1));
                    transform.position = new Vector3(topRight.x + offset.x, topRight.y + offset.y, transform.position.z);
                    break;
                case AnchorType.BottomLeft:
                    Vector3 bottomLeft = _camera.ViewportToWorldPoint(new Vector2(0, 0));
                    transform.position = new Vector3(bottomLeft.x + offset.x, bottomLeft.y + offset.y, transform.position.z);
                    break;
                case AnchorType.BottomRight:
                    Vector3 bottomRight = _camera.ViewportToWorldPoint(new Vector2(1, 0));
                    transform.position = new Vector3(bottomRight.x + offset.x, bottomRight.y + offset.y, transform.position.z);
                    break;
                case AnchorType.Center:
                    Vector3 center = _camera.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
                    transform.position = new Vector3(center.x + offset.x, center.y + offset.y, transform.position.z);
                    break;
                default:
                    break;
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
            SetAnchor();
        }


        void Update()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
            if (_editorWindow.x != _camera.pixelWidth || _editorWindow.y != _camera.pixelHeight)
            {
                _editorWindow = _camera.pixelRect.size;
                SetAnchor();
            }
        }
#endif

    }
}