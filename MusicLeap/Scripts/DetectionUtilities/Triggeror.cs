/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2018.                                 *
 * Leap Motion proprietary and confidential.                                  *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Leap.Unity {

  public class Triggeror : MonoBehaviour {

    [Tooltip("Dispatched when condition is detected.")]
    public UnityEvent OnTrigger;

    public virtual void Trigger(){
      OnTrigger.Invoke();
    }

  }
}
