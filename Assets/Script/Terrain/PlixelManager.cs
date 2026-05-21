using System.Collections;
using UnityEngine;

public class PlixelManager : Initializable
{
    public static int process = 0;
    public PlixelMapMob parent;
    public RectInt dirtyRect;
    public RectInt workRect;
    public bool dirty = false;

    public virtual void OnCreated()
    {
        dirtyRect = new RectInt(0, 0, parent._width - 1, parent._height - 1);
        Modify();
    }

    public void NotifyModified(RectInt modifiedArea)
    {
        if (dirty)
        {
            dirtyRect.xMin = Mathf.Min(dirtyRect.xMin, modifiedArea.xMin);
            dirtyRect.xMax = Mathf.Max(dirtyRect.xMax, modifiedArea.xMax);
            dirtyRect.yMin = Mathf.Min(dirtyRect.yMin, modifiedArea.yMin);
            dirtyRect.yMax = Mathf.Max(dirtyRect.yMax, modifiedArea.yMax);
        }
        else
        {
            dirtyRect = modifiedArea;
        }
        dirty = true;
    }

    protected void Modify()
    {
        if (dirty && IsReady())
        {
            workRect = dirtyRect;
            workRect.xMin = Mathf.Max(0, dirtyRect.xMin);
            workRect.yMin = Mathf.Max(0, dirtyRect.yMin);
            workRect.xMax = Mathf.Min(parent.GetWidth(), dirtyRect.xMax);
            workRect.yMax = Mathf.Min(parent.GetHeight(), dirtyRect.yMax);

            dirty = false;
            dirtyRect = default;
            work = StartCoroutine(ModifyCoroutine());
        }

    }
    Coroutine work;
    public bool IsReady()
    {
        return work == null;
    }
    protected virtual IEnumerator ModifyCoroutine()
    {
        yield return null;
        EndWork();
    }
    public


        bool Step()
    {
        if ( ++process > 3000)
        {
            process = 0;
            return true;
        }
        return false;
    }
    public void EndWork()
    {
        work = null;
    }
}
