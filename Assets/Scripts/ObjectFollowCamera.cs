﻿using UnityEngine;
using System.Collections;

public class ObjectFollowCamera : MonoBehaviour {

	public float TransitionTime = 1.0f;
	public float Distance = 10;
	public float ScreenSides = 0.15f;
	public Vector2 MoveSpeed;
	public Vector2 RotateAroundSpeed = Vector2.one * 30.0f;


	float currentTime;
	Vector3 _targetObject;
	public Vector3 TargetObject {
		get {
			return _targetObject;
		}
		set{
			_targetObject = value;
			currentTime = 0.0f;
		}
	}
	public void SetDirection(Vector3 dir)
	{
		Direction = dir;
	}

	public void SetDirection(Vector3 dir, bool invertIfNeeded)
	{
		if (!invertIfNeeded) {
			Direction = dir;
		} else {
			if ((Position - (TargetObject + dir)).sqrMagnitude > (Position - (TargetObject - dir)).sqrMagnitude)
				Direction = -dir;
			else
				Direction = dir;
		}
	}

	public void Rotate(float angle)
	{
		SetDirection (Quaternion.AngleAxis (angle, Vector3.up) * Direction);
	}


	public Vector3 Direction
	{ 
		get; 
		private set; 
	}

	Vector3 _Target;
	public Vector3 Target {
		get{
			return _Target;
		}
		private set {
			_Target = value;
			gameObject.transform.LookAt (_Target);
		}
	}

	Vector3 _position;
	public Vector3 Position
	{
		get{
			return _position;
		}
		private set{
			_position = value;
			gameObject.transform.position = _position;
			gameObject.transform.LookAt (_Target);
		}
	}



	bool middleButtonWasPressed = false;
	Vector2 mouseLastPosition = Vector2.zero;

	void Start () {
		//if (GetComponent<Rigidbody> ())
		//	GetComponent<Rigidbody> ().freezeRotation = true;

		currentTime = TransitionTime;
		TargetObject = Vector3.zero;
		SetDirection (Vector3.right + Vector3.up * 0.5f);
	}



	// Update is called once per frame
	void LateUpdate () {

		Target = Vector3.Lerp(Target, TargetObject, currentTime / TransitionTime);
		Position = Vector3.Lerp (Position, TargetObject + Direction * Distance, currentTime / TransitionTime);


		if (currentTime < TransitionTime)
			currentTime += Time.deltaTime;


		Distance += Input.GetAxis ("Mouse ScrollWheel");
		if (Distance < 0.01f)
			Distance = 0.01f;
		Vector2 pos = new Vector2 (Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
		// pos relative to lower left corner


		if (pos.x < ScreenSides) {
			float frac = ((ScreenSides - pos.x) / ScreenSides);
			Vector3 shift = -gameObject.transform.right * Time.deltaTime * MoveSpeed.x * frac;

			Position += shift;
			TargetObject += shift;
			Target += shift;
		}
		if (pos.x > 1 - ScreenSides) {

			float frac = (pos.x - (1.0f - ScreenSides)) / ScreenSides;
			Vector3 shift = gameObject.transform.right * Time.deltaTime * MoveSpeed.x * frac;

			Position += shift;
			TargetObject += shift;
			Target += shift;
		}

		if (pos.y < ScreenSides) {
			float frac = ((ScreenSides - pos.y) / ScreenSides);
			Vector3 forward = Vector3.Cross (Vector3.up, gameObject.transform.right);
			Vector3 shift = forward * Time.deltaTime * MoveSpeed.y * frac;
				
			Position += shift;
			TargetObject += shift;
			Target += shift;

		}
		if (pos.y > 1 - ScreenSides) {
			float frac = (pos.y - (1.0f - ScreenSides)) / ScreenSides;

			Vector3 forward = Vector3.Cross (Vector3.up, gameObject.transform.right);

			Vector3 shift = -forward * Time.deltaTime * MoveSpeed.y * frac;
			Position += shift;
			TargetObject += shift;
			Target += shift;
		}

		if (Input.GetMouseButton (2)) {
			if (!middleButtonWasPressed) {
				mouseLastPosition = Input.mousePosition;
			}

			Vector2 dif = (Vector2)Input.mousePosition - mouseLastPosition;

			Direction = Quaternion.AngleAxis (-dif.x * RotateAroundSpeed.x * Time.deltaTime, Vector3.up) * Direction;
			Direction = Quaternion.AngleAxis (-dif.y * RotateAroundSpeed.y * Time.deltaTime, transform.right) * Direction;


			mouseLastPosition = Input.mousePosition;
			middleButtonWasPressed = true;
		} else {
			middleButtonWasPressed = false;
		}
	}
}