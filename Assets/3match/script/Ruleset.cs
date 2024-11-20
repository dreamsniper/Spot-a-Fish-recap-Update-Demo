using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ruleset  {

    [System.Serializable]
    public class GemExplosionOutcome
    {
        public int damageOpponent; //how much damage delivers a gem
        public int damageMe; //how much damage the gem delivers to the one that explode it
        public int healMe;
    }

    public enum gameLoop
    {
        TurnBased,
        Realtime //player can move during gems falling
    }

    public enum win_requirement
    {
        destroy_all_tiles = 0,
        enemy_hp_is_zero = 1,
        collect_gems = 2,
        reach_target_score = 3,
        take_all_tokens = 4, //special token to move until the bottom of the board
        destroy_all_gems = 5,//require shuffle off and gem creation off
        destroy_all_padlocks = 6,
        destroy_all_blocks = 7,
        play_until_lose = 8,
        //reach_the_exit 	//move the player avatar to exit-tile
    }

    public enum WhenShowWinScreen
    {
        ReachMinimumWinRequisite,
        Reach3StarsWinRequisite,
        ContinuePlayUntilLose
    }


    public enum lose_requirement
    {
        timer = 0,
        player_hp_is_zero = 1,
        enemy_collect_gems = 2,
        enemy_reach_target_score = 3,
        player_have_zero_moves = 4,
        relax_mode = 5 // player can't lose

    }

    /*
    public enum tile_destroyed_give
    {
        nothing,
        more_time,
        more_hp,
        more_moves
    }*/

    public enum gem_emitter
    {
        off,
        normal,
        special
    }


    //bonus:
    public enum give_bonus
    {
        no,
        after_charge,
        after_big_explosion,
        from_stage_file_or_from_gem_emitter,
        advancedCharge
    }

    public enum trigger_by
    {
        OFF,
        click,
        switch_adjacent_gem,
        inventory,
        free_switch,
        //color,
    }

    public enum choose_bonus_by
    {
        gem_color,
        explosion_magnitude
    }


    //shuffle
    public enum no_more_moves_rule
    {
        shuffle,
        lose
    }
    
}
