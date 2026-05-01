using UnityEngine;


public class HexCell : MonoBehaviour
{

    public HexCoordinates coordinates;
    int height = 0;
    GameObject prop;


    public Color color;

    [SerializeField]
    HexCell[] neighbors;

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    /*internal void SpawnProps(GameObjectList gameObjectList)
    {
        if (gameObjectList.Count > 0)
            prop = GameObject.Instantiate(gameObjectList.Get(Random.Range(0, gameObjectList.Count)), transform.position + new Vector3(0, height, 0), Quaternion.identity, transform);
    }*/
}
