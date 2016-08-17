using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using GameSparks.Api.Responses;
using GameSparks.Core;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class GameSparksManager : MonoBehaviour {


	#region Singleton
	/// <summary>
	/// a refrence to the game-state manager object used for the singleton
	/// </summary>
	private static GameSparksManager instance = null;
	/// <summary>
	/// This method returns the current instance of this class.
	/// The singelton ensures that only one instance of the game-state manager exists.
	/// </summary>
	/// <returns>an instance of the Game-State Manager</returns>
	public static GameSparksManager Instance()
	{
		if(instance != null){	return instance; }
		Debug.LogError("GSM | GameSparks Not Initialized...");
		return instance;
	}
	#endregion

	void Awake()
	{
		
		if(instance == null){ // when the first GSM is activated, it should be null, so we create the reference

			Debug.Log ("GSM | Singleton Initilized...");
			instance = this;
			DontDestroyOnLoad(this.gameObject); // gamesparks manager persists throughout game
		}
		else {
			// if we load into a level that has the gamesparks manager present, it should be destroyed //
			// there can be only one! //
			Debug.Log ("GSM | Removed Duplicate...");
			Destroy(this.gameObject);
		}
	}


	// Use this for initialization
	void Start () {

		// This is a callback which is triggered when gamesparks connects and disconnects //
		// Note: on disconnect, this needs a request to timeout before it will know that the socket has closed //
		// i.e. we cannot tell the SDK the socket is closed if it is closed (since there is no connection) //
		GS.GameSparksAvailable += ((bool _isAvail) => {
			if (_isAvail) {
				Debug.LogWarning ("GameSparks Connected...");
			}else{
				Debug.LogWarning ("GameSparks Disconnected...");
			}
		});


		// THE FOLLOWING ARE EXAMPLE OF HOW WE CAN DETECT DIFFERNT TYPES OF SOCKET-MESSAGES COMING FROM THE SERVER //
		// THESE CAN BE CALLED FROM ANYWHER, THEY DONT NEED TO BE CALLED FROM THE GAMESPARKSMANAGER.cs //

		// ------- SESSION TERMINATED MESSAGE RECIEVED ------- //
		GameSparks.Api.Messages.SessionTerminatedMessage.Listener += (_message) => {
			// there is no other pertenant data returned here //
			// contact GameSparks if you need extra information to be returned from this message //
			Debug.LogWarning("GSM| Player Has Been Logged Out Due To Concurrent Login...");
		};
		// ----- GLOBAL INBOX MESSAGE RECIEVED ------ //
		GameSparks.Api.Messages.ScriptMessage_globalUserMessage.Listener += (_message) => {
			// there are 3 important things returned in this message //
			// [1] - the header. This is the 'title' or  'subject' of the message //
			// [2] - the body, which is the main text of the message (html can be included if you use the rich-text option in the unity GUI system //
			// [3] - the payload. This is JSON/GSData which can be checked to trigger events in the client //
			// [additional] - we can get the character id, and message id from these messages also //
			string header = _message.Data.GetString("header");
			string body = _message.Data.GetString("body");
			GSData payload = _message.Data.GetGSData("payload");
			string characterID = _message.Data.GetString("character_id");
			string messageID = _message.MessageId;
			// The message ID is very important, is it is used to tell which message to dismiss later //
			// therefore, when drawing message details you should cache the message ID for later use //
			Debug.LogWarning("GSM| New Global Message  \n"+_message.JSONString);
			Debug.LogWarning("GSM| Header: "+header);
			Debug.LogWarning("GSM| Body: "+body);
			Debug.LogWarning("GSM| Chacacter ID: "+characterID);
			Debug.LogWarning("GSM| Message ID: "+messageID);
		};
		// ------ PRIVATE MESSAGE RECIEVED ------ //
		GameSparks.Api.Messages.ScriptMessage_privateUserMessage.Listener += (_message) => {
			// there are sever important pieces of information assocaited with private messages //
			// [1] header (see global messages)
			// [2] body (see global messages)
			// [3] payload (see global message)
			// [4] messageId - This is needed in order to dismiss messages
			// [5] senderId - the characterID of the sender
			// [6] sender-name - the sender character's name
			string header = _message.Data.GetString("header");
			string body = _message.Data.GetString("body");
			GSData payload = _message.Data.GetGSData("payload");
			string senderID = _message.Data.GetString("from");
			string senderName = _message.Data.GetString("sender-name");
			string messageID = _message.MessageId;
			Debug.LogWarning("GSM| New Global Message  \n"+_message.JSONString);
			Debug.LogWarning("GSM| Header: "+header);
			Debug.LogWarning("GSM| Body: "+body);
			Debug.LogWarning("GSM| Sender ID: "+senderID);
			Debug.LogWarning("GSM| Sender Name: "+senderName);
			Debug.LogWarning("GSM| Message ID: "+messageID);
		};
	}

	public delegate void onRequestSucess ();
	public delegate void onRequestFailed (string errorString);

	#region AUTHENTICATION CALLS
	/// <summary>
	/// Auth callback.
	/// Can be used just to trigger some code when a request comes through (either sucessful or unsucessful)
	/// </summary>
	public delegate void onAuthFailed(string errorString);
	public delegate void onAuthSucess(string[] characterId_list, string _lastCharacterId);
	/// <summary>
	/// Logs the player into the gamesparks platform
	/// </summary>
	/// <param name="_userName">User name.</param>
	/// <param name="_password">Password.</param>
	public void Authenticate(string userName, string password, onAuthSucess onSucess, onAuthFailed onLoginFailed){
		Debug.Log(userName+"|"+password);
		Debug.Log ("GSM| Attempting Player Authentication....");
		new GameSparks.Api.Requests.AuthenticationRequest ()
			.SetPassword (password)
			.SetUserName (userName)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Authentication Sucessful \n" + response.DisplayName);
				if(onSucess != null){
						string lastCharacterId = string.Empty;
						if(response.ScriptData.GetString("last_character") != null){
							lastCharacterId = response.ScriptData.GetString("last_character");
						}
						onSucess(response.ScriptData.GetStringList("character_list").ToArray(), lastCharacterId);
				}
			} else {
				Debug.LogWarning ("GSM| Error Authenticating Player \n " + response.Errors.JSON);
				if(onLoginFailed != null){
						if(response.Errors.GetString("DETAILS") == "UNRECOGNISED"){
							onLoginFailed("details-unrecognised");
						}
				}
			}
		});

	}


	public delegate void onRegFailed(string suggestedName);
	public delegate void onRegSucess();
	/// <summary>
	/// Register the specified _userName, _displayName and _password.
	/// </summary>
	/// <param name="_userName">User name.</param>
	/// <param name="_displayName">Display name.</param>
	/// <param name="_password">Password.</param>
	public void Register(string userName, string displayName, string password, onRegSucess onRegSucess, onRegFailed onRegFailed){
		Debug.Log ("GSM| Attempting Registration...");
		new GameSparks.Api.Requests.RegistrationRequest ()
			.SetUserName (userName)
			.SetDisplayName (displayName)
			.SetPassword (password)
			.Send ((response) => {
				if(!response.HasErrors){
					Debug.Log ("GSM| Registration Sucessful \n" + response.UserId);
					if(onRegSucess != null){
						onRegSucess();
					}
				}else{
					Debug.LogWarning ("GSM| Error Registering Player \n " + response.Errors.JSON);
					if(onRegFailed != null && response.Errors.GetString("USERNAME") == "TAKEN"){
						onRegFailed(response.ScriptData.GetString("suggested-name"));
					}
				}
		});
	}
	#endregion



	#region INVENTORY CALLS
	/// <summary>
	/// Logevent callback.
	/// </summary>
	public delegate void logevent_callback(LogEventResponse _resp);
	public delegate void onGetInventory(Item[] items);
	public delegate void onGetInventoryFailed(string errorString);
	/// <summary>
	/// Gets the inventory.
	/// </summary>
	public void GetInventory(string character_id, onGetInventory onGetInventory, onGetInventoryFailed onGetInventoryFailed)
	{
		Debug.Log ("GSM| Fetching Inventory Items...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("getInventory")
			.SetEventAttribute("character_id", character_id)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log("GSM| Inventory Found...");
				if(onGetInventory != null){
						List<Item> items = new List<Item>();
						foreach(GSData item in response.ScriptData.GetGSDataList("item_list")){
							items.Add(new Item(
								item.GetInt("item_id").Value,
								item.GetString("name"),
								item.GetString("representation"),
								item.GetString("icon"),
								item.GetString("equipped"),
								item.GetString("is_special")
							));
						}


						onGetInventory(items.ToArray());
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);
					if(onGetInventoryFailed != null){
						if(response.BaseData.GetGSData("error") != null){
							Debug.LogError(response.BaseData.GetGSData("error").GetString("@getInventory"));
							onGetInventoryFailed(response.BaseData.GetGSData("error").GetString("@getInventory"));
						}
					}
			}
		});
	}
		

	/// <summary>
	/// Removes the item from the player inventory
	/// </summary>
	/// <param name="character_id">Character ID</param>
	/// <param name="item_id">Item ID</param>
	/// <param name="onRequestSucess">On request sucess.</param>
	/// <param name="onRequestFailed">On request failed.</param>
	public void RemoveItem(string character_id, int item_id, onRequestSucess onRequestSucess, onRequestFailed onRequestFailed)
	{
		Debug.Log ("Attempting To Remove Item...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("removeItem")
			.SetEventAttribute("character_id", character_id)
			.SetEventAttribute ("item_id", item_id)
			.SetDurable(true)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.LogWarning ("GSM| Item  Removed...");
					if (onRequestSucess != null) {
						onRequestSucess ();
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
					if(response.BaseData.GetGSData("error").GetString("@removeItem") != null){
						Debug.LogError(response.BaseData.GetGSData("error").GetString("@removeItem"));
						onRequestFailed(response.BaseData.GetGSData("error").GetString("@removeItem"));
					}
				
			}
		});
	}


	public void pickUpItem(int _item_id, int _scene_id, logevent_callback _callback)
	{
		Debug.Log ("Attempting To Drop Item...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("pickUpItem")
			.SetEventAttribute ("placedItemId", _item_id)
			.SetEventAttribute ("sceneId", _scene_id)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.LogWarning ("GSM| Item Picked Up...");
				if (_callback != null) {
					_callback (response);
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
			}
		});
	}


	public void moveItem(int _item_id, int _slot_id, logevent_callback _callback)
	{
		Debug.Log ("Attempting To Move Item...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("moveItem")
			.SetEventAttribute ("inventoryItemID", _item_id)
			.SetEventAttribute ("destinationSlot", _slot_id)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.LogWarning ("GSM| Item Moved...");
				if (_callback != null) {
					_callback (response);
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
			}
		});
	}


	public void equipItem(int _item_id, logevent_callback _callback)
	{
		Debug.Log ("Attempting To Equip Item...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("equipItem")
			.SetEventAttribute("inventoryItemID", _item_id)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.LogWarning ("GSM| Item Equipped...");
					if (_callback != null) {
						_callback (response);
					}
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}


	/// <summary>
	/// Uses the item.
	/// </summary>
	/// <param name="_item_id">Item identifier.</param>
	/// <param name="_callback">Callback.</param>
	public void useItem(int _item_id, logevent_callback _callback)
	{
		Debug.Log ("Attempting To Use Item...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("useItem")
			.SetEventAttribute("inventoryItemID", _item_id)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.LogWarning ("GSM| Item Used...");
					if (_callback != null) {
						_callback (response);
					}
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}

	#endregion


	#region SCENE CALLS
	/// <summary>
	/// Gets the state of the scene.
	/// </summary>
	/// <param name="character_id">Character identifier.</param>
	/// <param name="_island_id">Island identifier.</param>
	/// <param name="_scene_id">Scene ID</param>
	/// <param name="_callback">Callback.</param>
	public void GetSceneState(string character_id, int _island_id, int _scene_id, logevent_callback _callback)
	{
		Debug.Log ("Fecthing Scenes ...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getSceneState")
			.SetEventAttribute("character_id", character_id)
			.SetEventAttribute("island_id", _island_id)
			.SetEventAttribute("scene_id", _scene_id)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.LogWarning ("GSM| Scenes Retrieved...");
					if (_callback != null) {
						_callback (response);
					}
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}

	/// <summary>
	/// Sets the state of the scene.
	/// </summary>
	/// <param name="character_id">Character identifier.</param>
	/// <param name="_island_id">Island identifier.</param>
	/// <param name="_scene_id">Scene identifier.</param>
	/// <param name="_scene_data">Scene data.</param>
	/// <param name="_callback">Callback.</param>
	public void SetSceneState(string character_id, int _island_id, int _scene_id, GSRequestData _scene_data, logevent_callback _callback)
	{
		Debug.Log ("Attempting To Set Scene State ...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("setSceneState")
			.SetEventAttribute("character_id", character_id)
			.SetEventAttribute("island_id", _island_id)
			.SetEventAttribute("scene_id", _scene_id)
			.SetEventAttribute("states", _scene_data)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.LogWarning ("GSM| Scene Set...");
					if (_callback != null) {
						_callback (response);
					}
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}
	/// <summary>
	/// Logs the player as having entered a scene
	/// </summary>
	/// <param name="character_id">Character ID</param>
	/// <param name="scene_id">Scene ID</param>
	public void EnterScene(string character_id, int scene_id){
		Debug.Log ("Attempting To Enter Scene ...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("enterScene")
			.SetEventAttribute("character_id", character_id)
			.SetEventAttribute("scene_id", scene_id)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.LogWarning ("GSM| Entered Set...");
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}
	#endregion 


	#region Player Details
	/// <summary>
	/// Level and exp callback.
	/// Having the giveXP and GetXp calls return the same data, it means you can assign one call to this delegate
	/// and it will automatically re-draw the new xp and level values instead of requesting them again.
	/// </summary>
	public delegate void levelAndExp_callback(int _level, int _exp);
	/// <summary>
	/// Adds XP to the player and checks that the player has reached enough XP to gain a level.
	/// 
	/// </summary>
	/// <param name="_callback">character-id</param>
	/// <param name="_amount">Amount.</param>
	/// <param name="_callback">Callback.</param>
	public void GiveXp(string character_id, int _amount, levelAndExp_callback _callback){
		
		Debug.Log ("Attempting To Give Xp...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("giveXp")
			.SetEventAttribute("character_id", character_id)
			.SetEventAttribute("amount", _amount)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.LogWarning ("GSM| XP Granted...");
					// first we get the json where the level and experiance is //
					GSData resp = response.ScriptData.GetGSData("player_details");
					// then we pass the level and xp into the callback //
					_callback(resp.GetInt("level").Value, resp.GetInt("experience").Value);
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}


	/// <summary>
	/// Gets the level and experiance.
	/// </summary>
	/// <param name="_callback">character-id</param>
	/// <param name="_callback">Callback, returns the level and experience</param>
	public void GetLevelAndExperiance(string character_id, levelAndExp_callback _callback){

		Debug.Log ("Retrieving Player Level & Experiance...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getLevelAndExperience")
			.SetEventAttribute("character_id", character_id)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log ("GSM| Returned Level & XP...");
					// first we get the json where the level and experiance is //
					GSData resp = response.ScriptData.GetGSData("player_details");
					// then we pass the level and xp into the callback //
					_callback(resp.GetInt("level").Value, resp.GetInt("experience").Value);
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}



	/// <summary>
	/// Reset password on sucess callback.
	/// </summary>
	public delegate void resetPassword_onSucess_callback(string _newPassword);
	/// <summary>
	/// Reset password on failed callback.
	/// There are several types of error that can be returned.
	/// If the old-password was invalid, if the new-password was invalid.
	/// In the later case, the strength of the password will also be retuned.
	/// This can be removed later.
	/// </summary>
	public delegate void resetPassword_onFailed_callback(bool _invalidPassword, string _failedPassword, string _strength);
	/// <summary>
	/// This function will send a old and new password to the server.
	/// It will validate the old password and check the strength of the new password 
	/// </summary>
	/// <param name="old_password">Old password.</param>
	/// <param name="new_password">New password.</param>
	/// <param name="_onSucess">A callback for when the request is sucessful. Is nullable</param>
	/// <param name="_onFailed">A callback for when the request has failed. Is nullable</param>
	public void ResetPassword(string old_password, string new_password, resetPassword_onSucess_callback _onSucess, resetPassword_onFailed_callback _onFailed){
		if (old_password != string.Empty && new_password != string.Empty) {
			Debug.Log ("Retrieving Player Level & Experiance...");
			new GameSparks.Api.Requests.LogEventRequest().SetEventKey("resetPassword")
				.SetEventAttribute("old_password", old_password)
				.SetEventAttribute("new_password", new_password)
				.Send ((response) => {
					if (!response.HasErrors) {
						Debug.Log ("GSM| Password Changed...");
						if (_onSucess != null) {
							_onSucess (response.ScriptData.GetString("@resetPassword"));
						}
					} else {
						Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
						if (_onFailed != null) {
							GSData errors = response.Errors.GetGSData("@resetPassword");
							// get the strength of the password and the submitted password //
							if(errors.GetString("message") == "old-password-invalid"){ // the old-password didnt match
								_onFailed(true, null, null);
							}else { // failed a strength test
								string failedPassword = errors.GetString("failed-password");
								string strength = errors.GetString("strength");
								_onFailed(false, failedPassword, strength);
							}
						}
					}
				});
		} else {
			Debug.LogWarning ("GSM| old-password or new-password empty...");
		}
	}


	/// <summary>
	/// Registers the parent email.
	/// Does not return anything for the moment.
	/// Contact GameSparks if a response is needed.
	/// </summary>
	/// <param name="parent_email">parent_email, this is validated server-side aslo</param>
	public void RegisterParentEmail(string parent_email){
		Debug.Log ("GSM| Submitting Parent Email...");
		if (IsValidEmail (parent_email)) {
			new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("submitParentEmail")
				.SetEventAttribute ("parent_email", parent_email)
				.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log ("GSM| Parent Email Pending...");
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
		} else {
			Debug.LogWarning ("GSM| Invalid Email: "+parent_email);
		}
	}
	#endregion


	#region System
	/// <summary>
	/// Getserverversion callback.
	/// this is used to make sure that the user has a response from the server before trying to use the version-string.
	/// </summary>
	public delegate void getserverversion_callback(string _version);
	/// <summary>
	/// Requests the current version from ther server.
	/// </summary>
	/// <param name="_callback">Callback, returns version string</param>
	public void GetServerVersion(getserverversion_callback _callback){
		Debug.Log ("GSM| Fetching Server Version...");

		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getServerVersion")
			.Send ((response) => {
				if (!response.HasErrors) {
					string version = response.ScriptData.GetString("version");
					if(version != null){
						Debug.Log ("GSM| Server: "+version);
						_callback(version);
					}else{
						Debug.LogError("GSM| Server Error! [server version returned 'null']");
					}
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}
	#endregion


	#region Inbox System
	/// <summary>
	/// This delegate is used as a callback so that the list of messages can be returned instead of the whole response.
	/// The GSData can be parsed to get the specific message data.
	/// There is an example included in the UIManager.cs class
	/// </summary>
	public delegate void getMessages_callback(GSData[] _messages);
	/// <summary>
	/// Gets all messages from the given character_id
	/// 
	/// </summary>
	/// <param name="character_id">Character identifier.</param>
	/// <param name="type">global, private or both</param>
	/// <param name="offset">Offset.</param>
	/// <param name="limit">Limit.</param>
	/// <param name="_callback">Callback.</param>
	public void GetMessages(string character_id, string type, int offset, int limit, getMessages_callback _callback){
		Debug.Log ("GSM| Fetching Messages For Character - "+character_id);
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getMessages")
			.SetEventAttribute("character_id", character_id)
			.SetEventAttribute("type", type)
			.SetEventAttribute("limit", limit)
			.SetEventAttribute("offset", offset)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log("GSM| Retrieved Messages....");
					if(_callback != null){
						_callback(response.ScriptData.GetGSDataList("messages").ToArray());
					}

				} else {
					Debug.LogError("GSM|  Error Fetching Messages \n"+response.Errors.JSON);
				}
			});
	}
	/// <summary>
	/// Given the character_id and recipient_id, this will send a message to that character.
	/// If includes a header (subject) and body.
	/// </summary>
	/// <param name="header">Header.</param>
	/// <param name="body">Body.</param>
	/// <param name="characterTo">Character to.</param>
	/// <param name="characterFrom">Character from.</param>
	public void SendPrivateMessage(string header, string body, string characterTo, string characterFrom){
		Debug.Log ("GSM| Sending Private Message To "+characterTo);
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("sendPrivateMessage")
			.SetEventAttribute("header", header)
			.SetEventAttribute("body", body)
			.SetEventAttribute("payload", new GSRequestData())
			.SetEventAttribute("character_id_to", characterTo)
			.SetEventAttribute("character_id_from", characterFrom)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log("GSM| Message Sent....");
				} else {
					Debug.LogError("GSM| Message Not Sent");
				}
			});
	}
	/// <summary>
	/// Given the character_id and recipient_id, this will send a message to that character.
	/// If includes a header (subject) and body.
	/// In additional there is a field (optional) for payload, which allows specific JSON data to be sent if needed.
	/// </summary>
	/// <param name="header">Header.</param>
	/// <param name="body">Body.</param>
	/// <param name="payload">Payload.</param>
	/// <param name="characterTo">Character to.</param>
	/// <param name="characterFrom">Character from.</param>
	public void SendPrivateMessage(string header, string body, GSRequestData payload, string characterTo, string characterFrom){
		Debug.Log ("GSM| Sending Private Message To "+characterTo);
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("sendPrivateMessage")
			.SetEventAttribute("header", header)
			.SetEventAttribute("body", body)
			.SetEventAttribute("payload", payload)
			.SetEventAttribute("character_id_to", characterTo)
			.SetEventAttribute("character_id_from", characterFrom)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log("GSM| Message Sent....");
				} else {
					Debug.LogError("GSM| Message Not Sent");
				}
			});
	}

	/// <summary>
	/// This will delete the message with the given id from the player's feed
	/// </summary>
	/// <param name="messageID">Message ID</param>
	public void DeleteMessage(string messageID){
		Debug.Log ("GSM| Deleting Message "+messageID);
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("deleteMessage")
			.SetEventAttribute("message_id", messageID)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log("GSM| Message Deleted....");
				} else {
					Debug.LogError("GSM| Message Not Deleted");
				}
			});
	}

	public void ReadMessage(string messageID){
		Debug.Log ("GSM| Deleting Message "+messageID);
//		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("deleteMessage")
//			.SetEventAttribute("message_id", messageID)
//			.Send ((response) => {
//				if (!response.HasErrors) {
//					Debug.Log("GSM| Message Deleted....");
//				} else {
//					Debug.LogError("GSM| Message Not Deleted");
//				}
//			});
	}


	#endregion

	#region Get Available Islands
	/// <summary>
	/// Gets the available islands.
	/// </summary>
	/// <param name="character_id">Character identifier.</param>
	/// <param name="_callback">Callback.</param>
	public void GetAvailableIslands(string character_id, logevent_callback _callback){
		Debug.Log ("GSM| Fetching Available Islands...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getAvailableIslands")
			.SetEventAttribute("character_id", character_id)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log("GSM| Found Islands....");
					if(_callback != null){
						_callback(response);
					}
				} else {
					Debug.LogError("GSM| Error Fetching Islands...");
				}
			});
	}

	/// <summary>
	/// Visits the island.
	/// </summary>
	/// <param name="island_id">Island identifier.</param>
	/// <param name="character_id">Character identifier.</param>
	/// <param name="_onVisitSucess">On visit sucess.</param>
	/// <param name="_onVisitFailed">On visit failed.</param>
	public void VisitIsland(string character_id, int island_id, logevent_callback _onVisitSucess, logevent_callback _onVisitFailed){
		Debug.Log ("GSM|  Visiting Island...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("visitIsland")
			.SetEventAttribute("character_id", character_id)
			.SetEventAttribute("island_id", island_id)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log("GSM| Character Visited Island....");
					if(_onVisitSucess != null){
						_onVisitSucess(response);
					}
				} else {
					Debug.LogError("GSM| Error Visiting Island...");
					if(_onVisitFailed != null){
						_onVisitFailed(response);
					}
				}
			});
	}
	/// <summary>
	/// Marks a character as having left an island.
	/// </summary>
	/// <param name="island_id">Island identifier.</param>
	/// <param name="character_id">Character identifier.</param>
	public void LeaveIsland(int island_id, string character_id){
		Debug.Log ("GSM|  Leaving Island...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("leaveIsland")
			.SetEventAttribute("character_id", character_id)
			.SetEventAttribute("island_id", island_id)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log("GSM| Character Left Island....");
				} else {
					Debug.LogError("GSM| Error Leaving Island...");
				}
			});
	}
	/// <summary>
	/// marks a character as having completed an island.
	/// </summary>
	/// <param name="island_id">Island identifier.</param>
	/// <param name="character_id">Character identifier.</param>
	public void CompleteIsland(string character_id, int island_id){
		Debug.Log ("GSM|  Completing Island...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("completeIsland")
			.SetEventAttribute("character_id", character_id)
			.SetEventAttribute("island_id", island_id)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log("GSM| Character Completed Island....");
				} else {
					Debug.LogError("GSM| Error Completing Island...");
				}
			});
	}
	#endregion



	#region Character
	/// <summary>
	/// Gets the character information for the ID given.
	/// </summary>
	/// <param name="character_id">Character identifier.</param>
	public void GetCharacter(string character_id){
		Debug.Log ("GMS| Fetching Info For Character: "+character_id);
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getCharacter")
			.SetEventAttribute("character_id", character_id)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log("GSM| Found Character....");
				} else {
					Debug.LogError("GSM| Error Fetching Character...");
				}
			});
	}
	/// <summary>
	/// Gets the characters information in the list submitted
	/// </summary>
	/// <param name="character_ids">Character identifiers.</param>
	public void GetCharacters(List<string> character_ids){
		Debug.Log ("GMS| Fetching Info For Characters" );
		GSRequestData id_list = new GSRequestData ().AddStringList("list", character_ids);
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getCharacter")
			.SetEventAttribute("character_id", id_list)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log("GSM| Found Characters....");
				} else {
					Debug.LogError("GSM| Error Fetching Characters...");
				}
			});
	}

	/// <summary>
	/// Delegate used to return the ID of the new character.
	/// </summary>
	public delegate void createCharacter_callback(string _newCharacterID);
	/// <summary>
	/// Creates a new character and returns the new character ID
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="gender">M, F, or MF</param>
	/// <param name="_callback">Callback.</param>
	public void CreateCharacter(string name, string gender, createCharacter_callback _callback){
		Debug.Log ("GMS| Creating New Character: "+name);
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("createCharacter")
			.SetEventAttribute("name", name)
			.SetEventAttribute("gender", gender)
			.Send ((response) => {
				if (!response.HasErrors) {
					if(_callback != null){
						_callback(response.ScriptData.GetString("new-character-id"));
					}
					Debug.Log("GSM| Character Created....");
				} else {
					Debug.LogError("GSM| Error Creating Character...");
				}
			});
	}
	#endregion


	///////////////////////////////////////
	// ********* MISC FUNCTIONS ******** //
	///////////////////////////////////////

	/// <summary>
	/// USes a regex to check the email is valid before sending it to the server.
	/// </summary>
	/// <param name="_callback">email</param>
	private bool IsValidEmail(string _email)
	{
		string expresion;
		expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
		if (Regex.IsMatch(_email, expresion))
		{
			if (Regex.Replace(_email, expresion, string.Empty).Length == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}


}
