using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour{



    //public int chance_to_create_a_special_element;
        [HideInInspector]public int count_gems_created;
            //public bool reset_gem_creation_count_if_chance_to_create_a_special_element_fail;

        int create_a_special_element_each_n_gems_created;
            //public int create_a_special_element_each_n_gems_created_min;
            //public int create_a_special_element_each_n_gems_created_max;

 
    int[] creation_weight_chances_deck;//the sum of token + junk + bonuses
        //public int junk_creation_chance_weight;
            int total_weight;
            int partial_weight;
        //public int[] bonus_creation_chances_weight;
        //public int token_creation_chance_weight;
            //public int number_of_token_to_emit;
                public int number_of_token_emitted;
            //public bool emit_token_only_after_all_tiles_are_destroyed;
                bool you_can_emit_somethin_meanwhile_token_emission_is_disabled;
            [HideInInspector]public bool allow_token_emission;
    


    void Initiate_emitter_variables()// call from: Board_C.Initiate_board.Initiate_variables()
    {
        if (myRuleset.gem_emitter_rule == Ruleset.gem_emitter.special)
        {
            create_a_special_element_each_n_gems_created = Random.Range(myRuleset.create_a_special_element_each_n_gems_created_min, myRuleset.create_a_special_element_each_n_gems_created_max);

            
            total_weight = myRuleset.token_creation_chance_weight + myRuleset.junk_creation_chance_weight;
            partial_weight = myRuleset.junk_creation_chance_weight;

            for (int i = 0; i < myRuleset.bonus_creation_chances_weight.Length; i++)
                {
                total_weight += myRuleset.bonus_creation_chances_weight[i];
                partial_weight += myRuleset.bonus_creation_chances_weight[i];
                }


            //safety checks
            if (partial_weight > 0)
                you_can_emit_somethin_meanwhile_token_emission_is_disabled = true;
            else
                you_can_emit_somethin_meanwhile_token_emission_is_disabled = false;

            if (total_weight == 0)
                myRuleset.gem_emitter_rule = Ruleset.gem_emitter.normal;


            //put all the bonus weight in one array
            creation_weight_chances_deck = new int[total_weight];
            int count = 0;
            for (int i = 0; i < myRuleset.bonus_creation_chances_weight.Length; i++)
            {
                if (myRuleset.bonus_creation_chances_weight[i] > 0)
                {
                    for (int b = 0; b < myRuleset.bonus_creation_chances_weight[i]; b++)
                    {
                        creation_weight_chances_deck[count] = i;
                        count++;
                    }
                }
            }


            //add the junk weight 
            if (myRuleset.junk_creation_chance_weight > 0)
            {
                for (int i = 0; i < myRuleset.junk_creation_chance_weight; i++)
                {
                    creation_weight_chances_deck[count] = -100; //-100 = junk
                    count++;
                }
            }


            //add the token weight
            if (myRuleset.token_creation_chance_weight > 0)
            {
                number_of_token_emitted = 0;
                number_of_token_to_collect += myRuleset.number_of_token_to_emit;
                for (int i = 0; i < myRuleset.token_creation_chance_weight; i++)
                {
                    creation_weight_chances_deck[count] = -200; //-200 = token
                    count++;
                }
            }


        }


        if (myRuleset.token_creation_chance_weight > 0)
            {
            if (myRuleset.emit_token_only_after_all_tiles_are_destroyed)
                allow_token_emission = false;
            else
                allow_token_emission = true;
            }
        else
            allow_token_emission = false;
    }

    public bool Emit_special_element()
    {
        if (number_of_bonus_on_board + number_of_token_on_board + number_of_junk_on_board >= myRuleset.max_number_of_specials_on_board_at_the_same_time)
            return false;

        bool return_this = false;
        int randomRoll = Random.Range(1, 100);

        //print(count_gems_created + " >= " + create_a_special_element_each_n_gems_created + " ... " + randomRoll + " <= " + chance_to_create_a_special_element);

        if (myRuleset.gem_emitter_rule == Ruleset.gem_emitter.special && count_gems_created >= create_a_special_element_each_n_gems_created)
            {
            if (you_can_emit_somethin_meanwhile_token_emission_is_disabled || allow_token_emission)
                {
                if (randomRoll <= myRuleset.chance_to_create_a_special_element)
                    return_this = true;
                else
                    {
                    if (myRuleset.reset_gem_creation_count_if_chance_to_create_a_special_element_fail)
                        Reset_gem_creation_count();
                    }
                }
            }

        return return_this;
    }

    public int Random_choose_special_element_to_create()
    {
        int temp = 0;

        if (allow_token_emission) 
        {
            if (number_of_token_emitted < myRuleset.number_of_token_to_emit)//have token to emit
            {
                temp = creation_weight_chances_deck[Random.Range(0, total_weight)];
            }
            else
            {
                temp = creation_weight_chances_deck[Random.Range(0, partial_weight)];
            }
        }
        else
            temp = creation_weight_chances_deck[Random.Range(0, total_weight)];

        //check max allowed at the same time
        if (temp == -200 && number_of_token_on_board + 1 > myRuleset.max_number_of_tokens_on_board_at_the_same_time)
            temp = 0;
        else if (temp == -100 && number_of_junk_on_board + 1 > myRuleset.max_number_of_junks_on_board_at_the_same_time)
            temp = 0;
        else if (number_of_bonus_on_board + 1 > myRuleset.max_number_of_bonuses_on_board_at_the_same_time)
            temp = 0;

        if (temp != 0)
            Reset_gem_creation_count();

        //print("Emit: " + temp);
        return temp;  
    }

    void Reset_gem_creation_count()
        {
        count_gems_created = 0;
        create_a_special_element_each_n_gems_created = Random.Range(myRuleset.create_a_special_element_each_n_gems_created_min, myRuleset.create_a_special_element_each_n_gems_created_max);
        }
}
