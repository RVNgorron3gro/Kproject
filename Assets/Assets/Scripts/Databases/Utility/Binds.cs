using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Binds : MonoBehaviour
{
    [HideInInspector] public static Binds i;
    public string rebind = "";

    public BindsSetup defaultBindings;
    public List<Binding> sessionBindings;

    //This will contain all the information for a key binding
    [System.Serializable]
    public struct Binding
    {
        public string name;
        public KeyCode key;
        public bool locked;
    }

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
        GetSavedBinds();
    }

    public void AssignNewInput(string targetBind)
    {

        Event listener = Event.current;
        if (listener.isKey)
        {
            if (CheckForExistingBind(listener.keyCode))
            {
                Debug.Log("Bind already exists");
                UI_SystemMenu.i.Options_FinishRebind();
                rebind = "";
            }
            else
            {
                Binding locatedBind = GetBind(targetBind);
                if (!locatedBind.locked)
                {
                    if (listener.keyCode == KeyCode.Escape)
                    {
                        Debug.Log("Canceled Rebind!");
                        UI_SystemMenu.i.Options_FinishRebind();
                        rebind = "";
                    }
                    else
                    {
                        //locatedBind.key = listener.keyCode;
                        //currentBindings.contained[2].key = listener.keyCode;
                        Binding newBind = new Binding()
                        {
                            name = locatedBind.name,
                            key = listener.keyCode,
                        };
                        sessionBindings.Remove(locatedBind);
                        sessionBindings.Add(newBind);
                        SaveBinds();
                        GetSavedBinds();
                        UI_SystemMenu.i.Options_FinishRebind();
                        Debug.Log(locatedBind.key);
                        rebind = "";
                    }
                }
                else
                {
                    Debug.LogWarning("This action cannot be rebound!");
                    UI_SystemMenu.i.Options_FinishRebind();
                    rebind = "";
                }
            }
        }

    }

    bool CheckForExistingBind(KeyCode target)
    {
        for (int count = 0; count < sessionBindings.Count; count++)
        {
            if (sessionBindings[count].key == target)
            {
                return true;
            }
        }
        return false;
    }

    public Binding GetBind(string target)
    {
        for (int count = 0; count < sessionBindings.Count; count++)
        {
            if (sessionBindings[count].name == target)
            {
                return sessionBindings[count];
            }
        }
        Debug.LogError("COULD NOT FIND BIND!");
        return new Binding();
    }

    int GetBindID(string target)
    {
        for (int count = 0; count < sessionBindings.Count; count++)
        {
            if (sessionBindings[count].name == target)
            {
                return count;
            }
        }
        Debug.LogError("COULD NOT FIND BIND!");
        return -1;
    }

    public void RebuildSessionBindings()
    {
        Debug.Log("Rebuilding Bindings");
        sessionBindings.Clear();
        for (int count = 0; count < defaultBindings.contained.Count; count++)
        {
            sessionBindings.Add(defaultBindings.contained[count]);
        }
        SaveBinds();
    }

    void OnGUI()
    {
        if (rebind != "")
        {
            AssignNewInput(rebind);
        }
    }

    void SaveBinds()
    {
        Debug.Log("Saving Bindings");
        for (int count = 0; count < sessionBindings.Count; count++)
        {
            PlayerPrefs.SetInt("KeyBindings_" + sessionBindings[count].name, (int)sessionBindings[count].key);
        }
    }

    void GetSavedBinds()
    {
        Debug.Log("Retrieved Bindings");
        List<Binding> newBinds = new List<Binding>();
        bool failed = false;
        for (int count = 0; count < defaultBindings.contained.Count; count++)
        {
            string bindName = "KeyBindings_" + defaultBindings.contained[count].name;
            if (PlayerPrefs.HasKey(bindName))
            {
                Binding newBinding = new Binding()
                {
                    name = defaultBindings.contained[count].name,
                    key = (KeyCode)PlayerPrefs.GetInt(bindName)
                };
                newBinds.Add(newBinding);
            }
            else
            {
                RebuildSessionBindings();
                failed = true;
                break;
            }
        }

        if (!failed)
        {
            Debug.Log("===========APPLIED NEW BINDS");
            sessionBindings = newBinds;
        }
    }

    /*
    BindsSetup GetPlayerKeyBinds()
    {
        List<Binding> newBinds = new List<Binding>();
        for(int count = 0; count < defaultBindings.contained.Count; count++)
        {
            if (PlayerPrefs.GetInt("KeyBindings_" + defaultBindings.contained[count].name) != 0)
            {
                Binding newBinding = new Binding();
                newBinding.name = defaultBindings.contained[count].name;
                newBinding.key = (KeyCode)PlayerPrefs.GetInt("KeyBindings_" + defaultBindings.contained[count].name);
            }
            else
            {
                break;
            }
        }
    }
    */
}