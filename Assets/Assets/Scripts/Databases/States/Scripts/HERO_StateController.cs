using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StateTransfer
{
    public int StateID;
    public float Duration;
    public float MaxDuration;
    public int Stacks;

    public StateTransfer(int stateID, float duration, float maxDuration, int stacks)
    {
        StateID = stateID;
        Duration = duration;
        MaxDuration = maxDuration;
        Stacks = stacks;
    }
}

public class HERO_StateController : NetworkBehaviour
{
    [System.Serializable]
    public class Properties
    {
        public UnitCore causer;
        public int duration;
        public int durationMax;
        public List<int> tick;
        public int stacks;
    }

    public UnitCore unitCore;
    public List<State> activeStates;
    public List<Properties> properties;

    public State testState;
    public State testState2;

    [ServerCallback]
    void Start()
    {
        unitCore = GetComponent<UnitCore>();
    }

    [Server]
    public void AddState(UnitCore causer, State newState)
    {
        if (newState.independent)
        {
            CreateNewInstanceOfState(causer, newState);
        }
        else
        {
            if (!activeStates.Contains(newState))
            {
                CreateNewInstanceOfState(causer, newState);
            }
            else
            {
                if (newState.maxStacks != 0 && properties[activeStates.IndexOf(newState)].stacks != newState.maxStacks)
                {
                    properties[activeStates.IndexOf(newState)].stacks++;
                }
                else
                {
                    properties[activeStates.IndexOf(newState)].duration = newState.duration;
                }
            }
        }
    }

    [Server]
    public void AddStates(UnitCore causer, List<State> newStates)
    {
        for (int count = 0; count < newStates.Count; count++)
        {
            if (newStates[count].independent)
            {
                CreateNewInstanceOfState(causer, newStates[count]);
            }
            else
            {
                if (!activeStates.Contains(newStates[count]))
                {
                    CreateNewInstanceOfState(causer, newStates[count]);
                }
                else
                {
                    if (newStates[count].maxStacks != 0 && properties[activeStates.IndexOf(newStates[count])].stacks != newStates[count].maxStacks)
                    {
                        properties[activeStates.IndexOf(newStates[count])].stacks++;
                    }
                    else
                    {
                        properties[activeStates.IndexOf(newStates[count])].duration = newStates[count].duration;
                    }
                }
            }
        }
    }

    [Server]
    void CreateNewInstanceOfState(UnitCore causer, State newState)
    {
        activeStates.Add(newState);

        //Search for awake effects
        List<State.Effect> awakeEffects = new List<State.Effect>();
        for (int count = 0; count < newState.effects.Count; count++)
        {
            if (newState.effects[count].awakeEffect)
            {
                awakeEffects.Add(newState.effects[count]);
            }
        }

        //Create Duration List
        Properties newList = new Properties();

        //Causer
        newList.causer = causer;

        //Apply awake effects
        for (int count = 0; count < awakeEffects.Count; count++)
        {
            RunEffect(properties[count].causer, activeStates[count], awakeEffects[count].effector);
        }


        //Setup duration
        newList.duration = newList.durationMax = newState.duration;

        //Setup ticks
        newList.tick = new List<int>(newState.effects.Count);
        for (int count = 0; count < newState.effects.Count; count++)
        {
            newList.tick.Add(newState.effects[count].tickLength);
        }

        newList.stacks = 1;

        properties.Add(newList);
    }

    [Server]
    public void RemoveState(State targetState)
    {
        if (activeStates.Contains(targetState))
        {
            int index = activeStates.IndexOf(targetState);
            activeStates.Remove(targetState);
            properties.RemoveAt(index);
        }
    }

    [Server]
    public void RemoveStates(List<State> targetStates)
    {
        for (int count = 0; count < targetStates.Count; count++)
        {
            if (activeStates.Contains(targetStates[count]))
            {
                int index = activeStates.IndexOf(targetStates[count]);
                activeStates.Remove(targetStates[count]);
                properties.RemoveAt(index);
            }
        }
    }

    [Server]
    public void Tick()
    {
        for (int count = 0; count < activeStates.Count; count++)
        {
            if (properties[count].duration != 0)
            {
                for (int effect = 0; effect < activeStates[count].effects.Count; effect++)
                {
                    if (!activeStates[count].effects[effect].awakeEffect & !activeStates[count].effects[effect].destroyEffect)
                    {
                        if (properties[count].tick[effect] == 0)
                        {
                            RunEffect(properties[count].causer, activeStates[count], activeStates[count].effects[effect].effector);
                            properties[count].tick[effect] = activeStates[count].effects[effect].tickLength;
                        }
                        else
                        {
                            properties[count].tick[effect] = Mathf.Clamp(properties[count].tick[effect] - 1, 0, 10000);
                        }
                    }
                }
                properties[count].duration = Mathf.Clamp(properties[count].duration - 1, 0, 10000);
            }
            else
            {
                //Check if this state decays before destroying it
                if (activeStates[count].increaseDuration && properties[count].stacks > 1)
                {
                    //Remove a stack
                    properties[count].stacks--;

                    //Reset the duration of the state
                    properties[count].duration = properties[count].durationMax;

                    //Run any Destroy Effects
                    for (int effect = 0; effect < activeStates[count].effects.Count; effect++)
                    {
                        if (activeStates[count].effects[effect].destroyEffect)
                        {
                            RunEffect(properties[count].causer, activeStates[count], activeStates[count].effects[effect].effector);
                        }
                    }
                }
                else
                {
                    //Before removing this state, search for any destroy effects
                    for (int effect = 0; effect < activeStates[count].effects.Count; effect++)
                    {
                        if (activeStates[count].effects[effect].destroyEffect)
                        {
                            RunEffect(properties[count].causer, activeStates[count], activeStates[count].effects[effect].effector);
                        }
                    }

                    //Now remove this state
                    RemoveState(activeStates[count]);
                }
            }

            //HUD!!"£$"£$"£$"£$"!$£"!$%Q$£%%$W^£$%^%£^£%$^%$^$£%^$£%^%$£^£%$^£$%"^£$%^£$%^£$%^£$%^£$%^
        }
    }

    [Server]
    void RunEffect(UnitCore causer, State state, Effector targetEffect)
    {
        unitCore.RunEffector(new Source(causer, state), targetEffect, false);
    }

    //HUD
    [Command]
    public void CmdRequestStateUpdate()
    {
        StateTransfer[] to = new StateTransfer[activeStates.Count];
        for (int count = 0; count < to.Length; count++)
        {
            to[count] = new StateTransfer(StateList.i.contained.IndexOf(activeStates[count]), properties[count].duration, properties[count].durationMax, properties[count].stacks);
        }
        RpcCallbackStateUpdate(to);
    }

    [Server]
    public void RpcCallbackStateUpdate(StateTransfer[] activeStates)
    {
        if (isLocalPlayer)
        {
            UI_HUD.i.CallbackStateUpdate(activeStates);
        }
    }

    //GAMEPLAY UTILITY
    [Server]
    public bool HasState(State target)
    {
        for (int count = 0; count < activeStates.Count; count++)
        {
            if (activeStates[count] == target)
                return true;
        }
        return false;
    }

    [Server]
    public int GetNumberOfIndependentStates(State target)
    {
        int value = 0;
        for (int count = 0; count < activeStates.Count; count++)
        {
            if (activeStates[count] == target)
            {
                value++;
            }
        }
        return value;
    }
}