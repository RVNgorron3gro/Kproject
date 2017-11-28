using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomKeysCore : MonoBehaviour
{
    public bool Asking4Key = false;
    public int WhichKeyChanging = 0;

    //defaults

    //movement
    public KeyCode KeyUp = KeyCode.W;
    public int NumKeyUp = 0;
    public KeyCode KeyDown = KeyCode.S;
    public int NumKeyDown = 1;
    public KeyCode KeyLeft = KeyCode.A;
    public int NumKeyLeft = 2;
    public KeyCode KeyRight = KeyCode.D;
    public int NumKeyRight = 3;
    //4 keys (need to keep track of how many)

    //skills
    public KeyCode Skill1 = KeyCode.Q;
    public int NumSkill1 = 4;
    public KeyCode Skill2 = KeyCode.E;
    public int NumSkill2 = 5;
    public KeyCode Skill3 = KeyCode.R;
    public int NumSkill3 = 6;
    public KeyCode Skill4 = KeyCode.F;
    public int NumSkill4 = 7;
    public KeyCode Skill5 = KeyCode.G;
    public int NumSkill5 = 8;
    //9 keys

    //right and left hand
    public KeyCode Lhand = KeyCode.Mouse0;
    public int NumLhand = 9;
    public KeyCode Rhand = KeyCode.Mouse1;
    public int NumRhand = 10;
    //11 keys

    //items
    public KeyCode Item1 = KeyCode.Alpha1;
    public int NumItem1 = 11;
    public KeyCode Item2 = KeyCode.Alpha2;
    public int NumItem2 = 12;
    public KeyCode Item3 = KeyCode.Alpha3;
    public int NumItem3 = 13;
    public KeyCode Item4 = KeyCode.Alpha4;
    public int NumItem4 = 14;
    public KeyCode Item5 = KeyCode.Alpha5;
    public int NumItem5 = 15;
    public KeyCode Item6 = KeyCode.Alpha6;
    public int NumItem6 = 16;
    public KeyCode Item7 = KeyCode.Alpha7;
    public int NumItem7 = 17;
    public KeyCode Item8 = KeyCode.Alpha8;
    public int NumItem8 = 18;
    public KeyCode Item9 = KeyCode.Alpha9;
    public int NumItem9 = 19;
    public KeyCode Item10 = KeyCode.Alpha0;
    public int NumItem10 = 20;
    //21 keys

    //HUD
    public KeyCode OpenMap = KeyCode.C;
    public int NumOpenMap = 21;
    public KeyCode OpenChar = KeyCode.Tab;
    public int NumOpenChar = 22;
    //23 keys

    public List<KeyCode> keyList = new List<KeyCode>();

    public void WriteKeyList()
    {
        keyList.Add(KeyUp);
        keyList.Add(KeyDown);
        keyList.Add(KeyLeft);
        keyList.Add(KeyRight);
        keyList.Add(Skill1);
        keyList.Add(Skill2);
        keyList.Add(Skill3);
        keyList.Add(Skill4);
        keyList.Add(Skill5);
        keyList.Add(Lhand);
        keyList.Add(Rhand);
        keyList.Add(Item1);
        keyList.Add(Item2);
        keyList.Add(Item3);
        keyList.Add(Item4);
        keyList.Add(Item5);
        keyList.Add(Item6);
        keyList.Add(Item7);
        keyList.Add(Item8);
        keyList.Add(Item9);
        keyList.Add(Item10);
        keyList.Add(OpenMap);
        keyList.Add(OpenChar);
    }

    public void RestartKeyList()
    {
        keyList.Clear();
        WriteKeyList();
    }

    public void AssignOrNot()
    {
        Event e = Event.current;
        if (e.isKey)
        {
            if (keyList.Contains(e.keyCode))
            {
                Debug.Log(e.keyCode + " is already assigned");
                Asking4Key = false;
            }
            else
            {
                if (WhichKeyChanging == NumKeyUp)
                {
                    KeyUp = e.keyCode;
                }
                if (WhichKeyChanging == NumKeyDown)
                {
                    KeyDown = e.keyCode;
                }
                if (WhichKeyChanging == NumKeyLeft)
                {
                    KeyLeft = e.keyCode;
                }
                if (WhichKeyChanging == NumKeyRight)
                {
                    KeyRight = e.keyCode;
                }
                if (WhichKeyChanging == NumSkill1)
                {
                    Skill1 = e.keyCode;
                }
                if (WhichKeyChanging == NumSkill2)
                {
                    Skill2 = e.keyCode;
                }
                if (WhichKeyChanging == NumSkill3)
                {
                    Skill3 = e.keyCode;
                }
                if (WhichKeyChanging == NumSkill4)
                {
                    Skill4 = e.keyCode;
                }
                if (WhichKeyChanging == NumSkill5)
                {
                    Skill5 = e.keyCode;
                }
                if (WhichKeyChanging == NumLhand)
                {
                    Lhand = e.keyCode;
                }
                if (WhichKeyChanging == NumRhand)
                {
                    Rhand = e.keyCode;
                }
                if (WhichKeyChanging == NumItem1)
                {
                    Item1 = e.keyCode;
                }
                if (WhichKeyChanging == NumItem2)
                {
                    Item2 = e.keyCode;
                }
                if (WhichKeyChanging == NumItem3)
                {
                    Item3 = e.keyCode;
                }
                if (WhichKeyChanging == NumItem4)
                {
                    Item4 = e.keyCode;
                }
                if (WhichKeyChanging == NumItem5)
                {
                    Item5 = e.keyCode;
                }
                if (WhichKeyChanging == NumItem6)
                {
                    Item6 = e.keyCode;
                }
                if (WhichKeyChanging == NumItem7)
                {
                    Item7 = e.keyCode;
                }
                if (WhichKeyChanging == NumItem8)
                {
                    Item8 = e.keyCode;
                }
                if (WhichKeyChanging == NumItem9)
                {
                    Item9 = e.keyCode;
                }
                if (WhichKeyChanging == NumItem10)
                {
                    Item10 = e.keyCode;
                }
                if (WhichKeyChanging == NumOpenMap)
                {
                    OpenMap = e.keyCode;
                }
                if (WhichKeyChanging == NumOpenChar)
                {
                    OpenChar = e.keyCode;
                }
                RestartKeyList();
                Asking4Key = false;
            }
        }
    }

    public static CustomKeysCore instance;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("MORE THAN ONE INSTANCE OF CUSTOM KEYS CORE PRESENT");
            Destroy(this);
        }
    }

    void Start()
    {
        WriteKeyList();
        //get keycodes from an external file
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F10))
        {
            if (Asking4Key == false)
            {
                Asking4Key = true;
                /*if (KeyList.Contains(KeyCode.Z)){
					Debug.Log("S is already assigned");
				} else {
					KeyUp = KeyCode.Z;
					RestartKeyList();
				}*/
            }
            else
            {
                Asking4Key = false;
            }
        }
    }

    void OnGUI()
    {
        if (Asking4Key == true)
        {
            AssignOrNot();
        }
    }
}