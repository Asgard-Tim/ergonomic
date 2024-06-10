//======================             UpsyVR             ======================
//
//  Purpose: VRController State.
//
//  This script should be added on the "UpsyVR_VRController".
//
//  Ming Han started the program and wrote this script in August, 2018. Ming-Cheng Miao re-edited it in May, 2021
//============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpsyVR_ControllerEvents : MonoBehaviour {
	
	/// <summary>
	/// Left Controller State
	/// </summary>
	public bool Left_Trigger = false;
	public float Left_Trigger_Axis;
	public bool Left_Menu = false;
	public bool Left_System = false;
	public bool Left_Grip = false;
	public bool Left_Touchpad = false;
	public bool Left_TouchpadTouch = false;
    public bool Left_Triggerup = false;
    public Vector2 Left_Touchpad_Axis;
    
    /// <summary>
    /// Right Controller State
    /// </summary>
    public bool Right_Trigger = false;
	public float Right_Trigger_Axis;
	public bool Right_Menu = false;
	public bool Right_System = false;
	public bool Right_Grip = false;
	public bool Right_Touchpad = false;
	public bool Right_TouchpadTouch = false;
    public bool Right_Triggerup = false;
    public Vector2 Right_Touchpad_Axis;

	/// <summary>
	/// HMD
	/// </summary>
	private GameObject Frame, Camera;

	// Use this for initialization
	void Start ()
	{
		Frame = GameObject.Find("SteamVR");
		Camera = GameObject.Find("Camera (eye)");
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	/// <summary>
	/// Put the VR camera back to the center of the scene
	/// </summary>
	public void ReturnCenter()
	{
		float x = Camera.transform.localPosition.x;
		float z = Camera.transform.localPosition.z;
		float alpha = Camera.transform.localEulerAngles.y;
		Frame.transform.localPosition = new Vector3(-x * Mathf.Cos(alpha / 180 * Mathf.PI) + z * Mathf.Sin(alpha / 180 * Mathf.PI), 0f, -x * Mathf.Sin(alpha / 180 * Mathf.PI) - z * Mathf.Cos(alpha / 180 * Mathf.PI));
		Frame.transform.localEulerAngles = new Vector3(0f, -Camera.transform.localEulerAngles.y, 0f);
	}

	/// <summary>
	/// Set the position of the VR camera
	/// </summary>
	public void SetCamPos(Vector3 P)
	{
		float x = Camera.transform.localPosition.x;
		float y = Camera.transform.localPosition.y;
		float z = Camera.transform.localPosition.z;
		float alpha = Camera.transform.localEulerAngles.y;
		Frame.transform.localPosition = new Vector3(-x * Mathf.Cos(alpha / 180 * Mathf.PI) + z * Mathf.Sin(alpha / 180 * Mathf.PI) + P.x, -y + P.y, -x * Mathf.Sin(alpha / 180 * Mathf.PI) - z * Mathf.Cos(alpha / 180 * Mathf.PI) + P.z);
	}

	/// <summary>
	/// Set the rotation of the VR camera
	/// </summary>
	public void SetCamRot(float yaw)
	{
		Frame.transform.localEulerAngles = new Vector3(0f, -Camera.transform.localEulerAngles.y + yaw, 0f);
	}
}
