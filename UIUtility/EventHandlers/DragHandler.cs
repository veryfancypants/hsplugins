using System;
using UnityEngine.EventSystems;

namespace UILib.EventHandlers
{
    public class DragHandler : UIBehaviour, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action<PointerEventData> onBeginDrag;
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (this.onBeginDrag != null)
                this.onBeginDrag(eventData);
        }

        public event Action<PointerEventData> onInitializePotentialDrag;
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (this.onInitializePotentialDrag != null)
                this.onInitializePotentialDrag(eventData);
        }

        public event Action<PointerEventData> onDrag;
        public void OnDrag(PointerEventData eventData)
        {
            if (this.onDrag != null)
                this.onDrag(eventData);
        }

        public event Action<PointerEventData> onEndDrag;
        public void OnEndDrag(PointerEventData eventData)
        {
            if (this.onEndDrag != null)
                this.onEndDrag(eventData);
        }
    }
}
