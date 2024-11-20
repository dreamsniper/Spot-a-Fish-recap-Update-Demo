using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public partial class UIManager : MonoBehaviour
{


    [Header("In scene UI elements:")]
    public Board_C board;

    public Transform the_gui_score_of_this_move;

    public GameObject gui_info_screen;
    public Text infoScreenText;
    public GameObject gui_win_screen;
    WinScreenUI winScreenUI;
    public GameObject gui_lose_screen;

    public GameObject gui_timer;
    [HideInInspector] public Slider gui_timer_slider;
    public GameObject gui_board_hp;
    [HideInInspector] public Slider gui_board_hp_slider;
    public GameObject gui_vs;

    [Space()]
    public Color color_collect_done;
    public Color color_hp_damage;
    public Color color_hp_heal;

    [Space()]
    [Header("Bonus UI project prefabs:")]
    public GameObject gui_bonus_ico;
    [HideInInspector] public int slot_bonus_ico_selected = -1;
    public GameObject GemPoolCountObj;//for advanced charge bonus
    public GameObject advancedBonusButton;



    [System.Serializable]
    public class CharacterUI
    {

        public GameObject gui_name;
        [HideInInspector] public Text gui_name_text;
        public GameObject gui_avatar;
        public Color gui_color_on;
        public Color gui_color_off;

        [Space()]
        [Header("Armor Battle")]
        public GameObject gui_hp;
        public Slider gui_hp_slider;
        public Slider gui_hp_secondarySlider;
        [HideInInspector] public float hpSliderAnimationTime = 0.0f;
        [HideInInspector] public Image gui_secondarySliderImage;
        public Text gui_hp_text;
        public ugui_armor gui_armor;

        [Space()]
        [Header("Score")]
        public Text gui_score;

        [Space()]
        [Header("Progress bar")]
        public Text progressBarText;
        public Slider[] gui_progressBar_slider = new Slider[3];
        public GameObject[] starScore = new GameObject[3];

        [Space()]
        public GameObject gui_count;
        public Text gui_left_moves;
        //token
        public GameObject gui_token_count_ico;
        public Text gui_token_count_text;

        //bonus
        public GameObject gui_bonus_bar;
        [HideInInspector] public AdvancedBonusButton[] advancedBonusButton;
        [HideInInspector] public GameObject[] gui_bonus_ico_array;
        [HideInInspector] public GemPoolCount gemPoolCount;//for advanced charged bonus
        public bonus_inventory bonus_inventory_script;//for inventory bonus
    }

    [Space()]
    [Space()]
    [Header("CharacterUI:")]
    public CharacterUI playerUI;
    [Space()]
    public CharacterUI enemyUI;

    private void Awake()
    {
        winScreenUI = gui_win_screen.GetComponent<WinScreenUI>();
    }

    public void ShowWinScreen()
    {
        bool showStarScore = false;
        if (board.myRuleset.threeStarScore_onWinRequirement || board.myRuleset.threeStarScore_onLoseRequirement)
            showStarScore = true;

        winScreenUI.ShowScore(board.player.myCharacter.score, showStarScore,board.current_star_score);

        gui_win_screen.SetActive(true);
    }

    public void ResetUI()
    {
        gui_win_screen.SetActive(false);
        gui_lose_screen.SetActive(false);

        for (int i = 0; i < playerUI.gui_bonus_bar.transform.childCount; i++)
            Destroy(playerUI.gui_bonus_bar.transform.GetChild(i).gameObject);

        for (int i = 0; i < enemyUI.gui_bonus_bar.transform.childCount; i++)
            Destroy(enemyUI.gui_bonus_bar.transform.GetChild(i).gameObject);
    }


    public void Initiate_ugui()//call from Initiate_variables()
    {
        print("Initiate_ugui(): " + board.myRuleset.name);
        

        if (board.myRuleset.give_bonus_select == Ruleset.give_bonus.advancedCharge)
        {
            if (board.myRuleset.allTheBonusesShareTheSameGemPool)
            {
                GameObject temp = (GameObject)Instantiate(GemPoolCountObj);
                temp.transform.SetParent(playerUI.gui_bonus_bar.transform, false);

                board.player.myUI.gemPoolCount = temp.GetComponent<GemPoolCount>();
                board.player.myUI.gemPoolCount.StartMe(board, true);
            }

            playerUI.advancedBonusButton = new AdvancedBonusButton[board.player.myCharacter.advancedChargeBonuses.Count];
            bool[] advancedIconsUsedByPlayer = new bool[board.myTheme.gem_colors.Length];
            for (int i = 0; i < board.player.myCharacter.advancedChargeBonuses.Count; i++)
            {
                GameObject temp = (GameObject)Instantiate(advancedBonusButton);
                temp.transform.SetParent(playerUI.gui_bonus_bar.transform, false);

                playerUI.advancedBonusButton[i] = temp.GetComponent<AdvancedBonusButton>();
                playerUI.advancedBonusButton[i].StartMe(board.player.myCharacter.advancedChargeBonuses[i],i,true, board);

                //check icons need
                if (board.myRuleset.allTheBonusesShareTheSameGemPool)
                {
                    if (board.player.myCharacter.advancedChargeBonuses[i].AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.or)
                    {
                    for (int ico = 0; ico < board.player.myCharacter.advancedChargeBonuses[i].allowedGemColors.Length; ico++)
                        {
                            if (board.player.myCharacter.advancedChargeBonuses[i].allowedGemColors[ico])
                                advancedIconsUsedByPlayer[ico] = true;

                        }
                    }
                else if (board.player.myCharacter.advancedChargeBonuses[i].AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.and)
                    {
                        for (int ico = 0; ico < board.player.myCharacter.advancedChargeBonuses[i].targetCostByGemColor.Length; ico++)
                        {
                            if (board.player.myCharacter.advancedChargeBonuses[i].targetCostByGemColor[ico] > 0)
                                advancedIconsUsedByPlayer[ico] = true;
                        }
                    }
                }

            }

            if (board.myRuleset.allTheBonusesShareTheSameGemPool)
                playerUI.gui_bonus_bar.transform.GetChild(0).GetComponent<GemPoolCount>().ShowOnlyUsefulIcons(advancedIconsUsedByPlayer);

            playerUI.gui_bonus_bar.SetActive(true);

            enemyUI.advancedBonusButton = new AdvancedBonusButton[board.enemy.myCharacter.advancedChargeBonuses.Count];
            bool[] advancedIconsUsedByEnemy = new bool[board.myTheme.gem_colors.Length];
            if ((board.enemy.myCharacter.advancedChargeBonuses.Count > 0) && (board.myRuleset.versus))
            {
                
                if (board.myRuleset.allTheBonusesShareTheSameGemPool)
                {
                    GameObject temp = (GameObject)Instantiate(GemPoolCountObj);
                    temp.transform.SetParent(enemyUI.gui_bonus_bar.transform, false);

                    board.enemy.myUI.gemPoolCount = temp.GetComponent<GemPoolCount>();
                    board.enemy.myUI.gemPoolCount.StartMe(board, false);
                }


                for (int i = 0; i < board.enemy.myCharacter.advancedChargeBonuses.Count; i++)
                {
                    GameObject temp = (GameObject)Instantiate(advancedBonusButton);
                    temp.transform.SetParent(enemyUI.gui_bonus_bar.transform, false);

                    enemyUI.advancedBonusButton[i] = temp.GetComponent<AdvancedBonusButton>();
                    enemyUI.advancedBonusButton[i].StartMe(board.enemy.myCharacter.advancedChargeBonuses[i],i,false, board);


                    //check icons need
                    if (board.myRuleset.allTheBonusesShareTheSameGemPool)
                    {
                        if (board.enemy.myCharacter.advancedChargeBonuses[i].AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.or)
                        {
                            for (int ico = 0; ico < board.enemy.myCharacter.advancedChargeBonuses[i].allowedGemColors.Length; ico++)
                            {
                                if (board.enemy.myCharacter.advancedChargeBonuses[i].allowedGemColors[ico])
                                    advancedIconsUsedByEnemy[ico] = true;

                            }
                        }
                        else if (board.enemy.myCharacter.advancedChargeBonuses[i].AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.and)
                        {
                            for (int ico = 0; ico < board.enemy.myCharacter.advancedChargeBonuses[i].targetCostByGemColor.Length; ico++)
                            {
                                if (board.enemy.myCharacter.advancedChargeBonuses[i].targetCostByGemColor[ico] > 0)
                                    advancedIconsUsedByEnemy[ico] = true;
                            }
                        }
                    }

                }

                if (board.myRuleset.allTheBonusesShareTheSameGemPool)
                    enemyUI.gui_bonus_bar.transform.GetChild(0).GetComponent<GemPoolCount>().ShowOnlyUsefulIcons(advancedIconsUsedByEnemy);

                enemyUI.gui_bonus_bar.SetActive(true);
            }
        }
        else if (board.myRuleset.give_bonus_select == Ruleset.give_bonus.after_charge)
        {
            if (board.player.myCharacter.bonus_slot_availables > 0)
            {
               
                playerUI.gui_bonus_bar.SetActive(true);
                for (int i = 0; i < board.player.myCharacter.bonus_slot_availables; i++)
                {
                    if (board.player.myCharacter.bonus_slot[i] != Bonus.None)
                    {
                        playerUI.gui_bonus_ico_array[i] = (GameObject)Instantiate(gui_bonus_ico);
                        playerUI.gui_bonus_ico_array[i].transform.SetParent(playerUI.gui_bonus_bar.transform, false);
                        playerUI.gui_bonus_ico_array[i].name = (board.player.myCharacter.bonus_slot[i]).ToString();
                        playerUI.gui_bonus_ico_array[i].GetComponent<Image>().sprite = board.myTheme.gui_bonus[(int)board.player.myCharacter.bonus_slot[i] - 1];
                        ChargeBonusButton _bonus_button = (ChargeBonusButton)playerUI.gui_bonus_ico_array[i].GetComponent<ChargeBonusButton>();
                        _bonus_button.slot_number = i;
                        _bonus_button.myBonus = board.player.myCharacter.bonus_slot[i];
                        _bonus_button.player = true;
                        playerUI.gui_bonus_ico_array[i].transform.GetChild(1).GetComponent<Image>().sprite = board.myTheme.gem_colors[i];
                    }
                }
            }
            else
                playerUI.gui_bonus_bar.SetActive(false);

            if ((board.enemy.myCharacter.bonus_slot_availables > 0) && (board.myRuleset.versus))
            {
                enemyUI.gui_bonus_bar.SetActive(true);
                for (int i = 0; i < board.enemy.myCharacter.bonus_slot_availables; i++)
                {
                    if (board.enemy.myCharacter.bonus_slot[i] != Bonus.None)
                    {
                        enemyUI.gui_bonus_ico_array[i] = (GameObject)Instantiate(gui_bonus_ico);
                        enemyUI.gui_bonus_ico_array[i].transform.SetParent(enemyUI.gui_bonus_bar.transform, false);
                        enemyUI.gui_bonus_ico_array[i].name = (board.enemy.myCharacter.bonus_slot[i]).ToString();
                        enemyUI.gui_bonus_ico_array[i].GetComponent<Image>().sprite = board.myTheme.gui_bonus[(int)board.enemy.myCharacter.bonus_slot[i] - 1];
                        ChargeBonusButton _bonus_button = (ChargeBonusButton)enemyUI.gui_bonus_ico_array[i].GetComponent<ChargeBonusButton>();
                        _bonus_button.slot_number = i;
                        _bonus_button.myBonus = board.enemy.myCharacter.bonus_slot[i];
                        _bonus_button.player = false;
                        enemyUI.gui_bonus_ico_array[i].transform.GetChild(1).GetComponent<Image>().sprite = board.myTheme.gem_colors[i];
                    }
                }
            }
            else
                enemyUI.gui_bonus_bar.SetActive(false);

        }
        else
        {
            if (board.myRuleset.trigger_by_select == Ruleset.trigger_by.inventory)
            {
                playerUI.gui_bonus_bar.SetActive(true);
                

                if (board.myRuleset.versus)
                    enemyUI.gui_bonus_bar.SetActive(true);
                else
                    enemyUI.gui_bonus_bar.SetActive(false);
            }
            else
            {
                playerUI.gui_bonus_bar.SetActive(false);
                enemyUI.gui_bonus_bar.SetActive(false);
            }
        }

        gui_timer_slider = gui_timer.GetComponent<Slider>();
        gui_timer_slider.maxValue = board.myRuleset.timer;
        gui_timer_slider.value = board.time_left;

        gui_board_hp_slider = gui_board_hp.GetComponent<Slider>();
        gui_board_hp_slider.value = 0;

        InitiateCharaterUI(board.player);
        InitiateCharaterUI(board.enemy);

        if (board.myRuleset.win_requirement_selected == Ruleset.win_requirement.take_all_tokens)
        {
            playerUI.gui_token_count_ico.GetComponent<Image>().sprite = board.myTheme.token;
            playerUI.gui_token_count_text.color = Color.white;
            playerUI.gui_token_count_text.text = "0 / " + board.number_of_token_to_collect.ToString();
            playerUI.gui_token_count_ico.gameObject.SetActive(true);

        }
        else
            playerUI.gui_token_count_ico.gameObject.SetActive(false);

        Update_score();
        Auto_setup_gui();
    }

    void InitiateCharaterUI(Board_C.BoardCharacter thisCharacter)
    {
        thisCharacter.myUI.gui_name_text = thisCharacter.myUI.gui_name.GetComponent<Text>();

        thisCharacter.myUI.gui_hp_slider.maxValue = thisCharacter.myCharacter.maxHp;
        thisCharacter.myUI.gui_hp_slider.value = thisCharacter.myCharacter.currentHp;
        thisCharacter.myUI.gui_hp_secondarySlider.maxValue = thisCharacter.myCharacter.maxHp;
        thisCharacter.myUI.gui_hp_secondarySlider.value = thisCharacter.myCharacter.currentHp;
        thisCharacter.myUI.gui_secondarySliderImage = thisCharacter.myUI.gui_hp_secondarySlider.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        thisCharacter.myUI.gui_hp_text.text = thisCharacter.myCharacter.currentHp.ToString() + " / " + thisCharacter.myCharacter.maxHp.ToString();

        if (board.myRuleset.threeStarScore_onWinRequirement)
        {
            for (int i = 0; i < thisCharacter.myUI.gui_progressBar_slider.Length; i++)
            {
                if (thisCharacter.myUI.gui_progressBar_slider[i])
                {
                    thisCharacter.myUI.gui_progressBar_slider[i].maxValue = thisCharacter.myCharacter.target_score[2];

                    if (i < 2)
                        thisCharacter.myUI.gui_progressBar_slider[i].value = thisCharacter.myCharacter.target_score[i];
                }

                if (thisCharacter.myUI.starScore[i])
                    thisCharacter.myUI.starScore[i].SetActive(true);
            }
     
        }
        else
        {
            for (int i = 0; i < thisCharacter.myUI.gui_progressBar_slider.Length; i++)
            {
                if (thisCharacter.myUI.starScore[i])
                    thisCharacter.myUI.starScore[i].SetActive(false);
            }

            thisCharacter.myUI.gui_progressBar_slider[2].maxValue = thisCharacter.myCharacter.target_score[2];
            thisCharacter.myUI.gui_progressBar_slider[2].value = 0;
        }

        
    }



    void AutosetupCharacterArmor(Board_C.BoardCharacter thisCharacter)
    {

        thisCharacter.myUI.gui_hp.SetActive(true);
        if (board.myRuleset.use_armor && board.myRuleset.show_armor_UI)
        {
            thisCharacter.myUI.gui_armor.gameObject.SetActive(true);

            for (int i = 0; i < thisCharacter.myUI.gui_count.transform.childCount; i++)
            {
                if (i < board.myRuleset.gem_length)
                {
                    thisCharacter.myUI.gui_armor.transform.GetChild(i).gameObject.SetActive(true);
                    if (i > 4)
                        thisCharacter.myUI.gui_armor.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = board.myTheme.gem_colors[i];
                }
                else
                    thisCharacter.myUI.gui_armor.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
            thisCharacter.myUI.gui_armor.gameObject.SetActive(false);
    }

    void AutosetupCollectGems(Board_C.BoardCharacter thisCharacter)
    {
        print("AutosetupCollectGems");
        thisCharacter.myUI.gui_count.SetActive(true);

        thisCharacter.myUI.progressBarText.text = "";

        for (int i = 0; i < thisCharacter.myUI.gui_count.transform.childCount; i++)
        {
            if (i < board.myRuleset.gem_length && board.myRuleset.player.number_of_gems_to_destroy_to_win[i] > 0)
            {
                thisCharacter.myUI.gui_count.transform.GetChild(i).gameObject.SetActive(true);
                if (i > 4)
                    thisCharacter.myUI.gui_count.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = board.myTheme.gem_colors[i];
            }
            else
            {
                thisCharacter.myUI.gui_count.transform.GetChild(i).gameObject.SetActive(false);
                if (i < board.myRuleset.gem_length)
                    thisCharacter.myCharacter.thisGemColorIsCollected[i] = true;
            }
        }

        int totalGemsToCollect = 0;
        int[] _values = new int[3];
        for (int i = 0; i < thisCharacter.myCharacter.number_of_gems_to_destroy_to_win.Length; i++)
            totalGemsToCollect += thisCharacter.myCharacter.number_of_gems_to_destroy_to_win[i];

        //find values
        for (int i = 0; i < thisCharacter.myUI.gui_progressBar_slider.Length; i++)
        {

            totalGemsToCollect += thisCharacter.myCharacter.additionalGemsToCollecForStarScore[i];
            _values[i] = totalGemsToCollect;

        }
        //feed values
        for (int i = 0; i < thisCharacter.myUI.gui_progressBar_slider.Length; i++)
        {
            if (thisCharacter.myUI.gui_progressBar_slider[i] == null)
                continue;

            thisCharacter.myUI.gui_progressBar_slider[i].maxValue = totalGemsToCollect;

            if (i < 2)
                thisCharacter.myUI.gui_progressBar_slider[i].value = _values[i];
        }

    }

    void Auto_setup_gui()//call from Initiate_ugui()
    {
        board.globalRules.praise_script = board.globalRules.praise_obj.transform.GetChild(0).GetComponent<Praise>();

        if ((board.myRuleset.win_requirement_selected == Ruleset.win_requirement.enemy_hp_is_zero)
            || (board.myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
            || (board.myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_reach_target_score)
            )
        {
            board.player.myUI.gui_name.SetActive(true);
            board.player.myUI.gui_name_text.text = board.player.myCharacter.myName;
            board.enemy.myUI.gui_name.SetActive(true);
            board.enemy.myUI.gui_name_text.text = board.enemy.myCharacter.myName;
            gui_vs.SetActive(true);

            if (board.player.myCharacter.myAvatar == null)
                board.player.myUI.gui_avatar.SetActive(false);
            else
                {
                board.player.myUI.gui_avatar.GetComponent<Image>().sprite = board.player.myCharacter.myAvatar;
                board.player.myUI.gui_avatar.SetActive(true);
                }

            if (board.enemy.myCharacter.myAvatar == null)
                board.enemy.myUI.gui_avatar.SetActive(false);
            else
                {
                board.enemy.myUI.gui_avatar.GetComponent<Image>().sprite = board.enemy.myCharacter.myAvatar;
                board.enemy.myUI.gui_avatar.SetActive(true);
                }
        }
        else
        {
            board.player.myUI.gui_name.SetActive(true);
            board.enemy.myUI.gui_name.SetActive(false);
            gui_vs.SetActive(false);
            board.enemy.myUI.gui_avatar.SetActive(false);
            board.player.myUI.gui_avatar.SetActive(false);
        }

        

        board.player.myUI.bonus_inventory_script.MyStart();
        board.player.myUI.gui_armor.MyStart();
        board.player.myUI.gui_count.GetComponent<ugui_gem_count>().MyStart();
        if (board.myRuleset.versus)
        {
            board.enemy.myUI.bonus_inventory_script.MyStart();
            board.enemy.myUI.gui_armor.MyStart();
            board.enemy.myUI.gui_count.GetComponent<ugui_gem_count>().MyStart();
        }

        #region lose requirements
        if (board.myRuleset.lose_requirement_selected == Ruleset.lose_requirement.timer)
            gui_timer.SetActive(true);
        else
            gui_timer.SetActive(false);

        if (board.myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
        {
            board.audioManager.playerSFX.HPAudioSource = board.player.myUI.gui_hp.GetComponent<AudioSource>();
            AutosetupCharacterArmor(board.player);
        }
        else
        {
            board.player.myUI.gui_hp.SetActive(false);
            board.player.myUI.gui_armor.gameObject.SetActive(false);
        }

        if (board.myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_reach_target_score)
            board.enemy.myUI.progressBarText.text = board.enemy.myCharacter.score + " / " + board.enemy.myCharacter.target_score[2];

        if (board.myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
        {
            AutosetupCollectGems(board.enemy);
        }
        else
        {
            board.enemy.myUI.gui_count.SetActive(false);
        }

        if (board.myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_have_zero_moves)
        {
            board.player.myUI.gui_left_moves.enabled = true;
            Update_left_moves_text();
        }
        else
            board.player.myUI.gui_left_moves.enabled = false;

        if (board.myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_reach_target_score || board.myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
            board.enemy.myUI.gui_progressBar_slider[2].gameObject.SetActive(true);
        else
            board.enemy.myUI.gui_progressBar_slider[2].gameObject.SetActive(false);


        if (board.myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_have_zero_moves)
            board.player.myUI.gui_left_moves.gameObject.SetActive(true);
        else
            board.player.myUI.gui_left_moves.gameObject.SetActive(false);
        #endregion

        #region win requirements
        if (board.myRuleset.win_requirement_selected == Ruleset.win_requirement.destroy_all_tiles)
            gui_board_hp.SetActive(true);
        else
            gui_board_hp.SetActive(false);

        if (board.myRuleset.win_requirement_selected == Ruleset.win_requirement.reach_target_score || board.myRuleset.win_requirement_selected == Ruleset.win_requirement.collect_gems)
        {
            if (board.myRuleset.threeStarScore_onWinRequirement)
            {
                board.player.myUI.gui_progressBar_slider[0].gameObject.SetActive(true);
                board.player.myUI.gui_progressBar_slider[1].gameObject.SetActive(true);
            }
            else
            {
                board.player.myUI.gui_progressBar_slider[0].gameObject.SetActive(false);
                board.player.myUI.gui_progressBar_slider[1].gameObject.SetActive(false);
            }

            board.player.myUI.gui_progressBar_slider[2].gameObject.SetActive(true);

        }
        else
            {
            board.player.myUI.gui_progressBar_slider[0].gameObject.SetActive(false);
            board.player.myUI.gui_progressBar_slider[1].gameObject.SetActive(false);
            board.player.myUI.gui_progressBar_slider[2].gameObject.SetActive(false);
            }


        if (board.myRuleset.win_requirement_selected == Ruleset.win_requirement.enemy_hp_is_zero)
        {
            board.audioManager.enemySFX.HPAudioSource = board.enemy.myUI.gui_hp.GetComponent<AudioSource>();
            AutosetupCharacterArmor(board.enemy);
        }
        else
        {
            board.enemy.myUI.gui_hp.SetActive(false);
            board.enemy.myUI.gui_armor.gameObject.SetActive(false);
        }

        if (board.myRuleset.win_requirement_selected == Ruleset.win_requirement.collect_gems)
            AutosetupCollectGems(board.player);
        else
            board.player.myUI.gui_count.SetActive(false);
        #endregion
    }

}
