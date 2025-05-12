using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Prefabs")]
    public GameObject mainMenuPrefab;
    public GameObject addPanelPrefab;

    private GameObject currentMainMenu;
    private GameObject addPanelInstance;

    private GameObject selectedFurniture;
    private Renderer selectedFurnitureRenderer;
    private Material originalMaterial;

    public GameObject SelectedFurniture => selectedFurniture;

    // ============ MAIN MENU ============

    public void ShowMainMenu(GameObject newFurniture)
    {
        // Unselect any old item
        UnselectFurniture();

        // Highlight the new piece
        selectedFurniture = newFurniture;
        HighlightFurniture(selectedFurniture, true);

        // Destroy old main menu if present
        if (currentMainMenu != null)
        {
            Destroy(currentMainMenu);
            currentMainMenu = null;
        }

        // Spawn a fresh main menu
        currentMainMenu = Instantiate(mainMenuPrefab, GetSpawnPosition(), Quaternion.identity);
        currentMainMenu.transform.LookAt(Camera.main.transform, Vector3.up);
        currentMainMenu.transform.Rotate(0f, 180f, 0f);
        currentMainMenu.transform.localScale = Vector3.one * 0.005f;

        var mmActions = currentMainMenu.GetComponentInChildren<MainMenuActions>(true);
        if (mmActions != null)
            mmActions.menuManager = this;
    }

    public void RemoveSelectedFurniture()
    {
        if (selectedFurniture != null)
        {
            Destroy(selectedFurniture);
            selectedFurniture = null;
        }
        HideMainMenu();
    }

    public void HideMainMenu()
    {
        if (currentMainMenu != null)
        {
            Destroy(currentMainMenu);
            currentMainMenu = null;
        }
        UnselectFurniture();
    }

    // ============ ADD PANEL (OPEN/CLOSE MULTIPLE TIMES) ============

    public void ShowAddPanel()
    {
        // If we haven't created the Add Panel yet, instantiate it once
        if (addPanelInstance == null)
        {
            addPanelInstance = Instantiate(addPanelPrefab);
            addPanelInstance.SetActive(false);

            // Link references
            var addCtrl = addPanelInstance.GetComponentInChildren<AddPanelController>(true);
            if (addCtrl != null)
                addCtrl.menuManager = this;
        }


        // Position it in front of the camera each time
        var cam = Camera.main;
        if (cam != null)
        {
            Vector3 spawnPos = cam.transform.position +
                               cam.transform.forward * 1.5f +
                               cam.transform.up * -0.2f;

            addPanelInstance.transform.position = spawnPos;
            addPanelInstance.transform.LookAt(cam.transform, Vector3.up);
            addPanelInstance.transform.Rotate(0f, 180f, 0f);
        }

        // scale it
        addPanelInstance.transform.localScale = Vector3.one * 0.005f;

        // activate it
        addPanelInstance.SetActive(true);

        // Force the AddPanel to refresh its items
        var addController = addPanelInstance.GetComponentInChildren<AddPanelController>(true);
        if (addController != null)
        {
            // call OnCategoryChanged manually or rely on OnEnable
            addController.OnCategoryChanged(addController.categoryDropdown.value);
        }
    }

    public void HideAddPanel()
    {
        if (addPanelInstance != null)
            addPanelInstance.SetActive(false);
    }

    /// <summary>
    /// Called by AddPanelController when user clicks a furniture item to spawn
    /// </summary>
    public void AddNewFurniture(GameObject prefab)
    {
        if (!prefab) return;

        var cam = Camera.main;
        if (!cam) return;

        Vector3 spawnPos = cam.transform.position + cam.transform.forward * 2f;
        GameObject newObj = Instantiate(prefab, spawnPos, Quaternion.identity);

        var grab = newObj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Attach an OnActivate listener in code:
        //grab.activated.AddListener((args) => ShowMainMenu(newObj));
        grab.activated.AddListener((args) => ShowMainMenu(newObj));


        // Hide the Add Panel now that you spawned the furniture
        HideAddPanel();
    }

    // ============ HIGHLIGHT & UTILS ============

    public void UnselectFurniture()
    {
        if (selectedFurniture != null)
        {
            HighlightFurniture(selectedFurniture, false);
            selectedFurniture = null;
        }
    }

    private void HighlightFurniture(GameObject obj, bool highlight)
    {
        var rend = obj.GetComponentInChildren<Renderer>();
        if (!rend) return;

        if (highlight)
        {
            selectedFurnitureRenderer = rend;
            originalMaterial = rend.material;
            var highlightMat = new Material(originalMaterial);
            highlightMat.color = Color.yellow;
            rend.material = highlightMat;
        }
        else
        {
            if (selectedFurnitureRenderer && originalMaterial)
            {
                selectedFurnitureRenderer.material = originalMaterial;
            }
        }
    }

    private Vector3 GetSpawnPosition()
    {
        var cam = Camera.main;
        if (!cam) return Vector3.zero;

        Vector3 cp = cam.transform.position;
        Vector3 fwd = cam.transform.forward;
        float dist = 1.2f;
        Vector3 pos = cp + fwd * dist;
        pos.y = cp.y;
        return pos;
    }
}