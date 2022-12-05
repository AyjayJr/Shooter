using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private ModalWindowPopup popupRef;
    private ModalWindowPopup popup;

    public ModalWindowPopup CreatePopup()
    {
        if (!popup)
            popup = Instantiate(popupRef, null);
        GameManager.Instance.TogglePause();
        return popup;
    }

    public bool IsPopupActive()
    {
        if (popup)
            return true;
        return false;
    }
}
