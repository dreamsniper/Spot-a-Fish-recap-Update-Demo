using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "3Match/Theme")]
public class ThemeTemplate : ScriptableObject {

    //editor menu
        public bool show_s_sprites = true;
        public bool show_s_bonus_gui = true;
        public bool show_s_on_board_bonus = true;
        public bool show_s_tiles = true;
        public bool show_s_gems = true;
        public bool show_s_padlocks = true;
        public bool show_s_blocks = true;
        public bool show_s_misc = true;
        public bool show_s_fx = true;
        public bool show_s_frame = true;


    public Sprite[] tile_hp; //the avatar of the tile
    public Sprite[] gem_colors;

    public Sprite[] lock_gem_hp;
    public Sprite[] ice_hp;
    public Sprite[] fallingPadlock_hp;
    public Sprite[] cage_hp;

    public Sprite[] block_hp;
    public Sprite[] falling_block_hp;
    public Sprite[] generative_block_hp;

    public Sprite[] gui_bonus;
    public Sprite[] on_board_bonus_sprites;

    public Sprite immune_block;
    public Sprite junk;
    public Sprite token;

    
    /*
    public Sprite[] need_color;
    public Sprite[] item_color;
    public Sprite[] key_color;
    public Sprite[] door_color;
    public Sprite[] start_goal_path;
    */

    public bool show_frame_board_decoration;
    public Transform[] frame_elements;


    public bool bonus_have_explosion_fx;
    public Transform destroy_one_fx;
    public Transform destroy_3x3_fx;
    public Transform destroy_horizontal_fx;
    public Transform destroy_vertical_fx;
    public Transform destroy_horizontal_and_vertical_fx;


    public enum gem_explosion_fx_rule
    {
        no_fx,
        for_each_gem,
        only_for_big_explosion
    }
    public gem_explosion_fx_rule gem_explosion_fx_rule_selected = gem_explosion_fx_rule.no_fx;
    public Transform[] gem_explosion_fx;
    public Transform[] gem_big_explosion_fx;

    public bool show_chess_board_decoration;

    private void OnEnable()
    {
        if (tile_hp == null || tile_hp.Length != 4)
            tile_hp = new Sprite[4];

        if (gem_colors == null || gem_colors.Length != 7)
            gem_colors = new Sprite[7];

        if (gem_explosion_fx == null || gem_explosion_fx.Length != 7)
            gem_explosion_fx = new Transform[7];

        if (gem_big_explosion_fx == null || gem_big_explosion_fx.Length != 4)
            gem_big_explosion_fx = new Transform[4];
        
        if (lock_gem_hp == null || lock_gem_hp.Length != 3)
            lock_gem_hp = new Sprite[3];

        if (ice_hp == null || ice_hp.Length != 3)
            ice_hp = new Sprite[3];

        if (fallingPadlock_hp == null || fallingPadlock_hp.Length != 3)
            fallingPadlock_hp = new Sprite[3];

        if (cage_hp == null || cage_hp.Length != 3)
            cage_hp = new Sprite[3];

        if (block_hp == null || block_hp.Length != 3)
            block_hp = new Sprite[3];

        if (falling_block_hp == null || falling_block_hp.Length != 3)
            falling_block_hp = new Sprite[3];

        if (generative_block_hp == null || generative_block_hp.Length != 3)
            generative_block_hp = new Sprite[3];

        if (gui_bonus == null)
            gui_bonus = new Sprite[Enum.GetNames(typeof(Bonus)).Length];
        else if (gui_bonus.Length != (Enum.GetNames(typeof(Bonus)).Length))
            {
            Sprite[] temp = new Sprite[gui_bonus.Length];
            for (int i = 0; i < gui_bonus.Length; i++)
                temp[i] = gui_bonus[i];

            gui_bonus = new Sprite[Enum.GetNames(typeof(Bonus)).Length];

            for (int i = 0; i < temp.Length; i++)
                gui_bonus[i] = temp[i];
            }

        if (on_board_bonus_sprites == null)
            on_board_bonus_sprites = new Sprite[Enum.GetNames(typeof(Bonus)).Length];
        else if (on_board_bonus_sprites.Length != (Enum.GetNames(typeof(Bonus)).Length))
        {
            Sprite[] temp = new Sprite[on_board_bonus_sprites.Length];
            for (int i = 0; i < on_board_bonus_sprites.Length; i++)
                temp[i] = on_board_bonus_sprites[i];

            on_board_bonus_sprites = new Sprite[Enum.GetNames(typeof(Bonus)).Length];

            for (int i = 0; i < temp.Length; i++)
                on_board_bonus_sprites[i] = temp[i];
        }


        if (frame_elements == null || frame_elements.Length != 17)
            frame_elements = new Transform[17];
    }
}
