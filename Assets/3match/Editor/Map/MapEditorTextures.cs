using UnityEngine;
using UnityEditor;

public class MapEditorTextures
{

    #region icons
    public Texture2D no_tile;
    public Texture2D tile;
    public Texture2D tile_hp_0;
    public Texture2D tile_hp_1;
    public Texture2D tile_hp_2;
    public Texture2D tile_hp_3;

    public Texture2D gem_random;
    public Texture2D gem_color_A;
    public Texture2D gem_color_B;
    public Texture2D gem_color_C;
    public Texture2D gem_color_D;
    public Texture2D gem_color_E;
    public Texture2D gem_color_F;
    public Texture2D gem_color_G;

    public Texture2D junk;
    public Texture2D token;

    public Texture2D immune_block;

    public Texture2D block_1;
    public Texture2D block_2;
    public Texture2D block_3;

    public Texture2D generativeBlock_1;
    public Texture2D generativeBlock_2;
    public Texture2D generativeBlock_3;

    public Texture2D falling_block_1;
    public Texture2D falling_block_2;
    public Texture2D falling_block_3;

    public Texture2D path_tile;
    public Texture2D start_tile;
    public Texture2D goal_tile;

    public Texture2D padlock_1;
    public Texture2D padlock_2;
    public Texture2D padlock_3;

    public Texture2D falling_padlock_1;
    public Texture2D falling_padlock_2;
    public Texture2D falling_padlock_3;

    public Texture2D cage_1;
    public Texture2D cage_2;
    public Texture2D cage_3;

    public Texture2D time_bomb;

    public Texture2D bonus_destroyOne;
    public Texture2D bonus_bomb;
    public Texture2D bonus_horiz;
    public Texture2D bonus_vertic;
    public Texture2D bonus_switchGemTeleport;
    public Texture2D bonus_horiz_and_vertic;
    public Texture2D bonus_destroy_all_same_color;
    public Texture2D bonus_time;
    public Texture2D bonus_heal;
    public Texture2D bonus_damage;
    public Texture2D bonus_moves;

    public Texture2D ice_1;
    public Texture2D ice_2;
    public Texture2D ice_3;

    public Texture2D door_a;
    public Texture2D door_b;
    public Texture2D door_c;
    public Texture2D door_d;
    public Texture2D door_e;

    public Texture2D key_a;
    public Texture2D key_b;
    public Texture2D key_c;
    public Texture2D key_d;
    public Texture2D key_e;

    public Texture2D item_a;
    public Texture2D item_b;
    public Texture2D item_c;
    public Texture2D item_d;
    public Texture2D item_e;

    public Texture2D need_a;
    public Texture2D need_b;
    public Texture2D need_c;
    public Texture2D need_d;
    public Texture2D need_e;
    #endregion

    public void LoadTextures()
    {


        no_tile = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/no_tile_ico.png", typeof(Texture2D));
        tile = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/tile_ico.png", typeof(Texture2D));
        tile_hp_0 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/tile_hp_0_ico.png", typeof(Texture2D));
        tile_hp_1 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/tile_hp_1_ico.png", typeof(Texture2D));
        tile_hp_2 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/tile_hp_2_ico.png", typeof(Texture2D));
        tile_hp_3 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/tile_hp_3_ico.png", typeof(Texture2D));

        gem_random = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/gem_random.png", typeof(Texture2D));
        gem_color_A = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/gem_color_0.png", typeof(Texture2D));
        gem_color_B = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/gem_color_1.png", typeof(Texture2D));
        gem_color_C = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/gem_color_2.png", typeof(Texture2D));
        gem_color_D = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/gem_color_3.png", typeof(Texture2D));
        gem_color_E = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/gem_color_4.png", typeof(Texture2D));
        gem_color_F = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/gem_color_5.png", typeof(Texture2D));
        gem_color_G = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/gem_color_6.png", typeof(Texture2D));

        junk = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/junk_ico.png", typeof(Texture2D));
        token = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/token_ico.png", typeof(Texture2D));

        immune_block = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/immune_block_ico.png", typeof(Texture2D));

        block_1 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/block_1_ico.png", typeof(Texture2D));
        block_2 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/block_2_ico.png", typeof(Texture2D));
        block_3 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/block_3_ico.png", typeof(Texture2D));

        generativeBlock_1 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/generativeBlock_1_ico.png", typeof(Texture2D));
        generativeBlock_2 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/generativeBlock_2_ico.png", typeof(Texture2D));
        generativeBlock_3 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/generativeBlock_3_ico.png", typeof(Texture2D));

        falling_block_1 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/falling_block_1_ico.png", typeof(Texture2D));
        falling_block_2 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/falling_block_2_ico.png", typeof(Texture2D));
        falling_block_3 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/falling_block_3_ico.png", typeof(Texture2D));

        path_tile = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/tile_path_ico.png", typeof(Texture2D));
        start_tile = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/start_ico.png", typeof(Texture2D));
        goal_tile = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/goal_ico.png", typeof(Texture2D));

        padlock_1 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/padlock_1_ico.png", typeof(Texture2D));
        padlock_2 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/padlock_2_ico.png", typeof(Texture2D));
        padlock_3 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/padlock_3_ico.png", typeof(Texture2D));

        falling_padlock_1 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/falling_padlock_1_ico.png", typeof(Texture2D));
        falling_padlock_2 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/falling_padlock_2_ico.png", typeof(Texture2D));
        falling_padlock_3 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/falling_padlock_3_ico.png", typeof(Texture2D));

        cage_1 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/cage_1_ico.png", typeof(Texture2D));
        cage_2 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/cage_2_ico.png", typeof(Texture2D));
        cage_3 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/cage_3_ico.png", typeof(Texture2D));

        time_bomb = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/time_bomb_ico.png", typeof(Texture2D));

        bonus_destroyOne = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/bonus_destroyOne_ico.png", typeof(Texture2D));
        bonus_switchGemTeleport = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/bonus_switchGemTeleport_ico.png", typeof(Texture2D));
        bonus_bomb = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/bonus_bomb_ico.png", typeof(Texture2D));
        bonus_horiz = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/bonus_horiz_ico.png", typeof(Texture2D));
        bonus_vertic = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/bonus_vertic_ico.png", typeof(Texture2D));
        bonus_horiz_and_vertic = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/bonus_horiz_and_vertic_ico.png", typeof(Texture2D));
        bonus_destroy_all_same_color = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/bonus_destroy_all_same_color_ico.png", typeof(Texture2D));
        bonus_time = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/bonus_time_ico.png", typeof(Texture2D));
        bonus_heal = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/bonus_heal_ico.png", typeof(Texture2D));
        bonus_damage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/bonus_damage_ico.png", typeof(Texture2D));
        bonus_moves = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/bonus_moves_ico.png", typeof(Texture2D));

        ice_1 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/ice_1_ico.png", typeof(Texture2D));
        ice_2 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/ice_2_ico.png", typeof(Texture2D));
        ice_3 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/ice_3_ico.png", typeof(Texture2D));

        door_a = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/door_a_ico.png", typeof(Texture2D));
        door_b = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/door_b_ico.png", typeof(Texture2D));
        door_c = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/door_c_ico.png", typeof(Texture2D));
        door_d = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/door_d_ico.png", typeof(Texture2D));
        door_e = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/door_e_ico.png", typeof(Texture2D));

        key_a = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/key_a_ico.png", typeof(Texture2D));
        key_b = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/key_b_ico.png", typeof(Texture2D));
        key_c = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/key_c_ico.png", typeof(Texture2D));
        key_d = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/key_d_ico.png", typeof(Texture2D));
        key_e = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/key_e_ico.png", typeof(Texture2D));

        item_a = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/item_a_ico.png", typeof(Texture2D));
        item_b = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/item_b_ico.png", typeof(Texture2D));
        item_c = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/item_c_ico.png", typeof(Texture2D));
        item_d = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/item_d_ico.png", typeof(Texture2D));
        item_e = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/item_e_ico.png", typeof(Texture2D));

        need_a = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/need_a_ico.png", typeof(Texture2D));
        need_b = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/need_b_ico.png", typeof(Texture2D));
        need_c = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/need_c_ico.png", typeof(Texture2D));
        need_d = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/need_d_ico.png", typeof(Texture2D));
        need_e = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3match/editor_icons/need_e_ico.png", typeof(Texture2D));


    }

    public void ShowTextures(Rect target, BoardElementFile element)
    {
        if (element.tile.type == TypeOfTile.None)
            GUI.Label(target, no_tile);
        else
        {
            //tile
            if (element.tile.type == TypeOfTile.Normal)
                GUI.Label(target, tile);

            if (element.tile.hp == 0)
                GUI.Label(target, tile_hp_0);
            else if (element.tile.hp == 1)
                GUI.Label(target, tile_hp_1);
            else if (element.tile.hp == 2)
                GUI.Label(target, tile_hp_2);
            else if (element.tile.hp == 3)
                GUI.Label(target, tile_hp_3);

            //content
            if (element.content.type == TypeOfContent.Block)
            {
                if (element.content.hp == 1)
                    GUI.Label(target, block_1);
                else if (element.content.hp == 2)
                    GUI.Label(target, block_2);
                else if (element.content.hp == 3)
                    GUI.Label(target, block_3);
            }
            else if (element.content.type == TypeOfContent.FallingBlock)
            {
                if (element.content.hp == 1)
                    GUI.Label(target, falling_block_1);
                else if (element.content.hp == 2)
                    GUI.Label(target, falling_block_2);
                else if (element.content.hp == 3)
                    GUI.Label(target, falling_block_3);
            }
            else if (element.content.type == TypeOfContent.GenerativeBlock)
            {
                if (element.content.hp == 1)
                    GUI.Label(target, generativeBlock_1);
                else if (element.content.hp == 2)
                    GUI.Label(target, generativeBlock_2);
                else if (element.content.hp == 3)
                    GUI.Label(target, generativeBlock_3);
            }
            else if (element.content.type == TypeOfContent.Token)
                GUI.Label(target, token);
            else if (element.content.type == TypeOfContent.Junk)
                GUI.Label(target, junk);
            else if (element.content.type == TypeOfContent.Gem)
            {
                if (element.content.color == ContentColor.Random)
                    GUI.Label(target, gem_random);
                else if (element.content.color == ContentColor.A)
                    GUI.Label(target, gem_color_A);
                else if (element.content.color == ContentColor.B)
                    GUI.Label(target, gem_color_B);
                else if (element.content.color == ContentColor.C)
                    GUI.Label(target, gem_color_C);
                else if (element.content.color == ContentColor.D)
                    GUI.Label(target, gem_color_D);
                else if (element.content.color == ContentColor.E)
                    GUI.Label(target, gem_color_E);
                else if (element.content.color == ContentColor.F)
                    GUI.Label(target, gem_color_F);
                else if (element.content.color == ContentColor.G)
                    GUI.Label(target, gem_color_G);
            }
            else if (element.content.type == TypeOfContent.Bonus)
            {
                if (element.content.bonus == Bonus.DamageOpponent)
                    GUI.Label(target, bonus_damage);
                else if (element.content.bonus == Bonus.Destroy3x3)
                    GUI.Label(target, bonus_bomb);
                else if (element.content.bonus == Bonus.DestroyAllGemsWithThisColor)
                    GUI.Label(target, bonus_destroy_all_same_color);
                else if (element.content.bonus == Bonus.DestroyHorizontal)
                    GUI.Label(target, bonus_horiz);
                else if (element.content.bonus == Bonus.DestroyHorizontalAndVertical)
                    GUI.Label(target, bonus_horiz_and_vertic);
                else if (element.content.bonus == Bonus.DestroyOne)
                    GUI.Label(target, bonus_destroyOne);
                else if (element.content.bonus == Bonus.DestroyVertical)
                    GUI.Label(target, bonus_vertic);
                else if (element.content.bonus == Bonus.GiveMoreMoves)
                    GUI.Label(target, bonus_moves);
                else if (element.content.bonus == Bonus.GiveMoreTime)
                    GUI.Label(target, bonus_time);
                else if (element.content.bonus == Bonus.HealMe)
                    GUI.Label(target, bonus_heal);
                else if (element.content.bonus == Bonus.SwitchGemTeleport)
                    GUI.Label(target, bonus_switchGemTeleport);
            }
            //restrain
            if (element.restrain.type == TypeOfRestrain.Padlock)
            {
                if (element.restrain.hp == 1)
                    GUI.Label(target, padlock_1);
                else if (element.restrain.hp == 2)
                    GUI.Label(target, padlock_2);
                else if (element.restrain.hp == 3)
                    GUI.Label(target, padlock_3);
            }
            else if (element.restrain.type == TypeOfRestrain.Ice)
            {
                if (element.restrain.hp == 1)
                    GUI.Label(target, ice_1);
                else if (element.restrain.hp == 2)
                    GUI.Label(target, ice_2);
                else if (element.restrain.hp == 3)
                    GUI.Label(target, ice_3);
            }
            else if (element.restrain.type == TypeOfRestrain.FallingPadlock)
            {
                if (element.restrain.hp == 1)
                    GUI.Label(target, falling_padlock_1);
                else if (element.restrain.hp == 2)
                    GUI.Label(target, falling_padlock_2);
                else if (element.restrain.hp == 3)
                    GUI.Label(target, falling_padlock_3);
            }
            else if (element.restrain.type == TypeOfRestrain.Cage)
            {
                if (element.restrain.hp == 1)
                    GUI.Label(target, cage_1);
                else if (element.restrain.hp == 2)
                    GUI.Label(target, cage_2);
                else if (element.restrain.hp == 3)
                    GUI.Label(target, cage_3);
            }

        }
    }

}
