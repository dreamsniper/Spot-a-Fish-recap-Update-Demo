using UnityEngine;
using System.Collections;

public partial class tile_C : MonoBehaviour {

    public AudioSource audioSource;
    void Play_sfx(AudioClip clip)
    {
        if (clip == null)
            return;

        if (board.menuKitBridge.Stage_uGUI_obj)
        {
            board.menuKitBridge.SfxMenuKit(clip);

        }
        else
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

    }

}
