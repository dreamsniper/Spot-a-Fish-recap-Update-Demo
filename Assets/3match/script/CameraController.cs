using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Camera Board_camera;
    [HideInInspector]public CameraTemplate myCameraTemplate;

    public enum CameraPosition
    {
        centred_to_board,
        centred_to_move,
        //centred_to_player_avatar,
        //manual
    }
    //centred_to_move:
    float new_camera_position_x;
    float new_camera_position_y;
    Vector3 new_camera_position;


    public void Setup_camera(int _X_tiles, int _Y_tiles, /*Transform pivot_board,*/ bool Stage_uGUI_obj)//call from Awake
    {
        if (Board_camera)
        {
            if (Stage_uGUI_obj)//avoid 2 AudioListener when use menu kit
                Board_camera.GetComponent<AudioListener>().enabled = false;
            else
                Board_camera.GetComponent<AudioListener>().enabled = true;

            if (myCameraTemplate.camera_position_choice == CameraPosition.centred_to_board)
            {
                Board_camera.transform.position = new Vector3(/*pivot_board.transform.position.x +*/ (_X_tiles - 1) * 0.5f + myCameraTemplate.camera_adjust.x,
                                                               /*pivot_board.transform.position.y*/ + (_Y_tiles - 1) * -0.5f + myCameraTemplate.camera_adjust.y,
                                                               /*pivot_board.transform.position.z*/ + myCameraTemplate.camera_adjust.z
                                                               );
            }
            else if (myCameraTemplate.camera_position_choice == CameraPosition.centred_to_move)
            {
                Board_camera.transform.position = (/*pivot_board.transform.position + */myCameraTemplate.camera_adjust);
            }

            if (myCameraTemplate.adaptive_zoom)
            {
                //if (Screen.width < Screen.height)
                if (Board_camera.pixelWidth < Board_camera.pixelHeight)
                {
                    Board_camera.orthographicSize = (_X_tiles + (myCameraTemplate.camera_zoom * -(_X_tiles))) * 0.5f;
                }
                else
                    {
                    Board_camera.orthographicSize = (_Y_tiles + (myCameraTemplate.camera_zoom * -2)) * 0.5f;
                    }
            }
            else
            {
                Board_camera.orthographicSize = 4 + myCameraTemplate.camera_zoom * -1;
            }

            if (Board_camera.orthographicSize <= 0)
                Board_camera.orthographicSize = 1;
        }

    }


    public void Center_camera_to_move(int main_gem_selected_x, int main_gem_selected_y , int _X_tiles, int _Y_tiles, /*Transform pivot_board,*/ float accuracy)// call from: Board_C.Switch.SwitchingGems()
    {
        if (myCameraTemplate.camera_position_choice == CameraPosition.centred_to_move)
        {

            if (Vector2.Distance(new Vector2(main_gem_selected_x, main_gem_selected_y), new Vector2(new_camera_position_x, Mathf.Abs(new_camera_position_y))) >= myCameraTemplate.camera_move_tolerance)
            {

                new_camera_position_x = main_gem_selected_x /*- pivot_board.position.x*/;
                if (new_camera_position_x < myCameraTemplate.margin.x)
                    new_camera_position_x = myCameraTemplate.margin.x;
                else if (new_camera_position_x > _X_tiles - myCameraTemplate.margin.x)
                    new_camera_position_x = _X_tiles - myCameraTemplate.margin.x;

                new_camera_position_y = /*pivot_board.position.y -*/ main_gem_selected_y;
                if (new_camera_position_y * -1 < myCameraTemplate.margin.y)
                    new_camera_position_y = myCameraTemplate.margin.y * -1;
                else if (new_camera_position_y * -1 > _Y_tiles - myCameraTemplate.margin.y)
                    new_camera_position_y = (_Y_tiles - myCameraTemplate.margin.y) * -1;


                new_camera_position = new Vector3(new_camera_position_x, new_camera_position_y, Board_camera.transform.position.z);

                StartCoroutine(Move_Camera(accuracy));
            }
        }
    }


    IEnumerator Move_Camera(float accuracy)
    {

        while (Vector3.Distance(Board_camera.gameObject.transform.position, new_camera_position) > accuracy * 10)
        {
            yield return new WaitForSeconds(0.015f);
            Board_camera.gameObject.transform.Translate(((new_camera_position - Board_camera.gameObject.transform.position).normalized) * myCameraTemplate.camera_speed * Time.deltaTime, Space.World);
        }

    }
}
