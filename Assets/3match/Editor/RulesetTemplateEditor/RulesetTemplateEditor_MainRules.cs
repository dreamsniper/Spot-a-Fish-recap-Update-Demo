using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public partial class RulesetTemplateEditor : Editor
{
    bool menu_MainRules = true;

    public enum realtimeWinRequirement
    {
        destroy_all_tiles = 0,
        collect_gems = 2,
        reach_target_score = 3,
        take_all_tokens = 4,
        destroy_all_gems = 5,
        destroy_all_padlocks = 6,
        destroy_all_blocks = 7,
        play_until_lose = 8,
    }
    public realtimeWinRequirement realtimeWinRequirementSelected;

    public enum realtimeLoseRequirement
    {
        timer = 0,
        player_have_zero_moves = 4,
        relax_mode = 5
    }
    public realtimeLoseRequirement realtimeLoseRequirementSelected;

    void WhenShowWinScreen()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "WhenShowWinScreen");

        if (my_target.whenShowWinScreen == Ruleset.WhenShowWinScreen.Reach3StarsWinRequisite && !my_target.threeStarScore_onWinRequirement && !my_target.threeStarScore_onLoseRequirement)
            GUI.color = Color.red;
        else if (my_target.whenShowWinScreen == Ruleset.WhenShowWinScreen.ContinuePlayUntilLose && my_target.lose_requirement_selected == Ruleset.lose_requirement.relax_mode)
            {
            GUI.color = Color.red;
            EditorGUILayout.LabelField("ContinuePlayUntilLose is incompatible with lose requirement relax_mode !");
            }
        else
            GUI.color = Color.white;

        if (my_target.win_requirement_selected != Ruleset.win_requirement.play_until_lose && my_target.win_requirement_selected != Ruleset.win_requirement.enemy_hp_is_zero)
            my_target.whenShowWinScreen = (Ruleset.WhenShowWinScreen)EditorGUILayout.EnumPopup("Show win screen when", my_target.whenShowWinScreen);
        else
            my_target.whenShowWinScreen = Ruleset.WhenShowWinScreen.ReachMinimumWinRequisite;

        GUI.color = Color.white;

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }

    void MainRules()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "MainRules");

        menu_MainRules = EditorGUILayout.Foldout(menu_MainRules, "Main Rules");
        if (menu_MainRules)
        {
            EditorGUI.indentLevel++;

            my_target.start_after_selected = (RulesetTemplate.start_after)EditorGUILayout.EnumPopup("Start condition", my_target.start_after_selected);
                EditorGUI.indentLevel++;
                    if (my_target.start_after_selected == RulesetTemplate.start_after.time)
                    {
                
                        if (my_target.start_after_n_seconds < 0)
                            GUI.color = Color.red;
                        else
                            GUI.color = Color.white;

                        my_target.start_after_n_seconds = EditorGUILayout.FloatField("start after n seconds", my_target.start_after_n_seconds);

                        GUI.color = Color.white;
                
                    }
                    else if (my_target.start_after_selected == RulesetTemplate.start_after.press_button)
                    {
                        EditorGUILayout.LabelField("Info text:");
                         my_target.start_info_screen_text = EditorGUILayout.TextArea(my_target.start_info_screen_text, GUILayout.ExpandHeight(true));
                        
                    }
            EditorGUI.indentLevel--;

            my_target.gameLoop_selected = (Ruleset.gameLoop)EditorGUILayout.EnumPopup("* Game loop", my_target.gameLoop_selected);

            if (my_target.gameLoop_selected == Ruleset.gameLoop.TurnBased)
            {
                TurnRules();

                EditorGUI.indentLevel++;
                my_target.no_more_moves_rule_selected = (Ruleset.no_more_moves_rule)EditorGUILayout.EnumPopup("If no more moves", my_target.no_more_moves_rule_selected);
                    if (my_target.no_more_moves_rule_selected == Ruleset.no_more_moves_rule.shuffle)
                    {
                    EditorGUI.indentLevel++;
                    my_target.maxShuffleAttempts = EditorGUILayout.IntField("max shuffle attemps", my_target.maxShuffleAttempts);
                    if (my_target.maxShuffleAttempts < 1)
                        my_target.maxShuffleAttempts = 1;
                    EditorGUI.indentLevel--;
                    }
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();
                my_target.win_requirement_selected = (Ruleset.win_requirement)EditorGUILayout.EnumPopup("* Win condition", my_target.win_requirement_selected);
                    EditorGUI.indentLevel++;
                    CollectGemsInfo();
                    TakeAllTokensInfo();
                    ReachTargetScoreInfo();
                    EnemyHpIsZero();



                    WhenShowWinScreen();


                    EditorGUI.indentLevel--;

                EditorGUILayout.Space();/*
                if (my_target.lose_requirement_selected == Ruleset.lose_requirement.relax_mode && my_target.whenShowWinScreen == Ruleset.WhenShowWinScreen.ContinuePlayUntilLose)
                {
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Relax mode is incompatible with ContinuePlayUntilLose !");
                }
                else
                    GUI.color = Color.white;*/
                my_target.lose_requirement_selected = (Ruleset.lose_requirement)EditorGUILayout.EnumPopup("* Lose condition", my_target.lose_requirement_selected);
                GUI.color = Color.white;
                    EditorGUI.indentLevel++;
                    Timer();
                    PlayerHpIsZero();
                    EnemyCollectGems();
                    EnemyReachTargetScoreInfo();
                    PlayerHaveZeroMoves();
                    EditorGUI.indentLevel--;


                //check versus
                if ((my_target.win_requirement_selected == Ruleset.win_requirement.enemy_hp_is_zero)
                    || (my_target.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                    || (my_target.lose_requirement_selected == Ruleset.lose_requirement.enemy_reach_target_score))
                    {
                    my_target.versus = true;
                    if (my_target.win_requirement_selected == Ruleset.win_requirement.enemy_hp_is_zero || my_target.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                        my_target.use_armor = true;
                    else
                        my_target.use_armor = false;
                }
                else
                    {
                    my_target.versus = false;
                    my_target.use_armor = false;
                    }
            }
            else if (my_target.gameLoop_selected == Ruleset.gameLoop.Realtime)
            {
                my_target.versus = false;
                my_target.use_armor = false;


                EditorGUI.indentLevel++;
                my_target.no_more_moves_rule_selected = (Ruleset.no_more_moves_rule)EditorGUILayout.EnumPopup("If no more moves", my_target.no_more_moves_rule_selected);
                if (my_target.no_more_moves_rule_selected == Ruleset.no_more_moves_rule.shuffle)
                {
                    EditorGUI.indentLevel++;
                    my_target.maxShuffleAttempts = EditorGUILayout.IntField("max shuffle attemps", my_target.maxShuffleAttempts);
                    if (my_target.maxShuffleAttempts < 1)
                        my_target.maxShuffleAttempts = 1;
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();

                //win
                realtimeWinRequirementSelected = (realtimeWinRequirement)my_target.win_requirement_selected;

                if ((int)realtimeWinRequirementSelected == 0
                    || (int)realtimeWinRequirementSelected == 2
                    || (int)realtimeWinRequirementSelected == 3
                    || (int)realtimeWinRequirementSelected == 4
                    || (int)realtimeWinRequirementSelected == 5
                    || (int)realtimeWinRequirementSelected == 6
                    || (int)realtimeWinRequirementSelected == 7
                    || (int)realtimeWinRequirementSelected == 8)
                    GUI.color = Color.white;
                else
                    GUI.color = Color.red;

                realtimeWinRequirementSelected = (realtimeWinRequirement)EditorGUILayout.EnumPopup("* Win condition", realtimeWinRequirementSelected);
                my_target.win_requirement_selected = (Ruleset.win_requirement)realtimeWinRequirementSelected;
                GUI.color = Color.white;

                    EditorGUI.indentLevel++;
                    CollectGemsInfo();
                    TakeAllTokensInfo();
                    ReachTargetScoreInfo();

                WhenShowWinScreen();
                EditorGUI.indentLevel--;


                //lose
                EditorGUILayout.Space();
                realtimeLoseRequirementSelected = (realtimeLoseRequirement)my_target.lose_requirement_selected;

                if ((int)realtimeLoseRequirementSelected == 0
                    || (int)realtimeLoseRequirementSelected == 4
                    || (int)realtimeLoseRequirementSelected == 5)
                    GUI.color = Color.white;
                else
                    GUI.color = Color.red;

                realtimeLoseRequirementSelected = (realtimeLoseRequirement)EditorGUILayout.EnumPopup("* Lose condition", realtimeLoseRequirementSelected);
                my_target.lose_requirement_selected = (Ruleset.lose_requirement)realtimeLoseRequirementSelected;
                GUI.color = Color.white;

                    EditorGUI.indentLevel++;
                    Timer();
                    PlayerHaveZeroMoves();
                    EditorGUI.indentLevel--;



            }
            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }



}
