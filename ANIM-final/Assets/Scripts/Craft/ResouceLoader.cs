using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton loader that provides global access to game resources and survivor prefabs.
/// Persists across scenes via DontDestroyOnLoad.
/// </summary>
class ResouceLoader : MonoBehaviour
{
    public static ResouceLoader instance { get; private set; }

    [Header("Resources")]
    [SerializeField]
    List<RessourceBase> resourcesBase;

    [SerializeField]
    List<RessourceObject> resourcesObject;

    [Header("Survivors")]
    [SerializeField]
    List<GameObject> survivors;

    /// <summary>
    /// Enforces the singleton pattern. Destroys duplicate instances
    /// and persists this one across scene loads.
    /// </summary>
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy duplicate singleton
            return;
        }
        DontDestroyOnLoad(this);
        instance = this;
    }

    /// <summary>
    /// Returns the RessourceBase matching the given ResourceType, or null if not found.
    /// </summary>
    public RessourceBase FindByType(ResourceType type)
    {
        foreach (var rb in resourcesBase)
        {
            if (rb.type == type)
                return rb;
        }
        return null;
    }

    /// <summary>
    /// Returns the RessourceObject matching the given RessourceObjectType, or null if not found.
    /// </summary>
    public RessourceObject FindByType(RessourceObjectType type)
    {
        foreach (var ro in resourcesObject)
        {
            if (ro.type == type)
                return ro;
        }
        return null;
    }

    /// <summary>
    /// Returns the survivor prefab at the given index.
    /// </summary>
    public GameObject GetSurvivor(int survivorIndex)
    {
        return survivors[survivorIndex];
    }

    /// <summary>
    /// Returns the first 3 survivor prefabs from the list.
    /// </summary>
    public List<GameObject> GetSurvivors()
    {
        return survivors.GetRange(0, 3);
    }
}
