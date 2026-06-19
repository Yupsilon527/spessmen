using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenuController : MonoBehaviour
{
    public Transform parent;
    public GameObject playerButtonPrefab;
    public GameObject playerDividerPrefab;
    List<GameObject> entries = new();
    List<GameObject> dividers = new();
    Button closeButton;

    public delegate bool PlayerMenuAction();
    RectTransform rectTransform;

  
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            ShutDownMenu();
        }
    }
    public void OpenAtPosition(string[] Names, PlayerMenuAction[] ButtonAction, Vector3 Position)
    {
        Open();
        LoadEntries(Names, ButtonAction);
        //MoveToPosition(((Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) / 2f));
        MoveToPosition(Position);
    }
    public void OpenAtTarget(string[] Names, PlayerMenuAction[] ButtonAction, Transform target)
    {
        Open();
        LoadEntries(Names, ButtonAction);
        //MoveToPosition(((Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) / 2f));
        MoveToTarget(target);
    }
    public void MoveToTarget(Transform target)
    {
        MoveToPosition(target.position);
        RotateToPosition(target.rotation);
    }

    float rectHeight = 0;

    public string[] Names;
    public PlayerMenuAction[] Actions;
    public void LoadEntries(string[] names, PlayerMenuAction[] buttonAction)
    {
        ClearList();
        Names = names;
        int nEntries = Names.Length;
        if (buttonAction != null)
        {
            Actions = buttonAction;
            nEntries = Mathf.Min(Names.Length, Actions.Length);
        
        }

        if (nEntries > 0)
        {
            for (int i = 0; i< nEntries; i++)
            {
                if (Names[i] == "-")
                    PoolDivider(i);
                else
                    PoolButton(i);
            }

        }
        else { Close(); }
    }
    void PoolDivider(int pos)
    {
        foreach (var div in dividers)
        {
            if (div != null && !div.activeSelf)
            {
                div.SetActive(true);
                div.transform.SetAsLastSibling() ;
                return;
            }
        }
        GameObject d = Instantiate(playerDividerPrefab, transform);
        dividers.Add(d);
        d.SetActive(true);
        d.transform.SetAsLastSibling();
    }
    void PoolButton(int pos)
    {
        foreach (var div in entries)
        {
            if (div != null && !div.activeSelf)
            {
                div.SetActive(true);
                div.transform.SetAsLastSibling();
                AssignButton(div, pos);
                return;
            }
        }

        GameObject d = Instantiate(playerDividerPrefab, transform);
        dividers.Add(d);
        d.SetActive(true);
        d.transform.SetAsLastSibling();
        AssignButton(d, pos);
    }
    void AssignButton(GameObject listle, int i)
    {
        listle.name = "Entry " + i;
        Button lBtn = listle.GetComponent<Button>();
        listle.GetComponentInChildren<TextMeshProUGUI>().text = Names[i];
        if (Actions != null)
        {
            lBtn.enabled = true;
            lBtn.onClick.RemoveAllListeners();
            lBtn.onClick.AddListener(() => { if (Actions[i]()) { Close(); } });
            if (i == 0)
                lBtn.Select();
            if (i == Names.Length - 1)
                closeButton = lBtn;
        }
        else
        {
            lBtn.enabled = false;
        }
    }
    public void MoveToPosition(Vector2 position)
    {
        parent.transform.position = position;

    }
    public void RotateToPosition(Quaternion rotation)
    {
        parent.transform.rotation = rotation;
    }
    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void OnDisable()
    {
        ClearList();
    }
    void ClearList()
    {
        foreach (GameObject div in dividers)
        { div.SetActive(false); }
        foreach (GameObject listle in entries)
        { listle.SetActive(false); }
    }
    public void ShutDownMenu()
    {
        if (closeButton != null)
        {
            closeButton.onClick.Invoke();
        }
    }
}