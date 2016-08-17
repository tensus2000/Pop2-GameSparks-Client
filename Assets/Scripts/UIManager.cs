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

	public string character_id = "57b30ba318ead60489e130c8";

	public Text logText;
	private Queue<string> myLogQueue = new Queue<string>();
	private string myLog;

	public GameObject items, scenes, menu, playerDetails, inbox, islands, characters;

	public Button goto_sceneState_bttn, goto_islands_bttn, goto_chars_bttn,  goto_items_bttn, goto_player_bttn, goTo_player_inbox, goTo_menu_bttn1, goTo_menu_bttn2, goTo_menu_bttn3, goTo_menu_bttn4, goTo_menu_bttn5, goTo_menu_bttn6;

	public Button clearLog_bttn;
	public Text auth_username_txt;
	public InputField auth_password_input;
	public Button auth_bttn;
	public Text reg_username_txt, reg_password, reg_displayname, server_version_txt;
	public Button reg_bttn;

	public Button getInv_bttn, useItem_bttn, equipItem_bttn, moveItem_bttn, pickupItem_bttn, dropItem_bttn;

	public Dropdown dropdown_prefab;

	public InputField dropInvId, dropSceneId, dropX, dropY;
	public InputField pickupPlacedItemId, pickupSceneId;
	public InputField moveInvId, moveDestId;
	public InputField equipItemId;
	public InputField useItemId;


	public Button getScene_bttn, setScene_bttn, enterScene_bttn;
	public InputField scene_island_id, scene_scene_id, set_scene_id, set_island_id, set_x, set_y, set_type, set_direction;

	public GameObject blockout_panel;

	public Button grantXp_bttn;
	public InputField grantXp_field;
	public Text xpText, levelText;

	public Button reset_password_bttn;
	public InputField old_password, new_password;
	public Text reset_response;

	public Button parent_email_bttn;
	public InputField parent_email_input;

	public Button send_private_message_bttn;
	public InputField message_header, message_body, message_recipient;

	public Button get_messages_btt;
	public InputField message_type, message_offset, message_limit;


	public Button delete_bttn, read_message_bttn;
	public InputField message_id;

	public Button getIslands_bttn, visit_island_bttn, leave_island_bttn, complete_island_bttn;
	public InputField player_island_id;

	public Button get_char_bttn;
	public InputField get_char_input;

	public Button create_char_bttn;
	public InputField char_name_input, char_gender_input;


	// Use this for initialization
	void Start () {


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

		goto_islands_bttn.onClick.AddListener (() => {
			Debug.Log("Selected Island Options...");
			BringPanelForward(islands);

		});

		goto_chars_bttn.onClick.AddListener (() => {
			Debug.Log("Selected Island Options...");
			BringPanelForward(characters);

		});


		goTo_menu_bttn2.onClick = goTo_menu_bttn3.onClick = goTo_menu_bttn4.onClick = goTo_menu_bttn1.onClick;

		// all gamesparks calls are linked here //
		auth_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked on authentication button");
			GameSparksManager.Instance().Authenticate(auth_username_txt.text, auth_password_input.text, (_characterIds, _lastCharacter) => {
				// here is an example of how we can use the event callbacks. //
				// in this case, i will remove the menu-option blocker if the player has authenticated sucessfully //

				// the following is an example of how to get the last-character and character list back //
				string[] characterIds = _characterIds;
				string lastCharacter = _lastCharacter;

				Debug.Log("UIM| Character List: "+characterIds.Length);
				Debug.Log("UIM| Last Character: "+lastCharacter);

				server_version_txt.text = "Server Version: Requesting....";
				GameSparksManager.Instance().GetServerVersion((_version) => {
					server_version_txt.text = "Server Version: "+_version;
					blockout_panel.SetActive(false);
				});
			}, (_errorString) =>{
				// error-string can be checked for the following errors //
				if(_errorString == "details-unrecognised"){
					// for when the user-name or password is incorrect //
				}

			});
		});

		reg_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked on registration button");
			GameSparksManager.Instance().Register(reg_username_txt.text, reg_displayname.text, reg_password.text, () => {
				// here is an example of how we can use the event callbacks. //
				// in this case, i will remove the menu-option blocker if the player has authenticated sucessfully //
				server_version_txt.text = "Server Version: Requesting....";
				GameSparksManager.Instance().GetServerVersion((_version) => {
					server_version_txt.text = "Server Version: "+_version;
					blockout_panel.SetActive(false);
				});
			}, (_suggestedName)=>{
				Debug.LogWarning("UIM| Suggested Username: "+_suggestedName);
			});
		});

		getInv_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked on get-inventory button");
			GameSparksManager.Instance().GetInventory(character_id, (_items) => {
				foreach(Item item in _items){
					item.Print();
				}
//				// do whatever you want with the response here //
//				Dropdown[] deleteTest = FindObjectsOfType<Dropdown>();
//				Array.Clear(deleteTest, 0, deleteTest.Length);
//				GSData scriptData = resp.ScriptData;
//				Dropdown clone = (Dropdown)Instantiate(dropdown_prefab, new Vector3(-100, 158, 0), transform.rotation);
//				clone.transform.SetParent(items.transform, false);
//				List<GSData> list = scriptData.GetGSDataList("resp");
//				List<string> opts = new List<string>();
//				int i = 0;
//				foreach (GSData name in list)
//				{
//					Debug.Log(i + ": " + name.GetString(i.ToString()));
//					if (i == 0)
//						opts.Add("(0) Equipped: " + name.GetString(i.ToString()));
//					else
//						opts.Add(i + ": " + name.GetString(i.ToString()));
//					i += 1;
//				}
//				clone.AddOptions(opts);
//				clone.Show();
			}, (_errorString)=>{
				if(_errorString == "no-inventory"){
					// the player has no inventory
				}
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
			GameSparksManager.Instance().RemoveItem(character_id, int.Parse(dropInvId.text), null, null);
		});

		getScene_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on get scene button");
			GameSparksManager.Instance().GetSceneState(character_id, int.Parse(scene_island_id.text), int.Parse(scene_scene_id.text), null);
		});

		enterScene_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Enter Scene Button");
			GameSparksManager.Instance().EnterScene(character_id, int.Parse(scene_island_id.text));
		});

	

		setScene_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on set scene button");

			GSRequestData data = new GSRequestData();
			data.AddString("type", set_type.text);
			data.AddString("direction", set_direction.text);
			data.AddNumber("lastx", int.Parse(set_x.text));
			data.AddNumber("lasty", int.Parse(set_y.text));
			GameSparksManager.Instance().SetSceneState(character_id, int.Parse(set_island_id.text), int.Parse(set_scene_id.text), data,  null);
		});

		grantXp_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked Grant XP Buttn...");
			GameSparksManager.Instance().GiveXp(character_id, int.Parse(grantXp_field.text), (_level, _xp) =>{
				
				xpText.text = _xp.ToString();
				if(levelText != null){
					levelText.text = _level.ToString();
				}
			});

		});

		reset_password_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Reset Password Button...");
			GameSparksManager.Instance().ResetPassword(old_password.text, new_password.text, 
				(newPassword)=>{
					reset_response.text = "Sucess, New Password - "+newPassword;
				},
				(oldPasswordIncorrect, failedPassword, strength)=>{
					if(oldPasswordIncorrect){
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


		send_private_message_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Send Message Button...");
			// this is an example of a private message sent without a payload //
			GameSparksManager.Instance().SendPrivateMessage(message_header.text, message_body.text, message_recipient.text, character_id);
			// below is an example of how to send private messages with payload data //
			// the payload data can be anything the client wants to interperate as an action or trigger (or anything really) //
			// for example if we want a quest to start for the recipient when they recieve the data... //
			// we are going to send the JSON { "quest-trigger" : "message-recieved" }
			GSRequestData payloadData = new GSRequestData();
			payloadData.AddString("quest-trigger", "message-recieved");
			// we can then send it with the overloaded method.... //
			GameSparksManager.Instance().SendPrivateMessage(message_header.text, message_body.text, payloadData, message_recipient.text, character_id);

		});

		get_messages_btt.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Get Messages Button...");
			// if you want all messages, then the type entered is 'both'
			GameSparksManager.Instance().GetMessages(character_id, message_type.text, int.Parse(message_offset.text), int.Parse(message_limit.text), 
				(_messages) => {
					List<InboxMessage> inboxMessages = new List<InboxMessage>();
					for(var i = 0; i < _messages.Length; i++){
						inboxMessages.Add(
							new InboxMessage(_messages[i].GetString("messageId"),
								_messages[i].GetGSData("data").GetString("sender-name"),
								_messages[i].GetGSData("data").GetString("from"),
								_messages[i].GetGSData("data").GetString("header"),
								_messages[i].GetGSData("data").GetString("body")
							));
					}
					foreach(InboxMessage message  in inboxMessages){
						message.PrintMessageData();
					}
				});
		});


		delete_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Delete Message Button...");
			GameSparksManager.Instance().DeleteMessage(message_id.text);
		});

		read_message_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Read Message Button...");
			GameSparksManager.Instance().ReadMessage(message_id.text);
		});


		getIslands_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Get Available Islands button");
			GameSparksManager.Instance().GetAvailableIslands(character_id, (_resp) => {
				// at the moment this will return the entire response //
				// if there is to be a 'island' class cached at some point we can alter this to return an Island[] instead //
				List<GSData> islandList = _resp.ScriptData.GetGSDataList("islands");
				foreach(GSData islandData in islandList){
					Debug.LogWarning("Island OID: "+islandData.GetGSData("_id").GetString("$oid"));
					Debug.LogWarning("Island ID: "+islandData.GetInt("island_id").Value);
					Debug.LogWarning("Name: "+islandData.GetString("name"));
					Debug.LogWarning("Desc: "+islandData.GetString("description"));
					Debug.LogWarning("Gates: "+islandData.GetGSDataList("gates").Count);
					Debug.LogWarning("URLs: "+islandData.GetStringList("urls").Count);
				}
			});
		});


		visit_island_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Visit Island Button...");
			GameSparksManager.Instance().VisitIsland(character_id, int.Parse(player_island_id.text),  (_resp) => {

				Debug.Log("UIM| Island URLs: "+_resp.ScriptData.GetGSData("island_info").GetStringList("urls").Count);
				Debug.Log("UIM| Scene ID: "+_resp.ScriptData.GetGSData("island_info").GetString("scene_id"));

			}, (_resp) =>{ // if there was an error, there will be "error":{"@visitIsland":"invalid-island-id"}
				if(_resp.Errors != null){
					Debug.LogError("UIM| "+_resp.Errors.GetString("@visitIsland"));

				}
			});
		});

		leave_island_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Leave Island Button...");
			GameSparksManager.Instance().LeaveIsland(int.Parse(player_island_id.text), character_id);
		});

		complete_island_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Complete Island Button...");
			GameSparksManager.Instance().CompleteIsland(character_id, int.Parse(player_island_id.text));
		});

		get_char_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Get Character Button...");
			character_id = get_char_input.text;
			GameSparksManager.Instance().GetCharacter(get_char_input.text);
		});

		create_char_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Create Character Button...");

			GameSparksManager.Instance().CreateCharacter(char_name_input.text, char_gender_input.text, (_newCharacterID) => {
				Debug.Log("UIM| Character ID:"+_newCharacterID);
				character_id = _newCharacterID;
			});
		});


	}

	class InboxMessage {
		public InboxMessage(string _messageId, string _senderName, string _senderID, string _header, string _body){
			this.messageId = _messageId;
			this.senderName = _senderName;
			this.senderID = _senderID;
			this.header = _header;
			this.body = _body;
		}
		string messageId, senderName, senderID, header, body;


		public void PrintMessageData(){
			Debug.Log("Message ID: "+messageId);
			Debug.Log("Header: "+header);
			Debug.Log("Body: "+body);
			Debug.Log("Sender ID: "+senderID);
			Debug.Log("Sender Name: "+senderName);
		}
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

	/// <summary>
	/// manages the gamesparks log messages for printing to the screen
	/// </summary>
	/// <param name="logString">Log string.</param>
	/// <param name="stackTrace">Stack trace.</param>
	/// <param name="logType">Log type.</param>
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

	/// <summary>
	/// Brings the request options panel forward.
	/// </summary>
	/// <param name="_panel">Panel.</param>
	private void BringPanelForward(GameObject _panel){
		Debug.Log ("UIM| Bringing Forward "+_panel.gameObject.name);
		menu.SetActive(false);
		items.SetActive(false);
		scenes.SetActive(false);
		playerDetails.SetActive (false);
		inbox.SetActive (false);
		islands.SetActive (false);
		characters.SetActive (false);
		_panel.SetActive (true);

		if (_panel.gameObject.name == "player_panel") {
			Debug.Log ("UIM| Loading Player Details...");
			GameSparksManager.Instance ().GetLevelAndExperiance (character_id, (_level, _xp) => {
				xpText.text = _xp.ToString();
				levelText.text = _level.ToString();
			});
		}

	}
}




/// sample class structures //


public class Item{
	public Item(int item_id, string name, string icon, string equipped, string isSpecial, string respresentaion){
		this.item_id = item_id;
		this.name = name;
		this.icon = icon;
		this.equipped = equipped;
		this.isSpecial = isSpecial;
		this.representation = representation;
	}

	public void Print(){
		Debug.Log ("Item ID: "+item_id);
	}

	int item_id;
	string name, icon, representation, isSpecial, equipped;

}
