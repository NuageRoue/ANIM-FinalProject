using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoBar : MonoBehaviour
{
    [SerializeField] private Image bow, ladder, fishingRod;
    [SerializeField] private TextMeshProUGUI day, raft, wood, stone, food;

    int days, currentDay;

    public static InfoBar Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        HideIcons();
        SetInventory();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void HideIcon(Image icon)
    { 
        icon.color = Color.black;
    }

    void ShowIcon(Image icon)
    { 
        icon.color= Color.white;
    }

    void HideIcons()
    {
        HideIcon(bow);
        HideIcon(fishingRod);
        HideIcon(ladder);
    }


    void SetInventory()
    {
        wood.text = $"x0";
        stone.text = $"x0";
        food.text = $"x0";
        }
    public void UpdateInventory(Inventory inv)
    {
        foreach (var items in inv.baseResources.items)
        {
            switch (items.type)
            {
                case ResourceType.Wood:
                    wood.text = $"x{items.count}";
                    break;
                case ResourceType.Stone:
                    stone.text = $"x{items.count}";
                    break;
                case ResourceType.Food:
                    food.text = $"x{items.count}";
                    break;
            }
        }

        foreach (var items in inv.objectResources.items)
        {
            switch (items.type)
            {
                case RessourceObjectType.FISHING_ROD:
                    if (items.count > 0)
                        ShowIcon(fishingRod);
                    break;

                case RessourceObjectType.LADDER:
                    if (items.count > 0)
                        ShowIcon(ladder);
                    break;

                case RessourceObjectType.BOW:
                    if (items.count > 0)
                        ShowIcon(bow);
                    break;
            }
        }

        raft.text = $"RAFT: {inv.raftParts}/3";

    }

    public void SetDays(int days)
    { 
        this.days = days;
        currentDay = 1;
        DisplayDays();
    }

    void DisplayDays()
    {
        day.text = $"Day - {currentDay}/{days}";
    }

    public void IncreaseDay()
    {
        currentDay++;
        DisplayDays();
    }
}
