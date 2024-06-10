//======================             UpsyVR             ======================
//
//  Purpose: Left Controller event listener.
//
//  This script should be added on the Left Controller.
//
//  Ming Han started the program and wrote this script in August, 2018. Ming-Cheng Miao re-edited it in May, 2021
//============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpsyVR_LeftController : MonoBehaviour {

	SteamVR_Controller.Device device;
	public GameObject _Upsy_ControllerEvents;

    // Use this for initialization
    void Start ()
	{
		device = SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index);
		_Upsy_ControllerEvents = GameObject.Find ("UpsyVR_VRController") as GameObject;
	}
	
	// Update is called once per frame
	void Update ()
	{
		device = SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index);

		///   Trigger
		if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
		{
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_Trigger = true;	
		}
		else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
		{
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_Trigger = false;
		}
		else if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_Trigger_Axis = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis1).x;
		}
		else
        {
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_Trigger_Axis = 0f;

		}

		///   Grip
		if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
		{
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_Grip = true;
		}
		else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
		{
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_Grip = false;
		}

		///   Menu
		if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
		{
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_Menu = true;
		}
		else if (device.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
		{
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_Menu = false;
		}

		///   System
		if (device.GetPressDown(SteamVR_Controller.ButtonMask.System))
		{
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_System = true;
		}
		else if (device.GetPressUp(SteamVR_Controller.ButtonMask.System))
		{
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_System = false;
		}

		///   Touchpad
		if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
		{
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_Touchpad = true;
		}
		else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
		{
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_Touchpad = false;
		}
		else if (device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
		{
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_TouchpadTouch = true;
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_Touchpad_Axis = device.GetAxis();
		}
        else
        {
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_TouchpadTouch = false;
			_Upsy_ControllerEvents.GetComponent<UpsyVR_ControllerEvents>().Left_Touchpad_Axis = new Vector2(0f, 0f);
		}
	}
}
