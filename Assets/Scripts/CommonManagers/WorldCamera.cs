using UnityEngine;
using System.Collections;

public class WorldCamera : MonoBehaviour
{

	#region structs

	//box limit structs
	public struct BoxLimit
	{
		public float leftLimit;
		public float rightLimit;
		public float topLimit;
		public float bottomLimit;
	}

	#endregion

	#region class variables

	public static BoxLimit cameraLimits = new BoxLimit ();
	public static BoxLimit mouseScrollLimits = new BoxLimit ();
	public static WorldCamera instance;

	private float cameraMoveSpeed = 60.0f;
	private float shiftBonus = 45.0f;
	private float mouseBoundary = 25.0f;

	public static SetupAndSpawningScript setupScript;

	#endregion

	void Awake ()
	{
		instance = this;
		setupScript = GameObject.Find ("GameMaster").GetComponent<SetupAndSpawningScript> ();
	}

	void Start ()
	{
		//Declare camera limits
		cameraLimits.leftLimit = 0.0f; // 0
		cameraLimits.rightLimit = GameMasterScript.instance.gridWidth; // width
		cameraLimits.topLimit = 0.0f; // 0
		cameraLimits.bottomLimit = GameMasterScript.instance.gridHeight * -1; // -height

		//Declare mouse scroll limits
		mouseScrollLimits.leftLimit = mouseBoundary;
		mouseScrollLimits.rightLimit = mouseBoundary;
		mouseScrollLimits.topLimit = mouseBoundary;
		mouseScrollLimits.bottomLimit = mouseBoundary;

	}

	void Update ()
	{
		if (GameMasterScript.instance.IMB.currentState != InputManagerBehavior.InputState.disabled) {
			if (CheckIfUserCameraInput ()) {
				Vector3 cameraDesiredMove = GetDesiredTranslation ();

				if (!isDesiredPositionOverBoundaries (cameraDesiredMove)) {
					this.transform.Translate (cameraDesiredMove);
				}
			}
		}
	}

	//Check if the user is inputting commands for the camera to move
	public bool CheckIfUserCameraInput ()
	{
		bool keyboardMove;
		bool mouseMove;
		bool canMove;

		//check keyboard
		if (WorldCamera.AreCameraKeyboardButtonsPressed ()) {
			keyboardMove = true;
		} else {
			keyboardMove = false;
		}

		//check mouse position
		if (WorldCamera.IsMousePositionWithinBoundaries ()) {
			mouseMove = true;
		} else {
			mouseMove = false;
		}

		if (keyboardMove || mouseMove) {
			canMove = true;
		} else {
			canMove = false;
		}

		return canMove;
	}

	//Works out the cameras desired location depending on the players input
	public Vector3 GetDesiredTranslation ()
	{
		float moveSpeed = 0f;
		float desiredX = 0f;
		float desiredY = 0f;

		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
			moveSpeed = (cameraMoveSpeed + shiftBonus) * Time.deltaTime;
		} else {
			moveSpeed = cameraMoveSpeed * Time.deltaTime;
		}

		//Move via keyboard
		if (Input.GetKey (KeyCode.W)) {
			desiredY = moveSpeed;
		}

		if (Input.GetKey (KeyCode.S)) {
			desiredY = -moveSpeed;
		}

		if (Input.GetKey (KeyCode.A)) {
			desiredX = -moveSpeed;
		}

		if (Input.GetKey (KeyCode.D)) {
			desiredX = moveSpeed;
		}

		//move via mouse
		if (Input.mousePosition.x < mouseScrollLimits.leftLimit) {
			desiredX = -moveSpeed * Time.deltaTime;
		}

		if (Input.mousePosition.x > (Screen.width - mouseScrollLimits.rightLimit)) {
			desiredX = moveSpeed * Time.deltaTime;
		}

		if (Input.mousePosition.y < mouseScrollLimits.bottomLimit) {
			desiredY = -moveSpeed * Time.deltaTime;
		}

		if (Input.mousePosition.y > (Screen.height - mouseScrollLimits.topLimit)) {
			desiredY = moveSpeed * Time.deltaTime;
		}

		return new Vector3 (desiredX, desiredY, 0);
	}

	//checks if the desired position crosses boundaries
	public bool isDesiredPositionOverBoundaries (Vector3 desiredPosition)
	{
		bool overBoundaries = false;
		//check boundaries
		if ((this.transform.position.x + desiredPosition.x) < cameraLimits.leftLimit) {
			overBoundaries = true;
		}

		if ((this.transform.position.x + desiredPosition.x) > cameraLimits.rightLimit) {
			overBoundaries = true;
		}

		if ((this.transform.position.y + desiredPosition.y) > cameraLimits.topLimit) {
			overBoundaries = true;
		}

		if ((this.transform.position.y + desiredPosition.y) < cameraLimits.bottomLimit) {
			overBoundaries = true;
		}

		return overBoundaries;
	}

	public void GetDesiredZoom ()
	{
		float mouseScrollWheelDirection = Input.GetAxis ("Mouse ScrollWheel");

		if (mouseScrollWheelDirection > 0.0f) {
			//zoom in
			if (Camera.main.orthographicSize > 5) { //min zoom
				Camera.main.orthographicSize -= 10 * Time.deltaTime;
			}
		} else if (mouseScrollWheelDirection < 0.0f) {
			//zoom out
			if (Camera.main.orthographicSize < 7.5f) { //max zoom
				Camera.main.orthographicSize += 10 * Time.deltaTime;
			}
		}

	}


	#region Helper functions

	public static bool AreCameraKeyboardButtonsPressed ()
	{
		return Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.D);
	}

	public static bool IsMousePositionWithinBoundaries ()
	{
		return  (Input.mousePosition.x < mouseScrollLimits.leftLimit && Input.mousePosition.x > -5) ||
		(Input.mousePosition.x > (Screen.width - mouseScrollLimits.rightLimit) && Input.mousePosition.x < (Screen.width + 5)) ||
		(Input.mousePosition.y < mouseScrollLimits.bottomLimit && Input.mousePosition.y > -5) ||
		(Input.mousePosition.y > (Screen.height - mouseScrollLimits.topLimit) && Input.mousePosition.y < (Screen.height + 5));
	}

	#endregion


}
