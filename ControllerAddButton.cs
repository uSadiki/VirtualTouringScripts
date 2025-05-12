using UnityEngine;

public class ControllerAddButton : MonoBehaviour
{
    public MenuManager menuManager;
     public KeyCode addMenuKey;

    void Update()
    {
        if (Input.GetKeyDown(addMenuKey))
        {
            menuManager?.ShowAddPanel();
        }
    }
}