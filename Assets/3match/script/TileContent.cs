using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileContent : MonoBehaviour {

    public SpriteRenderer mySpriteRenderer;
    [SerializeField] Transform myAvatarTransform;
    [SerializeField] Animation myAnimation;

    //DEBUG
    public tile_C myPreviousTile;
    public tile_C myCurrentTile;

    public void TrackMyOwner(tile_C newCurrentTile)
    {
        myPreviousTile = myCurrentTile;
        myCurrentTile = newCurrentTile;
    }

    /*
    public bool ImOrphan()
    {
        if (!this.gameObject.activeSelf)
            return false;

        if (myCurrentTile == null)
        {
            gameObject.name += "  ORPHAN!";
            return true;
        }

        return false;
    }
    */


    public enum CurrentAnimation
    {
        None,
        Create,
        EndFall,
        Destroy,
        ShuffleIn,
        ShuffleOut
    }
    [HideInInspector]public CurrentAnimation currentAnimation = CurrentAnimation.None;

    public void ResetAvatarTranform()
    {
        myAvatarTransform.localPosition = Vector3.zero;
        myAvatarTransform.localRotation = Quaternion.identity;
        myAvatarTransform.localScale = Vector3.one;
    }

    public void PlayAnimation(CurrentAnimation animName)
    {
        myAnimation.Play(animName.ToString());
        currentAnimation = animName;
    }

    public float GetCurrentAnimationDuration()
    {
        return myAnimation[currentAnimation.ToString()].length;
    }

  
 
}
