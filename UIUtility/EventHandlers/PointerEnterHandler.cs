﻿using System;
using UnityEngine.EventSystems;

namespace UILib.EventHandlers
{
    public class PointerEnterHandler : UIBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<PointerEventData> onPointerEnter;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this.onPointerEnter != null)
                this.onPointerEnter(eventData);
        }

        public event Action<PointerEventData> onPointerExit;

        public void OnPointerExit(PointerEventData eventData)
        {
            if (this.onPointerExit != null)
                this.onPointerExit(eventData);
        }
    }
}
