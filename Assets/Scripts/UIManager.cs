using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using GameSparks.Core;
using GameSparks.Api.Messages;
using GameSparks.Api;
using System;


/// <summary>
/// GameSparksManager
/// Created by GameSparks for FEN Poptropica 2
/// Padraig O' Connor Aug 2 2016
/// v1.1
/// </summary>
public class UIManager : MonoBehaviour {

	public Text logText;
	private Queue<string> myLogQueue = new Queue<string>();
	private string myLog;

	public GameObject items, scenes, menu, playerDetails, inbox;

	public Button goto_sceneState_bttn, goto_items_bttn, goto_player_bttn, goTo_player_inbox, goTo_menu_bttn1, goTo_menu_bttn2;

	public Button clearLog_bttn;
	public Text auth_username_txt;
	public InputField auth_password_input;
	public Button auth_bttn;
	public Text reg_username_txt, reg_password, reg_displayname;
	public Button reg_bttn;

	public Button getInv_bttn, useItem_bttn, equipItem_bttn, moveItem_bttn, pickupItem_bttn, dropItem_bttn;

	public Dropdown dropdown_prefab;

	public InputField dropInvId, dropSceneId, dropX, dropY;
	public InputField pickupPlacedItemId, pickupSceneId;
	public InputField moveInvId, moveDestId;
	public InputField equipItemId;
	public InputField useItemId;


	public Button getState_bttn, setState_bttn;
	public InputField islandIDGet, sceneIDGet, islandIDSet, sceneIDSet, jsonIDSet;

	public GameObject blockout_panel;

	public Button grantXp_bttn;
	public InputField grantXp_field;
	public Text xpText, levelText;

	public Button reset_password_bttn;
	public InputField old_password, new_password;
	public Text reset_response;

	public Button parent_email_bttn;
	public InputField parent_email_input;

	// Use this for initialization
	void Start () {

		GS.GameSparksAvailable += ((bool _isAvail) => {
			if (_isAvail) {
				Debug.LogWarning ("GameSparks Connected...");
			}else{
				Debug.LogWarning ("GameSparks Disconnected...");
			}
		});
		blockout_panel.SetActive (true);
		BringPanelForward (menu);

		Application.logMessageReceivedThreaded += HandleLog;
		GSMessageHandler._AllMessages = HandleGameSparksMessageReceived;
		clearLog_bttn.onClick.AddListener (() => {
			myLog = string.Empty;
			logText.text = string.Empty;
		});
		goto_sceneState_bttn.onClick.AddListener (() => {
			Debug.Log("Selected Scene State Options...");
			BringPanelForward(scenes);
		});
		goto_items_bttn.onClick.AddListener (() => {
			Debug.Log("Selected Item Options...");
			BringPanelForward(items);
		});
		goTo_menu_bttn1.onClick.AddListener (() => {
			Debug.Log("Selected Menu Options...");
			BringPanelForward(menu);

		});
		goto_player_bttn.onClick.AddListener (() => {
			Debug.Log("Selected Player Details Options...");
			BringPanelForward(playerDetails);

		});
		goTo_player_inbox.onClick.AddListener (() => {
			Debug.Log("Selected Inbox Options...");
			BringPanelForward(inbox);

		});
		goTo_menu_bttn2.onClick = goTo_menu_bttn1.onClick;

		// all gamesparks calls are linked here //
		auth_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked on authentication button");
			GameSparksManager.Instance().Authenticate(auth_username_txt.text, auth_password_input.text, (_resp) => {
				// here is an example of how we can use the event callbacks. //
				// in this case, i will remove the menu-option blocker if the player has authenticated sucessfully //
				blockout_panel.SetActive(false);
			}, null);
		});

		reg_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked on registration button");
			GameSparksManager.Instance().Register(reg_username_txt.text, reg_displayname.text, reg_password.text, (_resp) => {
				// here is an example of how we can use the event callbacks. //
				// in this case, i will remove the menu-option blocker if the player has authenticated sucessfully //
				blockout_panel.SetActive(false);
			}, null);
		});

		getInv_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked on get-inventory button");
			GameSparksManager.Instance().getInventory((resp) => {
				// do whatever you want with the response here //
				Dropdown[] deleteTest = FindObjectsOfType<Dropdown>();
				Array.Clear(deleteTest, 0, deleteTest.Length);
				GSData scriptData = resp.ScriptData;
				Dropdown clone = (Dropdown)Instantiate(dropdown_prefab, new Vector3(-100, 158, 0), transform.rotation);
				clone.transform.SetParent(items.transform, false);
				List<GSData> list = scriptData.GetGSDataList("resp");
				List<string> opts = new List<string>();
				int i = 0;
				foreach (GSData name in list)
				{
					Debug.Log(i + ": " + name.GetString(i.ToString()));
					//string playerName = level.GetString("playerName");
					if (i == 0)
						opts.Add("(0) Equipped: " + name.GetString(i.ToString()));
					else
						opts.Add(i + ": " + name.GetString(i.ToString()));
					i += 1;
				}
				//clone.transform.position = new Vector3(-371, -15, 0);
				clone.AddOptions(opts);
				clone.Show();
			});


		});

		useItem_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Use-Item button");
			GameSparksManager.Instance().useItem(int.Parse(useItemId.text),  null);
		});

		equipItem_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Equip-Item button");
			GameSparksManager.Instance().equipItem(int.Parse(equipItemId.text),  null);
		});
			
		moveItem_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Move-Item button");
			GameSparksManager.Instance().moveItem(int.Parse(moveInvId.text), int.Parse(moveDestId.text),  null);
		});
			
		pickupItem_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Pickup-Item button");
			GameSparksManager.Instance().pickUpItem(int.Parse(pickupPlacedItemId.text), int.Parse(pickupSceneId.text), null);
		});

		dropItem_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Drop-Item button");
			GameSparksManager.Instance().dropItem(int.Parse(dropInvId.text), int.Parse(dropInvId.text), int.Parse(dropX.text), int.Parse(dropY.text), null);
		});

		getState_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on get scene button");
			GameSparksManager.Instance().dropItem(int.Parse(dropInvId.text), int.Parse(dropInvId.text), int.Parse(dropX.text), int.Parse(dropY.text), null);
		});

		setState_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on set scene button");

			GSRequestData data = new GSRequestData();
			data.AddObject("position" , new GSRequestData().AddString("type", "position").AddNumber("lastx", 20).AddNumber("lasty", 40).AddString("direction", "R"));
			GameSparksManager.Instance().setSceneState(int.Parse(islandIDSet.text), int.Parse(sceneIDSet.text), data,  null);
		});

		grantXp_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked Grant XP Buttn...");
			GameSparksManager.Instance().GiveXp(int.Parse(grantXp_field.text), (resp)=>{
				// in this case we want to re-request the player's XP and level, so we are in sync with the  server //
				GameSparksManager.Instance ().GetLevelAndExperiance ((_level, _xp) => {
					xpText.text = _xp.ToString();
					levelText.text = _level.ToString();
				});
			});

		});

		reset_password_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Reset Password Button...");
			GameSparksManager.Instance().ResetPassword(old_password.text, new_password.text, 
				(newPassword)=>{
					reset_response.text = "Sucess, New Password - "+newPassword;
				},
				(newPasswordNull, oldPasswordIncorrect, failedPassword, strength)=>{
					if(newPasswordNull){
						reset_response.text = "Failed,  New Password Empty ";
					}else if(oldPasswordIncorrect){
						reset_response.text = "Failed,  Old Password Invalid - "+old_password;
					}else{
						reset_response.text = "Failed,  Not Strong Enough - "+old_password;
					}
				});
		});

		parent_email_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Parent Email Button...");
			GameSparksManager.Instance().RegisterParentEmail(parent_email_input.text);
		});

	}



	void HandleGameSparksMessageReceived(GSMessage message)
	{
		HandleLog("MSG:" + message.JSONString);
	}

	public void HandleLog(string logString)
	{
		GS.GSPlatform.ExecuteOnMainThread(() => {
			HandleLog(logString, null, LogType.Log);
		});
	}

	void HandleLog(string logString, string stackTrace, LogType logType)
	{
		if (myLogQueue.Count > 30)
		{
			myLogQueue.Dequeue();
		}
		myLogQueue.Enqueue(logString);
		myLog = "";

		foreach (string logEntry in myLogQueue.ToArray())
		{
			myLog = logEntry + "\n\n" + myLog;
		}
		logText.text = myLog;
	}


	private void BringPanelForward(GameObject _panel){
		Debug.Log ("UIM| Bringing Forward "+_panel.gameObject.name);
		menu.SetActive(false);
		items.SetActive(false);
		scenes.SetActive(false);
		playerDetails.SetActive (false);
		inbox.SetActive (false);
		_panel.SetActive (true);

		if (_panel.gameObject.name == "player_panel") {
			Debug.Log ("UIM| Loading Player Details...");
			GameSparksManager.Instance ().GetLevelAndExperiance ((_level, _xp) => {
				xpText.text = _xp.ToString();
				levelText.text = _level.ToString();
			});
		}

	}
}
