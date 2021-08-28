using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UILib
{
    public class MovableWindow : UIBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private Vector2 _cachedDragPosition;
        private Vector2 _cachedMousePosition;
        private bool _pointerDownCalled = false;

        public event Action<PointerEventData> onPointerDown;
        public event Action<PointerEventData> onDrag;
        public event Action<PointerEventData> onPointerUp;

        public RectTransform toDrag;

        protected override void Awake()
        {
            base.Awake();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            this._pointerDownCalled = true;
            this._cachedDragPosition = this.toDrag.position;
            this._cachedMousePosition = Input.mousePosition;
            this.onPointerDown?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (this._pointerDownCalled == false)
                return;
            this.toDrag.position = this._cachedDragPosition + ((Vector2)Input.mousePosition - this._cachedMousePosition);
            this.onDrag?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (this._pointerDownCalled == false)
                return;
            this._pointerDownCalled = false;
            this.onPointerUp?.Invoke(eventData);
        }
    }
}
