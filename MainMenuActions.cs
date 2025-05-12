using UnityEngine;
using UnityEngine.UI;

public class MainMenuActions : MonoBehaviour
{
    public Button removeButton;
    public Button closeButton;
    public MenuManager menuManager;

    private void Awake()
    {
        if (removeButton != null)
            removeButton.onClick.AddListener(OnClickRemove);

        if (closeButton != null)
            closeButton.onClick.AddListener(OnClickClose);
    }

    private void OnClickRemove()
    {
        menuManager?.RemoveSelectedFurniture();
    }

    private void OnClickClose()
    {
        menuManager?.HideMainMenu();
    }
}
