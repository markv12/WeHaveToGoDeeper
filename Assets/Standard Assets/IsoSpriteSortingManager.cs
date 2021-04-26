using System.Collections.Generic;

public class IsoSpriteSortingManager : Singleton<IsoSpriteSortingManager>
{
    private static readonly List<IsoSpriteSorting> spriteList = new List<IsoSpriteSorting>(64);
    public static void RegisterSprite(IsoSpriteSorting newSprite)
    {
        spriteList.Add(newSprite);
    }

    public static void UnregisterSprite(IsoSpriteSorting spriteToRemove)
    {
        spriteList.Remove(spriteToRemove);
    }

    void Start()
    {
        UpdateSorting();
    }

    public static void UpdateSorting()
    {
        SortListSimple(spriteList);
        SetSortOrderBasedOnListOrder(spriteList);
    }

    private static void SetSortOrderBasedOnListOrder(List<IsoSpriteSorting> spriteList)
    {
        int orderCurrent = 0;
        for (int i = 0; i < spriteList.Count; i++)
        {
            spriteList[i].RendererSortingOrder = orderCurrent;
        }
    }

    private static void SortListSimple(List<IsoSpriteSorting> list)
    {
        list.Sort((a, b) =>
        {
            if (!a || !b)
            {
                return 0;
            }
            else
            {
                return IsoSpriteSorting.CompairIsoSortersBasic(a, b);
            }
        });
    }
}
