using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour {

    GameObject current_obj;
    GameObject previous_obj;

    Board_C board;

    // Use this for initialization
    void Start () {
        board = GetComponent<Board_C>();

    }
	
	// Update is called once per frame
	void LateUpdate () {

        if (board.game_end)
            return;

        if (Input.GetMouseButtonUp(0))
            MouseUp();

        if (EventSystem.current.IsPointerOverGameObject())//don't click through UI
            return; 

        Ray ray3d = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo3d;
        if (Physics.Raycast(ray3d, out hitInfo3d))
        {
            GameObject thisObj = hitInfo3d.collider.transform.gameObject;

            if (thisObj.GetComponent<tile_C>() != null)
                MouseOver_Tile(thisObj);

        }

    }


    void MouseUp()
    {
        board.touch_number--;
        if (board.touch_number < 0)
            board.touch_number = 0;
    }

    void MouseOver_Tile(GameObject thisObj)
    {

        //mouse enter and exit
        current_obj = thisObj;
        if (current_obj != previous_obj)
        {
            if (previous_obj != null)
                previous_obj.GetComponent<tile_C>().MyOnMouseExit();

            current_obj.GetComponent<tile_C>().MyOnMouseEnter();

            previous_obj = current_obj;
        }

        //mouse click
        if (Input.GetMouseButtonDown(0))
            thisObj.GetComponent<tile_C>().MyOnMouseDown();

        if (Input.GetMouseButtonUp(0))
            MouseUp();

        
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("board.number_of_gems_to_move: " + board.number_of_gems_to_move);
            thisObj.GetComponent<tile_C>().DebugClick();
        }
        




    }
}
