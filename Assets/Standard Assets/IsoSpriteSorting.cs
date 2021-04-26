using System;
using UnityEngine;

public class IsoSpriteSorting : MonoBehaviour
{
    private Transform t;

    private Vector3 SortingPoint1
    {
        get
        {
            return t.position;
        }
    }

    public Renderer[] renderersToSort;

#if UNITY_EDITOR
    public void SortScene()
    {
        IsoSpriteSorting[] isoSorters = FindObjectsOfType(typeof(IsoSpriteSorting)) as IsoSpriteSorting[];
        for (int i = 0; i < isoSorters.Length; i++)
        {
            isoSorters[i].Setup();
        }
        IsoSpriteSortingManager.UpdateSorting();
        for (int i = 0; i < isoSorters.Length; i++)
        {
            isoSorters[i].Unregister();
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
#endif

    void Awake()
    {
        if (Application.isPlaying)
        {
            _ = IsoSpriteSortingManager.Instance; //bring the instance into existence so the Update function will run;
            Setup();
        }
    }

    private void Setup()
    {
        t = transform;
        if (renderersToSort == null || renderersToSort.Length == 0)
        {
            renderersToSort = new Renderer[] { GetComponent<Renderer>() };
        }
        Register();
        Array.Sort(renderersToSort, (a, b) => a.sortingOrder.CompareTo(b.sortingOrder));
    }

    public static int CompairIsoSortersBasic(IsoSpriteSorting sprite1, IsoSpriteSorting sprite2)
    {
        float y1 = sprite1.SortingPoint1.y;
        float y2 = sprite2.SortingPoint1.y;
        return y2.CompareTo(y1);
    }

    public int RendererSortingOrder
    {
        get
        {
            if (renderersToSort.Length > 0)
            {
                return renderersToSort[0].sortingOrder;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            for (int j = 0; j < renderersToSort.Length; ++j)
            {
                renderersToSort[j].sortingOrder = value;
            }
        }
    }

    void OnDestroy()
    {
        if (Application.isPlaying)
        {
            Unregister();
        }
    }

    public void Register() {
        IsoSpriteSortingManager.RegisterSprite(this);
    }

    public void Unregister()
    {
        IsoSpriteSortingManager.UnregisterSprite(this);
    }
}
