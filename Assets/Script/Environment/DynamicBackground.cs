using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBackground : MonoBehaviour
{
    public enum Behavior
    {
        nothing = 0,
        looping = 1,
        border = 2,
    };
    [SerializeField]
    public Behavior loopsX;
    public Behavior loopsY;
    public Vector2 size = Vector2.zero;
    public Vector2 speed = Vector2.zero;
    public Vector2 limits = Vector2.zero;

    // Use this for initialization
    void Awake()
    {
        float oSize = Camera.main.orthographicSize;
        Vector2Int vAspect = Vector2Int.one;
        if (loopsX == Behavior.looping)
        {
            vAspect.x = Mathf.CeilToInt(Mathf.Max(oSize * Camera.main.aspect, oSize) / size.x * 2) + 4;
        }
        if (loopsY == Behavior.looping)
        {
            vAspect.y = Mathf.CeilToInt(Mathf.Min(oSize, oSize) / size.y * 2) + 4;
        }
        int nChildren = transform.childCount;
        for (int I = 0; I < nChildren; I++)
        {
            Transform tr = transform.GetChild(I);
            tr.transform.position = tr.transform.position + (Camera.main.orthographicSize * Camera.main.aspect + size.x) * Vector3.left;
        }

        for (float repeats = 1; repeats < vAspect.x; repeats++)
        {
            for (int I = 0; I < nChildren; I++)
            {
                Transform original = transform.GetChild(I);
                GameObject child = GameObject.Instantiate(original.gameObject);

                child.transform.SetParent(transform);
                child.transform.localPosition = original.transform.localPosition + Vector3.right * size.x * (vAspect.x - repeats);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 camPos = Camera.main.transform.position;

        Vector3 newPos = gameObject.transform.position;
        switch (loopsX)
        {
            case Behavior.nothing:
                newPos.x = camPos.x * speed.x;
                break;
            case Behavior.looping:
                newPos.x = camPos.x - ((camPos.x * speed.x) % (size.x)) - size.x;
                break;
            case Behavior.border:
                newPos.x = camPos.x + Mathf.Clamp(camPos.x - (camPos.x * speed.x), -limits.x, limits.x);
                break;
        }
        switch (loopsY)
        {
            case Behavior.nothing:
                newPos.y = camPos.y * speed.y;
                break;
            case Behavior.looping:
                newPos.y = camPos.y - ((camPos.y * speed.y) % (size.y)) - size.y;
                break;
            case Behavior.border:
                newPos.y = camPos.y + Mathf.Clamp((camPos.y * speed.y) - camPos.y, -limits.y, limits.y);
                break;
        }

        gameObject.transform.position = newPos;
    }
}