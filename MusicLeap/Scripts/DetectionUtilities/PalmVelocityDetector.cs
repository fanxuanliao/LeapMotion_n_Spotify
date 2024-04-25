using UnityEngine;
using System.Collections;
using Leap.Unity.Attributes;

namespace Leap.Unity {

  public class PalmVelocityDetector : Detector {
    /**
     * The interval at which to check palm direction.
     * @since 4.1.2
     */
    [Units("seconds")]
    [Tooltip("The interval in seconds at which to check this detector's conditions.")]
    [MinValue(0)]
    public float Period = .1f; //seconds
    /**
     * The HandModelBase instance to observe.
     * Set automatically if not explicitly set in the editor.
     * @since 4.1.2
     */
    [Tooltip("The hand model to watch. Set automatically if detector is on a hand.")]
    public HandModelBase HandModel = null;

    /**
     * Specifies how to interprete the direction specified by MovingDirection.
     *
     * - RelativeToCamera -- the target direction is defined relative to the camera's forward vector, i.e. (0, 0, 1) is the cmaera's
     *                       local forward direction.
     * - RelativeToHorizon -- the target direction is defined relative to the camera's forward vector,
     *                        except that it does not change with pitch.
     * - RelativeToWorld -- the target direction is defined as a global direction that does not change with camera movement. For example,
     *                      (0, 1, 0) is always world up, no matter which way the camera is moving.
     * - AtTarget -- a target object is used as the moving direction (The specified MovingDirection is ignored).
     *
     * In VR scenes, RelativeToHorizon with a direction of (0, 0, 1) for camera forward and RelativeToWorld with a direction
     * of (0, 1, 0) for absolute up, are often the most useful settings.
     * @since 4.1.2
     */
    [Header("Direction Settings")]
    [Tooltip("How to treat the target direction.")]
    public MovingType MovingType = MovingType.RelativeToHorizon;

    /**
    * The target direction as interpreted by the MovingType setting.
    * Ignored when Movingtype is "AtTarget."
    */
    [Tooltip("The target direction.")]
    [DisableIf("MovingType", isEqualTo: MovingType.AtTarget)]
    public Vector3 MovingDirection = Vector3.forward;

    /**
     * The object to point at when the MovingType is "AtTarget." Ignored otherwise.
     */
    [Tooltip("A target object(optional). Use MovingType.AtTarget")]
    [DisableIf("MovingType", isNotEqualTo: MovingType.AtTarget)]
    public Transform TargetObject = null;

    /**
     * The turn-on angle. The detector activates when the palm points within this
     * many degrees of the target direction.
     * @since 4.1.2
     */
    [Tooltip("The angle in degrees from the target direction at which to turn on.")]
    [Range(0, 180)]
    public float OnAngle = 45; // degrees

    /**
    * The turn-off angle. The detector deactivates when the palm points more than this
    * many degrees away from the target direction. The off angle must be larger than the on angle.
    */
    [Tooltip("The angle in degrees from the target direction at which to turn off.")]
    [Range(0, 180)]
    public float OffAngle = 65; //degrees

    /**
    * Minimal speed.
    */
    [Tooltip("Minimal speed.")]
    [MinValue(0)]
    public float MinSpeed = 1; //speeds

    /** Whether to draw the detector's Gizmos for debugging. (Not every detector provides gizmos.)
     */
    [Header("")]
    [Tooltip("Draw this detector's Gizmos, if any. (Gizmos must be on in Unity edtor, too.)")]
    public bool ShowGizmos = true;

    private IEnumerator watcherCoroutine;

    private void OnValidate(){
      if( OffAngle < OnAngle){
        OffAngle = OnAngle;
      }
    }

    private void Awake () {
      watcherCoroutine = palmWatcher();
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
      Vector3 velocity;
      while(true){
        if(HandModel != null){
          hand = HandModel.GetLeapHand();
          if(hand != null){
            velocity = hand.PalmVelocity.ToVector3();
            float angleTo = Vector3.Angle(velocity, selectedDirection(hand.PalmPosition.ToVector3()));
            float speed = velocity.sqrMagnitude;
            if(angleTo <= OnAngle && speed > MinSpeed){
              Activate();
            } else {
              Deactivate();
            }
          }
        }
        yield return new WaitForSeconds(Period);
      }
    }

    private Vector3 selectedDirection (Vector3 tipPosition) {
      switch (MovingType) {
        case MovingType.RelativeToHorizon:
          Quaternion cameraRot = Camera.main.transform.rotation;
          float cameraYaw = cameraRot.eulerAngles.y;
          Quaternion rotator = Quaternion.AngleAxis(cameraYaw, Vector3.up);
          return rotator * MovingDirection;
        case MovingType.RelativeToCamera:
          return Camera.main.transform.TransformDirection(MovingDirection);
        case MovingType.RelativeToWorld:
          return MovingDirection;
        case MovingType.AtTarget:
          if (TargetObject != null)
            return TargetObject.position - tipPosition;
          else return Vector3.zero;
        default:
          return MovingDirection;
      }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos(){
      if(ShowGizmos && HandModel != null && HandModel.IsTracked){
        Color centerColor;
        if (IsActive) {
          centerColor = OnColor;
        } else {
          centerColor = OffColor;
        }
        Hand hand = HandModel.GetLeapHand();
        Utils.DrawCone(hand.PalmPosition.ToVector3(), hand.PalmVelocity.ToVector3(), OnAngle, hand.PalmWidth, centerColor, 8);
        Utils.DrawCone(hand.PalmPosition.ToVector3(), hand.PalmVelocity.ToVector3(), OffAngle, hand.PalmWidth, LimitColor, 8);
        Gizmos.color = DirectionColor;
        Gizmos.DrawRay(hand.PalmPosition.ToVector3(), selectedDirection(hand.PalmPosition.ToVector3()));
      }
    }
    #endif
  }

  /**
  * Settings for handling moving conditions
  * - RelativeToCamera -- the target direction is defined relative to the camera's forward vector.
  * - RelativeToHorizon -- the target direction is defined relative to the camera's forward vector,
  *                        except that it does not change with pitch.
  * - RelativeToWorld -- the target direction is defined as a global direction that does not change with camera movement.
  * - AtTarget -- a target object is used to determine the moving direction.
  *
  * @since 4.1.2
  */
  public enum MovingType { RelativeToCamera, RelativeToHorizon, RelativeToWorld, AtTarget }
}
