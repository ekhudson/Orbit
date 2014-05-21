using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrawlerCharacterStateController : MonoBehaviour 
{
    public List<BrawlerCharacterState> CharacterStates;
    public BrawlerHitboxController HitboxController;

    private string mCurrentStateName = string.Empty;
    private Dictionary<string, BrawlerCharacterState> mStateDict = new Dictionary<string, BrawlerCharacterState>();
    private BrawlerPlayerComponent mPlayerComponent;

    private const string kDefaultStateName = "IDLE";

    public BrawlerCharacterState CurrentState
    {
        get
        {
            if (string.IsNullOrEmpty(mCurrentStateName))
            {
                mCurrentStateName = kDefaultStateName;
            }           

            return mStateDict[mCurrentStateName];
        }
    }

    private void Start()
    {
        SetupStateDictionary();
        mPlayerComponent = GetComponent<BrawlerPlayerComponent>();
    }

    private void FixedUpdate()
    {
        if (mPlayerComponent == null || mPlayerComponent.PlayerSpriteRenderer == null || CurrentState == null || CurrentState.CurrentAnimationClip == null)
        {
            return;
        }

        mPlayerComponent.PlayerSpriteRenderer.sprite = CurrentState.CurrentAnimationClip.CurrentSprite;
        HitboxController.ApplySettings(CurrentState.CurrentAnimationClip.CurrentFrameEntry.HeadBoxSettings, HitboxController.HeadCollider, mPlayerComponent.PlayerSpriteRenderer.sprite, mPlayerComponent.CurrentPlayerOrientation);
        HitboxController.ApplySettings(CurrentState.CurrentAnimationClip.CurrentFrameEntry.BodyBoxSettings, HitboxController.BodyCollider, mPlayerComponent.PlayerSpriteRenderer.sprite, mPlayerComponent.CurrentPlayerOrientation);
        HitboxController.ApplySettings(CurrentState.CurrentAnimationClip.CurrentFrameEntry.LegBoxSettings, HitboxController.LegCollider, mPlayerComponent.PlayerSpriteRenderer.sprite, mPlayerComponent.CurrentPlayerOrientation);
        HitboxController.ApplySettings(CurrentState.CurrentAnimationClip.CurrentFrameEntry.AttackBoxSettings, HitboxController.AttackCollider, mPlayerComponent.PlayerSpriteRenderer.sprite, mPlayerComponent.CurrentPlayerOrientation);
        HitboxController.ApplySettings(CurrentState.CurrentAnimationClip.CurrentFrameEntry.CollisionBoxSettings, HitboxController.CollisionCollider, mPlayerComponent.PlayerSpriteRenderer.sprite, mPlayerComponent.CurrentPlayerOrientation);
    }                         

    private void SetupStateDictionary()
    {
        mStateDict.Clear();

        foreach(BrawlerCharacterState state in CharacterStates)
        {
            mStateDict.Add(state.StateName, state);
        }
    }

    public void SetState(string newStateName)
    {
        if (newStateName == mCurrentStateName)
        {
            return;
        } 
        else if (!mStateDict.ContainsKey(newStateName))
        {
            Debug.LogError(string.Format("Tried to switch to animation state {0}, but that state doesn't exist on the Brawler State Controller for {1}", newStateName, gameObject.name));
            return;
        }
        

        mCurrentStateName = newStateName;
        CurrentState.ChooseRandomClip();
        CurrentState.CurrentAnimationClip.Play();
    }
}
