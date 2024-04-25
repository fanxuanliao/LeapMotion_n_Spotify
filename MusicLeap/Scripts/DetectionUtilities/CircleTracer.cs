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

  public class CircleTracer : Detector {

    static int NumDetectors = 4;

    public Detector quadrantDetector1;
    public Detector quadrantDetector2;
    public Detector quadrantDetector3;
    public Detector quadrantDetector4;

    private int progress = 0;

    private void Awake(){
      activateDetector(quadrantDetector1, delegate { QuadrantActive(1); });
      activateDetector(quadrantDetector2, delegate { QuadrantActive(2); });
      activateDetector(quadrantDetector3, delegate { QuadrantActive(3); });
      activateDetector(quadrantDetector4, delegate { QuadrantActive(4); });
    }

    private void activateDetector(Detector detector, UnityAction call){
      detector.OnActivate.RemoveListener(call); //avoid double subscription
      detector.OnActivate.AddListener(call);
    }

    private void deactivateDetector(Detector detector, UnityAction call){
      detector.OnDeactivate.RemoveListener(call); //avoid double subscription
      detector.OnDeactivate.AddListener(call);
    }

    private void OnDisable () {
      RunDeactivate();
    }

    private void RunActivate() {
      Activate();
      progress = 0;
    }

    private void RunDeactivate() {
      Deactivate();
      progress = 0;
    }

    private void CheckQuadrant(int d, int target, bool final=false) {
      if ( d == target ) {
        progress++;
        if ( final ) RunActivate();
      } else {
        RunDeactivate();
      }
    }

    private void QuadrantActive(int d) {
      if ( d == 1 ) {
        RunDeactivate();
      }

      switch (progress) {
        case 0: {
          CheckQuadrant(d, 1);
          break;
        }
        case 1: {
          CheckQuadrant(d, 2);
          break;
        }
        case 2: {
          CheckQuadrant(d, 3);
          break;
        }
        case 3: {
          CheckQuadrant(d, 4, true);
          break;
        }
      }

    }
  }
}
