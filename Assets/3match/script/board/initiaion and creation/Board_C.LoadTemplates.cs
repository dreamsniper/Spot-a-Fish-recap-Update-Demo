using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Board_C : MonoBehaviour
{
    
    [HideInInspector]public RulesetTemplate myRuleset;
    [HideInInspector]public ThemeTemplate myTheme;

    public class BoardCharacter
    {
        public bool isPlayer;
        public Character myCharacter;
        public UIManager.CharacterUI myUI;
        public AudioManager.CharacterSFX mySFX;
    }
    [HideInInspector] public BoardCharacter player;
    [HideInInspector] public BoardCharacter enemy;
    [HideInInspector] public int currentEnemy = 0;
    [HideInInspector] public BoardCharacter activeCharacter;
    [HideInInspector] public BoardCharacter passiveCharacter;

    public GlobalRules globalRules;
    public GarbageManager garbageManager;
    public CameraController cameraController;
    public MenuKitBridge menuKitBridge;
    public AudioManager audioManager;
    public UIManager uIManager;



}
