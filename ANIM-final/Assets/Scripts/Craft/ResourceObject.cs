using System;
using UnityEngine;

public enum RessourceObjectType
{
    FISHING_ROD,
    LADDER,
    BOW,
}

// TODO: Serialize
[Serializable]
public class RessourceObject
{
    [SerializeField]
    public string name;

    [SerializeField]
    public RessourceObjectType type;

    [SerializeField]
    public Sprite sprite = null;
}

// TODO: Serialize
[Serializable]
public class RessourceBase
{
    [SerializeField]
    public string name;

    [SerializeField]
    public ResourceType type;

    [SerializeField]
    public Sprite sprite = null;
}
