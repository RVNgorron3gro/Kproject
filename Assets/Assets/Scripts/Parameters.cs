using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters : MonoBehaviour
{
    public static Parameters i;

    void Awake()
    {
        if (!i)
        {
            i = this;
        }
        else
        {
            Debug.LogError(this + " : THERE ARE MULTIPLE INSTANCES OF THIS SCRIPT");
            Destroy(this);
        }
    }

    public List<Defs.LayoutSize> uHContainer;
    public Resource Health = new Resource();
    public Resource Stamina = new Resource();
    public Resource Mana = new Resource();
    public Resource Bloodlust = new Resource();
    public Resource Sunlight = new Resource();
    public Resource Moonlight = new Resource();
    public Resource Curse = new Resource();
    public Resource Corruption = new Resource();
    public Resource Darkness = new Resource();

    [System.Serializable]
    public class Resource
    {
        public string Name;
        public Color Color;
        public Sprite Icon;
        public bool Popups;
        public float Max;
        public float Val;
        public float PCT()
        {
            return Val / Max;
        }
        public float PCT(float amount)
        {
            return amount / Max;
        }

        //Unit HUD
        public List<Defs.LayoutSize> uHBar = new List<Defs.LayoutSize>();
        public bool reductionAnimation;
        public int guideLineInterval;

        //Player HUD
        public Defs.LayoutSize pHBar;
    }

    public Resource GetResource(Defs.ResourceTypes type, float max)
    {
        string name = "NULL";
        Color color = Color.black;
        Sprite icon = null;
        bool popups = false;
        switch (type)
        {
            case Defs.ResourceTypes.Health:
                name = Health.Name;
                color = Health.Color;
                icon = Health.Icon;
                popups = Health.Popups;
                break;
            case Defs.ResourceTypes.Stamina:
                name = Stamina.Name;
                color = Stamina.Color;
                icon = Stamina.Icon;
                popups = Stamina.Popups;
                break;
            case Defs.ResourceTypes.Mana:
                name = Mana.Name;
                color = Mana.Color;
                icon = Mana.Icon;
                popups = Mana.Popups;
                break;
            case Defs.ResourceTypes.Bloodlust:
                name = Bloodlust.Name;
                color = Bloodlust.Color;
                icon = Bloodlust.Icon;
                popups = Bloodlust.Popups;
                break;
            case Defs.ResourceTypes.Sunlight:
                name = Sunlight.Name;
                color = Sunlight.Color;
                icon = Sunlight.Icon;
                popups = Sunlight.Popups;
                break;
            case Defs.ResourceTypes.Moonlight:
                name = Moonlight.Name;
                color = Moonlight.Color;
                icon = Moonlight.Icon;
                popups = Moonlight.Popups;
                break;
            case Defs.ResourceTypes.Curse:
                name = Curse.Name;
                color = Curse.Color;
                icon = Curse.Icon;
                popups = Curse.Popups;
                break;
            case Defs.ResourceTypes.Corruption:
                name = Corruption.Name;
                color = Corruption.Color;
                icon = Corruption.Icon;
                popups = Corruption.Popups;
                break;
            case Defs.ResourceTypes.Darkness:
                name = Darkness.Name;
                color = Darkness.Color;
                icon = Darkness.Icon;
                popups = Darkness.Popups;
                break;
        }
        Resource resource = new Resource()
        {
            Name = name,
            Color = color,
            Icon = icon,
            Popups = popups,
            Max = max,
            Val = max,
        };
        return resource;
    }

    public Resource GetResourceDefinition(int num)
    {
        switch (num)
        {
            case 0:
                return Health;
            case 1:
                return Stamina;
            case 2:
                return Mana;
            case 3:
                return Bloodlust;
            case 4:
                return Sunlight;
            case 5:
                return Moonlight;
            case 6:
                return Curse;
            case 7:
                return Corruption;
            case 8:
                return Darkness;
            default:
                Debug.LogError("OUT OF RANGE!");
                return null;
        }
    }

    public Resource GetResourceDefinition(Defs.ResourceTypes type)
    {
        switch (type)
        {
            case Defs.ResourceTypes.Health:
                return Health;
            case Defs.ResourceTypes.Stamina:
                return Stamina;
            case Defs.ResourceTypes.Mana:
                return Mana;
            case Defs.ResourceTypes.Bloodlust:
                return Bloodlust;
            case Defs.ResourceTypes.Sunlight:
                return Sunlight;
            case Defs.ResourceTypes.Moonlight:
                return Moonlight;
            case Defs.ResourceTypes.Curse:
                return Curse;
            case Defs.ResourceTypes.Corruption:
                return Corruption;
            case Defs.ResourceTypes.Darkness:
                return Darkness;
            default:
                Debug.LogError("OUT OF RANGE!");
                return null;
        }
    }

}