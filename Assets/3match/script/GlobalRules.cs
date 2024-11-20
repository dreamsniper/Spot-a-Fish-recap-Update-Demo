using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//rules that will be the same in all the stages
public class GlobalRules : MonoBehaviour {

    public Board_C board;

    [Space()]
    [Header("Player info")]
    public string playerName;
    public Sprite playerAvatar;

    [Space()]
    [Header("Global Templates")]
    public RulesetTemplate defaultRules;
    public ThemeTemplate defaultTheme;
    public CameraTemplate defaultCamera;
    public Sprite defalutBackground;

    //hint
    [Space()]
    [Header("Hint")]
    public Transform my_hint;
    public bool show_hint;
    public float show_hint_after_n_seconds;

    [Space()]
    [Header("Praise")]
    public bool praise_the_player;
    public bool for_big_explosion;
    public bool for_secondary_explosions;
    public bool for_explode_same_color_again;
    public bool for_gain_a_star;
    public bool for_gain_a_turn;
    public GameObject praise_obj;
    public Praise praise_script;

    [Space()]
    [Header("Score rewards")]
    public bool show_score_of_this_move;
    public int score_reward_for_damaging_tiles;
    public int[] score_reward_for_explode_gems; //3 gems; 4; 5; 6; and 7
    public int score_reward_for_each_explode_gems_in_secondary_explosion;
    public float score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently;
    public float score_reward_for_secondary_combo_explosions;
    public int every_second_saved_give;
    public int every_move_saved_give;
    public int every_hp_saved_give;

    [Space()]
    [Header("Bonus")]
    public bool use_heal_cost_a_turn;
    public bool use_damage_cost_a_turn;
    public float add_time_bonus;
    public int add_moves_bonus;
    //linear esplosion propagation rules
    public bool linear_explosion_stop_against_empty_space;
    public bool linear_explosion_stop_against_block;
    public bool linear_explosion_stop_against_bonus;
    public bool linear_explosion_stop_against_token;

    [Space()]
    [Header("Animation timing")]
    public float accuracy;
    public float falling_speed;
    public float switch_speed;
    public float animationDelay = 0.03f;
    [Space()]
    [Header("Enemy AI")]
    public float enemy_move_delay = 1;//how much seconds pass between the enemy clicks (It is of use to show to palyer what the enemy is doing);



    private void Awake()
    {
        board = GetComponent<Board_C>();
    }

    //contents properties:
    public bool CanSwitch(Content thisContent)
    {
        if (thisContent.type == TypeOfContent.Block)
            return false;

        return true;
    }

    public bool CanFall(Content thisContent)
    {
        if (thisContent.type == TypeOfContent.Block)
            return false;

        return true;
    }

    public bool Can3Match(Content thisContent)
    {
        if (thisContent.type == TypeOfContent.Gem)
            return true;

        return false;
    }

    public bool CanShuffle(Content thisContent)
    {
        if (thisContent.type == TypeOfContent.Gem)
            return true;

        return false;
    }

    public bool DestroyWhenReachBottom(Content thisContent)
    {
        if (thisContent.type == TypeOfContent.Junk || thisContent.type == TypeOfContent.Token)
            return true;
        if (thisContent.type == TypeOfContent.Bonus && board.myRuleset.trigger_by_select == Ruleset.trigger_by.inventory)
            return true;
            
        return false;
    }

    public bool DamagedByNearExplosions(Content thisContent)
    {
        if (thisContent.type == TypeOfContent.Block)
            return true;

        return false;
    }
}
