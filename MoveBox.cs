using UnityEngine;
using System;
using System.Collections;

public class MoveBox : MonoBehaviour {

	//lerping
	public Vector3 startPosition;
	public Vector3 leftPosition;
	public Vector3 rightPosition;
	public Vector3 upPosition;
	public Vector3 downPosition;

	public Vector3 upOffset;
	public Vector3 downOffset;
	public Vector3 leftOffset;
	public Vector3 rightOffset;

	public bool pushingLeft = false;
	public bool pushingRight = false;
	public bool pushingUp = false;
	public bool pushingDown = false;

	public bool pullingLeft = false;
	public bool pullingRight = false;
	public bool pullingUp = false;
	public bool pullingDown = false;

	public bool pushing = false;
	public bool pulling = false;

	public bool movingLeft = false;
	public bool movingRight = false;
	public bool movingUp = false;
	public bool movingDown = false;

	public float distToAdjacent;
	public float distToRobot;
	public float pullDist;
	public RaycastHit hit;
	public int raycastLayer;

	public bool beingPushed = false;
	public bool pushingOtherBox = false;

	public GameObject beingPushedBy;
	public bool childAgainstWall = false;

	public bool hitSomething = false;

	public GameObject robot;
	public Vector3 leftRobotPosition;
	public Vector3 rightRobotPosition;
	public Vector3 upRobotPosition;
	public Vector3 downRobotPosition;

	public GameObject ghostbox;
	public Vector3 leftGhostboxPosition;
	public Vector3 rightGhostboxPosition;
	public Vector3 upGhostboxPosition;
	public Vector3 downGhostboxPosition;

	public Vector3 leftPullGhostboxPosition;
	public Vector3 rightPullGhostboxPosition;
	public Vector3 upPullGhostboxPosition;
	public Vector3 downPullGhostboxPosition;

	public bool movingDisabled = false;

	//time
	public TimeSpan startTime;
	public float startSeconds;
	public float currentSeconds;
	public float totalSeconds;

	public bool timeSet = false;
	public bool startCurrentSeconds = false;
	
	public TimeManagementScript timeManagementScript;
	
	public bool created = false;

	//pulling from CollisionsScript
	public CollisionsScript center;
	public CollisionsScript left;
	public CollisionsScript right;
	public CollisionsScript up;
	public CollisionsScript down;

	public GameObject sphere;
	
	//preserve between scenes but don't spawn again if it already exists
	void Awake() {
		if (!created) {
			DontDestroyOnLoad (this);
			created = true;
		} else {
			Destroy(this.gameObject);
		}
		
		//pulling from CollisionsScript
		center = this.transform.Find ("center").GetComponent<CollisionsScript>();
		left = this.transform.Find ("left").GetComponent<CollisionsScript>();
		right = this.transform.Find ("right").GetComponent<CollisionsScript>();
		up = this.transform.Find ("up").GetComponent<CollisionsScript>();
		down = this.transform.Find ("down").GetComponent<CollisionsScript>();
	}

	//hide when not in swamp
	void OnLevelWasLoaded() {
		if (Application.loadedLevelName != "swamp") {
			this.GetComponent<Renderer>().enabled = false;
			this.GetComponent<Collider>().isTrigger = true;
		} else {
			this.GetComponent<Renderer>().enabled = true;
			this.GetComponent<Collider>().isTrigger = false;
		}
	}

	void Start () {
		//lerping
		startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		leftPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2);
		rightPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 2);
		upPosition = new Vector3(transform.position.x + 2, transform.position.y, transform.position.z);
		downPosition = new Vector3(transform.position.x - 2, transform.position.y, transform.position.z);

		upOffset = startPosition + new Vector3(0.84f, 0, 0);
		downOffset = startPosition + new Vector3(-0.84f, 0, 0);

		//replace with directional pos x4
		leftRobotPosition = startPosition + new Vector3(0, 0, 1.75f);
		rightRobotPosition = startPosition + new Vector3(0, 0, -1.75f);

		//replace with directional pos x4
		leftGhostboxPosition = startPosition + new Vector3(0, 0, 2);
		rightGhostboxPosition = startPosition + new Vector3(0, 0, -2);

		//replace with directional pos x4
		leftPullGhostboxPosition = startPosition + new Vector3(0, 0, 3);
		rightPullGhostboxPosition = startPosition + new Vector3(0, 0, -3);
	
		//set it up so you can pull currentSeconds from TimeManagementScript
		GameObject time = GameObject.Find ("time");
		timeManagementScript = time.GetComponent<TimeManagementScript>();

		distToAdjacent = 1.16f;
		distToRobot = 2.66f;
		pullDist = 3.16f;
		raycastLayer = 1 << 8;
	}

	void Update () {
		if (gameObject.tag == "box" && !timeSet && center.looking && (left.left || right.right || up.up || down.down) && !movingDisabled) {

			//pushing
			if (left.left && Input.GetButtonDown ("Push")) {
				movingDisabled = true;
				pushingRight = true;
				Pushing(Vector3.back, Vector3.forward);
			}
			if (right.right && Input.GetButtonDown ("Push")) {;
				movingDisabled = true;
				pushingLeft = true;
				Pushing(Vector3.forward, Vector3.back);
			}
			if (up.up && Input.GetButtonDown ("Push")) {
				movingDisabled = true;
				pushingDown = true;
				//coroutine
			}
			if (down.down && Input.GetButtonDown ("Push")) {
				movingDisabled = true;
				pushingUp = true;
				//coroutine
			}

			//pulling
			if (left.left && Input.GetButtonDown ("Pull")) {
				movingDisabled = true;
				pullingLeft = true;
				Pulling(Vector3.forward);
			}
			if (right.right && Input.GetButtonDown ("Pull")) {
				movingDisabled = true;
				pullingRight = true;
				Pulling(Vector3.back);
			}
		}

		//get currentSeconds from TimeManagementScript
		if (startCurrentSeconds) {
			currentSeconds = timeManagementScript.currentSeconds;
		}

		if (movingRight) {
			transform.position = Vector3.Lerp(startPosition, rightPosition, (currentSeconds - startSeconds) / totalSeconds);
		}

		if (movingLeft) {
			transform.position = Vector3.Lerp(startPosition, leftPosition, (currentSeconds - startSeconds) / totalSeconds);
		}

		if (movingDown) {
			transform.position = Vector3.Lerp(startPosition, downPosition, (currentSeconds - startSeconds) / totalSeconds);
		}

		if (movingUp) {
			transform.position = Vector3.Lerp(startPosition, upPosition, (currentSeconds - startSeconds) / totalSeconds);
		}

		//reset when done moving!
		if (startSeconds + totalSeconds < currentSeconds) {
			StopCoroutine("StartMoving");
			startCurrentSeconds = false;
			transform.position = new Vector3(Mathf.Round (transform.position.x), Mathf.Round (transform.position.y), Mathf.Round (transform.position.z));
			movingLeft = false;
			movingRight = false;
			movingUp = false;
			movingDown = false;
			ResetPushDirections();
			ResetPullDirections();
			pushing = false;
			pulling = false;
			if (this.beingPushedBy != null) {
				this.beingPushedBy = null;
			}
			startSeconds = 0;
			currentSeconds = 0;
			timeSet = false;
			if (robot != null) {
				Destroy(robot);
			}
			if (ghostbox != null) {
				Destroy(ghostbox);
			}
			startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			leftPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2);
			rightPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 2);
			upPosition = new Vector3(transform.position.x + 2, transform.position.y, transform.position.z);
			downPosition = new Vector3(transform.position.x - 2, transform.position.y, transform.position.z);
			upOffset = startPosition + new Vector3(0.84f, 0, 0);
			downOffset = startPosition + new Vector3(-0.84f, 0, 0);
			//reset leftOffset
			//reset rightOffset
			//replace with directional pos x4
			leftRobotPosition = startPosition + new Vector3(0, 0, 1.75f);
			rightRobotPosition = startPosition + new Vector3(0, 0, -1.75f);
			//replace with directional pos x4
			leftGhostboxPosition = startPosition + new Vector3(0, 0, 2);
			rightGhostboxPosition = startPosition + new Vector3(0, 0, -2);
			//replace with directional pos x4
			leftPullGhostboxPosition = startPosition + new Vector3(0, 0, 3);
			rightPullGhostboxPosition = startPosition + new Vector3(0, 0, -3);
			this.gameObject.tag = "box";
			movingDisabled = false;
		}
	}

	public IEnumerator StartMoving() {
		if (!startCurrentSeconds) {

			timeSet = true;
			startTime = DateTime.Now.TimeOfDay;
			startSeconds = (float)startTime.TotalSeconds;

			Debug.Log (startSeconds);

			if (pushingLeft) {
				ghostbox.transform.position = leftGhostboxPosition;
				movingLeft = true;
			}
			if (pushingRight) {
				ghostbox.transform.position = rightGhostboxPosition;
				movingRight = true;
			}
			//add directions x2

			if (pullingLeft) {
				ghostbox.transform.position = leftPullGhostboxPosition;
				movingLeft = true;
			}
			if (pullingRight) {
				ghostbox.transform.position = rightPullGhostboxPosition;
				movingRight = true;
			}
			//add directions x2

			this.gameObject.tag = "movingbox";

			if (this.beingPushedBy == null) {
				robot = (GameObject)Instantiate(Resources.Load ("robot"));

				if (pushingRight || pullingLeft) {
					robot.transform.position = leftRobotPosition;
				}

				if (pushingLeft || pullingRight) {
					robot.transform.position = rightRobotPosition;
				}
				//add directions x2

				robot.transform.parent = this.gameObject.transform;
				//check if colliding w/ player and move him/her; should this be in robot script?
			}

			startCurrentSeconds = true;
		}
		yield return null;
	}

	public void Pushing(Vector3 forwardDirection, Vector3 backwardDirection) {
		//z axis is fucked so left/right (back/forward) are reversed
		if (this.beingPushedBy == null) {
			if (Physics.Raycast (upOffset, backwardDirection, out hit, distToRobot, raycastLayer)
			    || Physics.Raycast (downOffset, backwardDirection, out hit, distToRobot, raycastLayer)) {
				if (hit.collider.gameObject.tag == "ghostbox"
				    || hit.collider.gameObject.tag == "robot"
				    || hit.collider.gameObject.tag == "movingbox") {
					Debug.Log ("robot cannot be spawned in space a box is moving to");
					ResetPushDirections();
					movingDisabled = false;
					hitSomething = true;
				}
				Debug.Log (hit.collider.gameObject.name);
			}
		}
		if (!hitSomething
		    && (Physics.Raycast (upOffset, forwardDirection, out hit, distToAdjacent, raycastLayer)
		    || Physics.Raycast (downOffset, forwardDirection, out hit, distToAdjacent, raycastLayer))) {
			if (hit.collider.gameObject.tag == "box") {
				Debug.Log ("hitting an adjacent box");
				sphere = (GameObject)Instantiate(Resources.Load ("Sphere"));
				sphere.transform.position = hit.point;
				hit.collider.gameObject.GetComponent<MoveBox>().beingPushedBy = this.gameObject;
				if (pushingRight) {
					hit.collider.gameObject.GetComponent<MoveBox>().pushingRight = true;
					hit.collider.gameObject.GetComponent<MoveBox>().Pushing(Vector3.back, Vector3.forward);
				}
				if (pushingLeft) {
					hit.collider.gameObject.GetComponent<MoveBox>().pushingLeft = true;
					hit.collider.gameObject.GetComponent<MoveBox>().Pushing(Vector3.forward, Vector3.back);
				}
				//add directions x2
				//iterate the childAgainstWall bool upward to any parents
				if (childAgainstWall) {
					if (this.beingPushedBy != null) {
						this.beingPushedBy.GetComponent<MoveBox>().childAgainstWall = true;
						this.beingPushedBy = null;
					}
					childAgainstWall = false;
					ResetPushDirections();
					movingDisabled = false;
				}
				//move if no children are against wall
				else if (!childAgainstWall) {
					ghostbox = (GameObject)Instantiate(Resources.Load ("ghostbox"));
					StartCoroutine("StartMoving");
				}
			}
			else if (hit.collider.gameObject.tag == "pushboundary"
			         || hit.collider.gameObject.tag == "ghostbox"
			         || hit.collider.gameObject.tag == "movingbox"
			         || hit.collider.gameObject.tag == "robot") {
				//insert GUI here: "hitting a wall" || "hitting a ghostbox" || "hitting a movingbox"
				Debug.Log("hitting push boundary, ghostbox, or moving box");
				if (this.beingPushedBy != null) {
					this.beingPushedBy.GetComponent<MoveBox>().childAgainstWall = true;
					this.beingPushedBy = null;
				}
				ResetPushDirections();
				movingDisabled = false;
			}
			else {
				ghostbox = (GameObject)Instantiate(Resources.Load ("ghostbox"));
				StartCoroutine("StartMoving");
			}
		}
		else if (!hitSomething) {
			ghostbox = (GameObject)Instantiate(Resources.Load ("ghostbox"));
			StartCoroutine("StartMoving");
		}
		hitSomething = false;
	}

	public void ResetPushDirections() {
		pushingLeft = false;
		pushingRight = false;
		pushingUp = false;
		pushingDown = false;
	}

	public void Pulling(Vector3 pullDirection) {
		if (Physics.Raycast (upOffset, pullDirection, out hit, pullDist, raycastLayer)
		    || Physics.Raycast (downOffset, pullDirection, out hit, pullDist, raycastLayer)) {
			if (hit.collider.gameObject.tag == "pullboundary"
			    || hit.collider.gameObject.tag == "ghostbox"
			    || hit.collider.gameObject.tag == "robot"
			    || hit.collider.gameObject.tag == "movingbox"
			    || hit.collider.gameObject.tag == "box") {
				Debug.Log("hitting pull boundary, ghostbox, robot, or moving box");
				ResetPullDirections();
				movingDisabled = false;
			}
			else {
				ghostbox = (GameObject)Instantiate(Resources.Load ("pull_ghostbox"));
				StartCoroutine("StartMoving");
			}
		}
		else {
			ghostbox = (GameObject)Instantiate(Resources.Load ("pull_ghostbox"));
			StartCoroutine("StartMoving");
		}
	}

	public void ResetPullDirections() {
		pullingLeft = false;
		pullingRight = false;
		pullingUp = false;
		pullingDown = false;
	}
}
