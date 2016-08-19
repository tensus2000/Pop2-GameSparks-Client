using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using GameSparks.Api.Responses;
using GameSparks.Core;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using GameSparks.Api.Messages;
/// <summary>
/// GameSparks Manager Class
/// To setup this class, add it to an empty gameobject and also attach the class GameSparksUnity.cs
/// July 2016, Sean Durkan
/// </summary>
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
		if (instance != null) {
			return instance;
		}
		Debug.LogError ("GSM | GameSparks Not Initialized...");
		return instance;
	}
	#endregion

	void Awake()
	{
		if (instance == null) { // when the first GSM is activated, it should be null, so we create the reference
			Debug.Log ("GSM | Singleton Initilized...");
			instance = this;
			DontDestroyOnLoad (this.gameObject); // gamesparks manager persists throughout game
		} else {
			// if we load into a level that has the gamesparks manager present, it should be destroyed //
			// there can be only one! //
			Debug.Log ("GSM | Removed Duplicate...");
			Destroy (this.gameObject);
		}
	}



	// Use this for initialization
	void Start () {

		// This is a callback which is triggered when gamesparks connects and disconnects //
		// Note: on disconnect, this needs a request to timeout before it will know that the socket has closed //
		// i.e. we cannot tell the SDK the socket is closed if it is closed (since there is no connection) //
		GS.GameSparksAvailable += ((bool _isAvail) => {
			OnGSAvailable (_isAvail);
		});
		// These callbacks will send the message-recieved events back to the class they are being called from
		GameSparks.Api.Messages.ScriptMessage_privateUserMessage.Listener += (message) => {
			OnNewPrivateMessage (new InboxMessage (message.MessageId, message.Data.GetString ("sender-name"), message.Data.GetString ("from"), message.Data.GetString ("header"), message.Data.GetString ("body"), message.Data.GetGSData ("payload")));
		};
		GameSparks.Api.Messages.ScriptMessage_globalUserMessage.Listener += (message) => {
			OnNewGlobalMessage (new InboxMessage (message.MessageId, "Poptropica", "Poptropica", message.Data.GetString ("header"), message.Data.GetString ("body"), message.Data.GetGSData ("payload")));
		};
		GameSparks.Api.Messages.SessionTerminatedMessage.Listener += (message) => {
			OnSessionTerminated ();
		};
	}

	#region Callbacks For Socket-Messages
	public event GSAvailable OnGSAvailable;
	public delegate void GSAvailable(bool _isAvailable);
	public event MessageDelegate OnNewPrivateMessage, OnNewGlobalMessage;
	public delegate void MessageDelegate(InboxMessage message);
	public delegate void MessageEvent();
	public event MessageEvent OnSessionTerminated;
	#endregion
	/// <summary>
	/// This delegate allows a callback to be used in any request where you want an action to take place
	/// once the request is completed, but there is no specific information you need from the response.
	/// </summary>
	public delegate void onRequestSucess ();
	/// <summary>
	/// This is a generic delegate used for callbacks where a request failed, and there are specific error messages
	/// to be returned from the server.
	/// </summary>
	public delegate void onRequestFailed (string errorString);
	#region AUTHENTICATION CALLS
	/// <summary>
	/// Upon sucessful authentication we return all the player's characters, as well as the ID or the last character they used.
	/// </summary>
	public delegate void onAuthSucess(string[] characterIDs, string lastCharacterID);
	/// <summary>
	/// Logs the player into Pop2
	/// </summary>
	/// <param name="userName">User name.</param>
	/// <param name="password">Password.</param>
	/// <param name="onSucess">Returns array or character ID and the ID of the last character the player used</param>
	/// <param name="onRequestFailed">If user-name or password are wrong "details-unrecognised"</param>
	public void Authenticate(string userName, string password, onAuthSucess onSucess, onRequestFailed onRequestFailed)
	{
		Debug.Log ("UserName:" + userName + ", Password:" + password);
		Debug.Log ("GSM| Attempting Player Authentication....");
		new GameSparks.Api.Requests.AuthenticationRequest ()
			.SetPassword (password)
			.SetUserName (userName)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Authentication Sucessful \n" + response.DisplayName);
				if (onSucess != null) {
					string lastCharacterId = string.Empty;
					// check that we have a last-character saved //
					if (response.ScriptData.GetString ("last_character") != null) {
						lastCharacterId = response.ScriptData.GetString ("last_character");
					}
					onSucess (response.ScriptData.GetStringList ("character_list").ToArray (), lastCharacterId);
				}
			} else {
				Debug.LogWarning ("GSM| Error Authenticating Player \n " + response.Errors.JSON);
				if (onRequestFailed != null) {
					if (response.Errors.GetString ("DETAILS") == "UNRECOGNISED") {
						onRequestFailed ("details-unrecognised");
					}
				}
			}
		});
	}

	/// <summary>
	/// Used for registration requests, returns a suggested username if the username was taken.
	/// </summary>
	public delegate void onRegFailed(string suggestedName);
	/// <summary>
	/// Register the specified userName, displayName and password.
	/// </summary>
	/// <param name="_userName">User name.</param>
	/// <param name="_displayName">Display name.</param>
	/// <param name="_password">Password.</param>
	public void Register(string userName, string displayName, string password, onRequestSucess onRequestSucess, onRegFailed onRegFailed)
	{
		Debug.Log ("UserName:" + userName + ", Password:" + password + ", Display Name:" + displayName);
		Debug.Log ("GSM| Attempting Registration...");
		new GameSparks.Api.Requests.RegistrationRequest ()
			.SetUserName (userName)
			.SetDisplayName (displayName)
			.SetPassword (password)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Registration Sucessful \n" + response.UserId);
				if (onRequestSucess != null) {
					onRequestSucess ();
				}
			} else {
				Debug.LogWarning ("GSM| Error Registering Player \n " + response.Errors.JSON);
				// we need to check that these parameters are not null before sending the callback
				if (onRegFailed != null && response.Errors.GetString ("USERNAME") != null && response.Errors.GetString ("USERNAME") == "TAKEN") {
					onRegFailed (response.ScriptData.GetString ("suggested-name"));
				}
			}
		});
	}
	#endregion



	#region INVENTORY CALLS
	/// <summary>
	/// The item id of the item added to the character's inventory
	/// </summary>
	public delegate void onItemRemoved(int item_id);
	/// <summary>
	/// Removes the item from the player inventory
	/// </summary>
	/// <param name="character_id">Character ID</param>
	/// <param name="item_id">Item ID</param>
	/// <param name="onRequestSucess">On request sucess.</param>
	/// <param name="onRequestFailed">
	/// If the item-id is invalid - "invalid-item-id"</param>
	public void RemoveItem(string character_id, int item_id, onItemRemoved onItemRemoved, onRequestFailed onRequestFailed)
	{
		Debug.Log ("Attempting To Remove Item...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("removeItem")
			.SetEventAttribute ("character_id", character_id)
			.SetEventAttribute ("item_id", item_id)
			.SetDurable (true)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.LogWarning ("GSM| Item  Removed...");
				if (onItemRemoved != null) {
					onItemRemoved ((int)response.ScriptData.GetNumber ("item_id").Value);
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				// check if the error-string is there before sending it to the callback
				if (onRequestFailed != null && response.BaseData.GetGSData ("error").GetString ("@removeItem") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@removeItem"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@removeItem"));
				}
			}
		});
	}
	/// <summary>
	/// The item id of the item added to the character's inventory
	/// </summary>
	public delegate void onItemPickedUp(int item_id);
	/// <summary>
	/// Adds an item to the inventory of the requested character
	/// </summary>
	/// <param name="character_id">Character identifier.</param>
	/// <param name="item_id">Item ID</param>
	/// <param name="scene_id">Scene ID.</param>
	/// <param name="onItemPickedUp">On item picked up.</param>
	/// <param name="onRequestFailed">if a duplicate is added- "player-has-item",
	/// If the scene ID is invalid -  "invalid-scene-id",
	/// If the item-id is invalid - "invalid-item-id"</param>
  	public void PickUpItem(string character_id, int item_id, int scene_id, onItemPickedUp onItemPickedUp, onRequestFailed onRequestFailed)
	{
		Debug.Log ("Attempting To Pickup Item...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("pickUpItem")
			.SetEventAttribute ("character_id", character_id)
			.SetEventAttribute ("item_id", item_id)
			.SetEventAttribute ("scene_id", scene_id)
			.SetDurable (true)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.LogWarning ("GSM| Item Picked Up...");
				if (onItemPickedUp != null) {
					onItemPickedUp ((int)response.ScriptData.GetNumber ("item_id").Value);
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				// check if the error-string is there before sending it to the callback
				if (onRequestFailed != null && response.BaseData.GetGSData ("error").GetString ("@pickUpItem") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@pickUpItem"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@pickUpItem"));
				}
			}
		});
	}

	// THIS HAS BEEN DEPRECIATED //
//	public void moveItem(int _item_id, int _slot_id, logevent_callback _callback)
//	{
//		Debug.Log ("Attempting To Move Item...");
//		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("moveItem")
//			.SetEventAttribute ("inventoryItemID", _item_id)
//			.SetEventAttribute ("destinationSlot", _slot_id)
//			.Send ((response) => {
//			if (!response.HasErrors) {
//				Debug.LogWarning ("GSM| Item Moved...");
//				if (_callback != null) {
//					_callback (response);
//				}
//			} else {
//				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
//			}
//		});
//	}

	/// <summary>
	/// The item id of the item equipped in the character's inventory
	/// </summary>
	public delegate void onItemEquipped(int item_id);
	/// <summary>
	/// Equips the item.
	/// </summary>
	/// <param name="character_id">Character ID</param>
	/// <param name="_item_id">Item ID</param>
	/// <param name="onItemEquipped">returns the item-ID equipped</param>
	/// <param name="onRequestFailed">,
	/// If the item-id is invalid - "invalid-item-id"</param>
	public void EquipItem(string character_id, int item_id, string equip_location, onItemEquipped onItemEquipped, onRequestFailed onRequestFailed)
	{
		Debug.Log ("Attempting To Equip Item...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("equipItem")
			.SetEventAttribute ("character_id", character_id)
			.SetEventAttribute ("item_id", item_id)
			.SetEventAttribute ("equip_location", equip_location)
			.SetDurable (true)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.LogWarning ("GSM| Item Equipped...");
				if (onItemEquipped != null && response.ScriptData.GetInt ("item_id") != null) {
					onItemEquipped (response.ScriptData.GetInt ("item_id").Value);
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				// check if the error-string is there before sending it to the callback
				if (onRequestFailed != null && response.Errors.GetString ("@equipItem") != null) {
					onRequestFailed (response.Errors.GetString ("@equipItem"));
				}
			}
		});
	}

	/// <summary>
	/// The item id of the item added to the character's inventory
	/// </summary>
	public delegate void onItemUsed(int item_id);
	/// <summary>
	/// Uses the item.
	/// </summary>
	/// <param name="character_id">Character ID</param>
	/// <param name="item_id">Item ID</param>
	/// <param name="onItemUsed">Returns the item ID</param>
	/// <param name="onRequestFailed">"no-inventory"</param>
	public void UseItem(string character_id, int item_id, onItemUsed onItemUsed, onRequestFailed onRequestFailed)
	{
		Debug.Log ("Attempting To Use Item...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("useItem")
			.SetEventAttribute ("character_id", character_id)
			.SetEventAttribute ("item_id", item_id)
			.SetDurable (true)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.LogWarning ("GSM| Item Used...");
				if (onItemUsed != null && response.ScriptData.GetInt ("item_id") != null) {
					onItemUsed (response.ScriptData.GetInt ("item_id").Value);
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				// check if the error-string is there before sending it to the callback
				if (onRequestFailed != null && response.Errors.GetString ("@useItem") != null) {
					onRequestFailed (response.Errors.GetString ("@useItem"));
				}
			}
		});
	}

	/// <summary>
	/// returns an array of items
	/// </summary>
	public delegate void onGetInventory(Item[] items);
	/// <summary>
	/// On get inventory failed.
	/// </summary>
	public delegate void onGetInventoryFailed(string errorString);
	/// <summary>
	/// Gets the inventory.
	/// </summary>
	/// <param name="character_id">Character ID</param>
	/// <param name="onGetInventory">callback for getting the list of items returned</param>
	/// <param name="onGetInventoryFailed">If there is no inventory record - "no-inventory"</param>
	public void GetInventory(string character_id, onGetInventory onGetInventory, onGetInventoryFailed onGetInventoryFailed)
	{
		Debug.Log ("GSM| Fetching Inventory Items...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("getInventory")
			.SetEventAttribute ("character_id", character_id)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Inventory Found...");
				if (onGetInventory != null) {
					List<Item> items = new List<Item> ();
					// go through all the items in teh response and cache them to be returned by the callback //
					foreach (GSData item in response.ScriptData.GetGSDataList("item_list")) {
						items.Add (new Item (item.GetInt ("item_id").Value, item.GetString ("name"), item.GetString ("representation"), item.GetString ("icon"), item.GetString ("equipped"), item.GetString ("is_special")));
					}
					onGetInventory (items.ToArray ());
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);
				if (onGetInventoryFailed != null) {
					// check if the error-string is there before sending it to the callback
					if (response.BaseData.GetGSData ("error") != null) {
						Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@getInventory"));
						onGetInventoryFailed (response.BaseData.GetGSData ("error").GetString ("@getInventory"));
					}
				}
			}
		});
	}
	#endregion


	#region SCENES STATE API CALLS

	/// <summary>
	/// returns the scene-state
	/// </summary>
	public delegate void onSceneStateFound(SceneState sceneState);
	/// <summary>
	/// Gets the state of the scene.
	/// </summary>
	/// <param name="character_id">Character ID</param>
	/// <param name="island_id">Island ID</param>
	/// <param name="scene_id">Scene ID</param>
	/// <param name="onSceneStateFound">returns the scene-state</param>
	/// <param name="onRequestFailed">If the player has no record - "no-player-scene-record"
	/// If the scene ID is incorrect - "invalid-scene-id"</param>
	public void GetSceneState(string character_id, int island_id, int scene_id, onSceneStateFound onSceneStateFound, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GSM| Fetching Scenes ...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("getSceneState")
			.SetEventAttribute ("character_id", character_id)
			.SetEventAttribute ("island_id", island_id)
			.SetEventAttribute ("scene_id", scene_id)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.LogWarning ("GSM| Scenes Retrieved...");
				if (onSceneStateFound != null && response.ScriptData.GetGSData ("state") != null) {
					onSceneStateFound (new SceneState (response.ScriptData.GetGSData ("state").GetString ("type"),response.ScriptData.GetGSData ("state").GetString ("direction"),response.ScriptData.GetGSData ("state").GetInt ("lastx").Value,response.ScriptData.GetGSData ("state").GetInt ("lasty").Value));
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				// we'll make sure the callback is not null, and that we have an error in the response//
				if (onRequestFailed != null && response.Errors.GetString ("@getSceneState") != null) {
					onRequestFailed (response.Errors.GetString ("@getSceneState"));
				}
			}
		});
	}

	/// <summary>
	/// Sets the state of the scene.
	/// </summary>
	/// <param name="character_id">Character ID</param>
	/// <param name="island_id">Island ID</param>
	/// <param name="scene_id">Scene ID</param>
	/// <param name="newScene">New scene.</param>
	/// <param name="onRequestSucess">On request sucess.</param>
	/// <param name="onRequestFailed">On request failed.</param>
	public void SetSceneState(string character_id, int island_id, int scene_id, SceneState newScene, onRequestSucess onRequestSucess, onRequestFailed onRequestFailed)
	{
		Debug.Log ("Attempting To Set Scene State ...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("setSceneState")
			.SetEventAttribute ("character_id", character_id)
			.SetEventAttribute ("island_id", island_id)
			.SetEventAttribute ("scene_id", scene_id)
			.SetEventAttribute ("state", newScene.ToGSData ())
			.SetDurable (true)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.LogWarning ("GSM| Scene Set...");
				if (onRequestSucess != null) {
					onRequestSucess ();
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				if (onRequestFailed != null && response.Errors.GetString ("@setSceneState") != null) {
					onRequestFailed (response.Errors.GetString ("@setSceneState"));
				}
			}
		});
	}


	public delegate void onEnterScene(int island_id, int scene_id);
	/// <summary>
	///  Registers that a character has entered the scene
	/// </summary>
	/// <param name="character_id">Character ID.</param>
	/// <param name="scene_id">Scene ID</param>
	/// <param name="onEnterScene">On enter scene.</param>
	/// <param name="onRequestFailed">if the update was unsucessful - "dberror", 
	/// if the scene ID was invalid - "invalid-scene-id"</param>
	public void EnterScene(string character_id, int scene_id, onEnterScene onEnterScene, onRequestFailed onRequestFailed)
	{
		Debug.Log ("Attempting To Enter Scene ...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("enterScene")
			.SetEventAttribute ("character_id", character_id)
			.SetEventAttribute ("scene_id", scene_id)
			.SetDurable (true)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.LogWarning ("GSM| Entered Scene...");
				if (onEnterScene != null) {
					onEnterScene ((int)response.ScriptData.GetNumber ("island_id").Value, (int)response.ScriptData.GetNumber ("scene_id").Value);
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				if (onRequestFailed != null && response.Errors.GetString ("@enterScene") != null) {
					onRequestFailed (response.Errors.GetString ("@enterScene"));
				}
			}
		});
	}
	#endregion 


	#region Character API calls
	/// <summary>
	/// Adds XP to the player and checks that the player has reached enough XP to gain a level.
	/// </summary>
	/// <param name="_callback">character-id</param>
	/// <param name="_amount">Amount.</param>
	/// <param name="_callback">Callback.</param>
	public void GiveExperience(string character_id, int _amount, onLevelAndExperiance _callback, onRequestFailed onRequestFailed)
	{
		Debug.Log ("Attempting To Give Xp...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("giveXp")
			.SetEventAttribute ("character_id", character_id)
			.SetEventAttribute ("amount", _amount)
			.SetDurable (true)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.LogWarning ("GSM| XP Granted...");
				// first we get the json where the level and experiance is //
				GSData resp = response.ScriptData.GetGSData ("player_details");
				// then we pass the level and xp into the callback //
				_callback (resp.GetInt ("level").Value, resp.GetInt ("experience").Value);
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
			}
		});
	}

	/// <summary>
	/// Level and exp callback.
	/// Having the giveXP and GetXp calls return the same data, it means you can assign one call to this delegate
	/// and it will automatically re-draw the new xp and level values instead of requesting them again.
	/// </summary>
	public delegate void onLevelAndExperiance(int level, int xp);
	/// <summary>
	/// Gets the level and experiance.
	/// </summary>
	/// <param name="_callback">character-id</param>
	/// <param name="_callback">Callback, returns the level and experience</param>
	public void GetLevelAndExperiance(string character_id, onLevelAndExperiance _callback, onRequestFailed onRequestFailed)
	{
		Debug.Log ("Retrieving Player Level & Experiance...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("getLevelAndExperience")
			.SetEventAttribute ("character_id", character_id)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Returned Level & XP...");
				// first we get the json where the level and experiance is //
				GSData resp = response.ScriptData.GetGSData ("player_details");
				// then we pass the level and xp into the callback //
				_callback (resp.GetInt ("level").Value, resp.GetInt ("experience").Value);
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				if (onRequestFailed != null && response.Errors.GetString ("@getLevelAndExperience") != null) {
					onRequestFailed (response.Errors.GetString ("@getLevelAndExperience"));
				}
			}
		});
	}
	#endregion

	#region Player API calls
	/// <summary>
	/// Reset password on sucess callback.
	/// </summary>
	public delegate void onResetPasswordSucess(string _newPassword);
	/// <summary>
	/// This function will send a old and new password to the server.
	/// It will validate the old password and check the strength of the new password 
	/// </summary>
	/// <param name="old_password">Old password.</param>
	/// <param name="new_password">New password.</param>
	/// <param name="_onSucess">A callback for when the request is sucessful. Is nullable</param>
	/// <param name="_onFailed">A callback for when the request has failed. Is nullable</param>
	public void ResetPassword(string oldPassword, string newPassword, onResetPasswordSucess onSucess, onRequestFailed onRequestFailed)
	{
		if (oldPassword != string.Empty && newPassword != string.Empty) {
			Debug.Log ("Retrieving Player Level & Experiance...");
			new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("resetPassword")
				.SetEventAttribute ("old_password", oldPassword)
				.SetEventAttribute ("new_password", newPassword)
				.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log ("GSM| Password Changed...");
					if (onSucess != null) {
						onSucess (response.ScriptData.GetString ("@resetPassword"));
					}
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
					if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
						Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@resetPassword"));
						onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@resetPassword"));
					}
				}
			});
		} else {
			Debug.LogWarning ("GSM| old-password or new-password empty...");
		}
	}
	/// <summary>
	/// Registers the parent email.
	/// </summary>
	/// <param name="parentEmail">Parent email.</param>
	/// <param name="onRequestSucess">On request sucess.</param>
	/// <param name="onRequestFailed">"invalid-email"</param>
	public void RegisterParentEmail(string parentEmail, onRequestSucess onRequestSucess, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GSM| Submitting Parent Email...");
		if (IsValidEmail (parentEmail)) {
			new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("submitParentEmail")
				.SetEventAttribute ("parent_email", parentEmail)
				.SetDurable (true)
				.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log ("GSM| Parent Email Pending...");
					if (onRequestSucess != null) {
						onRequestSucess ();
					}
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
					if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
						Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@submitParentEmail"));
						onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@submitParentEmail"));
					}
				}
			});
		} else {
			Debug.LogWarning ("GSM| Invalid Email: " + parentEmail);
		}
	}
	#endregion


	#region System
	/// <summary>
	/// Getserverversion callback.
	/// this is used to make sure that the user has a response from the server before trying to use the version-string.
	/// </summary>
	public delegate void onGetServerVersion(string version);
	/// <summary>
	/// Requests the current version from ther server.
	/// </summary>
	/// <param name="_callback">Callback, returns version string</param>
	public void GetServerVersion(onGetServerVersion onGetServerVersion, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GSM| Fetching Server Version...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("getServerVersion")
			.Send ((response) => {
			if (!response.HasErrors) {
				string version = response.ScriptData.GetString ("version");
				if (version != null) {
					Debug.Log ("GSM| Server: " + version);
					onGetServerVersion (version);
				} else {
					Debug.LogWarning ("GSM| Server Error! [server version returned 'null']");
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				if (onRequestFailed != null && response.Errors.GetString ("@getServerVersion") != null) {
					onRequestFailed (response.Errors.GetString ("@getServerVersion"));
				}
			}
		});
	}
	#endregion


	#region Inbox System
	/// <summary>
	/// returns am array of inbox messages from the callback
	/// </summary>
	public delegate void onGetMessages(InboxMessage[] messages);
	/// <summary>
	/// Gets un-dismissed messages of a single character
	/// </summary>
	/// <param name="character_id">Character identifier.</param>
	/// <param name="type">Type.</param>
	/// <param name="offset">Offset.</param>
	/// <param name="limit">Limit.</param>
	/// <param name="_callback">Callback.</param>
	/// <param name="onRequestFailed">On request failed.</param>
	public void GetMessages(string character_id, string type, int offset, int limit, onGetMessages onGetMessages, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GSM| Fetching Messages For Character - " + character_id);
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("getMessages")
			.SetEventAttribute ("character_id", character_id)
			.SetEventAttribute ("type", type)
			.SetEventAttribute ("limit", limit)
			.SetEventAttribute ("offset", offset)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Retrieved Messages....");
				if (onGetMessages != null) {
					List<InboxMessage> messageList = new List<InboxMessage> ();
					foreach (GSData message  in response.ScriptData.GetGSDataList("messages")) {
						messageList.Add (new InboxMessage (message.GetString ("messageId"), message.GetGSData ("data").GetString ("sender-name"), message.GetGSData ("data").GetString ("from"), message.GetGSData ("data").GetString ("header"), message.GetGSData ("data").GetString ("body"), message.GetGSData ("data").GetGSData ("payload")));
					}
					onGetMessages (messageList.ToArray ());
				}

			} else {
				Debug.LogWarning ("GSM|  Error Fetching Messages \n" + response.Errors.JSON);
				if (onRequestFailed != null && response.Errors.GetString ("@getMessages") != null) {
					onRequestFailed (response.Errors.GetString ("@getMessages"));
				}
			}
		});
	}
	/// <summary>
	/// Sends the private message.
	/// </summary>
	/// <param name="header">Header.</param>
	/// <param name="body">Body.</param>
	/// <param name="characterTo">Character ID to send the message to.</param>
	/// <param name="characterFrom">Your character ID</param>
	/// <param name="onRequestSucess">On request sucess.</param>
	/// <param name="onRequestFailed">On request failed, error-string</param>
	public void SendPrivateMessage(string header, string body, string characterTo, string characterFrom, onRequestSucess onRequestSucess, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GSM| Sending Private Message To " + characterTo);
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("sendPrivateMessage")
			.SetEventAttribute ("header", header)
			.SetEventAttribute ("body", body)
			.SetEventAttribute ("payload", new GSRequestData ())
			.SetEventAttribute ("character_id_to", characterTo)
			.SetEventAttribute ("character_id_from", characterFrom)
			.SetDurable (true)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Message Sent....");
				if (onRequestSucess != null) {
					onRequestSucess ();
				}
			} else {
				Debug.LogWarning ("GSM| Message Not Sent...");
				if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@sendPrivateMessage"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@sendPrivateMessage"));
				}
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
	/// <param name="characterTo">Character ID to send to</param>
	/// <param name="characterFrom">Character ID from.</param>
	public void SendPrivateMessage(string header, string body, GSRequestData payload, string characterTo, string characterFrom, onRequestSucess onRequestSucess, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GSM| Sending Private Message To " + characterTo);
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("sendPrivateMessage")
			.SetEventAttribute ("header", header)
			.SetEventAttribute ("body", body)
			.SetEventAttribute ("payload", payload)
			.SetEventAttribute ("character_id_to", characterTo)
			.SetEventAttribute ("character_id_from", characterFrom)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Message Sent....");
				if (onRequestSucess != null) {
					onRequestSucess ();
				}
			} else {
				Debug.LogWarning ("GSM| Message Not Sent");
				if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@sendPrivateMessage"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@sendPrivateMessage"));
				}
			}
		});
	}
	/// <summary>
	/// Deletes a message given the message ID
	/// </summary>
	/// <param name="messageID">Message ID.</param>
	/// <param name="onRequestSucess">On request sucess.</param>
	/// <param name="onRequestFailed">On request failed.</param>
	public void DeleteMessage(string messageID, onRequestSucess onRequestSucess, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GSM| Deleting Message " + messageID);
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("deleteMessage")
			.SetEventAttribute ("message_id", messageID)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Message Deleted....");
				if (onRequestSucess != null) {
					onRequestSucess ();
				}
			} else {
				Debug.LogWarning ("GSM| Message Not Deleted");
				if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@deleteMessage"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@deleteMessage"));
				}
			}
		});
	}

	public void ReadMessage(string messageID){

		// THIS IS YET TO BE IMPLEMENTED //


//		Debug.Log ("GSM| Deleting Message "+messageID);
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
	/// returns an array of islands
	/// </summary>
	public delegate void getIslands(Island[] islands);
	/// <summary>
	/// Gets the available islands.
	/// </summary>
	/// <param name="character_id">Character identifier.</param>
	/// <param name="_callback">Callback.</param>
	public void GetAvailableIslands(string character_id, getIslands getIslands, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GSM| Fetching Available Islands...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("getAvailableIslands")
			.SetEventAttribute ("character_id", character_id)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Found Islands....");
				if (getIslands != null && response.ScriptData.GetGSDataList ("islands") != null) {
					List<Island> islandList = new List<Island> ();
					// add this data to the islands
					foreach (GSData island in response.ScriptData.GetGSDataList("islands")) {
						List<Island.Gate> gateList = new List<Island.Gate> ();
						// get all the gates //
						foreach (GSData gate in island.GetGSDataList("gates")) {
							gateList.Add (new Island.Gate (gate.GetString ("gate_type"), gate.GetString ("start_date"), gate.GetString ("end_date"), 
								(gate.GetInt ("min_level") != null ? gate.GetInt ("min_level").Value : 100),
								(gate.GetInt ("min_level") != null ? gate.GetInt ("max_level").Value : -1),
								gate.GetString ("product_id")));
						}
						islandList.Add (new Island (island.GetInt ("island_id").Value, island.GetString ("name"), island.GetString ("description"), gateList.ToArray (), island.GetStringList ("urls").ToArray ()));
					}
					getIslands (islandList.ToArray ());
				}
			} else {
				Debug.LogWarning ("GSM| Error Fetching Islands...");
				if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@getAvailableIslands"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@getAvailableIslands"));
				}
			}
		});
	}

	/// <summary>
	/// returns the id of the island visited
	/// </summary>
	public delegate void onIslandVisited(int island_id);
	/// <summary>
	/// Visits the island.
	/// </summary>
	/// <param name="island_id">Island ID</param>
	/// <param name="character_id">Character ID</param>
	/// <param name="_onVisitSucess">On visit sucess.</param>
	/// <param name="_onVisitFailed">On visit failed.</param>
	public void VisitIsland(string character_id, int island_id, onIslandVisited onIslandVisited, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GSM|  Visiting Island...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("visitIsland")
			.SetEventAttribute ("character_id", character_id)
			.SetEventAttribute ("island_id", island_id)
			.SetDurable (true)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Character Visited Island....");
				if (onIslandVisited != null) {
					onIslandVisited (response.ScriptData.GetInt ("island_id").Value);
				}
			} else {
				Debug.LogWarning ("GSM| Error Visiting Island...");
				if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@visitIsland"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@visitIsland"));
				}
			}
		});
	}
	/// <summary>
	/// Marks a character as having left an island.
	/// </summary>
	/// <param name="island_id">Island ID</param>
	/// <param name="character_id">Character ID</param>
	public void LeaveIsland(string character_id, int island_id, onRequestSucess onRequestSucess, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GSM|  Leaving Island...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("leaveIsland")
			.SetEventAttribute ("character_id", character_id)
			.SetEventAttribute ("island_id", island_id)
			.SetDurable (true)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Character Left Island....");
				if (onRequestSucess != null) {
					onRequestSucess ();
				}
			} else {
				Debug.LogWarning ("GSM| Error Leaving Island...");
				if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@leaveIsland"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@leaveIsland"));
				}
			}
		});
	}
	/// <summary>
	/// Marks a character as having completed an island.
	/// </summary>
	/// <param name="island_id">Island ID</param>
	/// <param name="character_id">Character ID</param>
	public void CompleteIsland(string character_id, int island_id, onRequestSucess onRequestSucess, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GSM|  Completing Island...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("completeIsland")
			.SetEventAttribute ("character_id", character_id)
			.SetEventAttribute ("island_id", island_id)
			.SetDurable (true)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Character Completed Island....");
				if (onRequestSucess != null) {
					onRequestSucess ();
				}
			} else {
				Debug.LogWarning ("GSM| Error Completing Island...");
				if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@completeIsland"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@completeIsland"));
				}
			}
		});
	}
	#endregion



	#region Character API Calls
	/// <summary>
	/// returns the character ID
	/// </summary>
	public delegate void onGetCharacter(Character character);
	/// <summary>
	/// Gets the character.
	/// </summary>
	/// <param name="character_id">Character ID</param>
	/// <param name="onGetCharacter">On get character.</param>
	/// <param name="onRequestFailed">"invalid-char-id"</param>
	public void GetCharacter(string character_id, onGetCharacter onGetCharacter, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GMS| Fetching Info For Character: " + character_id);
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("getCharacter")
			.SetEventAttribute ("character_id", character_id)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Found Character....");
				onGetCharacter (new Character (response.ScriptData.GetGSData ("character").GetGSData ("_id").GetString ("$oid"), response.ScriptData.GetGSData ("character").GetInt ("level").Value, (int)response.ScriptData.GetGSData ("character").GetNumber ("experience").Value, response.ScriptData.GetGSData ("character").GetString ("name"), response.ScriptData.GetGSData ("character").GetString ("gender")));
			} else {
				Debug.LogWarning ("GSM| Error Fetching Character...");
				if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@getCharacter"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@getCharacter"));
				}
			}
		});
	}
	/// <summary>
	/// returns a list of characterIDs
	/// </summary>
	public delegate void onGetCharacters(Character[] characters);
	/// <summary>
	/// Gets the characters.
	/// </summary>
	/// <param name="character_ids">Character ID List</param>
	/// <param name="onGetCharacters">returns an array of characters</param>
	/// <param name="onRequestFailed">On request failed.</param>
	public void GetCharacters(List<string> character_ids, onGetCharacters onGetCharacters, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GMS| Fetching Info For Characters");
		GSRequestData id_list = new GSRequestData ().AddStringList ("list", character_ids);
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("getCharacter")
			.SetEventAttribute ("character_id", id_list)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Found Characters....");
				List<Character> charList = new List<Character> ();
				foreach (GSData character in response.ScriptData.GetGSDataList("character")) {
					charList.Add (new Character (character.GetGSData ("_id").GetString ("$oid"), character.GetInt ("level").Value, character.GetInt ("experience").Value, character.GetString ("name"), character.GetString ("gender")));
				}
				onGetCharacters (charList.ToArray ());
			} else {
				Debug.LogWarning ("GSM| Error Fetching Characters...");
				if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@getCharacter"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@getCharacter"));
				}
			}
		});
	}

	/// <summary>
	/// Delegate used to return the ID of the new character.
	/// </summary>
	public delegate void onCharacterCreated(string newCharacterID);
	/// <summary>
	/// Creates a new character and returns the new character ID
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="gender">M, F, or MF</param>
	/// <param name="_callback">Callback.</param>
	public void CreateCharacter(string name, string gender, onCharacterCreated onCharacterCreated, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GMS| Creating New Character: " + name);
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("createCharacter")
			.SetEventAttribute ("name", name)
			.SetEventAttribute ("gender", gender)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Character Created....");
				if (onCharacterCreated != null) {
					onCharacterCreated (response.ScriptData.GetString ("new-character-id"));
				}
			} else {
				Debug.LogWarning ("GSM| Error Creating Character...");
				if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@createCharacter"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@createCharacter"));
				}
			}
		});
	}
	#endregion


	#region Character Outfit
	/// <summary>
	/// Sets the outfit in the character's outfit collection
	/// </summary>
	/// <param name="character_id">Character ID</param>
	/// <param name="outfit">Outfit.</param>
	/// <param name="onRequestSucess">On request sucess.</param>
	/// <param name="onRequestFailed">On request failed.</param>
	public void SetOutfit(string character_id, Outfit outfit, onRequestSucess onRequestSucess, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GMS| Setting Outfit...");
		if (outfit != null) {
			GSRequestData outfitData = new GSRequestData ();
			outfitData.AddString ("FixedCostumeApplied", "false");
			outfitData.AddString ("SkinColor", outfit.skin_color);
			outfitData.AddString ("HairColor", outfit.hair_color);
			outfitData.AddString ("Shirt", outfit.shirt);
			outfitData.AddString ("Pants", outfit.pants);
			outfitData.AddString ("Hair", outfit.hair);
			outfitData.AddString ("FaceMark", outfit.face_mark);
			outfitData.AddString ("Helmet", outfit.helmet);
			new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("setOutfit")
				.SetEventAttribute ("character_id", character_id)
				.SetEventAttribute ("outfit", outfitData)
				.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log ("GSM| Outfit Set....");
					if (onRequestSucess != null) {
						onRequestSucess ();
					}
				} else {
					Debug.LogWarning ("GSM| Error Creating Character...");
					if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
						Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@setOutfit"));
						onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@setOutfit"));
					}
				}
			});

		} else {
			Debug.LogWarning ("GSM| No outfit submitted...");
		}
	}
	/// <summary>
	/// On get outfit.
	/// </summary>
	public delegate void onGetOutfit(Outfit outfit);
	/// <summary>
	/// Gets the outfit.
	/// </summary>
	/// <param name="character_id">Character identifier.</param>
	/// <param name="onGetOutfit">On get outfit.</param>
	/// <param name="onRequestFailed">On request failed.</param>
	public void GetOutfit(string character_id, onGetOutfit onGetOutfit, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GMS| Fetching Outfit...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("getOutfit")
				.SetEventAttribute ("character_id", character_id)
				.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Outfit Retrieved....");
				if (onGetOutfit != null && response.ScriptData.GetGSData ("outfit") != null) {
					onGetOutfit (new Outfit (response.ScriptData.GetGSData ("outfit").GetInt ("outfit_id").Value, response.ScriptData.GetGSData ("outfit").GetString ("SkinColor"), response.ScriptData.GetGSData ("outfit").GetString ("HairColor"), response.ScriptData.GetGSData ("outfit").GetString ("Shirt"), response.ScriptData.GetGSData ("outfit").GetString ("Pants"), response.ScriptData.GetGSData ("outfit").GetString ("Hair"), response.ScriptData.GetGSData ("outfit").GetString ("FaceMark"), response.ScriptData.GetGSData ("outfit").GetString ("Helmet")));
				}
			} else {
				Debug.LogWarning ("GSM| Error Creating Character...");
				if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@getOutfit"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@getOutfit"));
				}
			}
		});
	}
	/// <summary>
	/// returns the adornment
	/// </summary>
	public delegate void onGetAdornment(Adornment adornment);
	/// <summary>
	/// Gets the adornment details for the ID requested
	/// </summary>
	/// <param name="adornment_id">Adornment ID</param>
	/// <param name="onGetAdornment">returns the adornment</param>
	/// <param name="onRequestFailed">On request failed.</param>
	public void GetAdornment(int adornment_id, onGetAdornment onGetAdornment, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GMS| Fetching Adornment...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("getAdornment")
			.SetEventAttribute ("adornment_id", adornment_id)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Adornment Retrieved....");
				if (onGetAdornment != null && response.ScriptData.GetGSData ("adornment") != null) {
					List<Adornment.Restriction> restList = new List<Adornment.Restriction> ();
					foreach (GSData rest in response.ScriptData.GetGSData("adornment").GetGSDataList("restrictions")) {
						restList.Add (new Adornment.Restriction (rest.GetString ("restriction_type"), rest.GetInt ("min_level").Value, rest.GetInt ("max_level").Value));
					}
					onGetAdornment (new Adornment (response.ScriptData.GetGSData ("adornment").GetInt ("adornment_id").Value, response.ScriptData.GetGSData ("adornment").GetString ("name"), response.ScriptData.GetGSData ("adornment").GetInt ("assetbundle_id").Value, restList.ToArray ()));
				}
			} else {
				Debug.LogWarning ("GSM| Error Fetching Adornment...");
				if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@getAdornment"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@getAdornment"));
				}
			}
		});
	}

	/// <summary>
	/// returns an array of adornments
	/// </summary>
	public delegate void onGetAdornments(Adornment[] adornments);
	/// <summary>
	/// Gets the adornments.
	/// </summary>
	/// <param name="adornment_ids">Adornment ID list.</param>
	/// <param name="onGetAdornments">returns array of adornments</param>
	/// <param name="onRequestFailed">On request failed.</param>
	public void GetAdornments(List<int> adornment_ids, onGetAdornments onGetAdornments, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GMS| Fetching Adornments...");
		if (adornment_ids != null) {
			GSRequestData adList = new GSRequestData ();
			adList.AddNumberList ("adornment_ids", adornment_ids);
			new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("getAdornments")
				.SetEventAttribute ("adornment_ids", adList)
				.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log ("GSM| Adornments Retrieved....");
					if (onGetAdornments != null && response.ScriptData.GetGSDataList ("adornments") != null) {
						List<Adornment> adsList = new List<Adornment> ();
						foreach (GSData ads in response.ScriptData.GetGSDataList("adornments")) {
							List<Adornment.Restriction> restList = new List<Adornment.Restriction> ();
							foreach (GSData rest in ads.GetGSDataList("restrictions")) {
								restList.Add (new Adornment.Restriction (rest.GetString ("restriction_type"), rest.GetInt ("min_level").Value, rest.GetInt ("max_level").Value));
							}
							adsList.Add (new Adornment (ads.GetInt ("adornment_id").Value, ads.GetString ("name"), ads.GetInt ("assetbundle_id").Value, restList.ToArray ()));
						}
						onGetAdornments (adsList.ToArray ());
					}
				} else {
					Debug.LogWarning ("GSM| Error Fetching Adornment...");
					if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
						Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@getAdornment"));
						onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@getAdornment"));
					}
				}
			});
		} else {
			Debug.LogWarning ("GSM| Must Submit Valid List Of Adornments...");
		}
	}

	/// <summary>
	/// returns true or false if the adornment is available
	/// </summary>
	public delegate void isAdornmentAvailable(bool isAvailable);
	/// <summary>
	/// Checks if the adornmnet is available
	/// </summary>
	/// <param name="character_id">Character ID</param>
	/// <param name="adornment_id">Adornment ID</param>
	/// <param name="isAdornmentAvailable">returns bool</param>
	/// <param name="onRequestFailed">On request failed.</param>
	public void IsAdornmentAvailable(string character_id, int adornment_id, isAdornmentAvailable isAdornmentAvailable, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GMS| Checking is Adornment is Available...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("isAdornmentAvailable")
			.SetEventAttribute ("character_id", character_id)
			.SetEventAttribute ("adornment_id", adornment_id)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Adornments Retrieved....");
				if (isAdornmentAvailable != null && response.ScriptData.GetBoolean ("available") != null) {
					isAdornmentAvailable (response.ScriptData.GetBoolean ("available").Value);
				}
			} else {
				Debug.LogWarning ("GSM| Error Fetching Adornment...");
				if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@isAdornmentAvailable"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@isAdornmentAvailable"));
				}
			}
		});
	}
	/// <summary>
	/// return the costume ID
	/// </summary>
	public delegate void onSetFixedCostume(int costume_id);
	/// <summary>
	/// Sets the fixed costume give the out-fit ID
	/// </summary>
	/// <param name="character_id">Character ID</param>
	/// <param name="outfit_id">Outfit ID</param>
	/// <param name="onSetFixedCostume">costume ID.</param>
	/// <param name="onRequestFailed">On request failed.</param>
	public void SetFixedCostume(string character_id, int outfit_id, onSetFixedCostume onSetFixedCostume, onRequestFailed onRequestFailed)
	{
		Debug.Log ("GMS| Setting Fixed Costume...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("setFixedCostume")
			.SetEventAttribute ("character_id", character_id)
			.SetEventAttribute ("outfit_id", outfit_id)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Adornments Retrieved....");
				if (onSetFixedCostume != null && response.ScriptData.GetInt ("costume-id") != null) {
					onSetFixedCostume (response.ScriptData.GetInt ("costume-id").Value);
				}
			} else {
				Debug.LogWarning ("GSM| Error Fetching Adornment...");
				if (onRequestFailed != null && response.BaseData.GetGSData ("error") != null) {
					Debug.LogWarning (response.BaseData.GetGSData ("error").GetString ("@setFixedCostume"));
					onRequestFailed (response.BaseData.GetGSData ("error").GetString ("@setFixedCostume"));
				}
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


