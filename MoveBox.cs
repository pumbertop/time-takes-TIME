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

	public bool pulling = false;

	public bool movingLeft = false;
	public bool movingRight = false;
	public bool movingUp = false;
	public bool movingDown = false;

	public float distToAdjacent;
	public RaycastHit hit;
	public int raycastLayer;

	public bool beingPushed = false;
	public bool pushingOtherBox = false;

	public GameObject beingPushedBy;
	public bool childAgainstWall = false;

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
	
		//set it up so you can pull currentSeconds from TimeManagementScript
		GameObject time = GameObject.Find ("time");
		timeManagementScript = time.GetComponent<TimeManagementScript>();

		distToAdjacent = 1.2f;
		raycastLayer = 1 << 8;
	}

	void Update () {
		if (gameObject.tag == "box" && !timeSet && center.looking && (left.left || right.right || up.up || down.down) && !movingDisabled) {
			if (left.left && Input.GetButtonDown ("Push")) {
				movingDisabled = true;
				PushingRight();
				//pushingRight = false;
			}
			if (right.right && Input.GetButtonDown ("Push")) {;
				movingDisabled = true;
				//coroutine
			}
			if (up.up && Input.GetButtonDown ("Push")) {
				movingDisabled = true;
				//coroutine
			}
			if (down.down && Input.GetButtonDown ("Push")) {
				movingDisabled = true;
				//coroutine
			}
			if (Input.GetButtonDown ("Pull")) {
				movingDisabled = true;
				//coroutine
			}
		}

		//get currentSeconds from TimeManagementScript
		if (startCurrentSeconds) {
			currentSeconds = timeManagementScript.currentSeconds;
		}

		if (movingRight) {
			transform.position = Vector3.Lerp(startPosition, rightPosition, (currentSeconds - startSeconds) / totalSeconds);
		}

		//reset when done moving!
		if (startSeconds + totalSeconds < currentSeconds) {
			StopCoroutine("StartMoving");
			startCurrentSeconds = false;
			transform.position = new Vector3(Mathf.Round (transform.position.x), Mathf.Round (transform.position.y), Mathf.Round (transform.position.z));
			pushingLeft = false;
			pushingRight = false;
			pushingUp = false;
			pushingDown = false;
			movingLeft = false;
			movingRight = false;
			movingUp = false;
			movingDown = false;
			if (this.beingPushedBy != null) {
				this.beingPushedBy = null;
			}
			startSeconds = 0;
			currentSeconds = 0;
			timeSet = false;
			startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			leftPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2);
			rightPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 2);
			upPosition = new Vector3(transform.position.x + 2, transform.position.y, transform.position.z);
			downPosition = new Vector3(transform.position.x - 2, transform.position.y, transform.position.z);
			upOffset = startPosition + new Vector3(0.84f, 0, 0);
			downOffset = startPosition + new Vector3(-0.84f, 0, 0);
			movingDisabled = false;
		}
	}

	public IEnumerator StartMoving() {
		if (!startCurrentSeconds) {
			timeSet = true;
			startTime = DateTime.Now.TimeOfDay;
			startSeconds = (float)startTime.TotalSeconds;

			Debug.Log (startSeconds);

			startCurrentSeconds = true;
		}
		yield return null;
	}

	void PushingRight() {
		//z axis is fucked so left/right are reversed
		if (//replace this with raycasts x4; all using the same dist, checking for adj box, robot, ghostbox, boundary (x2 in opposite direction); then use tags if u need different effects
		    Physics.Raycast (downOffset, Vector3.back, out hit, distToAdjacent, raycastLayer)) {
			if (hit.collider.gameObject.tag == "box") {
				Debug.Log ("hitting an adjacent box");
				sphere = (GameObject)Instantiate(Resources.Load ("Sphere"));
				sphere.transform.position = hit.point;
				hit.collider.gameObject.GetComponent<MoveBox>().beingPushedBy = this.gameObject;
				hit.collider.gameObject.GetComponent<MoveBox>().PushingRight();
				//iterate the childAgainstWall bool upward to any parents
				if (childAgainstWall) {
					if (this.beingPushedBy != null) {
						this.beingPushedBy.GetComponent<MoveBox>().childAgainstWall = true;
						this.beingPushedBy = null;
					}
					pushingRight = false;
					childAgainstWall = false;
					movingDisabled = false;
				}
				//move if no children are against wall
				else if (!childAgainstWall) {
					movingRight = true;
					StartCoroutine("StartMoving");
				}
			}
			else if (hit.collider.gameObject.tag == "pushboundary" || hit.collider.gameObject.tag == "ghostbox") {
				//insert GUI here: "hitting a wall" || "hitting a ghostbox"
				Debug.Log("hitting push boundary or ghostbox");
				if (this.beingPushedBy != null) {
					this.beingPushedBy.GetComponent<MoveBox>().childAgainstWall = true;
					this.beingPushedBy = null;
				}
				pushingRight = false;
				movingDisabled = false;
			}
		}
		else {
			sphere = (GameObject)Instantiate(Resources.Load ("Sphere"));
			sphere.transform.position = downOffset + new Vector3(0, 0, -distToAdjacent);
			movingRight = true;
			StartCoroutine("StartMoving");
		}
	}
}
