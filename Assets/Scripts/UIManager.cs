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

	public Button goto_sceneState_bttn, goto_characters_bttn, goto_islands_bttn, goto_chars_bttn, goto_items_bttn, goto_player_bttn, goTo_player_inbox, goTo_menu_bttn1, goTo_menu_bttn2, goTo_menu_bttn3, goTo_menu_bttn4, goTo_menu_bttn5, goTo_menu_bttn6;

	public Button clearLog_bttn;
	public Text auth_username_txt;
	public InputField auth_password_input;
	public Button auth_bttn;
	public Text reg_username_txt, reg_password, reg_displayname, server_version_txt;
	public Button reg_bttn;

	public Button getScene_bttn, setScene_bttn, enterScene_bttn;
	public InputField scene_island_id, scene_scene_id, set_scene_id, set_island_id, set_x, set_y, set_type, set_direction;

	public GameObject blockout_panel;



	public Button reset_password_bttn;
	public InputField old_password, new_password;

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

	public Button add_item_bttn, remove_item_bttn, equip_item_bttn, use_item_bttn, get_inv_bttn;
	public InputField pickup_item_id, pickup_scene_id, equip_location, equip_item_id, use_item_id;

	public Button grantXp_bttn;
	public InputField grantXp_field;
	public Text xpText, levelText;


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


		goTo_menu_bttn2.onClick = goTo_menu_bttn3.onClick = goTo_menu_bttn4.onClick = goTo_menu_bttn5.onClick = goTo_menu_bttn6.onClick =  goTo_menu_bttn1.onClick;

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
				}, (_errorString)=>{
					if(_errorString == "db-error"){

					}
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
				}, (_errorString)=>{
					if(_errorString == "db-error"){

						}
				});
			}, (_suggestedName)=>{
				Debug.LogWarning("UIM| Suggested Username: "+_suggestedName);
			});
		});

		get_inv_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked on get-inventory button");
			GameSparksManager.Instance().GetInventory(character_id, (_items) => {
				foreach(Item item in _items){
					item.Print();
				}
			}, (_errorString)=>{
				if(_errorString == "no-inventory"){
					// the player has no inventory
				}
			});
		});

		use_item_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Use-Item button");
			GameSparksManager.Instance().UseItem(character_id, int.Parse(use_item_id.text), (_item_id)=>{
				Debug.Log("Item ID: "+_item_id);
				// and request the inventory again to make sure it was picked up //
				GameSparksManager.Instance().GetInventory(character_id , null,  null);
			}, (_errorString)=>{

			});
		});

		equip_item_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Equip-Item button");
			GameSparksManager.Instance().EquipItem(character_id, int.Parse(equip_item_id.text), equip_location.text, (_item_id)=>{
				Debug.Log("Item ID: "+_item_id);
				// and request the inventory again to make sure it was picked up //
				GameSparksManager.Instance().GetInventory(character_id , null,  null);
			}, (_errorString)=>{
				if(_errorString == "invalid-item-id"){

				}else if(_errorString == "invalid-character-id"){

				}else if(_errorString == "min-level-not-met"){

				}else if(_errorString == "max-level-exceeded"){

				}
			});
		});
			
		add_item_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Pickup-Item button");
			GameSparksManager.Instance().PickUpItem(character_id, int.Parse(pickup_item_id.text), int.Parse(pickup_scene_id.text), (_itemID)=>{
				Debug.Log("UIM| Picked Up Item:"+_itemID);
				// and request the inventory again to make sure it was picked up //
				GameSparksManager.Instance().GetInventory(character_id , null,  null);
			}, (_errorString)=>{
				if(_errorString == "item-not-found-in-scene"){

				}else if(_errorString == "invalid-scene-id"){

				}
			});
		});

		remove_item_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Drop-Item button");
			GameSparksManager.Instance().RemoveItem(character_id, int.Parse(pickup_item_id.text), (_itemID)=>{
				Debug.Log("UIM| Picked Up Item:"+_itemID);
				// and request the inventory again to make sure it was picked up //
				GameSparksManager.Instance().GetInventory(character_id , null,  null);
			}, (_errorString)=>{
				if(_errorString == "item-not-found-in-scene"){

				}else if(_errorString == "invalid-scene-id"){

				}
			});
		});

		getScene_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on get scene button");
			GameSparksManager.Instance().GetSceneState(character_id, int.Parse(scene_island_id.text), int.Parse(scene_scene_id.text), (_sceneState)=>{
				// callback will have the scene-state which was returned //
				// you can use it from here //
				_sceneState.Print();
			}, (_errorString)=>{
				if(_errorString == "no-player-scene-record"){
					// when the player does not yet have any scene information
				}else if (_errorString == "invalid-scene-id"){
					// if the scene-id given was invalid, or the player had no information for that specific scene
				}
			});
		});

		enterScene_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Enter Scene Button");
			GameSparksManager.Instance().EnterScene(character_id, int.Parse(scene_island_id.text), (island_id, scene_id)=>{
				// on entered scene sucessfully //
				Debug.Log("Scene ID: "+scene_id+", Island ID: "+island_id);
			}, (_errorString)=>{
				if(_errorString == "invalid-scene-id"){
				
				}else if(_errorString == "\"dberror\""){
					// if the request failed to update the collection
				}
			});
		});

	

		setScene_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on set scene button");
			SceneState newScene = new SceneState(set_type.text, set_direction.text, int.Parse(set_x.text), int.Parse(set_y.text));
			GameSparksManager.Instance().SetSceneState(character_id, int.Parse(set_island_id.text), int.Parse(set_scene_id.text), newScene,  null, null);
		});

		grantXp_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked Grant XP Buttn...");
			GameSparksManager.Instance().GiveExperience(character_id, int.Parse(grantXp_field.text), (_level, _xp) =>{
				Debug.Log("XP:"+_xp+", Level:"+_level);
				xpText.text = _xp.ToString();
				levelText.text = _level.ToString();
			}, (_errorString)=>{
				if(_errorString == "no-level-definition"){	

				}
			});
		});

		reset_password_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Reset Password Button...");
			GameSparksManager.Instance().ResetPassword(old_password.text, new_password.text, (_newPassword) =>{
				// password set //
			}, (_errorString)=>{
				
			});
		});

		parent_email_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Parent Email Button...");
			GameSparksManager.Instance().RegisterParentEmail(parent_email_input.text, ()=>{
				// parent email pending validation	
			}, (_errorString)=>{
				if(_errorString == "invalid-email"){

				}
			});
		});


		send_private_message_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Send Message Button...");
			// this is an example of a private message sent without a payload //
			GameSparksManager.Instance().SendPrivateMessage(message_header.text, message_body.text, message_recipient.text, character_id, ()=>{
				// on message sent
			}, (_errorString)=>{
				if(_errorString == "invalid-character-id"){

				}
			});
			// below is an example of how to send private messages with payload data //
			// the payload data can be anything the client wants to interperate as an action or trigger (or anything really) //
			// for example if we want a quest to start for the recipient when they recieve the data... //
			// we are going to send the JSON { "quest-trigger" : "message-recieved" }
			GSRequestData payloadData = new GSRequestData();
			payloadData.AddString("quest-trigger", "message-recieved");
			// we can then send it with the overloaded method.... //
			GameSparksManager.Instance().SendPrivateMessage(message_header.text, message_body.text, payloadData, message_recipient.text, character_id, null, null);

		});

		get_messages_btt.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Get Messages Button...");
			// if you want all messages, then the type entered is 'both'
			GameSparksManager.Instance().GetMessages(character_id, message_type.text, int.Parse(message_offset.text), int.Parse(message_limit.text), 
				(_messages) => {
					foreach(InboxMessage message  in _messages){
						message.Print();
					}
				}, (_errorString)=>{
					
				});
		});


		delete_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Delete Message Button...");
			GameSparksManager.Instance().DeleteMessage(message_id.text, ()=>{
				// message was deleted
			}, (_errorString)=>{
				if(_errorString == "not-deleted"){

				}
			});
		});

		read_message_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Read Message Button...");
			GameSparksManager.Instance().ReadMessage(message_id.text);
		});


		getIslands_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Get Available Islands button");
			GameSparksManager.Instance().GetAvailableIslands(character_id, (_islands) => {
				foreach(Island island in _islands){
					island.Print();
				}
			}, (_errorString)=>{
				
			});
		});


		visit_island_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Visit Island Button...");
			GameSparksManager.Instance().VisitIsland(character_id, int.Parse(player_island_id.text),  (_islandID) => {
				Debug.Log("Island ID:"+_islandID);

			}, (_errorString) =>{ // if there was an error, there will be "error":{"@visitIsland":"invalid-island-id"}
				if(_errorString == "invalid-island-id"){

				}
			});
		});

		leave_island_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Leave Island Button...");
			GameSparksManager.Instance().LeaveIsland( character_id, int.Parse(player_island_id.text), ()=>{
				// visit to island sucessful //
			}, (_errorString)=>{
				if(_errorString == "invalid-island-id"){
					
				}else if(_errorString == "dberror"){

				}
			});
		});

		complete_island_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Complete Island Button...");
			GameSparksManager.Instance().CompleteIsland(character_id, int.Parse(player_island_id.text), ()=>{
				// player completed island
			}, (_errorString)=>{
				if(_errorString == "invalid-island-id"){

				}else if(_errorString == "dberror"){

				}
			});
		});

		get_char_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Get Character Button...");
			character_id = get_char_input.text;
			GameSparksManager.Instance().GetCharacter(get_char_input.text, (_character)=>{
				_character.Print();
			},(_errorString)=>{
				if(_errorString == "invalid-char-id"){

				}
			});
		});

		create_char_bttn.onClick.AddListener (() => {
			Debug.Log("UIM| Clicked On Create Character Button...");

			GameSparksManager.Instance().CreateCharacter(char_name_input.text, char_gender_input.text, (_newCharacterID) => {
				Debug.Log("UIM| Character ID:"+_newCharacterID);
				character_id = _newCharacterID;
			}, (_errorString)=>{
				if(_errorString == "name-already-taken"){

				}else if(_errorString == "invalid-gender"){

				}
			});
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

		if (_panel.gameObject.name == "character_panel") {
			Debug.Log ("UIM| Loading Player Details...");
			GameSparksManager.Instance ().GetLevelAndExperiance (character_id, (_level, _xp) => {
				xpText.text = _xp.ToString();
				levelText.text = _level.ToString();
				Debug.Log("XP:"+_xp+", Level:"+_level);
			}, (_errorString)=>{
				if(_errorString == "invalid-character-id"){

				}
			});
		}

	}
}





