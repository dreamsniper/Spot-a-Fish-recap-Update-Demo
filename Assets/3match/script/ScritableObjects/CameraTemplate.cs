using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "3Match/Camera")]
public class CameraTemplate : ScriptableObject
{

    public Vector3 camera_adjust;
    public float camera_zoom;
    public bool adaptive_zoom;
    public CameraController.CameraPosition camera_position_choice = CameraController.CameraPosition.centred_to_board;

    //centred_to_move:
    public float camera_speed;
    float new_camera_position_x;
    float new_camera_position_y;
    public float camera_move_tolerance;
    public Vector2 margin;


}
