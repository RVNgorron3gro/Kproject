using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AbilitiesEditor : EditorWindow
{/*
    Texture2D headerSectionTexture;
    Color headerSectionColor = Color.magenta;

    Texture2D selectionSectionTexture;
    Color selectionSectionColor = Color.white;

    Texture2D generalSectionTexture;
    Color generalSectionColor = Color.cyan;

    Texture2D mechanicsSectionTexture;
    Color mechanicsSectionColor = new Color(150, 150, 0);

    Texture2D instructionsSectionTexture;

    Rect headerSection;
    Rect selectionSection;
    Rect generalSection;
    Rect mechanicsSection;
    Rect instructionsSection;

    static Abilities abilitySheet;

    [MenuItem("Window/Ability Editor")]
    static void OpenWindow()
    {
        AbilitiesEditor window = (AbilitiesEditor)GetWindow(typeof(AbilitiesEditor));
        window.minSize = new Vector2(640, 480);
        window.Show();
    
        
        int warrior = 0, commander = 0, scout = 0;
        for(int count = 0; count < instance.contained.Count; count++)
        {
            switch (instance.contained[count].pClass)
            {
                case PClass.Warrior:
                    warrior++;
                    break;
                case PClass.Commander:
                    commander++;
                    break;
                case PClass.Scout:
                    scout++;
                    break;
            }
        }
        EditorGUILayout.LabelField("Current Abilities " + instance.contained.Count + " (<color=red>" + warrior +"</color>, <color=blue>" + commander +"</color>, <color=lightgreen> " + scout + "</color>)");





        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Ability"))
        {
            instance.contained.Add(new Ability());
        }
        if (GUILayout.Button("Delete Ability"))
        {
            instance.contained.RemoveAt(instance.contained.Count - 1);
        }
        EditorGUILayout.EndHorizontal();
        
    }

    void OnEnable()
    {
        abilitySheet = Resources.Load<Abilities>("Ability Sheet");
        InitTexture();
    }
    
    void InitTexture()
    {
        headerSectionTexture = new Texture2D(1, 1);
        headerSectionTexture.SetPixel(0, 0, headerSectionColor);
        headerSectionTexture.Apply();

        selectionSectionTexture = new Texture2D(1, 1);
        selectionSectionTexture.SetPixel(0, 0, selectionSectionColor);
        selectionSectionTexture.Apply();

        generalSectionTexture = new Texture2D(1, 1);
        generalSectionTexture.SetPixel(0, 0, generalSectionColor);
        generalSectionTexture.Apply();

        mechanicsSectionTexture = new Texture2D(1, 1);
        mechanicsSectionTexture.SetPixel(0, 0, mechanicsSectionColor);
        mechanicsSectionTexture.Apply();
    }

    void OnGUI()
    {
        DrawLayouts();
        DrawHeader();
        DrawGeneral();
        DrawMechanics();
        DrawInstruction();
    }

    void DrawLayouts()
    {
        //Header
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = Screen.width;
        headerSection.height = 60;

        GUI.DrawTexture(headerSection, headerSectionTexture);

        //Selection
        selectionSection.x = 0;
        selectionSection.y = 60;
        selectionSection.width = (Screen.width / 4);
        selectionSection.height = Screen.height - 60;

        GUI.DrawTexture(selectionSection, selectionSectionTexture);

        //General
        generalSection.x = (Screen.width / 4) * 1;
        generalSection.y = 60;
        generalSection.width = (Screen.width / 4);
        generalSection.height = Screen.height - 60;

        GUI.DrawTexture(generalSection, generalSectionTexture);

        //Mechanics
        mechanicsSection.x = (Screen.width / 4) * 2;
        mechanicsSection.y = 60;
        mechanicsSection.width = (Screen.width / 4);
        mechanicsSection.height = Screen.height - 60;

        GUI.DrawTexture(mechanicsSection, mechanicsSectionTexture);

        //Instructions
        instructionsSection.x = (Screen.width / 4) * 2;
        instructionsSection.y = 60;
        instructionsSection.width = (Screen.width / 4);
        instructionsSection.height = Screen.height - 60;

        GUI.DrawTexture(instructionsSection, instructionsSectionTexture);
    }

    void DrawHeader()
    {
        GUILayout.BeginArea(headerSection);
        GUILayout.Label("Abilities Editor");
        GUILayout.EndArea();
    }

    void DrawSelection()
    {
        GUILayout.BeginArea(selectionSection);

        for(int count = 0; count < abilitySheet.contained.Count; count++)
        {

        }

        GUILayout.EndArea();
    }

    void DrawGeneral()
    {
        GUILayout.BeginArea(generalSection);

        GUILayout.EndArea();
    }

    void DrawMechanics()
    {
        GUILayout.BeginArea(mechanicsSection);

        GUILayout.EndArea();
    }

    void DrawInstruction()
    {
        GUILayout.BeginArea(instructionsSection);

        GUILayout.EndArea();
    }
*/
}