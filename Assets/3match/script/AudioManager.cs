using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Board_C board;

    public AudioClip explosion_sfx;
    public AudioClip end_fall_sfx;
    [HideInInspector] public float latest_sfx_time;
    public AudioClip bad_move_sfx;
    public AudioClip win_sfx;
    public AudioClip lose_sfx;
    [HideInInspector] public AudioClip[] bonus_sfx;
    [HideInInspector] public int play_this_bonus_sfx = -1; // -1 = don't play

    [System.Serializable]
    public class CharacterSFX
    {
        public AudioSource HPAudioSource;
        public AudioClip loseHP;
        public AudioClip gainHP;
        public AudioClip unharmed;
    }

    public CharacterSFX playerSFX;
    public CharacterSFX enemySFX;

    private AudioSource audioSource; // Cache the AudioSource

    void Start()
    {
        // Cache the AudioSource component
        audioSource = GetComponent<AudioSource>();
    }

    public void Play_sfx(AudioClip my_clip)
    {
        if (board.menuKitBridge.Stage_uGUI_obj) // Use menu kit win screen
        {
            board.menuKitBridge.SfxMenuKit(my_clip);
        }
        else
        {
            if (my_clip != null)
            {
                // Stop the currently playing sound before playing a new one
                audioSource.Stop(); // Stop the current sound
                audioSource.clip = my_clip;
                audioSource.Play(); // Play the new sound
            }
        }
    }

    public void Play_bonus_sfx(int bonus_id, bool call_from_GUIbutton = false)
    {
        if (bonus_sfx[bonus_id] != null)
        {
            play_this_bonus_sfx = bonus_id;
            Play_sfx(bonus_sfx[bonus_id]);
        }
        else
        {
            play_this_bonus_sfx = -1;
        }

        if (call_from_GUIbutton)
        {
            play_this_bonus_sfx = -1;
        }
    }

    public AudioClip GetGemExplosionSfx(int gemColor)
    {
        return explosion_sfx;
    }
}
