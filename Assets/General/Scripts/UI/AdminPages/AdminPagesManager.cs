using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminPagesManager : MonoBehaviour
{
    public GameObject tabButtonPrefab;
    public Transform tabButtonContainer;
    private Button currentButton;
    public Page[] pages;
    private GameObject selectedPage;
    // Start is called before the first frame update
    void OnEnable()
    {
        if(tabButtonContainer.childCount < 2) SpawnTabButtons();

        if(selectedPage != null) selectedPage.SetActive(true);
        else selectedPage = pages[pages.Length-1].page;
    }

    private void SpawnTabButtons()
    {
        for (int p = 0; p < pages.Length; p++)
        {
            GameObject newButton = Instantiate(tabButtonPrefab, tabButtonContainer);

            // change tab icon
            Image btnIcon = newButton.transform.Find("Icon").GetComponent<Image>();
            btnIcon.sprite = pages[p].icon;

            // change tab label
            TMPro.TextMeshProUGUI label = newButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            label.text = pages[p].page.name;

            // Assign event ot button On Click ()            
            Button btn = newButton.GetComponent<Button>();
            int index = p;
            btn.onClick.AddListener(delegate{
                currentButton.interactable = true;
                selectedPage.SetActive(false);
                currentButton.transform.Find("Selectable Image").gameObject.SetActive(true);
               // CloseAllPages();
                 pages[index].page.SetActive(true);
                 selectedPage = pages[index].page;
                 currentButton = btn;
                 currentButton.interactable = false;
            });
        }

        currentButton = tabButtonContainer.GetComponentInChildren<Button>();
    }

    public void CloseAdminPages()
    {
        CloseAllPages();
        gameObject.SetActive(false);
    }

    private void CloseAllPages()
    {
        for (int p = 0; p < pages.Length; p++)
        {
            pages[p].page.SetActive(false);
        }
    }
}

[System.Serializable]
public struct Page
{
    public GameObject page;
    public Sprite icon;
}