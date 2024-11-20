using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public partial class BoardManager : MonoBehaviour {

    public string stageFilesPath;
    public StageFile currentStage;
    public Board_C board;

    int board_array_master_length = 16;

    public GameObject tile_obj;
    public GameObject tile_content;
    public GameObject over_gem;

    public Transform frame_pivot;
    public Image imageBK;

    

    // Use this for initialization
    void Awake () {
        board.boardGenerated = false;
        board.player = new Board_C.BoardCharacter();
        board.enemy = new Board_C.BoardCharacter();

    }

    public void LoadCurrentStage()
    {
        if (board.boardGenerated)
            EraseStage();

        if (currentStage != null)
            LoadStage();
        else
            Debug.LogWarning("Stage not found!");
    }

    public void LoadStage(string stageName)
    {
        if (board.boardGenerated)
            EraseStage();

        currentStage = Resources.Load(stageFilesPath + stageName) as StageFile;

        if (currentStage != null)
            LoadStage();
        else
            Debug.LogWarning("Stage: " + stageFilesPath + stageName + " not found!");
    }

    public void EraseStage()
    {
        print("---EraseStage---");
        board.boardGenerated = false;
        board.stage_started = false;
        board.myTheme = null;
        board.myRuleset = null;
        board.player = null;
        board.enemy = null;
        board.Reset_variables();
        board.uIManager.gui_info_screen.SetActive(false);

        //delete board
        for (int y = 0; y < board._Y_tiles; y++)
        {
            for (int x = 0; x < board._X_tiles; x++)
            {
                if (board.script_tiles_array == null)
                    continue;

                board.garbageManager.GarbageTile(board.script_tiles_array[x, y]);

               // for (int t = 0; t < board.board_array_master.GetLength(2); t++)
               //     board.board_array_master[x, y, t] = 0;
            }
        }

        //destroy frame
        for (int i = frame_pivot.childCount-1; i >= 0; i--)
            Destroy(frame_pivot.GetChild(i).gameObject);
    }

    void LoadStage()
    {
        print("LoadStage");
        if (currentStage.camera)
            board.cameraController.myCameraTemplate = currentStage.camera;
        else
            board.cameraController.myCameraTemplate = board.globalRules.defaultCamera;

        if (currentStage.theme)
            board.myTheme = currentStage.theme;
        else
            board.myTheme = board.globalRules.defaultTheme;

        if (currentStage.background)
            imageBK.sprite = currentStage.background;
        else
            imageBK.sprite = board.globalRules.defalutBackground;

        if (currentStage.rules)
            board.myRuleset = currentStage.rules;
        else
            board.myRuleset = board.globalRules.defaultRules;

        board.player = new Board_C.BoardCharacter();
        board.player.myCharacter = LoadCharacter(board.myRuleset.player);
        board.player.myCharacter.myName = board.globalRules.playerName;
        board.player.myCharacter.myAvatar = board.globalRules.playerAvatar;
        board.player.myUI = board.uIManager.playerUI;
        board.player.isPlayer = true;

        board.enemy = new Board_C.BoardCharacter();
        board.enemy.myCharacter = LoadCharacter(board.myRuleset.enemies[0]);

        board.enemy.myUI = board.uIManager.enemyUI;
        board.enemy.isPlayer = false;

        board.activeCharacter = board.player;
        if (board.myRuleset.versus)
            board.passiveCharacter = board.enemy;


        board.Initiate_variables();

        Load_board(currentStage);
        Create_new_board();

        board.cameraController.Setup_camera(board._X_tiles, board._Y_tiles, /*board.pivot_board,*/ board.menuKitBridge.Stage_uGUI_obj);

        board.boardGenerated = true;

        board.InitiateGame();

    }

    Character LoadCharacter(Character source)
    {
        Character target = new Character();

        target.myName = source.myName;
        target.myAvatar = source.myAvatar;

        target.armor = new Character.gemColorArmor[source.armor.Length];
        for (int i = 0; i < source.armor.Length; i++)
            target.armor[i] = source.armor[i];

        target.maxHp = source.maxHp;
        target.currentHp = target.maxHp;
        target.previousHp = target.currentHp;

        target.score = 0;

        //keep trak of gem destroyed and collected
        target.totalNumberOfGemsDestroyed = new int[board.myRuleset.gem_length];
        target.numberOfGemsCollect = new int[board.myRuleset.gem_length];
        target.thisGemColorIsCollected = new bool[board.myRuleset.gem_length];
        target.additionalGemsToCollecForStarScore = new int[source.additionalGemsToCollecForStarScore.Length];
        for (int i = 0; i < source.additionalGemsToCollecForStarScore.Length; i++)
            target.additionalGemsToCollecForStarScore[i] = source.additionalGemsToCollecForStarScore[i];

        target.totalNumberOfGemsRemaining = 0;
        for (int n = 0; n < source.number_of_gems_to_destroy_to_win.Length; n++)
            target.totalNumberOfGemsRemaining += source.number_of_gems_to_destroy_to_win[n];

        target.totalNumberOfGemsRequiredColletted = 0;//source.totalNumberOfGemsRequiredColletted;
        target.totalNumberOfExtraGemsCollettedAfterTheRequired = 0;

        target.currentMovesLeft = board.myRuleset.max_moves; //source.currentMovesLeft;

        target.currentChainLenght = 0;

        //bonus stuff
        target.bonus_slot = new Bonus[source.bonus_slot.Length];
        for (int i = 0; i < source.bonus_slot.Length; i++)
            target.bonus_slot[i] = source.bonus_slot[i];

        target.bonus_inventory = new int[source.bonus_inventory.Length];
        for (int i = 0; i < source.bonus_inventory.Length; i++)
            target.bonus_inventory[i] = source.bonus_inventory[i];

        target.bonus_slot_availables = source.bonus_slot_availables;

        target.charge_bonus_cost = new int[source.charge_bonus_cost.Length];
        for (int i = 0; i < source.charge_bonus_cost.Length; i++)
            target.charge_bonus_cost[i] = source.charge_bonus_cost[i];

        target.heal_me_hp_bonus = source.heal_me_hp_bonus;
        target.damage_opponent_bonus = source.heal_me_hp_bonus;

        target.advancedChargeBonuses = source.advancedChargeBonuses;

        target.gemColorAdvancedChargeBonusPool = new int[source.gemColorAdvancedChargeBonusPool.Length];
        for (int i = 0; i < source.gemColorAdvancedChargeBonusPool.Length; i++)
            target.gemColorAdvancedChargeBonusPool[i] = source.gemColorAdvancedChargeBonusPool[i];

        

        if (board.myRuleset.give_bonus_select == Ruleset.give_bonus.advancedCharge)
            target.bonus_ready = new bool[target.advancedChargeBonuses.Count];
        else
            target.bonus_ready = new bool[board.myRuleset.gem_length];

        target.filling_bonus = new int[board.myRuleset.gem_length];

        //win requirements:
        target.target_score = new int[source.target_score.Length];
            for (int i = 0; i < source.target_score.Length; i++)
                target.target_score[i] = source.target_score[i];
        target.number_of_gems_to_destroy_to_win = new int[source.number_of_gems_to_destroy_to_win.Length];
        for (int i = 0; i < source.number_of_gems_to_destroy_to_win.Length; i++)
            target.number_of_gems_to_destroy_to_win[i] = source.number_of_gems_to_destroy_to_win[i];





        //AI
        if (source.AI_selected != Board_C.enemy_AI.none)
        {

            target.AI_selected = source.AI_selected;
            target.chance_of_use_best_move = source.chance_of_use_best_move;
            //just deal damage
            target.justDealDamage_min = source.justDealDamage_min;
            target.justDealDamage_max = source.justDealDamage_max;
            //by_hand_setup
            target.temp_enemy_AI_preference_order = new Board_C.enemy_AI_manual_setup[source.temp_enemy_AI_preference_order.Length];
            for (int i = 0; i < source.temp_enemy_AI_preference_order.Length; i++)
                target.temp_enemy_AI_preference_order[i] = source.temp_enemy_AI_preference_order[i];
            //bonus
            target.chance_of_use_bonus = source.chance_of_use_bonus;
            target.chance_of_use_best_bonus = source.chance_of_use_bonus;
            //advancedAI (range value 0.0 to 1.0)
                target.howMuchImportantIs_GainATurn = source.howMuchImportantIs_GainATurn;
                target.howMuchImportantIs_ChargeBonuses = source.howMuchImportantIs_ChargeBonuses;
                //collect gems
                target.howMuchImportantIs_CollectGems = source.howMuchImportantIs_CollectGems;
                target.howMuchImportantIs_CollectGems_need_by_the_opponent =source.howMuchImportantIs_CollectGems_need_by_the_opponent;
                //battle
                target.howMuchImportantIs_DealDamage = source.howMuchImportantIs_DealDamage;
                target.howMuchImportantIs_AvoidDamage = source.howMuchImportantIs_AvoidDamage;
                target.howMuchImportantIs_HealMe = source.howMuchImportantIs_HealMe;
                target.howMuchImportantIs_NotHealThePlayer = source.howMuchImportantIs_NotHealThePlayer;
        }

        return target;

}



    void Load_board(StageFile fileMap)
    {
        if (fileMap == null)
            Debug.LogError(currentStage.name + " don't have a map!");

        board._X_tiles = fileMap.width;
        board._Y_tiles = fileMap.height;

        board.isSwitching = new bool[board._X_tiles, board._Y_tiles];
        board.board_array_master = new int[board._X_tiles, board._Y_tiles, board_array_master_length];
        board.script_tiles_array = new tile_C[board._X_tiles, board._Y_tiles];


        if (board.myRuleset.allow2x2Explosions)
        {
            board.thisGemCan2x2Explode = new Dictionary<Vector2Int, Board_C.Square2x2ExplosionInfo>();
            /*
            board.thisGemCan2x2Explode = new Board_C.Square2x2ExplosionInfo[board._X_tiles, board._Y_tiles];
            for (int y = 0; y < board._Y_tiles; y++)
            {
                for (int x = 0; x < board._X_tiles; x++)
                    board.thisGemCan2x2Explode[x, y] = new Board_C.Square2x2ExplosionInfo();

            }
            */
        }


        for (int y = 0; y < board._Y_tiles; y++)
        {
            for (int x = 0; x < board._X_tiles; x++)
            {
                BoardElementFile temp = fileMap.GetTile(x, y);

                if (temp.tile.type == TypeOfTile.None)
                {
                    board.board_array_master[x, y, 0] = -1;//no tile
                    board.board_array_master[x, y, 1] = -99;//no tile content
                    board.board_array_master[x, y, 2] = 0;// no special tile
                    board.board_array_master[x, y, 3] = 0;// no padlock
                    board.board_array_master[x, y, 4] = 0;// no special content
                    board.board_array_master[x, y, 14] = 0;//block hp
                }
                else if (temp.tile.type == TypeOfTile.Normal)
                {
                    board.board_array_master[x, y, 0] = temp.tile.hp;
                    board.board_array_master[x, y, 2] = 0;// no special tile

                    if (temp.restrain.type == TypeOfRestrain.None)
                        board.board_array_master[x, y, 3] = 0;// no padlock
                    else
                    {
                        if (temp.restrain.type == TypeOfRestrain.Padlock)
                            board.board_array_master[x, y, 3] = 1;
                        else if (temp.restrain.type == TypeOfRestrain.Ice)
                            board.board_array_master[x, y, 3] = 2;
                        else if (temp.restrain.type == TypeOfRestrain.FallingPadlock)
                            board.board_array_master[x, y, 3] = 3;
                        else if (temp.restrain.type == TypeOfRestrain.Cage)
                            board.board_array_master[x, y, 3] = 4;

                        board.board_array_master[x, y, 15] = temp.restrain.hp;
                    }

                    if (temp.content.type == TypeOfContent.None)
                    {
                        board.board_array_master[x, y, 1] = -99;//no tile content
                        board.board_array_master[x, y, 4] = 0;// no special content
                        board.board_array_master[x, y, 14] = 0;//block hp
                    }
                    else if (temp.content.type == TypeOfContent.Block)
                    {
                        board.board_array_master[x, y, 1] = 40 + temp.content.hp;//block
                        board.board_array_master[x, y, 14] = temp.content.hp;//block hp
                    }
                    else if (temp.content.type == TypeOfContent.FallingBlock)
                    {
                        board.board_array_master[x, y, 1] = 50 + temp.content.hp;//block
                        board.board_array_master[x, y, 14] = temp.content.hp;//block hp
                    }
                    else if (temp.content.type == TypeOfContent.GenerativeBlock)
                    {
                        board.board_array_master[x, y, 1] = 60 + temp.content.hp;//block
                        board.board_array_master[x, y, 14] = temp.content.hp;//block hp
                    }
                    else if (temp.content.type == TypeOfContent.Token)
                    {
                        board.board_array_master[x, y, 1] = 9;//special
                        board.board_array_master[x, y, 4] = -200;//token
                    }
                    else if (temp.content.type == TypeOfContent.Junk)
                    {
                        board.board_array_master[x, y, 1] = 9;//special
                        board.board_array_master[x, y, 4] = -100;//junk
                    }
                    else if (temp.content.type == TypeOfContent.Bonus)
                    {
                        board.board_array_master[x, y, 1] = 9;//special
                        board.board_array_master[x, y, 4] = (int)temp.content.bonus;//bonus
                    }
                    else if (temp.content.type == TypeOfContent.Gem)
                    {
                        if (temp.content.color == ContentColor.Random)
                            board.board_array_master[x, y, 1] = 10;//random gem
                        else
                            board.board_array_master[x, y, 1] = (int)temp.content.color;
                    }

                }

            }
        }

        PlaceRandomJunk(board.myRuleset.randomJunks, fileMap);
        PlaceRandomPadlocks(board.myRuleset.randomPadlocks, fileMap);
        PlaceRandomIce(board.myRuleset.randomIces, fileMap);
        PlaceRandomBlocks(board.myRuleset.randomBlocks, fileMap);
        PlaceRandomFallingBlock(board.myRuleset.randomFallingBlocks, fileMap);
        PlaceRandomGenerativeBlock(board.myRuleset.randomGenerativeBlocks, fileMap);

    }

    void Load_board(TextAsset file_txt_asset)
    {
        if (file_txt_asset)
        {
            string fileContents = file_txt_asset.text;


            string[] parts = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            board._X_tiles = Int16.Parse(parts[0]);
            board._Y_tiles = Int16.Parse(parts[1]);

            board.isSwitching = new bool[board._X_tiles, board._Y_tiles];
            board.board_array_master = new int[board._X_tiles, board._Y_tiles, board_array_master_length];
            board.script_tiles_array = new tile_C[board._X_tiles, board._Y_tiles];

            if (board.myRuleset.allow2x2Explosions)
            {
                board.thisGemCan2x2Explode = new Dictionary<Vector2Int, Board_C.Square2x2ExplosionInfo>();
                /*
                board.thisGemCan2x2Explode = new Board_C.Square2x2ExplosionInfo[board._X_tiles, board._Y_tiles];
                for (int y = 0; y < board._Y_tiles; y++)
                {
                    for (int x = 0; x < board._X_tiles; x++)
                        board.thisGemCan2x2Explode[x, y] = new Board_C.Square2x2ExplosionInfo();

                }
                */
            }

            for (int y = 0; y < board._Y_tiles + 2; y++)
            {
                if (y > 1)
                {
                    for (int x = 0; x < board._X_tiles; x++)
                    {
                        string[] tile = parts[y].Split(new string[] { "|" }, StringSplitOptions.None);

                        for (int z = 0; z < 5; z++)
                        {
                            string[] tile_characteristic = tile[x].Split(new string[] { "," }, StringSplitOptions.None);
                            board.board_array_master[x, y - 2, z] = Int16.Parse(tile_characteristic[z]);
                        }
                    }

                }
            }
        }
        else
        {
            board.cameraController.Board_camera.backgroundColor = Color.red;
            Debug.LogError("Stage file is empty");
        }
    }


    void Create_new_board()
    {

        if (board.myRuleset.show_token_after_all_tiles_are_destroyed && board.myRuleset.win_requirement_selected == Ruleset.win_requirement.take_all_tokens)
            board.token_place_card = new bool[board._X_tiles, board._Y_tiles];

        board.number_of_bottom_tiles = new int[4];

        

        for (int y = 0; y < board._Y_tiles; y++)
        {
            for (int x = 0; x < board._X_tiles; x++)
            {
                //board.moveInfos.Add(new Vector2Int(x, y), new Board_C.MoveInfo());

                if (board.board_array_master[x, y, 0] > -1)//if there is a tile
                {
                   

                    Vector2 this_position = new Vector2(x /*+ board.pivot_board.position.x*/, -y /*+ board.pivot_board.position.y*/);

                    //generate tile
                    //GameObject tempTile = (GameObject)Instantiate(tile_obj, this_position, Quaternion.identity);
                    GameObject tempTile = board.garbageManager.RecycleTile(this_position);

                    tempTile.name = "x:" + x + ",y:" + y;
                    board.script_tiles_array[x, y] = tempTile.GetComponent<tile_C>();
                    board.script_tiles_array[x, y].board = this.GetComponent<Board_C>();

                    //tempTile.transform.parent = board.pivot_board;
                    board.total_tiles++;
                    //search leader tiles

                    if ((y == 0) || ((y > 0) && (board.board_array_master[x, y - 1, 0] == -1))) //if the tile is on the first row or don't have another tile over
                    {
                        board.board_array_master[x, y, 12] = 1;//this is a leader tile
                        board.number_of_tiles_leader++;
                    }
                    //search bottom tiles for everi orientation:
                    //orientation 0 = 0°
                    if ((y == board._Y_tiles - 1) || ((y < board._Y_tiles - 1) && (board.board_array_master[x, y + 1, 0] == -1)))
                    {
                        board.number_of_bottom_tiles[0]++;
                    }
                    //orientation 1 = 90°
                    if ((x == board._X_tiles - 1) || ((x < board._X_tiles - 1) && (board.board_array_master[x + 1, 1, 0] == -1)))
                    {
                        board.number_of_bottom_tiles[1]++;
                    }
                    //orientation 2 = 180°
                    if ((y == 0) || ((y > 0) && (board.board_array_master[x, y - 1, 0] == -1))) //if the tile is on the first row or don't have another tile over
                    {
                        board.number_of_bottom_tiles[2]++;
                    }
                    //orientation 3 = 270°
                    if ((x == 0) || ((x > 0) && (board.board_array_master[x - 1, y, 0] == -1))) //if the tile is on the first row or don't have another tile over
                    {
                        board.number_of_bottom_tiles[3]++;
                    }


                    if (board.myTheme.show_frame_board_decoration)
                    {
                        Transform temp_frame;
                        if (y == 0) //make upper border
                        {
                            temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[0], this_position, Quaternion.identity);
                            temp_frame.parent = frame_pivot;
                            if (x == 0)
                            {
                                temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position, Quaternion.identity);
                                temp_frame.parent = frame_pivot;

                                temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[3], this_position, Quaternion.identity); //left border
                                temp_frame.parent = frame_pivot;
                            }
                            else if (x == board._X_tiles - 1)
                            {
                                temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position, Quaternion.identity);
                                temp_frame.parent = frame_pivot;
                                temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[2], this_position, Quaternion.identity);
                                temp_frame.parent = frame_pivot;
                            }
                        }
                        else if (y == board._Y_tiles - 1)//make bottom border
                        {
                            temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[1], this_position, Quaternion.identity);
                            temp_frame.parent = frame_pivot;
                            if (x == 0)
                            {
                                temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position, Quaternion.identity);
                                temp_frame.parent = frame_pivot;
                                temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[3], this_position, Quaternion.identity);
                                temp_frame.parent = frame_pivot;
                            }
                            else if (x == board._X_tiles - 1)
                            {
                                temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position, Quaternion.identity);
                                temp_frame.parent = frame_pivot;
                                temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[2], this_position, Quaternion.identity);
                                temp_frame.parent = frame_pivot;
                            }
                        }
                        else
                        {
                            if (x == 0) //make left border
                            {
                                temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[3], this_position, Quaternion.identity);
                                temp_frame.parent = frame_pivot;
                            }
                            else if (x == board._X_tiles - 1)//right border
                            {
                                temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[2], this_position, Quaternion.identity); //left border
                                temp_frame.parent = frame_pivot;
                            }
                        }
                    }
                }
                else // no tile here
                {
                    if (board.myTheme.show_frame_board_decoration)
                    {
                        Transform temp_frame;
                        Vector2 this_position = new Vector2(x /*+ board.pivot_board.position.x*/, -y /*+ board.pivot_board.position.y*/);

                        if (y == 0)
                        {
                            if (x == 0)
                            {
                                if ((board.board_array_master[x + 1, y, 0] > -1)//tile R and down
                                    && (board.board_array_master[x, y + 1, 0] > -1))
                                {
                                    //corner in
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[4], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position + (-Vector2.up), Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x + 1, y, 0] > -1)//tile R
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[3], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corner out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x, y + 1, 0] > -1) //tile down
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[0], this_position + (-Vector2.up), Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position + (-Vector2.up), Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x + 1, y + 1, 0] > -1)//only corner
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position + Vector2.right + (-Vector2.up), Quaternion.identity); //corner out 
                                    temp_frame.parent = frame_pivot;
                                }
                            }
                            else if (x == board._X_tiles - 1)//top right corner
                            {
                                if ((board.board_array_master[x - 1, y, 0] > -1)
                                    && (board.board_array_master[x, y + 1, 0] > -1))
                                {
                                    //corne in
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[5], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position + (-Vector2.up), Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x - 1, y, 0] > -1)//tile L
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[2], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corner out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x, y + 1, 0] > -1) //tile down
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[0], this_position + (-Vector2.up), Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position + (-Vector2.up), Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x - 1, y + 1, 0] > -1)//only corner
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.right + (-Vector2.up), Quaternion.identity); //corner out 
                                    temp_frame.parent = frame_pivot;
                                }

                            }
                            else //top line, middle 
                            {
                                if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                    && (board.board_array_master[x - 1, y, 0] > -1) // L
                                    && (board.board_array_master[x, y + 1, 0] > -1)) //down
                                {
                                    //U shape
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[15], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                            && (board.board_array_master[x - 1, y, 0] > -1)) // L
                                {

                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[2], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[3], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                         && (board.board_array_master[x, y + 1, 0] > -1)) //down
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[4], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if ((board.board_array_master[x - 1, y, 0] > -1)//tile L 
                                         && (board.board_array_master[x, y + 1, 0] > -1)) //down
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[5], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x, y + 1, 0] > -1) //down
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[0], this_position - Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x - 1, y, 0] > -1)//tile L 
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[2], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    if (board.board_array_master[x + 1, y + 1, 0] > -1)
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position + Vector2.right - Vector2.up, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }


                                }
                                else if (board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[3], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    if (board.board_array_master[x - 1, y + 1, 0] > -1)
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.right - Vector2.up, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                                //check corners
                                else
                                {
                                    if (board.board_array_master[x + 1, y + 1, 0] > -1)//only corner
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position + Vector2.right + (-Vector2.up), Quaternion.identity); //corner out 
                                        temp_frame.parent = frame_pivot;
                                    }
                                    if (board.board_array_master[x - 1, y + 1, 0] > -1)//only corner
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.right + (-Vector2.up), Quaternion.identity); //corner out 
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                            }
                        }
                        else if (y == board._Y_tiles - 1)//last line
                        {
                            if (x == 0)
                            {
                                if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                    && (board.board_array_master[x, y - 1, 0] > -1))//up
                                {
                                    //corner in
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[6], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                {
                                    //corner in
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[3], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                }
                                else if (board.board_array_master[x, y - 1, 0] > -1) //up
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[1], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x + 1, y - 1, 0] > -1)//only corner
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.right + Vector2.up, Quaternion.identity); //corner out 
                                    temp_frame.parent = frame_pivot;
                                }
                            }
                            else if (x == board._X_tiles - 1)//bottom right corner
                            {
                                if ((board.board_array_master[x - 1, y, 0] > -1) //L
                                    && (board.board_array_master[x, y - 1, 0] > -1)) //up
                                {
                                    //corner in
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[7], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x - 1, y, 0] > -1) //L
                                {
                                    //corne in
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[2], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x, y - 1, 0] > -1) //up
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[1], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x - 1, y - 1, 0] > -1)//only corner
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position - Vector2.right + Vector2.up, Quaternion.identity); //corner out 
                                    temp_frame.parent = frame_pivot;
                                }
                            }
                            else //bottom middle line
                            {
                                if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                    && (board.board_array_master[x - 1, y, 0] > -1) // L
                                    && (board.board_array_master[x, y - 1, 0] > -1)) //up
                                {
                                    //U shape
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[14], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                         && (board.board_array_master[x - 1, y, 0] > -1)) // L
                                {

                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[2], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[3], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                ///
                                else if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                         && (board.board_array_master[x, y - 1, 0] > -1)) //up
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[6], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if ((board.board_array_master[x - 1, y, 0] > -1)//tile L 
                                         && (board.board_array_master[x, y - 1, 0] > -1)) //up
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[7], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x, y - 1, 0] > -1) //down
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[1], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x - 1, y, 0] > -1)//tile L 
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[2], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    if (board.board_array_master[x + 1, y - 1, 0] > -1)
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.right + Vector2.up, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                                else if (board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[3], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    if (board.board_array_master[x - 1, y - 1, 0] > -1)
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position - Vector2.right + Vector2.up, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                                else //check corners
                                {
                                    if (board.board_array_master[x + 1, y - 1, 0] > -1)//only corner
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.right + Vector2.up, Quaternion.identity); //corner out 
                                        temp_frame.parent = frame_pivot;
                                    }
                                    if (board.board_array_master[x - 1, y - 1, 0] > -1)//only corner
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position - Vector2.right + Vector2.up, Quaternion.identity); //corner out 
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                            }

                        }
                        else//middle lines
                        {
                            if (x == 0)//first column
                            {
                                if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                    && (board.board_array_master[x, y - 1, 0] > -1) //up
                                    && (board.board_array_master[x, y + 1, 0] > -1)) //down
                                {
                                    //U shape
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[12], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position - Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if ((board.board_array_master[x, y - 1, 0] > -1) //up
                                         && (board.board_array_master[x, y + 1, 0] > -1)) //down
                                {
                                    //up
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[1], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //down
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[0], this_position - Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position - Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                         && (board.board_array_master[x, y - 1, 0] > -1)) //up
                                {
                                    //corner in
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[6], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                         && (board.board_array_master[x, y + 1, 0] > -1)) //down
                                {
                                    //corner in
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[4], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position - Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                {
                                    //corner in
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[3], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x, y + 1, 0] > -1) //down
                                {
                                    //down
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[0], this_position - Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corner
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position + (-Vector2.up), Quaternion.identity); //corner out 
                                    temp_frame.parent = frame_pivot;

                                    if (board.board_array_master[x + 1, y - 1, 0] > -1)//only corner
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.right + Vector2.up, Quaternion.identity); //corner out 
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                                else if (board.board_array_master[x, y - 1, 0] > -1)//up
                                {
                                    //up
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[1], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corner
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.up, Quaternion.identity); //corner out 
                                    temp_frame.parent = frame_pivot;

                                    if (board.board_array_master[x + 1, y + 1, 0] > -1)//only corner
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position + Vector2.right + (-Vector2.up), Quaternion.identity); //corner out 
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                                else //check corners
                                {
                                    if (board.board_array_master[x + 1, y + 1, 0] > -1)//only corner
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position + Vector2.right + (-Vector2.up), Quaternion.identity); //corner out 
                                        temp_frame.parent = frame_pivot;
                                    }
                                    if (board.board_array_master[x + 1, y - 1, 0] > -1)//only corner
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.right + Vector2.up, Quaternion.identity); //corner out 
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                            }
                            else if (x == board._X_tiles - 1)//last column
                            {
                                if ((board.board_array_master[x - 1, y, 0] > -1)//tile L 
                                    && (board.board_array_master[x, y - 1, 0] > -1) //up
                                    && (board.board_array_master[x, y + 1, 0] > -1)) //down
                                {
                                    //U shape
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[13], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if ((board.board_array_master[x, y - 1, 0] > -1) //up
                                         && (board.board_array_master[x, y + 1, 0] > -1)) //down
                                {
                                    //up
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[1], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //down
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[0], this_position - Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if ((board.board_array_master[x - 1, y, 0] > -1)//tile L 
                                         && (board.board_array_master[x, y - 1, 0] > -1)) //up
                                {
                                    //corner in
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[7], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if ((board.board_array_master[x - 1, y, 0] > -1)//tile L 
                                         && (board.board_array_master[x, y + 1, 0] > -1)) //down
                                {
                                    //corner in
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[5], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corners out
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x - 1, y, 0] > -1)//tile L 
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[2], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if (board.board_array_master[x, y - 1, 0] > -1) //up
                                {
                                    Debug.Log(x + "," + y);
                                    //up
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[1], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corner
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    if (board.board_array_master[x - 1, y + 1, 0] > -1)//only corner
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.right + (-Vector2.up), Quaternion.identity); //corner out 
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                                else if (board.board_array_master[x, y + 1, 0] > -1) //down
                                {
                                    //down
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[0], this_position - Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    //corner
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    if (board.board_array_master[x - 1, y - 1, 0] > -1)//only corner
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position - Vector2.right + Vector2.up, Quaternion.identity); //corner out 
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                                else //check corners
                                {
                                    if (board.board_array_master[x - 1, y - 1, 0] > -1)//only corner
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position - Vector2.right + Vector2.up, Quaternion.identity); //corner out 
                                        temp_frame.parent = frame_pivot;
                                    }
                                    if (board.board_array_master[x - 1, y + 1, 0] > -1)//only corner
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.right + (-Vector2.up), Quaternion.identity); //corner out 
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                            }
                            else //middle columns
                            {
                                if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                    && (board.board_array_master[x - 1, y, 0] > -1)//tile L
                                    && (board.board_array_master[x, y - 1, 0] > -1) //up
                                    && (board.board_array_master[x, y + 1, 0] > -1)) //down
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[16], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                //vertical
                                else if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                    && (board.board_array_master[x - 1, y, 0] > -1)//tile L
                                    && (board.board_array_master[x, y - 1, 0] > -1)) //up
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[14], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                    && (board.board_array_master[x - 1, y, 0] > -1)//tile L
                                    && (board.board_array_master[x, y + 1, 0] > -1)) //down
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[15], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                         && (board.board_array_master[x - 1, y, 0] > -1))//tile L
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[2], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[3], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                //horizontal
                                else if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                    && (board.board_array_master[x, y - 1, 0] > -1) //up
                                    && (board.board_array_master[x, y + 1, 0] > -1)) //down
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[12], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if ((board.board_array_master[x - 1, y, 0] > -1)//tile L
                                    && (board.board_array_master[x, y - 1, 0] > -1) //up
                                    && (board.board_array_master[x, y + 1, 0] > -1)) //down
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[13], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                else if ((board.board_array_master[x, y - 1, 0] > -1) //up
                                         && (board.board_array_master[x, y + 1, 0] > -1)) //down
                                {
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[0], this_position - Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[1], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                }
                                //L shape corners
                                else if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                         && (board.board_array_master[x, y - 1, 0] > -1)) //up

                                {
                                    //corner in
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[6], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                    if (board.board_array_master[x - 1, y + 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.up - Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                                else if ((board.board_array_master[x + 1, y, 0] > -1)//tile R 
                                         && (board.board_array_master[x, y + 1, 0] > -1)) //down

                                {
                                    //corner in
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[4], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                    //corner out
                                    if (board.board_array_master[x - 1, y - 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position + Vector2.up - Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                                else if ((board.board_array_master[x - 1, y, 0] > -1)//tile L
                                         && (board.board_array_master[x, y + 1, 0] > -1)) //down

                                {
                                    //corner in
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[5], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;
                                    if (board.board_array_master[x + 1, y - 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.up + Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                                else if ((board.board_array_master[x - 1, y, 0] > -1)//tile L
                                         && (board.board_array_master[x, y - 1, 0] > -1)) //up

                                {
                                    //corner in
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[7], this_position, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    if (board.board_array_master[x + 1, y + 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position - Vector2.up + Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                                //T shape
                                else if (board.board_array_master[x, y - 1, 0] > -1) //up
                                {
                                    //side
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[1], this_position + Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    if (board.board_array_master[x + 1, y + 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position - Vector2.up + Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }

                                    if (board.board_array_master[x - 1, y + 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.up - Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                                else if (board.board_array_master[x, y + 1, 0] > -1) //down
                                {
                                    //side
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[0], this_position - Vector2.up, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    if (board.board_array_master[x + 1, y - 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.up + Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                    if (board.board_array_master[x - 1, y - 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position + Vector2.up - Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                                else if (board.board_array_master[x + 1, y, 0] > -1)//tile R
                                {
                                    //side
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[3], this_position + Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    if (board.board_array_master[x - 1, y - 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position + Vector2.up - Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                    if (board.board_array_master[x - 1, y + 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.up - Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                                else if (board.board_array_master[x - 1, y, 0] > -1)//tile L
                                {
                                    //side
                                    temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[2], this_position - Vector2.right, Quaternion.identity);
                                    temp_frame.parent = frame_pivot;

                                    if (board.board_array_master[x + 1, y + 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position - Vector2.up + Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                    if (board.board_array_master[x + 1, y - 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.up + Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                }
                                else //check only corners
                                {
                                    if (board.board_array_master[x + 1, y + 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[11], this_position - Vector2.up + Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                    if (board.board_array_master[x + 1, y - 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[9], this_position + Vector2.up + Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                    if (board.board_array_master[x - 1, y + 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[10], this_position - Vector2.up - Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                    if (board.board_array_master[x - 1, y - 1, 0] > -1)//corner out
                                    {
                                        temp_frame = (Transform)Instantiate(board.myTheme.frame_elements[8], this_position + Vector2.up - Vector2.right, Quaternion.identity);
                                        temp_frame.parent = frame_pivot;
                                    }
                                }

                            }
                        }
                    }
                }

                if ((board.board_array_master[x, y, 4] == -200) && (board.myRuleset.show_token_after_all_tiles_are_destroyed))//token
                {
                    //take note of the token position and create a normal gem here
                    board.number_of_token_to_collect++;
                    board.token_place_card[x, y] = true;
                    board.board_array_master[x, y, 4] = 0;
                    board.board_array_master[x, y, 1] = 10;
                }

                if (board.board_array_master[x, y, 1] == 10)//if there is a random gem here
                {
                    board.board_array_master[x, y, 1] = UnityEngine.Random.Range(0, board.myRuleset.gem_length);
                }


                if (board.board_array_master[x, y, 1] >= 0)//if there is something in this tile
                {
                    //this thing can fall?
                    if (board.board_array_master[x, y, 3] == 0) // no restraint
                    {
                        if ((board.board_array_master[x, y, 1] < 40) //is a gem, junk or token
                            || ((board.board_array_master[x, y, 1] >= 51) && (board.board_array_master[x, y, 1] <= 59)) // is a falling block
                            || ((board.board_array_master[x, y, 1] >= 70) && (board.board_array_master[x, y, 1] <= 79))) // is a key

                        {
                            board.board_array_master[x, y, 10] = 1;
                            //if is junk or token: explode when reach the bottom of the board
                            if ((board.board_array_master[x, y, 1] >= 20) && (board.board_array_master[x, y, 1] <= 39))
                                board.board_array_master[x, y, 10] = 2;
                        }
                    }
                    else//NEW CODE:
                    {
                        //the gem in the paddlock can fall
                        if (board.board_array_master[x, y, 3] == 3      //= falling padlock
                            || board.board_array_master[x, y, 3] == 4) // = cage
                            board.board_array_master[x, y, 10] = 1;//can fall
                    }
                }
            }
        }

        //board.tileContents = new List<TileContent>();
        board.immovable_elements = new List<tile_C>();
        board.generativeBlockInfos = new List<Board_C.GenerativeBlockInfo>();
        board.tiles_leader_array = new tile_C[board.number_of_tiles_leader];//the tiles in this array will create the falling gems
        int leader_tiles_count = 0;
        board.total_gems_on_board_at_start = 0;
        //feed bottom tiles array:
        //search most long bottom tile list:
        int temp_bottom_tiles_array_lenght = 0;
        for (int i = 0; i < 4; i++)
        {
            if (board.number_of_bottom_tiles[i] > temp_bottom_tiles_array_lenght)
                temp_bottom_tiles_array_lenght = board.number_of_bottom_tiles[i];
        }
        board.bottom_tiles_array = new tile_C[4, temp_bottom_tiles_array_lenght];
        int[] temp_bottom_tiles_array_count = new int[4];

        for (int y = 0; y < board._Y_tiles; y++)
        {
            for (int x = 0; x < board._X_tiles; x++)
            {
                if (board.board_array_master[x, y, 0] > -1)//if there is a tile
                {
                    Vector2 this_position = new Vector2(x /*+ board.pivot_board.position.x*/, -y /*+ board.pivot_board.position.y*/);

                    board.Avoid_triple_color_gem(x, y);

                    //create visual representation 
                    //the tile
                    tile_C tile_script = board.script_tiles_array[x, y];
                    tile_script._x = x;
                    tile_script._y = y;


                    //and is leader...
                    if (board.board_array_master[x, y, 12] == 1)
                    {
                        board.tiles_leader_array[leader_tiles_count] = tile_script;
                        leader_tiles_count++;
                    }

                    //orientation 0 = 0°
                    if ((y == board._Y_tiles - 1) || ((y < board._Y_tiles - 1) && (board.board_array_master[x, y + 1, 0] == -1)))
                    {
                        board.bottom_tiles_array[0, temp_bottom_tiles_array_count[0]] = tile_script;
                        temp_bottom_tiles_array_count[0]++;
                    }
                    //orientation 1 = 90°
                    if ((x == board._X_tiles - 1) || ((x < board._X_tiles - 1) && (board.board_array_master[x + 1, 1, 0] == -1)))
                    {
                        board.bottom_tiles_array[1, temp_bottom_tiles_array_count[1]] = tile_script;
                        temp_bottom_tiles_array_count[1]++;
                    }
                    //orientation 2 = 180°
                    if ((y == 0) || ((y > 0) && (board.board_array_master[x, y - 1, 0] == -1)))
                    {
                        board.bottom_tiles_array[2, temp_bottom_tiles_array_count[2]] = tile_script;
                        temp_bottom_tiles_array_count[2]++;
                    }
                    //orientation 3 = 270°
                    if ((x == 0) || ((x > 0) && (board.board_array_master[x - 1, y, 0] == -1)))
                    {
                        board.bottom_tiles_array[3, temp_bottom_tiles_array_count[3]] = tile_script;
                        temp_bottom_tiles_array_count[3]++;
                    }

                    SpriteRenderer sprite_hp = tile_script.GetComponent<SpriteRenderer>();
                    if (board.myTheme.show_chess_board_decoration)
                    {
                        if (y % 2 == 0)
                        {
                            if (x % 2 == 0)
                                sprite_hp.sprite = board.myTheme.tile_hp[0];
                            else
                                sprite_hp.sprite = board.myTheme.tile_hp[1];
                        }
                        else
                        {
                            if (x % 2 == 0)
                                sprite_hp.sprite = board.myTheme.tile_hp[1];
                            else
                                sprite_hp.sprite = board.myTheme.tile_hp[0];
                        }
                    }
                    else
                        sprite_hp.sprite = board.myTheme.tile_hp[board.board_array_master[x, y, 0]];

                    /*if ((board.board_array_master[x, y, 0] == 0) && (board.board_array_master[x, y, 2] > 0)) //if this is a special tile and is visible
                    {
                        if ((board.board_array_master[x, y, 2] >= 1) && (board.board_array_master[x, y, 2] <= 9))
                            sprite_hp.sprite = start_goal_path[board.board_array_master[x, y, 2] - 1];
                        else if ((board.board_array_master[x, y, 2] >= 10) && (board.board_array_master[x, y, 2] <= 19))
                            sprite_hp.sprite = door_color[board.board_array_master[x, y, 2] - 10];

                    }*/
                    //update hp board
                    board.HP_board += board.board_array_master[x, y, 0];


                    //create gem
                    if ((board.board_array_master[x, y, 1] >= 0) && (board.board_array_master[x, y, 1] < 9))//if here go a gem
                    {
                        //I can put the gem on the board
                        board.total_gems_on_board_at_start++;
                        GameObject tempGameObject = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                        tile_script.myContent = tempGameObject.GetComponent<TileContent>();
                        //debug:
                        //board.tileContents.Add(tile_script.myContent);
                        tile_script.myContent.TrackMyOwner(tile_script);
                        ////

                        //tile_script.myContent.transform.parent = board.pivot_board.transform;

                        tile_script.myContent.name = "gem" + board.board_array_master[x, y, 1].ToString();
                        //gem color:
                        tile_script.myContent.mySpriteRenderer.sprite = board.myTheme.gem_colors[board.board_array_master[x, y, 1]];

                        //create a restrainf padlock, ice, falling padlock, cage
                        if (board.board_array_master[x, y, 3] > 0)
                        {
                            if (board.board_array_master[x, y, 3] == 1)//padlock
                            {
                                board.padlock_count++;

                                tile_script.my_padlock = board.garbageManager.RecyclePadlock(this_position);
                                tile_script.my_padlock.name = "padlock";
                                SpriteRenderer sprite_lock = tile_script.my_padlock.GetComponent<SpriteRenderer>();
                                sprite_lock.sprite = board.myTheme.lock_gem_hp[board.board_array_master[x, y, 15]-1];
                            }
                            else if (board.board_array_master[x, y, 3] == 2)//ice
                            {
                                board.ice_count++;

                                tile_script.my_ice = board.garbageManager.RecycleIce(this_position);
                                tile_script.my_ice.name = "ice";
                                SpriteRenderer sprite_ice = tile_script.my_ice.GetComponent<SpriteRenderer>();
                                sprite_ice.sprite = board.myTheme.ice_hp[board.board_array_master[x, y, 15]-1];
                            }
                            else if (board.board_array_master[x, y, 3] == 3)//falling padlock
                            {
                                board.padlock_count++;

                                tile_script.my_padlock = board.garbageManager.RecycleFallingPadlock(this_position);
                                tile_script.my_padlock.transform.SetParent(tile_script.myContent.transform);
                                tile_script.my_padlock.name = "falling padlock";
                                SpriteRenderer sprite_lock = tile_script.my_padlock.GetComponent<SpriteRenderer>();
                                sprite_lock.sprite = board.myTheme.fallingPadlock_hp[board.board_array_master[x, y, 15] - 1];
                            }
                            else if (board.board_array_master[x, y, 3] == 4)//cage
                            {
                                //board.padlock_count++;

                                tile_script.my_padlock = board.garbageManager.RecycleCage(this_position);
                                tile_script.my_padlock.name = "cage";
                                SpriteRenderer sprite_lock = tile_script.my_padlock.GetComponent<SpriteRenderer>();
                                sprite_lock.sprite = board.myTheme.cage_hp[board.board_array_master[x, y, 15] - 1];
                            }

                            board.immovable_elements.Add(tile_script);
                        }


                    }
                    else //there is somethin that not is a gem
                    {
                        //auxiliary gem in garbage that will be use when this tile will be free
                        board.garbageManager.StoreAuxiliaryWhenGenrateBoard(tile_content, x, y);

                        if (board.board_array_master[x, y, 1] == 9) //this is a special content
                        {
                            GameObject tempGameObject = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            tile_script.myContent = tempGameObject.GetComponent<TileContent>();
                            //tile_script.myContent.transform.parent = board.pivot_board;
                            if (board.board_array_master[x, y, 4] == -100)//junk
                            {
                                tile_script.myContent.name = "junk";
                                board.number_of_junk_on_board++;
                                tile_script.myContent.mySpriteRenderer.sprite = board.myTheme.junk;
                            }
                            else if (board.board_array_master[x, y, 4] == -200)//token
                            {
                                board.number_of_token_on_board++;

                                tile_script.myContent.name = "token";
                                tile_script.myContent.mySpriteRenderer.sprite = board.myTheme.token;

                            }
                            else if (board.board_array_master[x, y, 4] > 0)//bonus
                            {
                                board.number_of_bonus_on_board++;
                                tile_script.myContent.name = "bonus";
                                tile_script.myContent.mySpriteRenderer.sprite = board.myTheme.on_board_bonus_sprites[board.board_array_master[x, y, 4]];
                            }
                        }
                        else if (board.board_array_master[x, y, 1] == 40)//immune block
                        {
                            GameObject tempGameObject = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            tile_script.myContent = tempGameObject.GetComponent<TileContent>();
                            //tile_script.myContent.transform.parent = board.pivot_board;
                            tile_script.myContent.name = "immune_block";

                            //tile_script.myContent.mySpriteRenderer = tile_script.my_gem.transform.GetChild(0).GetComponent<SpriteRenderer>();
                            tile_script.myContent.mySpriteRenderer.sprite = board.myTheme.immune_block;
                        }
                        else if ((board.board_array_master[x, y, 1] > 40) && (board.board_array_master[x, y, 1] < 50))//block
                        {
                            board.board_array_master[x, y, 14] = (board.board_array_master[x, y, 1] - 40);

                            GameObject tempGameObject = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            tile_script.myContent = tempGameObject.GetComponent<TileContent>();
                            //tile_script.myContent.transform.parent = board.pivot_board;
                            tile_script.myContent.name = "block";
                            board.block_count++;

                            board.immovable_elements.Add(tile_script);

                            tile_script.myContent.mySpriteRenderer.sprite = board.myTheme.block_hp[board.board_array_master[x, y, 1] - 41];
                        }
                        else if ((board.board_array_master[x, y, 1] > 50) && (board.board_array_master[x, y, 1] < 60))// falling block
                        {
                            board.board_array_master[x, y, 14] = (board.board_array_master[x, y, 1] - 50);

                            GameObject tempGameObject = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            tile_script.myContent = tempGameObject.GetComponent<TileContent>();
                            //tile_script.myContent.transform.parent = board.pivot_board;
                            tile_script.myContent.name = "falling_block";
                            board.block_count++;

                            //tile_script.myContent.mySpriteRenderer = tile_script.my_gem.transform.GetChild(0).GetComponent<SpriteRenderer>();
                            tile_script.myContent.mySpriteRenderer.sprite = board.myTheme.falling_block_hp[board.board_array_master[x, y, 1] - 51];
                        }
                        else if ((board.board_array_master[x, y, 1] > 60) && (board.board_array_master[x, y, 1] < 70))//generative block
                        {
                            board.board_array_master[x, y, 14] = (board.board_array_master[x, y, 1] - 60);

                            GameObject tempGameObject = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            tile_script.myContent = tempGameObject.GetComponent<TileContent>();
                            //tile_script.myContent.transform.parent = board.pivot_board;
                            tile_script.myContent.name = "generative block";
                            board.block_count++;

                            board.immovable_elements.Add(tile_script);

                            //find where generate the new gems
                            Board_C.GenerativeBlockInfo myInfo = new Board_C.GenerativeBlockInfo();
                            myInfo.myPosition = new Vector2Int(x, y);
                            myInfo.myGenerativeTargertPosition = new Vector2Int(x, y+1);

                            //this block can't generate stuff under it
                            if (myInfo.myGenerativeTargertPosition.y >= board._Y_tiles || board.board_array_master[myInfo.myGenerativeTargertPosition.x, myInfo.myGenerativeTargertPosition.y,0] < 0)
                            {
                                myInfo.generatorIsOn = false;
                            }
                            else
                                myInfo.generatorIsOn = true;

                            board.generativeBlockInfos.Add(myInfo);

                            tile_script.myContent.mySpriteRenderer.sprite = board.myTheme.generative_block_hp[board.board_array_master[x, y, 1] - 61];
                        }
                        else //empty tile, just put a content in the garbage for later
                        {
                            GameObject tempGameObject = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            board.garbageManager.Put_in_garbage(tempGameObject.GetComponent<TileContent>());
                        }
                        /*else if ((board.board_array_master[x, y, 1] >= 60) && (board.board_array_master[x, y, 1] < 70))// need
                        {
                            tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            tile_script.my_gem.transform.parent = board.pivot_board;
                            tile_script.myContent.myGameObject.name = "need";

                            tile_script.myContent.mySpriteRenderer = tile_script.my_gem.transform.GetChild(0).GetComponent<SpriteRenderer>();
                            tile_script.myContent.mySpriteRenderer.sprite = need_color[board.board_array_master[x, y, 1] - 60];
                        }*/
                        /*else if ((board.board_array_master[x, y, 1] >= 70) && (board.board_array_master[x, y, 1] < 80))// key
                        {
                            tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            tile_script.my_gem.transform.parent = board.pivot_board;
                            tile_script.myContent.myGameObject.name = "key";

                            tile_script.myContent.mySpriteRenderer = tile_script.my_gem.transform.GetChild(0).GetComponent<SpriteRenderer>();
                            tile_script.myContent.mySpriteRenderer.sprite = key_color[board.board_array_master[x, y, 1] - 70];
                        }*/

                        /*
                        else if ((board.board_array_master[x, y, 2] >= 20) && (board.board_array_master[x, y, 2] <= 29))//item
                        {
                            tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            tile_script.my_gem.transform.parent = board.pivot_board;
                            tile_script.myContent.myGameObject.name = "item" + board.board_array_master[x, y, 2].ToString();

                            tile_script.myContent.mySpriteRenderer = tile_script.my_gem.transform.GetChild(0).GetComponent<SpriteRenderer>();
                            tile_script.myContent.mySpriteRenderer.sprite = item_color[board.board_array_master[x, y, 2] - 20];

                        }*/
                    }

                }
                else //there are no tile here
                    board.board_array_master[x, y, 1] = -99;//no color here
            }

        }

        if (board.myRuleset.win_requirement_selected == Ruleset.win_requirement.take_all_tokens)
        {
            if (board.number_of_token_on_board > 0)
                board.number_of_token_to_collect += board.number_of_token_on_board;
            else
            {
                if (board.myRuleset.show_token_after_all_tiles_are_destroyed && (board.HP_board == 0))
                {
                    board.Show_all_token_on_board();
                }
            }
            if (board.number_of_token_to_collect > 0)
                board.uIManager.Update_token_count();
            else
                Debug.LogWarning("win condition is 'Take_all_tolens' but this stage file don't have token!");
        }

        if ((board.number_of_bonus_on_board > 0) && (board.myRuleset.trigger_by_select == Ruleset.trigger_by.OFF))
        {
            Debug.LogWarning("This stage file have on board bonus, but you don't have setup any rule to trigger it. So, by default, these bonus will be trigger on click");
            board.myRuleset.trigger_by_select = Ruleset.trigger_by.click;
        }

        Debug.Log("Board created. HP board = " + board.HP_board + " win condition = " + board.myRuleset.win_requirement_selected + " ... lose condition = " + board.myRuleset.lose_requirement_selected);

        if (board.myRuleset.emit_token_only_after_all_tiles_are_destroyed && board.HP_board == 0)
            board.allow_token_emission = true;



        board.uIManager.gui_board_hp_slider.maxValue = board.HP_board;
        board.Search_max_bonus_values_for_charge_bonus();
    }



}
