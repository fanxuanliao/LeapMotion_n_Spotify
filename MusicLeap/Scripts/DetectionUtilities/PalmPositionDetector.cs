/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2018.                                 *
 * Leap Motion proprietary and confidential.                                  *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap.Unity.Attributes;

namespace Leap.Unity {

  public class PalmPositionDetector : Detector {
    /**
     * The interval at which to check palm direction.
     * @since 4.1.2
     */
    [Units("seconds")]
    [Tooltip("The interval in seconds at which to check this detector's conditions.")]
    [MinValue(0)]
    public float Period = .01f; //seconds
    /**
     * The HandModelBase instance to observe.
     * Set automatically if not explicitly set in the editor.
     * @since 4.1.2
     */
    [Tooltip("The hand model to watch. Set automatically if detector is on a hand.")]
    public HandModelBase HandModel = null;

    /**
    * Plane normal direction.
    */
    [Tooltip("The normal direction.")]
    public Vector3 NormalDirection = Vector3.up;
    Vector3 NormalizedDirection;

    /**
     * The distance in meters between this game object and the target game object that
     * will pass the proximity check.
     * @since 4.1.2
     */
    [Header("Distance Settings")]
    [Tooltip("The target distance in meters to activate the detector.")]
    public float OnDistance = .1f; //meters

    /**
     * The distance in meters between this game object and the target game object that
     * will turn off the detector.
     * @since 4.1.2
     */
    [Tooltip("The distance in meters at which to deactivate the detector.")]
    public float OffDistance = -.1f; //meters

    private IEnumerator watcherCoroutine;

    private void OnValidate(){
      if( OffDistance > OnDistance){
        OffDistance = OnDistance;
      }
    }

    private void Awake () {
      watcherCoroutine = palmWatcher();
      NormalizedDirection = NormalDirection.normalized;
    }

    private void OnEnable () {
      StartCoroutine(watcherCoroutine);
    }

    private void OnDisable () {
      StopCoroutine(watcherCoroutine);
      Deactivate();
    }

    private IEnumerator palmWatcher() {
      Hand hand;
      Vector3 position;
      while(true){
        if(HandModel != null){
          hand = HandModel.GetLeapHand();
          if(hand != null){
            position = hand.PalmPosition.ToVector3();
            float distance = Vector3.Dot(position, NormalizedDirection);
            if(distance >= OnDistance){
              Activate();
            } else if(distance < OffDistance) {
              Deactivate();
            }
          }
        }
        yield return new WaitForSeconds(Period);
      }
    }
  }
}
