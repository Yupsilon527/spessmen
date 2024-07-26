using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerMenuController : MonoBehaviour
{
    List<GameObject> entries;

    public delegate bool PlayerMenuAction();
    RectTransform rectTransform;

    private void Awake()
    {
        entries = new List<GameObject> { transform.GetChild(0).gameObject };

        rectTransform = entries[0].GetComponent<RectTransform>();
    }
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
    public void OpenAtTarget(string[] Names, PlayerMenuAction[] ButtonAction, Transform Target)
    {
        Open();
        LoadEntries(Names, ButtonAction);
        //MoveToPosition(((Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) / 2f));
        MoveToPosition(Target.position);
        RotateToPosition(Target.rotation);
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
            Rect posRect = new Rect(transform.position.x, transform.position.y, rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);

            rectHeight = 0;
            for (int i = 0; i < Names.Length; i++)
            {
                if (Names[i] == "-")
                {
                    if (i > 0 && Names[i - 1] != "-")
                        rectHeight += posRect.height * .3f;
                }
                else
                {
                    rectHeight += posRect.height;
                }
            }

            //GetComponent<RectTransform>().sizeDelta = new Vector2(posRect.width, rectHeight);

            rectHeight = 0;
            int Delta = 0;
            for (int i = 0; i < Names.Length; i++)
            {
                string Zim = Names[i];
                if (Names[i] == "-")
                {
                    Delta++;
                    if (i > 0 && Names[i - 1] != "-")
                        rectHeight += posRect.height * .3f;
                }
                else
                {
                    int Value = i - Delta;
                    GameObject listle = i < entries.Count ? entries[i] : null;

                    if (listle == null)
                    {
                        listle = GameObject.Instantiate(entries[0]);
                        listle.transform.SetParent(transform);
                        entries.Add(listle);
                    }
                    listle.SetActive(true);
                    listle.name = "Entry " + Value;

                    RectTransform tRect = listle.GetComponent<RectTransform>();
                    tRect.anchoredPosition = Vector2.down * (rectHeight - (tRect.rect.height * .5f * nEntries));
                    tRect.localScale = Vector3.one;
                    tRect.localRotation = Quaternion.identity;
                    tRect.sizeDelta = entries[0].GetComponent<RectTransform>().sizeDelta;

                    listle.GetComponentInChildren<TextMeshProUGUI>().text = Zim;

                    Button lBtn = listle.GetComponent<Button>();
                    
                    if (Actions != null)
                    {
                        lBtn.enabled = true;
                        lBtn.onClick.RemoveAllListeners();
                        lBtn.onClick.AddListener(() => { if (Actions[Value]()) { Close(); } });
                        if (i == 0)
                            lBtn.Select();
                        if (i == Names.Length - 1)
                            closeButton = lBtn;
                    }
                    else
                    {
                        lBtn.enabled = false;
                    }
                    rectHeight += posRect.height;
                }
            }

            /*for (int i = 0; i < entries.Count; i++)
            {
                GameObject btnT = entries[i < 1 ? (entries.Count - 1) : 1];
                GameObject btnM = entries[i];
                GameObject btnC = entries[(i + 1) % entries.Count];

                if (btnM.gameObject.TryGetComponent(out Button btnComp))
                {
                    Navigation navigation = new Navigation();

                    navigation.mode = Navigation.Mode.Explicit;
                    navigation.selectOnUp = btnT.GetComponent<Button>();
                    navigation.selectOnDown = btnT.GetComponent<Button>();

                    btnComp.navigation = navigation;
                }
            }*/
            transform.position += rectHeight * .5f * Vector3.up;
        }
        else { Close(); }
    }
    public void MoveToPosition(Vector2 position)
    {
        RectTransform rT = GetComponent<RectTransform>();
        rT.position = position;

        /*Vector2 dims = new Vector2(
            (Screen.width / rT.parent.localScale.x - rT.rect.width) ,
            (Screen.height / rT.parent.localScale.y - rT.rect.height) 
            ) * .5f;

        rT.anchoredPosition = new Vector2(
                Mathf.Clamp(position.x / rT.parent.localScale.x + rT.rect.width*.5f, -dims.x, dims.x) ,
                Mathf.Clamp(position.y / rT.parent.localScale.y - rT.rect.height * .5f, -dims.y, dims.y)
                );*/
    }
    public void RotateToPosition(Quaternion rotation)
    {
        transform.rotation = rotation;
        //transform.position += transform.up * Names.Length * .5f * rectHeight;
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
        foreach (GameObject listle in entries)
        { listle.SetActive(false); }
    }
    Button closeButton;
    public void ShutDownMenu()
    {
        if (closeButton != null)
        {
            closeButton.onClick.Invoke();
        }
    }
}