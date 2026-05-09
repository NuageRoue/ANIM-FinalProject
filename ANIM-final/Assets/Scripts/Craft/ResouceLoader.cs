using System.Collections.Generic;
using UnityEngine;

class ResouceLoader : MonoBehaviour
{
    public static ResouceLoader instance { get; private set; }

    [SerializeField]
    List<RessourceBase> resourcesBase;

    [SerializeField]
    List<RessourceObject> resourcesObject;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        instance = this;
    }

    public RessourceBase FindByType(ResourceType type)
    {
        foreach (var rb in resourcesBase)
        {
            if (rb.type == type)
                return rb;
        }
        return null;
    }

    public RessourceObject FindByType(RessourceObjectType type)
    {
        foreach (var ro in resourcesObject)
        {
            if (ro.type == type)
                return ro;
        }
        return null;
    }
}
