using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using UnityEngine.Video;

public class ModalWindowPopup : MonoBehaviour
{
    #region Properties
    [Header("Header")]
    [SerializeField] private Transform headerArea;
    [SerializeField] private TextMeshProUGUI titleField;

    [Header("Horizontal")]
    [SerializeField] private Transform horizontalLayoutArea;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI iconText;
    public TextMeshProUGUI HorizontalMainText { get => iconText; set => iconText = value; }

    [Header("Vertical")]
    [SerializeField] private Transform contentArea;
    [SerializeField] private Transform verticalLayoutArea;
    [SerializeField] private RawImage mainImage;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private TextMeshProUGUI imageText;

    [Header("Footer")]
    [SerializeField] private Transform footerArea;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI confirmText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button altButton;
    [SerializeField] private TextMeshProUGUI altText;
    #endregion

    #region Private Fields
    // assigned in functions
    private Action onConfirmAction;
    private Action onCancelAction;
    private Action onAltAction;
    private GameObject customGameObjectToDestroy = null;
    #endregion

    #region LifeCycle

    void Start()
    {
        confirmButton.onClick.AddListener(Confirm);
        closeButton.onClick.AddListener(Cancel);
        altButton.onClick.AddListener(Alt);
    }
    #endregion

    #region Public Methods
    public void ShowAsVertical(string title, VideoClip clip, RenderTexture videoToShow, string message, string confirmButtonText, string cancelButtonText,
        Action confirmCallback, Action cancelCallback, string altButtonText = "", Action altCallback = null)
    {
        horizontalLayoutArea.gameObject.SetActive(false);
        verticalLayoutArea.gameObject.SetActive(true);

        bool hasTitle = !string.IsNullOrEmpty(title);
        headerArea.gameObject.SetActive(hasTitle);
        titleField.text = title;

        mainImage.texture = videoToShow;
        mainImage.SetNativeSize();
        videoPlayer.targetTexture = videoToShow;
        videoPlayer.clip = clip;
        videoPlayer.Play();
        imageText.text = message;

        onConfirmAction = confirmCallback;
        confirmText.text = confirmButtonText;

        bool hasCancel = cancelCallback != null;
        closeButton.gameObject.SetActive(true);
        onCancelAction = cancelCallback;

        bool hasAlt = altCallback != null;
        altButton.gameObject.SetActive(hasAlt);
        altText.text = altButtonText;
        onAltAction = altCallback;

        gameObject.SetActive(true);
    }
    public void ShowAsVertical(string title, VideoClip clip, RenderTexture videoToShow, string message, Action confirmCallback)
    {
        ShowAsVertical(title, clip, videoToShow, message, "OK", "", confirmCallback, null);
    }
    public void ShowAsVertical(string title, VideoClip clip, RenderTexture videoToShow, string message, Action confirmCallback, Action cancelCallback)
    {
        ShowAsVertical(title, clip, videoToShow, message, "Confirm", "Cancel", confirmCallback, cancelCallback);
    }

    public void ShowAsHorizontal(string title, Sprite icon, string message, string confirmButtonText, string cancelButtonText,
        Action confirmCallback, Action cancelCallback, string altButtonText = "", Action altCallback = null)
    {
        horizontalLayoutArea.gameObject.SetActive(true);
        verticalLayoutArea.gameObject.SetActive(false);

        bool hasTitle = string.IsNullOrEmpty(title);
        headerArea.gameObject.SetActive(hasTitle);
        titleField.text = title;

        iconImage.sprite = icon;
        iconText.text = message;

        onConfirmAction = confirmCallback;
        confirmText.text = confirmButtonText;

        bool hasCancel = cancelCallback != null;
        closeButton.gameObject.SetActive(hasCancel);
        onCancelAction = cancelCallback;

        bool hasAlt = altCallback != null;
        altButton.gameObject.SetActive(hasAlt);
        altText.text = altButtonText;
        onAltAction = altCallback;

        gameObject.SetActive(true);
    }
    public void ShowAsHorizontal(string title, Sprite icon, string message, Action confirmCallback)
    {
        ShowAsHorizontal(title, icon, message, "OK", "", confirmCallback, null);
    }
    public void ShowAsHorizontal(string title, Sprite icon, string message, Action confirmCallback, Action cancelCallback)
    {
        ShowAsHorizontal(title, icon, message, "Confirm", "Cancel", confirmCallback, cancelCallback);
    }

    // Horizontal with no image
    public void ShowAsPrompt(string title, string message, string confirmButtonText, string cancelButtonText,
            Action confirmCallback, Action cancelCallback, string altButtonText = "", Action altCallback = null)
    {
        horizontalLayoutArea.gameObject.SetActive(true);
        iconContainer.gameObject.SetActive(false);
        verticalLayoutArea.gameObject.SetActive(false);

        bool hasTitle = string.IsNullOrEmpty(title);
        headerArea.gameObject.SetActive(hasTitle);
        titleField.text = title;

        iconText.text = message;

        confirmButton.gameObject.SetActive(true);
        onConfirmAction = confirmCallback;
        confirmText.text = confirmButtonText;

        bool hasCancel = cancelCallback != null;
        closeButton.gameObject.SetActive(hasCancel);
        onCancelAction = cancelCallback;

        bool hasAlt = altCallback != null;
        altButton.gameObject.SetActive(hasAlt);
        altText.text = altButtonText;
        onAltAction = altCallback;

        gameObject.SetActive(true);
    }
    public void ShowAsPrompt(string title, string message, Action confirmCallback)
    {
        ShowAsPrompt(title, message, "OK", "", confirmCallback, null);
    }

    public TextMeshProUGUI ShowAsCoolDownPrompt(string title, string message, Action confirmCallback)
    {
        ShowAsPrompt(title, message, "OK", "", confirmCallback, null);
        return iconText;
    }

    public void ShowAsPrompt(string title, string message, Action confirmCallback, Action cancelCallback)
    {
        ShowAsPrompt(title, message, "Confirm", "Cancel", confirmCallback, cancelCallback);
    }

    #endregion

    #region Private Methods
    private void Confirm()
    {
        onConfirmAction?.Invoke();
        Cleanup();
    }
    private void Cancel()
    {
        onCancelAction?.Invoke();
        GameManager.Instance.TogglePause();
        Cleanup();
    }
    private void Alt()
    {
        onAltAction?.Invoke();
        Cleanup();
    }

    private IEnumerator WaitAndClose(float delay)
    {
        yield return new WaitForSeconds(delay);
        Cleanup();
    }

    private void Cleanup()
    {
        if (customGameObjectToDestroy)
        {
            Destroy(customGameObjectToDestroy);
            onAltAction?.Invoke();
        }
        Destroy(this.gameObject);
    }
    #endregion
}