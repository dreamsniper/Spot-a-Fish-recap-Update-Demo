using UnityEngine;
using System.Collections;
using UnityEditor;
//using System.IO;
using System;

public class MapEditorWindow : EditorWindow
{
    MapEditorTextures mapEditorTextures;
		bool can_redo;
    /* 0 = tile [-1 = "no tile"; 0 = "hp = 0"; 1 = "hp 1"...]
	 * 1 = gem [-99 = no gem; 
	 * 			from 0 to 8 color gem; 
	 * 			9 = special:
	 * 				junk; 
	 * 				token; 
	 * 				bonus;
	 * 			10 = random gem; 
	 * 			40 = immune block
	 * 			41 = block hp 1
	 * 			42 = block hp 2
	 * 			43 = block hp 3
	 * 			51 = falling block hp 1
	 * 			52 = falling block hp 2
	 * 			53 = falling block hp 3
	 * 			60 = need a
	 * 			61 = need b
	 * 			62 = need c
	 * 			63 = need d
	 * 			64 = need e
	 * 			70 = key a
	 * 			71 = key b
	 * 			72 = key c
	 * 			73 = key d
	 * 			74 = key e

	 * 
	 * 				213 = destroy single random gems (meteor shower)
	 * 2 = special tile:
	 * 		0 = no special
	 * 		1 = start
	 * 		2 = goal
	 * 		3 = path
	 * 		10 = door a
	 * 		11 = door b
	 * 		12 = door c
	 * 		13 = door d
	 * 		14 = door e
	 * 		20 = item a
	 * 		21 = item b
	 * 		22 = item c
	 * 		23 = item d
	 * 		24 = item e
	 * 3 = restraint [0 = no restraint; 1 = padlock: 2 = ice; 3 = falling padlock; 4 = cage
	 * 4 = special [0 = no ice; 
	 * 	 * 
	 * 				1= destroy_one
	 * 				2= Switch_gem_teleport
	 * 				3= bomb
	 * 				4= horiz
	 * 				5= vertic
	 * 				6= horiz_and_vertic
	 * 				7= destroy_all_same_color
	 * 
	 * 				8= more_time
	 * 				9= more_moves
	 * 				10= more_hp
     * 				11 = damage opponent
	 * 				12= rotate_board_L
	 * 				13= rotate_board_R
	 * 
	 * 				-100 = junk
	 * 				-200 = token
	 */

    BoardElementFile brush;
    public enum BrushColor
    {
        Random = -1,
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5,
        G = 6,
    }
    StageFile boardMap;
    BoardElementFile[] mapUndo;
    BoardElementFile[] mapRedo;

    enum tile_hp
	{
		no_tile = -99,
		hp_0 = 0,
		hp_1 = 1,
		hp_2 = 2,
		hp_3 = 3
	}
	tile_hp tile_selected = tile_hp.hp_1;
    /*
	enum special_0_hp_tile
	{
		none
		
		start,
		goal,
		path,
		door_a,
		door_b,
		door_c,
		door_d,
		door_e,
		item_a,
		item_b,
		item_c,
		item_d,
		item_e
	}
	special_0_hp_tile special_0_hp_tile_selected = special_0_hp_tile.none;
    */
	enum tile_content
	{
		empty,
		random_gem,
		gem_color_0,
		gem_color_1,
		gem_color_2,
		gem_color_3,
		gem_color_4,
		gem_color_5,
		gem_color_6,
		block_hp_1,
		block_hp_2,
		block_hp_3,
		/*
		immune_block,
        */
		falling_block_hp_1,
		falling_block_hp_2,
		falling_block_hp_3,
        generativeBlock_hp_1,
        generativeBlock_hp_2,
        generativeBlock_hp_3,
        junk,
		token,
		//bonus_destroy_one,
		//bonus_switch_gem_teleport,
		bonus_bomb,
		bonus_horiz,
		bonus_vertic,
		bonus_horiz_and_vertic,
		bonus_destroy_all_same_color,
		
		bonus_more_time,
		bonus_more_moves,
		bonus_more_hp,
        bonus_damage_opponent
        /*
		bonus_rotate_board_L
		bonus_rotate_board_R

		need_a,
		need_b,
		need_c,
		need_d,
		need_e,
		key_a,
		key_b,
		key_c,
		key_d,
		key_e*/

    }
    tile_content tile_content_selected = tile_content.random_gem;

	enum restraint
	{
		none,
		padlock_hp_1,
		padlock_hp_2,
		padlock_hp_3,

		ice_hp_1,
		ice_hp_2,
		ice_hp_3,

        falling_padlock_hp_1,
        falling_padlock_hp_2,
        falling_padlock_hp_3,

        cage_hp_1,
        cage_hp_2,
        cage_hp_3,

    }
	restraint restraint_selected = restraint.none;
	/*
	enum special
	{
		none
		time_bomb,

	}
	special special_selected = special.none;*/

	bool a_tile_is_selected;
	bool allow_padlock_and_complication;
	//bool start_or_goal_is_selected;
	

	public GameObject editable_tile_obj;
	

	float size_button = 50;

	
	int x_button_group;
	int y_button_group;
	
	//board dimensions
		int x_tiles = 15;
		int board_x_tiles;
		int y_tiles = 15;
		int board_y_tiles;
		//limits of board dimensions
		int MAX_x = 30;
		int MAX_y = 30;
		int MIN_x = 5;
		int MIN_y = 5;
	bool updated_x_value;
	bool updated_y_value;
	

	public static bool _show_board = false; 


	string be_careful = "You'll lose all unsaved modifications";


	Rect windowRect;
	int window_edges = 20;
	int up_start_position = 100;

	Rect create_board_window = new Rect(20,
	                                  	0,
	                                 	0,
	                                    0);
	Rect tiles_window 		= new Rect(20 + 430,
	                            		0,
	                            		300,0);
	Rect preview_window 	= new Rect(20 + 735,
	                                0,
	                                80,118);
		Rect preview_tile = new Rect(10,60,60,60);




	// Scroll position
	Vector2 scrollPos = Vector2.zero;


    public static StageFile mapToEdit;

    static void Init() {

        MapEditorWindow window = (MapEditorWindow)EditorWindow.CreateInstance (typeof (MapEditorWindow));
        window.Show();
    }

    public static void MyInit(StageFile thisMap = null)
    {
        //don't open more than a window 
        MapEditorWindow window = (MapEditorWindow)EditorWindow.GetWindow(typeof(MapEditorWindow));
        if (window)
            window.Close();

        mapToEdit = thisMap;
        Init();
    }

    void OnEnable()
		{
        mapEditorTextures = new MapEditorTextures();
        mapEditorTextures.LoadTextures();

        brush = new BoardElementFile();
        boardMap = ScriptableObject.CreateInstance("StageFile") as StageFile;

        if (mapToEdit != null)
            LoadMap(mapToEdit);


        }

    private void OnInspectorUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            Debug.Log("click!");
    }

    bool painting = false;
    bool mouseDown = false;
    bool[] tileClicked;
    void OnGUI () {

        BeginWindows ();
			create_board_window  = GUILayout.Window(0,create_board_window ,Create_board_window, "File");
			GUILayout.Window(1,preview_window,Tile_preview, "Preview" , GUILayout.Width(80),GUILayout.Height(100));	
			GUILayout.Window(1,preview_window,Tile_preview, "Preview" , "window");

			tiles_window = GUILayout.Window(2,tiles_window,Tiles_tools, "Tile proprieties");
			if (_show_board)
				{

            //paint test
            if (allowPaintMode)
                {
                if (mouseDown)
                    Repaint();

                Event e = Event.current;
                if (e.type == EventType.MouseUp)
                {
                    mouseDown = false;

                    if (painting)
                    {
                        painting = false;
                    }
                }
                else if (e.type == EventType.MouseDown && e.button == 0)
                {
                    if (!mouseDown)
                    {
                        mouseDown = true;
                        tileClicked = new bool[x_tiles * y_tiles];
                    }
                }
                if (mouseDown && e.type == EventType.MouseDrag && e.button == 0)
                    painting = true;
                }

            windowRect = new Rect(window_edges, window_edges+up_start_position, position.width-window_edges*2, position.height-window_edges*2-up_start_position);
				windowRect = GUILayout.Window(3, windowRect, Board_window, "Board");

            

        }
		EndWindows ();
	

	}


    void NewMap(int x, int y)
    {
        boardMap.width = board_x_tiles = x_tiles = x;
        boardMap.height = board_y_tiles = y_tiles = y;
        tileClicked = new bool[x_tiles * y_tiles];
        boardMap.map = new BoardElementFile[boardMap.width * boardMap.height];
        mapRedo = new BoardElementFile[x_tiles * y_tiles];
        mapUndo = new BoardElementFile[x_tiles * y_tiles];
        mapUndo = new BoardElementFile[x_tiles * y_tiles];
        Array.Copy(boardMap.map, mapUndo, boardMap.map.Length);
        can_redo = false;
    }

    void Board_creation()
	{
		_show_board = false;

        NewMap(x_tiles, y_tiles);

        Fill();

		_show_board = true;
    }

	void Create_board_window(int windowID)
	{
        if (GUILayout.Button("Create new map"))
			{
			if (_show_board)
				{
				if (EditorUtility.DisplayDialog("New map",be_careful,"Ok","Cancel") )
					Board_creation();
					
				}
			else
				Board_creation();
			}

		EditorGUILayout.BeginHorizontal();
			x_tiles = EditorGUILayout.IntSlider ("x", x_tiles, MIN_x, MAX_x);
			y_tiles = EditorGUILayout.IntSlider ("y", y_tiles, MIN_y, MAX_y);
		EditorGUILayout.EndHorizontal();
        
        if (_show_board)
            {
            if (GUILayout.Button("Save"))
                SaveMap("NewMap");
            }


    }


	void Fill()
	{
        Debug.Log("Fill()");
		if (_show_board)
			{
            Array.Copy(boardMap.map, mapUndo, boardMap.map.Length);
			can_redo = false;
			}

		for(int y = 0; y < board_y_tiles; y++)
		{
			for (int x = 0; x < board_x_tiles; x++)
			{
               boardMap.SetTile(x,y,CopyBoardElementFile(brush));
			}
		}
	}



    BrushColor brushColor;
    void Tiles_tools(int windowID)
	{


        tile_selected = (tile_hp)EditorGUILayout.EnumPopup("tile", tile_selected);
        if (tile_selected != tile_hp.no_tile)
				{
				a_tile_is_selected = true;

                    if (tile_selected == tile_hp.hp_0)
                        brush.tile = boardMap.InstantiateTileElement(TypeOfTile.Normal, 0);
                    else if (tile_selected == tile_hp.hp_1)
                        brush.tile = boardMap.InstantiateTileElement(TypeOfTile.Normal, 1);
                    else if (tile_selected == tile_hp.hp_2)
                        brush.tile = boardMap.InstantiateTileElement(TypeOfTile.Normal, 2);
                    else if (tile_selected == tile_hp.hp_3)
                        brush.tile = boardMap.InstantiateTileElement(TypeOfTile.Normal, 3);

        }
			else
				{
                brush.tile = boardMap.InstantiateTileElement(TypeOfTile.None, -99);
                brush.content = boardMap.EmptyContent(); ;
                brush.restrain = boardMap.IntantiateRestrain(TypeOfRestrain.None, 0);

				a_tile_is_selected = false;

				tile_content_selected = tile_content.empty;

				restraint_selected = restraint.none;

				}
				
			
			if ( (tile_content_selected == tile_content.random_gem)
			    || (tile_content_selected == tile_content.gem_color_0)
			    || (tile_content_selected == tile_content.gem_color_1)
			    || (tile_content_selected == tile_content.gem_color_2)
			    || (tile_content_selected == tile_content.gem_color_3)
			    || (tile_content_selected == tile_content.gem_color_4)
		    	|| (tile_content_selected == tile_content.gem_color_5)
		    	|| (tile_content_selected == tile_content.gem_color_6)
		   	 	)
			{
				allow_padlock_and_complication = true;
			}
			else
				{
				allow_padlock_and_complication = false;

						restraint_selected = restraint.none;

				}
			
			if (a_tile_is_selected)
				{
					tile_content_selected = (tile_content)EditorGUILayout.EnumPopup("tile content",tile_content_selected);
                    if (tile_content_selected == tile_content.empty)
                        {
                        brush.content = boardMap.EmptyContent(); 
                        }
                     else
                        {
                        if (tile_content_selected == tile_content.block_hp_1)
                            brush.content = boardMap.IntantiateBlock(1, TypeOfContent.Block);
                        else if (tile_content_selected == tile_content.block_hp_2)
                            brush.content = boardMap.IntantiateBlock(2, TypeOfContent.Block);
                        else if (tile_content_selected == tile_content.block_hp_3)
                            brush.content = boardMap.IntantiateBlock(3, TypeOfContent.Block);

                        else if (tile_content_selected == tile_content.falling_block_hp_1)
                            brush.content = boardMap.IntantiateBlock(1, TypeOfContent.FallingBlock);
                        else if (tile_content_selected == tile_content.falling_block_hp_2)
                            brush.content = boardMap.IntantiateBlock(2, TypeOfContent.FallingBlock);
                        else if (tile_content_selected == tile_content.falling_block_hp_3)
                            brush.content = boardMap.IntantiateBlock(3, TypeOfContent.FallingBlock);

                        else if (tile_content_selected == tile_content.generativeBlock_hp_1)
                            brush.content = boardMap.IntantiateBlock(1, TypeOfContent.GenerativeBlock);
                        else if (tile_content_selected == tile_content.generativeBlock_hp_2)
                            brush.content = boardMap.IntantiateBlock(2, TypeOfContent.GenerativeBlock);
                        else if (tile_content_selected == tile_content.generativeBlock_hp_3)
                            brush.content = boardMap.IntantiateBlock(3, TypeOfContent.GenerativeBlock);

                        else if (tile_content_selected == tile_content.bonus_bomb)
                            brush.content = boardMap.IntantiateBonus(Bonus.Destroy3x3);
                        else if (tile_content_selected == tile_content.bonus_damage_opponent)
                            brush.content = boardMap.IntantiateBonus(Bonus.DamageOpponent);
                        else if (tile_content_selected == tile_content.bonus_destroy_all_same_color)
                            brush.content = boardMap.IntantiateBonus(Bonus.DestroyAllGemsWithThisColor);
                        else if (tile_content_selected == tile_content.bonus_horiz)
                            brush.content = boardMap.IntantiateBonus(Bonus.DestroyHorizontal);
                        else if (tile_content_selected == tile_content.bonus_horiz_and_vertic)
                            brush.content = boardMap.IntantiateBonus(Bonus.DestroyHorizontalAndVertical);
                        else if (tile_content_selected == tile_content.bonus_more_hp)
                            brush.content = boardMap.IntantiateBonus(Bonus.HealMe);
                        else if (tile_content_selected == tile_content.bonus_more_moves)
                            brush.content = boardMap.IntantiateBonus(Bonus.GiveMoreMoves);
                        else if (tile_content_selected == tile_content.bonus_more_time)
                            brush.content = boardMap.IntantiateBonus(Bonus.GiveMoreTime);
                        else if (tile_content_selected == tile_content.bonus_vertic)
                            brush.content = boardMap.IntantiateBonus(Bonus.DestroyVertical);

                        else if (tile_content_selected == tile_content.gem_color_0)
                            brush.content = boardMap.IntantiateGem((ContentColor)0);
                        else if (tile_content_selected == tile_content.gem_color_1)
                            brush.content = boardMap.IntantiateGem((ContentColor)1);
                        else if (tile_content_selected == tile_content.gem_color_2)
                            brush.content = boardMap.IntantiateGem((ContentColor)2);
                        else if (tile_content_selected == tile_content.gem_color_3)
                            brush.content = boardMap.IntantiateGem((ContentColor)3);
                        else if (tile_content_selected == tile_content.gem_color_4)
                            brush.content = boardMap.IntantiateGem((ContentColor)4);
                        else if (tile_content_selected == tile_content.gem_color_5)
                            brush.content = boardMap.IntantiateGem((ContentColor)5);
                        else if (tile_content_selected == tile_content.gem_color_6)
                            brush.content = boardMap.IntantiateGem((ContentColor)6);
                        else if (tile_content_selected == tile_content.random_gem)
                            brush.content = boardMap.IntantiateGem(ContentColor.Random);

                        else if (tile_content_selected == tile_content.junk)
                            brush.content = boardMap.IntantiateJunk();
                        else if (tile_content_selected == tile_content.token)
                            brush.content = boardMap.IntantiateToken();
                
                    }

            if (allow_padlock_and_complication)
            {
                restraint_selected = (restraint)EditorGUILayout.EnumPopup("restraint", restraint_selected);

                if (restraint_selected == restraint.none)
                    brush.restrain = boardMap.IntantiateRestrain(TypeOfRestrain.None, 0);
                else  if (restraint_selected == restraint.padlock_hp_1)
                    brush.restrain = boardMap.IntantiateRestrain(TypeOfRestrain.Padlock, 1);
                else if (restraint_selected == restraint.padlock_hp_2)
                    brush.restrain = boardMap.IntantiateRestrain(TypeOfRestrain.Padlock, 2);
                else if (restraint_selected == restraint.padlock_hp_3)
                    brush.restrain = boardMap.IntantiateRestrain(TypeOfRestrain.Padlock, 3);
                else if (restraint_selected == restraint.ice_hp_1)
                    brush.restrain = boardMap.IntantiateRestrain(TypeOfRestrain.Ice, 1);
                else if (restraint_selected == restraint.ice_hp_2)
                    brush.restrain = boardMap.IntantiateRestrain(TypeOfRestrain.Ice, 2);
                else if (restraint_selected == restraint.ice_hp_3)
                    brush.restrain = boardMap.IntantiateRestrain(TypeOfRestrain.Ice, 3);
                else if (restraint_selected == restraint.falling_padlock_hp_1)
                    brush.restrain = boardMap.IntantiateRestrain(TypeOfRestrain.FallingPadlock, 1);
                else if (restraint_selected == restraint.falling_padlock_hp_2)
                    brush.restrain = boardMap.IntantiateRestrain(TypeOfRestrain.FallingPadlock, 2);
                else if (restraint_selected == restraint.falling_padlock_hp_3)
                    brush.restrain = boardMap.IntantiateRestrain(TypeOfRestrain.FallingPadlock, 3);
                else if (restraint_selected == restraint.cage_hp_1)
                    brush.restrain = boardMap.IntantiateRestrain(TypeOfRestrain.Cage, 1);
                else if (restraint_selected == restraint.cage_hp_2)
                    brush.restrain = boardMap.IntantiateRestrain(TypeOfRestrain.Cage, 2);
                else if (restraint_selected == restraint.cage_hp_3)
                    brush.restrain = boardMap.IntantiateRestrain(TypeOfRestrain.Cage, 3);
            }
            else
            {
                brush.restrain = boardMap.IntantiateRestrain(TypeOfRestrain.None, 0);
            }
        }
		
	}


    void Tile_preview(int windowID)
	{
        mapEditorTextures.ShowTextures(preview_tile, brush);

        if (_show_board)
			{
			if (GUILayout.Button("Fill"))
				Fill();

			if (can_redo)
				{
				if (GUILayout.Button("Redo"))
					Board_redo();
				}
			else
				{
				if (GUILayout.Button("Undo"))
					Board_undo();
				}
			}
	}

	void Board_redo()
	{
        Debug.Log("Board_redo()");
        Array.Copy(mapRedo, boardMap.map, mapRedo.Length);
        can_redo = false;
	}

	void Board_undo()
	{
        Debug.Log("Board_undo()");
        mapRedo = new BoardElementFile[x_tiles * y_tiles];

        Array.Copy(boardMap.map, mapRedo, boardMap.map.Length);
        Array.Copy(mapUndo, boardMap.map, mapUndo.Length);
        can_redo = true;
	}

	void Board_window(int windowID) {

		float window_size_x = (float)(size_button*board_x_tiles);
		float window_size_y = (float)(size_button*board_y_tiles);

		GUILayout.Label("");

		scrollPos = GUI.BeginScrollView (
			new Rect (0, 15, windowRect.width, windowRect.height-15), 
			scrollPos, 
			new Rect (0, 0, window_size_x, window_size_y)
			);

			for(int y = 0; y < board_y_tiles; y++)
				{
				for (int x = 0; x < board_x_tiles; x++)
					{
                    TileInput(x, y);
                    Show_tile(x,y);
                    }
				}

        GUI.EndScrollView ();
		
	}

    bool allowPaintMode = true;
    void TileInput(int x, int y)
    {
        if (!allowPaintMode)
            return;

        if (tileClicked[boardMap.GetTileIndexFromCoordinates(x,y)])
            return;


        Rect tempRect = new Rect(x * size_button, y * size_button, size_button, size_button);
        if (!tempRect.Contains(Event.current.mousePosition))
            return;

        if (painting)
        {
            tileClicked[boardMap.GetTileIndexFromCoordinates(x, y)] = true;
            boardMap.SetTile(x, y, CopyBoardElementFile(brush));
        }
        else
        {
            if (mouseDown)
            {
            tileClicked[boardMap.GetTileIndexFromCoordinates(x, y)] = true;
                Array.Copy(boardMap.map, mapUndo, boardMap.map.Length);
                can_redo = false;

                boardMap.SetTile(x, y, CopyBoardElementFile(brush));
            }

        }
    }

    void Show_tile(int x, int y)
	{

        Rect tempRect = new Rect(x * size_button, y * size_button, size_button, size_button);

        if (!allowPaintMode)
        {
            if (GUI.Button(tempRect, mapEditorTextures.tile))
            {
                Array.Copy(boardMap.map, mapUndo, boardMap.map.Length);
                can_redo = false;
                boardMap.SetTile(x, y, CopyBoardElementFile(brush));
                Debug.Log("click tile: " + x + "," + y + " = " + boardMap.GetTile(x,y).content.type);
            }
        }

        mapEditorTextures.ShowTextures(tempRect, boardMap.GetTile(x, y));
        
	}




    BoardElementFile CopyBoardElementFile(BoardElementFile target)
    {

        BoardElementFile copy = new BoardElementFile();
        copy.content = target.content;
        copy.tile = target.tile;
        copy.restrain = target.restrain;

        return copy;
    }

    void SaveMap(string levelName)
    {
        Debug.Log("SaveMap: " + boardMap.map.Length);

        StageFile asset = mapToEdit;

        if (mapToEdit == null)
            asset = ScriptableObject.CreateInstance("BoardMap") as StageFile;


            asset.map = new BoardElementFile[boardMap.map.Length];

            asset.height = board_y_tiles;
            asset.width = board_x_tiles;
        
            for (int i = 0; i < boardMap.map.Length;i ++)
                {
                asset.map[i] = new BoardElementFile();
                asset.map[i] = CopyBoardElementFile(boardMap.map[i]);
                }



        //Write to file
        if (mapToEdit == null)
        {
            AssetDatabase.CreateAsset(asset, "Assets/3match/Resources/Maps/" + levelName + ".asset");
            AssetDatabase.Refresh();
        }
    }

    void LoadMap(int currentLevel)
    {

        StageFile asset = Resources.Load("Maps/" + "NewMap") as StageFile;
        if (asset == null)
            return;

        NewMap(asset.width, asset.height);

        for (int i = 0; i < asset.map.Length; i++)
            boardMap.map[i] = CopyBoardElementFile(asset.map[i]);

        Array.Copy(boardMap.map, mapUndo, boardMap.map.Length);
        _show_board = true;

    }

    void LoadMap(StageFile asset)
    {
        if (asset == null)
            return;

        if (asset.map == null || asset.map.Length < 10)
        {
            Board_creation();
            return;
        }

        NewMap(asset.width, asset.height);

        for (int i = 0; i < asset.map.Length; i++)
            boardMap.map[i] = CopyBoardElementFile(asset.map[i]);

        Array.Copy(boardMap.map, mapUndo, boardMap.map.Length);
        _show_board = true;
    }

}
