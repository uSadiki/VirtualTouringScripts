using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AddPanelController : MonoBehaviour
{
    public MenuManager menuManager;

    public TMP_Dropdown categoryDropdown;
    public Transform contentParent; 
    public Button backButton;

    [System.Serializable]
    public class FurnitureItem
    {
        public string itemName;
        public Sprite thumbnailSprite;
        public GameObject prefab;
    }

    public List<FurnitureItem> beds;
    public List<FurnitureItem> chairs;
    public List<FurnitureItem> sofas;
    public List<FurnitureItem> tables;
    public List<FurnitureItem> tvs;
    public List<FurnitureItem> tvstands;

    private void Awake()
    {
        categoryDropdown.onValueChanged.AddListener(OnCategoryChanged);

        if (backButton != null)
            backButton.onClick.AddListener(() => menuManager?.HideAddPanel());
    }

    private void OnEnable()
    {
        // Refresh items
        OnCategoryChanged(categoryDropdown.value);
    }

    public void OnCategoryChanged(int idx)
    {
        // Make sure the scrollview is empty
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Get the items corresponding the dropdown value
        var cat = categoryDropdown.options[idx].text;
        var items = GetItemsForCategory(cat);
        if (items == null || items.Count == 0) return;

        // Fill the scrollview
        foreach (var fi in items)
        {
            GameObject itemGO = new GameObject(fi.itemName,
                typeof(RectTransform), typeof(CanvasRenderer),
                typeof(Image), typeof(Button));
            itemGO.transform.SetParent(contentParent, false);

            var rt = itemGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(200, 100);
            rt.localScale = Vector3.one;

            var img = itemGO.GetComponent<Image>();
            img.sprite = fi.thumbnailSprite;
            img.preserveAspect = true;

            var btn = itemGO.GetComponent<Button>();
            btn.onClick.AddListener(() => OnClickItem(fi.prefab));
        }
    }

    private List<FurnitureItem> GetItemsForCategory(string catName)
    {
        switch(catName)
        {
            case "Beds":      return beds;
            case "Chairs":    return chairs;
            case "Sofas":     return sofas;
            case "Tables":    return tables;
            case "TVs":       return tvs;
            case "TV Stands": return tvstands;
            default:          return null;
        }
    }

    private void OnClickItem(GameObject pf)
    {
        if (menuManager != null)
            menuManager.AddNewFurniture(pf);
    }
}
