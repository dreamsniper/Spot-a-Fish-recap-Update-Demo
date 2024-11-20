using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "3Match/Ruleset")]
public class RulesetTemplate : ScriptableObject {

    public Character player;
    public List<Character> enemies;

    //start rule
    public enum start_after
    {
        time,
        press_button
    }
    public start_after start_after_selected = start_after.time;
    public float start_after_n_seconds;
    public string start_info_screen_text;

    //shuffle rule
    public Ruleset.no_more_moves_rule no_more_moves_rule_selected;
    public int maxShuffleAttempts = 5;

    //random placement
    public bool show_randomObstacles;
    public int randomJunks;
    public int randomPadlocks;
    public int randomIces;
    public int randomBlocks;
    public int randomFallingBlocks;
    public int randomGenerativeBlocks;


    //gem rules
    int maxGemscolors = 7;
    [Range(4, 7)]
    public int gem_length;
    public bool diagonal_falling;
    public bool allow2x2Explosions;
    public Ruleset.GemExplosionOutcome[] gemExplosionOutcomes;

    //gameplay rules
    public Ruleset.gameLoop gameLoop_selected = Ruleset.gameLoop.TurnBased;

    public Ruleset.win_requirement win_requirement_selected;
    public Ruleset.lose_requirement lose_requirement_selected;

    //armor battle rules:
    public bool use_armor;
    public bool show_armor_UI;


    //emitter rules
    public Ruleset.gem_emitter gem_emitter_rule = Ruleset.gem_emitter.normal;
    public int create_a_special_element_each_n_gems_created_min;
    public int create_a_special_element_each_n_gems_created_max;
    public int chance_to_create_a_special_element;
    public bool reset_gem_creation_count_if_chance_to_create_a_special_element_fail;
    public int max_number_of_specials_on_board_at_the_same_time = 100;
    //token
    public int token_creation_chance_weight;
        public int number_of_token_to_emit;
    public int max_number_of_tokens_on_board_at_the_same_time = 1;
        public bool emit_token_only_after_all_tiles_are_destroyed;
    //junk
    public int junk_creation_chance_weight;
        public int max_number_of_junks_on_board_at_the_same_time = 10;
    //bonus
    public int[] bonus_creation_chances_weight;
    public int max_number_of_bonuses_on_board_at_the_same_time = 10;




    //turn based rules:
    public int max_moves;
    public bool lose_turn_if_bad_move;
    public bool gain_turn_if_explode_same_color_of_previous_move;
        public int move_gained_for_explode_same_color_in_two_adjacent_turn;
    public bool gain_turn_if_explode_more_than_3_gems;
        public int[] move_gained_when_explode_more_than_3_gems = new int[4];
    public bool gain_turn_if_secondary_explosion;
    public int seconday_explosion_maginiture_needed_to_gain_a_turn;
    public int combo_lenght_needed_to_gain_a_turn;
    public int[] movesToSparedForThreeStarScore = new int[3];
    //versus turn rules
    public bool versus = false;//true = play versus AI
    public bool chain_turns_limit;
    public int max_chain_turns;


    //tiles damage rules
    public bool explosions_damages_tiles = true;
    public int tile_gift_hp;
    public int tile_gift_moves;
    public float tile_gift_time;

    //timer rules
    public float timer;
    public float time_bonus_for_secondary_explosion;
    public float time_bonus_for_gem_explosion;
    public float[] percentualTimeSparedForThreeStarScore = new float[3];

    //win requirements:
    public bool show_token_after_all_tiles_are_destroyed;
    public Ruleset.WhenShowWinScreen whenShowWinScreen;
    //public bool continue_to_play_after_win_until_lose_happen;
    public bool threeStarScore_onWinRequirement;
    public bool threeStarScore_onLoseRequirement;
    public int[] percentualPlayerHPSparedForStarScore = new int[3];


    //bonus
    public Ruleset.give_bonus give_bonus_select;
    public Ruleset.trigger_by trigger_by_select;

    public Ruleset.choose_bonus_by choose_bonus_by_select;
    public Bonus[] color_explosion_give_bonus;
    public Bonus[] ColorExplosion2x2GivesThisBonus;

    public Bonus[] big_explosion_up_give_bonus;
    public Bonus[] big_explosion_down_give_bonus;
    public Bonus[] big_explosion_left_give_bonus;
    public Bonus[] big_explosion_right_give_bonus;
    public Bonus[] Explosion2x2GivesThisBonus;
    
    //advanced charge bonus
    public bool allTheBonusesShareTheSameGemPool;


        
    private void OnEnable()
    {
        if (color_explosion_give_bonus == null || color_explosion_give_bonus.Length != maxGemscolors)
            color_explosion_give_bonus = new Bonus[maxGemscolors];

        if (ColorExplosion2x2GivesThisBonus == null || ColorExplosion2x2GivesThisBonus.Length != maxGemscolors)
            ColorExplosion2x2GivesThisBonus = new Bonus[maxGemscolors];

        if (big_explosion_up_give_bonus == null || big_explosion_up_give_bonus.Length != 4)
            big_explosion_up_give_bonus = new Bonus[4];
        if (big_explosion_down_give_bonus == null || big_explosion_down_give_bonus.Length != 4)
            big_explosion_down_give_bonus = new Bonus[4];
        if (big_explosion_left_give_bonus == null || big_explosion_left_give_bonus.Length != 4)
            big_explosion_left_give_bonus = new Bonus[4];
        if (big_explosion_right_give_bonus == null || big_explosion_right_give_bonus.Length != 4)
            big_explosion_right_give_bonus = new Bonus[4];

        if (Explosion2x2GivesThisBonus == null || Explosion2x2GivesThisBonus.Length != 4)
            Explosion2x2GivesThisBonus = new Bonus[4];

        if (gemExplosionOutcomes == null || gemExplosionOutcomes.Length != maxGemscolors)
            gemExplosionOutcomes = new Ruleset.GemExplosionOutcome[maxGemscolors];

        if (bonus_creation_chances_weight == null || bonus_creation_chances_weight.Length < Enum.GetValues(typeof(Bonus)).Length)
            bonus_creation_chances_weight = new int[Enum.GetValues(typeof(Bonus)).Length];


        if (player == null)
            player = NewCharacter();

        if (enemies == null)
            enemies = new List<Character>();

        if (enemies.Count < 1)
        {
            Character tempEnemy = NewCharacter();
            enemies.Add(tempEnemy);
        }
    }

    public Character NewCharacter()
    {
        Character character = new Character();

        character.advancedChargeBonuses = new List<AdvancedChargeBonus>();
        character.advancedChargeBonuses.Clear();
        character.gemColorAdvancedChargeBonusPool = new int[maxGemscolors];

        character.bonus_inventory = new int[Enum.GetNames(typeof(Bonus)).Length];

        character.armor = new Character.gemColorArmor[maxGemscolors];
        character.number_of_gems_to_destroy_to_win = new int[maxGemscolors];
        character.charge_bonus_cost = new int[maxGemscolors];
        character.temp_enemy_AI_preference_order = new Board_C.enemy_AI_manual_setup[maxGemscolors];
        character.additionalGemsToCollecForStarScore = new int[3];

        return character;
    }
}
