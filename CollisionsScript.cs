using UnityEngine;
using System.Collections;

public class CollisionsScript : MonoBehaviour {

	public bool left = false;
	public bool right = false;
	public bool up = false;
	public bool down = false;
	public bool looking = false;

	public bool outsidePushBoundary = true;
	public bool outsideHPullBoundary = true;
	public bool outsideVPullBoundary = true;

	public bool otherBoxInTheWay = false;

	public bool pushingAgainstOtherBox = false;
	public GameObject child;
	public GameObject newParent;

	public GameObject child2;
	public GameObject newParent2;

	public bool ghostBoxInTheWay = false;
	public bool robotInTheWay = false;

	void OnTriggerEnter(Collider other) {
		//check which box player is in
		if (this.gameObject.name == "left" && other.gameObject.tag == "Player") {
			left = true;
		}
		if (this.gameObject.name == "right" && other.gameObject.tag == "Player") {
			right = true;
		}
		if (this.gameObject.name == "up" && other.gameObject.tag == "Player") {
			up = true;
		}
		if (this.gameObject.name == "down" && other.gameObject.tag == "Player") {
			down = true;
		}

		//check if playereyes are looking at box
		if (this.gameObject.name == "center" && other.gameObject.name == "playereyes") {
			//say playereyes are not looking if the box is already being pushed by another box
			//if (!this.gameObject.transform.parent.gameObject.GetComponent<PushBox>().beingPushed) {
				looking = true;
			//}
		}

		//check if a collider is inside push boundary
		if ((this.gameObject.tag == "hcollider" || this.gameObject.tag == "vcollider") && other.gameObject.tag == "pushboundary") {
			outsidePushBoundary = false;
		}

		//check if a horizontal collider is inside horizontal (left-right) pull boundary
		if (this.gameObject.tag == "hcollider" && other.gameObject.tag == "hpullboundary") {
			outsideHPullBoundary = false;
		}

		//check if a vertical collider is inside vertical (up-down) pull boundary
		if (this.gameObject.tag == "vcollider" && other.gameObject.tag == "vpullboundary") {
			outsideVPullBoundary = false;
		}

		//check if a horizontal collider moves inside another box's horizontal collider (to disable pulling when there isn't enough space)
		if (this.gameObject.tag == "hcollider"  && other.gameObject.tag == "hcollider") {
			otherBoxInTheWay = true;
		}

		//check if a vertical collider moves inside another box's vertical collider (to disable pulling when there isn't enough space)
		if (this.gameObject.tag == "vcollider" && other.gameObject.tag == "vcollider") {
			otherBoxInTheWay = true;
		}

		//check if a box is going to be pushed into another box
		if ((this.gameObject.tag == "hcollider" || this.gameObject.tag == "vcollider") && other.gameObject.name == "center") {
			newParent = this.gameObject.transform.parent.gameObject;
			child = other.gameObject.transform.parent.gameObject;
			//child.transform.parent = newParent.transform;

			pushingAgainstOtherBox = true;
		}

		/*if (this.gameObject.name == "center" && (other.gameObject.tag == "hcollider" || other.gameObject.tag == "vcollider") /*&& pushingAgainstOtherBox) {
			//PushBox beingPushed = GetComponentInParent<PushBox>();
			//bool beingPushed = GetComponentInParent<PushBox>().beingPushed;
			//Debug.Log(beingPushed);
			if (GetComponentInParent<PushBox>().beingPushed) {
				Debug.Log ("gets this far");
				newParent2 = this.gameObject.transform.parent.gameObject;
				child2 = other.gameObject.transform.parent.gameObject;
				child2.transform.parent = newParent2.transform;
			}
		}*/

		if ((this.gameObject.tag == "hcollider" || this.gameObject.tag == "vcollider") && other.gameObject.tag == "ghostcenter") {
			ghostBoxInTheWay = true;
		}

		if (this.gameObject.tag == "hcollider" && other.gameObject.tag == "ghosthcollider") {
			robotInTheWay = true;
		}

		if (this.gameObject.tag == "vcollider" && other.gameObject.tag == "ghostvcollider") {
			robotInTheWay = true;
		}
	}

	void OnTriggerExit(Collider other) {
		//check if player leaves box
		if (this.gameObject.name == "left" && other.gameObject.tag == "Player") {
			left = false;
		}
		if (this.gameObject.name == "right" && other.gameObject.tag == "Player") {
			right = false;
		}
		if (this.gameObject.name == "up" && other.gameObject.tag == "Player") {
			up = false;
		}
		if (this.gameObject.name == "down" && other.gameObject.tag == "Player") {
			down = false;
		}

		//check if playereyes look away from box
		if (this.gameObject.name == "center" && other.gameObject.name == "playereyes") {
			looking = false;
		}

		//check if a collider moves outside push boundary
		if ((this.gameObject.tag == "hcollider" || this.gameObject.tag == "vcollider") && other.gameObject.tag == "pushboundary") {
			outsidePushBoundary = true;
		}

		//check if a collider moves outside pull boundaries

		if (this.gameObject.tag == "hcollider" && other.gameObject.tag == "hpullboundary") {
			outsideHPullBoundary = true;
		}

		if (this.gameObject.tag == "vcollider" && other.gameObject.tag == "vpullboundary") {
			outsideVPullBoundary = true;
		}

		//check if a horizontal collider moves outside another box's horizontal collider (to enable pulling)
		if (this.gameObject.tag == "hcollider" && other.gameObject.tag == "hcollider") {
			otherBoxInTheWay = false;
		}

		//check if a vertical collider moves outside another box's vertical collider (to enable pulling)
		if (this.gameObject.tag == "vcollider" && other.gameObject.tag == "vcollider") {
			otherBoxInTheWay = false;
		}

		//check if box is no longer against another box
		if ((this.gameObject.tag == "hcollider" || this.gameObject.tag == "vcollider") && other.gameObject.name == "center") {
			pushingAgainstOtherBox = false;
		}

		if ((this.gameObject.tag == "hcollider" || this.gameObject.tag == "vcollider") && other.gameObject.tag == "ghostcenter") {
			ghostBoxInTheWay = false;
		}

		if (this.gameObject.tag == "hcollider" && other.gameObject.tag == "ghosthcollider") {
			robotInTheWay = false;
		}
		
		if (this.gameObject.tag == "vcollider" && other.gameObject.tag == "ghostvcollider") {
			robotInTheWay = false;
		}
	}
}
