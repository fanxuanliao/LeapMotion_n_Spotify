import sys
sys.path.insert(0, "./leaplib")
import Leap
from Leap import CircleGesture, KeyTapGesture, ScreenTapGesture, SwipeGesture
import os, pygame, random, wx

last_gesture = ""

class SampleListener(Leap.Listener):
    finger_names = ['Thumb', 'Index', 'Middle', 'Ring', 'Pinky']
    bone_names = ['Metacarpal', 'Proximal', 'Intermediate', 'Distal']
    state_names = ['STATE_INVALID', 'STATE_START', 'STATE_UPDATE', 'STATE_END']

    def on_init(self, controller):
        print "Initialized"

    def on_connect(self, controller):
        print "Connected"

        # Enable gestures
        controller.enable_gesture(Leap.Gesture.TYPE_CIRCLE);
        controller.enable_gesture(Leap.Gesture.TYPE_KEY_TAP);
        controller.enable_gesture(Leap.Gesture.TYPE_SCREEN_TAP);
        controller.enable_gesture(Leap.Gesture.TYPE_SWIPE);

    def on_disconnect(self, controller):
        # Note: not dispatched when running in a debugger.
        print "Disconnected"

    def on_exit(self, controller):
        print "Exited"

    def on_frame(self, controller):
        global last_gesture
        # Get the most recent frame and report some basic information
        frame = controller.frame()
        previous = controller.frame(1)
        """
        print "Frame id: %d, timestamp: %d, hands: %d, fingers: %d, tools: %d, gestures: %d" % (
              frame.id, frame.timestamp, len(frame.hands), len(frame.fingers), len(frame.tools), len(frame.gestures()))
        """
        """
        # Get hands
        for hand in frame.hands:

            handType = "Left hand" if hand.is_left else "Right hand"

            print "  %s, id %d, position: %s" % (
                handType, hand.id, hand.palm_position)

            # Get the hand's normal vector and direction
            normal = hand.palm_normal
            direction = hand.direction

            # Calculate the hand's pitch, roll, and yaw angles
            print "  pitch: %f degrees, roll: %f degrees, yaw: %f degrees" % (
                direction.pitch * Leap.RAD_TO_DEG,
                normal.roll * Leap.RAD_TO_DEG,
                direction.yaw * Leap.RAD_TO_DEG)

            # Get arm bone
            arm = hand.arm
            print "  Arm direction: %s, wrist position: %s, elbow position: %s" % (
                arm.direction,
                arm.wrist_position,
                arm.elbow_position)

            # Get fingers
            for finger in hand.fingers:

                print "    %s finger, id: %d, length: %fmm, width: %fmm" % (
                    self.finger_names[finger.type],
                    finger.id,
                    finger.length,
                    finger.width)

                # Get bones
                for b in range(0, 4):
                    bone = finger.bone(b)
                    print "      Bone: %s, start: %s, end: %s, direction: %s" % (
                        self.bone_names[bone.type],
                        bone.prev_joint,
                        bone.next_joint,
                        bone.direction)
        """
        # Get tools
        for tool in frame.tools:

            print "  Tool id: %d, position: %s, direction: %s" % (
                tool.id, tool.tip_position, tool.direction)

        # Get gestures
        for gesture in frame.gestures():
            """
            if gesture.type == Leap.Gesture.TYPE_CIRCLE:
                circle = CircleGesture(gesture)
                # Determine clock direction using the angle between the pointable and the circle normal
                if circle.pointable.direction.angle_to(circle.normal) <= Leap.PI/2:
                    clockwiseness = "clockwise"
                    #if last_gesture == "clockwise":
                    #    print "clockwise"
                    last_gesture = "clockwise"
                else:
                    clockwiseness = "counterclockwise"
                    #if last_gesture == "counterclockwise":
                    #    print "counterclockwise"
                    last_gesture = "counterclockwise"

                # Calculate the angle swept since the last frame
                swept_angle = 0
                if circle.state != Leap.Gesture.STATE_START:
                    previous_update = CircleGesture(controller.frame(1).gesture(circle.id))
                    swept_angle =  (circle.progress - previous_update.progress) * 2 * Leap.PI

                print "  Circle id: %d, %s, progress: %f, radius: %f, angle: %f degrees, %s" % (
                        gesture.id, self.state_names[gesture.state],
                        circle.progress, circle.radius, swept_angle * Leap.RAD_TO_DEG, clockwiseness)
            """
            if gesture.type == Leap.Gesture.TYPE_SWIPE:
                swipe = SwipeGesture(gesture)
                if swipe.direction[0] > 0 and -0.5 < swipe.direction[1] < 0.5:
                    if last_gesture == "swipe_right":
                        continue
                    else:
                        print "swipe to right"
                    last_gesture = "swipe_right"
                elif swipe.direction[0] < 0 and -0.5 < swipe.direction[1] < 0.5:
                    if last_gesture == "swipe_left":
                        continue
                    else:
                        print "swipe to left"
                    last_gesture = "swipe_left"
                elif swipe.direction[1] < 0:
                    if last_gesture == "swipe_down":
                        last_gesture = ""
                        break
                    else:
                        print "swipe to down"
                    last_gesture = "swipe_down"
                """
                print "  Swipe id: %d, state: %s, position: %s, direction: %s, speed: %f" % (
                        gesture.id, self.state_names[gesture.state],
                        swipe.position, swipe.direction, swipe.speed)
                """
            """
            if gesture.type == Leap.Gesture.TYPE_KEY_TAP:
                keytap = KeyTapGesture(gesture)
                print "key tap"
                last_gesture = "key_tap"

                print "  Key Tap id: %d, %s, position: %s, direction: %s" % (
                        gesture.id, self.state_names[gesture.state],
                        keytap.position, keytap.direction )
            """
            """
            if gesture.type == Leap.Gesture.TYPE_SCREEN_TAP:
                screentap = ScreenTapGesture(gesture)
                print "screen tap"
                last_gesture = "screen_tap"

                print "  Screen Tap id: %d, %s, position: %s, direction: %s" % (
                        gesture.id, self.state_names[gesture.state],
                        screentap.position, screentap.direction )
            """
        """
        if not (frame.hands.is_empty and frame.gestures().is_empty):
            print ""
        """
    def state_string(self, state):
        if state == Leap.Gesture.STATE_START:
            return "STATE_START"

        if state == Leap.Gesture.STATE_UPDATE:
            return "STATE_UPDATE"

        if state == Leap.Gesture.STATE_STOP:
            return "STATE_STOP"

        if state == Leap.Gesture.STATE_INVALID:
            return "STATE_INVALID"

musicUrlList = []

def musicUrlLoader():
	fileList = os.listdir(".")
	for filename in fileList:
		if filename.endswith(".mp3"):
			print("Find music file",filename)
			musicUrlList.append(filename)

class MyMusicPlayer(wx.Frame):
	def __init__(self,superion):
		wx.Frame.__init__(self,parent = superion, title = 'Xinspurer Player',size = (400,300))
		musicUrlLoader()
		MainPanel = wx.Panel(self)
		MainPanel.SetBackgroundColour('black')

		self.ShowInfoText = wx.StaticText(parent = MainPanel, label = 'Player not started', pos = (100,100)
										  ,size = (185,25),style = wx.ALIGN_CENTER_VERTICAL)
		self.ShowInfoText.SetBackgroundColour('white')

		self.isPaused = False
		self.StartPlayButton = wx.Button(parent = MainPanel, label = 'Random play', pos = (100,150))
		self.Bind(wx.EVT_BUTTON, self.OnStartClicked, self.StartPlayButton)

		self.PauseOrContinueButton = wx.Button(parent = MainPanel, label = 'Player paused', pos = (200,150))
		self.Bind(wx.EVT_BUTTON, self.OnPauseOrContinueClicked, self.PauseOrContinueButton)
		self.PauseOrContinueButton.Enable(False)

		pygame.mixer.init()


	def OnStartClicked(self,event):
		self.isPaused = False
		self.PauseOrContinueButton.Enable(True)

		self.willPlayMusic = random.choice(musicUrlList)
		pygame.mixer.music.load(self.willPlayMusic.encode())
		pygame.mixer.music.play()

		self.ShowInfoText.SetLabel("Now played:"+self.willPlayMusic)


	def OnPauseOrContinueClicked(self,event):
		if not self.isPaused:
			self.isPaused = True
			pygame.mixer.music.pause()
			self.PauseOrContinueButton.SetLabel('Keep playing')

			self.ShowInfoText.SetLabel('Player is paused')
		else:
			self.isPaused = False
			pygame.mixer.music.unpause()
			self.PauseOrContinueButton.SetLabel('Stop playing')

			self.ShowInfoText.SetLabel("Now playing:" + self.willPlayMusic)

def main():
    # Create a sample listener and controller
    listener = SampleListener()
    controller = Leap.Controller()

    # Have the sample listener receive events from the controller
    controller.add_listener(listener)
    """
    app = wx.App()
    myMusicPlayer = MyMusicPlayer(None)
    myMusicPlayer.Show()
    app.MainLoop()
    """
    # Keep this process running until Enter is pressed
    print "Press Enter to quit..."
    try:
        sys.stdin.readline()
    except KeyboardInterrupt:
        pass
    finally:
        # Remove the sample listener when done
        controller.remove_listener(listener)


if __name__ == "__main__":
    main()
