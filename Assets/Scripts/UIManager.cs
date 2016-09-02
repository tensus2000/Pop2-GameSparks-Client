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
/// v1.5
/// </summary>
public class UIManager : MonoBehaviour {

	public string character_id = "57b30ba318ead60489e130c8";

	public Text logText;
	private Queue<string> myLogQueue = new Queue<string>();
	private string myLog;

	public GameObject items, scenes, scene_states, menu, playerDetails, inbox, islands, characters, outfits, non_auth;

    public Button goto_nonAuth_bttn, goto_scenes_bttn, goto_sceneState_bttn, goto_characters_bttn, goto_outfits_bttn, goto_islands_bttn, goto_chars_bttn, goto_items_bttn, goto_player_bttn, goTo_player_inbox, goTo_menu_bttn1, goTo_menu_bttn2, goTo_menu_bttn3, goTo_menu_bttn4, goTo_menu_bttn5, goTo_menu_bttn6, goTo_menu_bttn7, goTo_menu_bttn8, goTo_menu_bttn9;

	public Button clearLog_bttn;
	public InputField auth_password_input, auth_username_txt;
	public Button auth_bttn;
	public InputField reg_username_txt, reg_password, reg_displayname, reg_age, reg_gender;
	public Text server_version_txt;
	public Button reg_bttn;

	public Button getScene_bttn, setScene_bttn, enterScene_bttn;
	public InputField scene_island_id, scene_scene_id, set_scene_id, set_island_id, set_x, set_y, set_type, set_direction;

	public GameObject blockout_panel;

	public Button parent_email_bttn, get_parent_emai_bttn, delete_parent_email_bttn;
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

    public Button set_fixed_costume_bttn, is_adornments_available_bttn, get_adornment_bttn;//, get_outfit_bttn;//, set_outfit_bttn;
    public InputField eyes, marks, mouth, makeup, hair, facial, shirt, helmet, pants, bangs, shoes, overshirt, wristfont, overpants, backhand, hat, pack;
    public Text outfit_title;

    public Button outfit_back_bttn, set_outfit_bttn, create_outfit_bttn, get_outfit_bttn;
    public GameObject set_outfit_panel;

	public Button reset_email_bttn;
	public InputField username_password_reset;

	public Button get_char_names_Bttn;
	public Text get_char_names_output;
	public InputField names_count;

    public Button get_scenes_bttn;
    public InputField get_scenes_island_id;

	// Use this for initialization
	void Start () {


		// --> EXAMPLE, THIS WILL BE TRIGGERED WITH GAMESPARKS CONNECTS OR DISCONNECTS //
		GameSparksManager.Instance ().OnGSAvailable += (_isAvail) => {
			if (_isAvail) {
				Debug.LogWarning ("GameSparks Connected...");
			} else {
				Debug.LogWarning ("GameSparks Disconnected...");
			}
		};
		// --> EXAMPLE, THIS WILL BE TRIGGERED WHEN A PRIVATE MESSAGE COMES THROUGH THE WEBSOCKET //
		GameSparksManager.Instance ().OnNewPrivateMessage += (message) => {
			message.Print ();
		};
		// --> EXAMPLE, THIS WILL BE TRIGGERED WHEN A GLOBAL MESSAGE COMES THROUGH THE WEBSOCKET //
		GameSparksManager.Instance ().OnNewGlobalMessage += (message) => {
			message.Print ();
		};
		// --> EXAMPLE, THIS WILL BE TRIGGERED IF THE PLAYER WAS KICKED FROM THE SERVER DUE TO CONCURRENT LOGIN //
		GameSparksManager.Instance ().OnSessionTerminated += () => {
			Debug.LogWarning ("Session Terminated...");
		};

		#region SET UI MANAGER
		blockout_panel.SetActive (true);
		BringPanelForward (menu);
		Application.logMessageReceivedThreaded += HandleLog;
		GSMessageHandler._AllMessages = HandleGameSparksMessageReceived;
		clearLog_bttn.onClick.AddListener (() => {
			myLog = string.Empty;
			logText.text = string.Empty;
		});
		#endregion

		#region Main Menu UI Buttons
		goto_sceneState_bttn.onClick.AddListener (() => {
			Debug.Log ("Selected Scene State Options...");
			BringPanelForward (scene_states);
		});
		goto_items_bttn.onClick.AddListener (() => {
			Debug.Log ("Selected Item Options...");
			BringPanelForward (items);
		});
		goTo_menu_bttn1.onClick.AddListener (() => {
			Debug.Log ("Selected Menu Options...");
			BringPanelForward (menu);
		});
		goto_player_bttn.onClick.AddListener (() => {
			Debug.Log ("Selected Player Details Options...");
			BringPanelForward (playerDetails);
		});
		goTo_player_inbox.onClick.AddListener (() => {
			Debug.Log ("Selected Inbox Options...");
			BringPanelForward (inbox);
		});
		goto_islands_bttn.onClick.AddListener (() => {
			Debug.Log ("Selected Island Options...");
			BringPanelForward (islands);
		});
		goto_chars_bttn.onClick.AddListener (() => {
			Debug.Log ("Selected Character Options...");
			BringPanelForward (characters);
		});
		goto_outfits_bttn.onClick.AddListener (() => {
			Debug.Log ("Selected Outfits Options...");
			BringPanelForward (outfits);
		});
        goto_nonAuth_bttn.onClick.AddListener (() => {
            Debug.Log ("Selected NonAuth Options...");
            BringPanelForward (non_auth);
        });
        goto_scenes_bttn.onClick.AddListener (() => {
            Debug.Log ("Selected Scenes Options...");
            BringPanelForward (scenes);
        });

        goTo_menu_bttn2.onClick = goTo_menu_bttn3.onClick = goTo_menu_bttn9.onClick = goTo_menu_bttn4.onClick = goTo_menu_bttn5.onClick = goTo_menu_bttn8.onClick = goTo_menu_bttn6.onClick = goTo_menu_bttn7.onClick = goTo_menu_bttn1.onClick;
		#endregion

		#region AUTHENTICATION & REGISTRATION EXAMPLES
		// all gamesparks calls are linked here //
		auth_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on authentication button");
			GameSparksManager.Instance ().Authenticate (auth_username_txt.text, auth_password_input.text, (_authResponse) => {
				Debug.Log("UIM| Authentication Successful...");
				// here is an example of how we can use the event callbacks. //
				// in this case, i will remove the menu-option blocker if the player has authenticated successfully //

				// the following is an example of how to get the last-character and character list back //
				string[] characterIds = _authResponse.characterIDs;
				string lastCharacter = _authResponse.lastCharacterID;

				Debug.Log ("UIM| Character List: " + characterIds.Length);
				Debug.Log ("UIM| Last Character: " + lastCharacter);

				// now we can check if the player has a parent email registered or not //
				Debug.LogWarning("Player Has Parent Email: "+_authResponse.hasParentEmail);
				

				server_version_txt.text = "Server Version: Requesting....";
				GameSparksManager.Instance ().GetServerVersion ((_version) => {
					server_version_txt.text = "Server Version: " + _version;
					blockout_panel.SetActive (false);
				}, (_error) => {
					Debug.LogError("UIM| "+_error.ToString());
				});
            }, (_authFailed) => {
                Debug.LogError("UIM| "+_authFailed.errorMessage.ToString());
                if(_authFailed.isPop1Player != string.Empty){
                    Debug.LogWarning("Pop1 User: "+_authFailed.isPop1Player);
                }
               
			});
		});

		reg_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on registration button");
			GameSparksManager.Instance ().Register (reg_username_txt.text, reg_displayname.text, reg_password.text, int.Parse(reg_age.text), reg_gender.text,  (_playerID) => {
				// here is an example of how we can use the event callbacks. //
				// in this case, i will remove the menu-option blocker if the player has authenticated successfully //
				server_version_txt.text = "Server Version: Requesting....";
				GameSparksManager.Instance ().GetServerVersion ((_version) => {
					server_version_txt.text = "Server Version: " + _version;
					blockout_panel.SetActive (false);
				}, (_error)=>{
					Debug.LogError("UIM| "+_error.ToString());
				});
				}, (_error, _suggestedName) => {
    				Debug.LogError ("UIM| Error: " + _error.ToString());
    				Debug.LogWarning ("UIM| Suggested Username: " + _suggestedName);

			});
		});
		#endregion


		#region INVENTORY EXAMPLES
		get_inv_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on get-inventory button");
			GameSparksManager.Instance ().GetInventory (character_id, (_items) => {
				foreach (Item item in _items) {
					item.Print ();
				}
			}, (_error) => {
				Debug.LogError("UIM| "+_error.ToString());
			});
		});

		use_item_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Use-Item button");
			GameSparksManager.Instance ().UseItem (character_id, int.Parse (use_item_id.text), (_item_id) => {
				Debug.Log ("Item ID: " + _item_id);
				// and request the inventory again to make sure it was picked up //
				GameSparksManager.Instance ().GetInventory (character_id, null, null);
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});

		equip_item_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Equip-Item button");
			GameSparksManager.Instance ().EquipItem (character_id, int.Parse (equip_item_id.text), equip_location.text, (_item_id) => {
				Debug.Log ("Item ID: " + _item_id);
				// and request the inventory again to make sure it was picked up //
				GameSparksManager.Instance ().GetInventory (character_id, null, null);
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});
			
		add_item_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Pickup-Item button");
			GameSparksManager.Instance ().PickUpItem (character_id, int.Parse (pickup_item_id.text), int.Parse (pickup_scene_id.text), (_itemID) => {
				Debug.Log ("UIM| Picked Up Item:" + _itemID);
				// and request the inventory again to make sure it was picked up //
				GameSparksManager.Instance ().GetInventory (character_id, null, null);
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});

		remove_item_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Drop-Item button");
			GameSparksManager.Instance ().RemoveItem (character_id, int.Parse (pickup_item_id.text), (_itemID) => {
				Debug.Log ("UIM| Picked Up Item:" + _itemID);
				// and request the inventory again to make sure it was picked up //
				GameSparksManager.Instance ().GetInventory (character_id, null, null);
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());

			});
		});
		#endregion

		#region SCENE EXAMPLES
		getScene_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on get scene button");
			GameSparksManager.Instance ().GetSceneState (character_id, int.Parse (scene_island_id.text), int.Parse (scene_scene_id.text), (_sceneState) => {
				// callback will have the scene-state which was returned //
				// you can use it from here //
				_sceneState.Print ();
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());

			});
		});

		enterScene_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Enter Scene Button");
			GameSparksManager.Instance ().EnterScene (character_id, int.Parse (scene_island_id.text), (island_id, scene_id) => {
				// on entered scene successfully //
				Debug.Log ("Scene ID: " + scene_id + ", Island ID: " + island_id);
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());

			});
		});

		setScene_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on set scene button");
			SceneState newScene = new SceneState (set_type.text, set_direction.text, int.Parse (set_x.text), int.Parse (set_y.text));
			GameSparksManager.Instance ().SetSceneState (character_id, int.Parse (set_island_id.text), int.Parse (set_scene_id.text), newScene, null, null);
		});

        get_scenes_bttn.onClick.AddListener (() => {
            Debug.Log ("UIM| Clicked on Enter Scene Button");
            GameSparksManager.Instance ().GetScenes (character_id, int.Parse(get_scenes_island_id.text), (_sceneList) => {
                // on entered scene successfully //
                for(int i = 0; i < _sceneList.Length; i++){
                    _sceneList[i].Print();
                }
            }, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());

            });
        });
		#endregion 

		#region CHARACTER XP EXAMPLES
		grantXp_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked Grant XP Buttn...");
			int xp;
			if(int.TryParse (grantXp_field.text, out xp) == true){
				GameSparksManager.Instance ().GiveExperience (character_id, xp, (_level, _xp) => {
					Debug.Log ("XP:" + _xp + ", Level:" + _level);
					xpText.text = _xp.ToString ();
					levelText.text = _level.ToString ();
				}, (_error) => {
                    Debug.LogWarning("UIM| "+_error.errorMessage.ToString());
				});
			}else{
				Debug.LogError("UIM| Please Enter A Valid Number...");
			}
		});
		#endregion

		#region PLAYER ACCOUNT EXAMPLES
//		reset_email_bttn.onClick.AddListener (() => {
//			Debug.Log ("UIM| Clicked On Reset Password Button...");
//			GameSparksManager.Instance ().SendResetPasswordEmail(reset_email_input.text, (_newPassword) => {
//				// password set //
//			}, (_errorString) => {
//				
//			});
//		});
		get_parent_emai_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Get Parent Email Button...");
			GameSparksManager.Instance ().GetParentEmailStatus ((_parentEmailList) => {
				for(var i =0; i < _parentEmailList.Length; i++){
					_parentEmailList[i].Print();
				}
				// parent email pending validation	
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});
//
		parent_email_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Parent Email Button...");
			GameSparksManager.Instance ().RegisterParentEmail (parent_email_input.text, () => {
				// parent email pending validation	
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});

		reset_email_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Send Password Reset Button...");
            GameSparksManager.Instance ().SendResetPasswordEmail (username_password_reset.text,  () => {
				// parent email pending validation	
				Debug.Log ("UIM| Email Sent...");
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});

        delete_parent_email_bttn.onClick.AddListener (() => {
            Debug.Log ("UIM| Clicked On Delete Password Button...");
            GameSparksManager.Instance ().DeleteParentEmailHistory (() => {
                Debug.Log ("UIM| Email History Deleted...");
            }, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
            });
        });
    
		#endregion

		#region INBOX SYSTEM EXAMPLES
		send_private_message_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Send Message Button...");
			// this is an example of a private message sent without a payload //
			GameSparksManager.Instance ().SendPrivateMessage (message_header.text, message_body.text, message_recipient.text, character_id, () => {
				// on message sent
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
			// below is an example of how to send private messages with payload data //
			// the payload data can be anything the client wants to interperate as an action or trigger (or anything really) //
			// for example if we want a quest to start for the recipient when they recieve the data... //
			// we are going to send the JSON { "quest-trigger" : "message-recieved" }
			GSRequestData payloadData = new GSRequestData ();
			payloadData.AddString ("quest-trigger", "message-recieved");
			// we can then send it with the overloaded method.... //
			GameSparksManager.Instance ().SendPrivateMessage (message_header.text, message_body.text, payloadData, message_recipient.text, character_id, null, null);

		});

		get_messages_btt.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Get Messages Button...");
			// if you want all messages, then the type entered is 'both'
			GameSparksManager.Instance ().GetMessages (character_id, message_type.text, int.Parse (message_offset.text), int.Parse (message_limit.text), 
				(_messages) => {
					foreach (InboxMessage message  in _messages) {
						message.Print ();
					}
				}, (_errorString) => {
					
			});
		});

		delete_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Delete Message Button...");
			GameSparksManager.Instance ().DeleteMessage (message_id.text, () => {
				// message was deleted
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});

		read_message_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Read Message Button...");
			GameSparksManager.Instance ().ReadMessage (message_id.text);
		});
		#endregion 

		#region ISLANDS EXAMPLE
		getIslands_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Get Available Islands button");
			GameSparksManager.Instance ().GetAvailableIslands (character_id, (_islands) => {
				foreach (Island island in _islands) {
					island.Print ();
				}
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});

		visit_island_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Visit Island Button...");
			GameSparksManager.Instance ().VisitIsland (character_id, int.Parse (player_island_id.text), (_islandID) => {
				Debug.Log ("Island ID:" + _islandID);

			}, (_error) => { // if there was an error, there will be "error":{"@visitIsland":"invalid-island-id"}
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});

		leave_island_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Leave Island Button...");
			GameSparksManager.Instance ().LeaveIsland (character_id, int.Parse (player_island_id.text), () => {
				// visit to island successful //
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});

		complete_island_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Complete Island Button...");
			GameSparksManager.Instance ().CompleteIsland (character_id, int.Parse (player_island_id.text), () => {
				// player completed island
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});
		#endregion

		#region CHARACTER EXAMPLES
		get_char_names_Bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Get Character Names Button...");
			GameSparksManager.Instance ().GenerateCharacterNames (int.Parse (names_count.text), (_newNames) => {
				get_char_names_output.text = _newNames[0];
				foreach(string name in _newNames){
					Debug.LogWarning("UIM New Name: "+name);
				}
			}, (_errorString) => {
				Debug.LogError ("UIM| Error Generating Character Name...");
			});
		});


		get_char_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Get Character Button...");
			character_id = get_char_input.text;
			GameSparksManager.Instance ().GetCharacter (get_char_input.text, (_character) => {
				_character.Print ();
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});

		create_char_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Create Character Button...");

			GameSparksManager.Instance ().CreateCharacter (char_name_input.text, char_gender_input.text, (_newCharacterID) => {
				Debug.Log ("UIM| Character ID:" + _newCharacterID);
				character_id = _newCharacterID;

				// when we create the character we can call the get-level and XP request again to get the right fields //
				GameSparksManager.Instance ().GetLevelAndExperience (character_id, (_level, _xp) => {
					xpText.text = _xp.ToString();
					levelText.text = _level.ToString();
					Debug.Log("XP:"+_xp+", Level:"+_level);
				}, (_error)=>{
                    Debug.LogError("UIM| "+_error.errorMessage.ToString());
				});

			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});
		#endregion 

		#region CHARACTER OUTFIT EXAMPLES
//		set_fixed_costume_bttn.onClick.AddListener (() => {
//			Debug.Log ("UIM| Clicked On Set Fixed Costume Button...");
//			GameSparksManager.Instance ().SetFixedCostume (character_id, int.Parse (fixed_costume_id.text), (_costume_id) => {
//				Debug.Log ("UIM| Costume ID:" + _costume_id);
//			}, (_error) => {
//                Debug.LogError("UIM| "+_error.errorMessage.ToString());
//			});
//		});
//
//		is_adornments_available_bttn.onClick.AddListener (() => {
//			Debug.Log ("UIM| Clicked On Is Adornment Available Button...");
//			GameSparksManager.Instance ().IsAdornmentAvailable (character_id, int.Parse (check_adornment_id.text), (_isAvailable) => {
//				if (_isAvailable) {
//					Debug.Log ("UIM| Adornment Is Available...");
//				} else {
//					Debug.Log ("UIM| Adornment Is UnAvailable...");
//				}
//
//			}, (_error) => {
//                Debug.LogError("UIM| "+_error.errorMessage.ToString());
//			});
//		});

		// --> EXAMPLE, BELOW IS AND EXAMPLE OF HOW TO REQUEST MULTIPLE ADORNMENTS BY ID //

//		get_adornments_bttn.onClick.AddListener (() => {
//			Debug.Log("UIM| Clicked On Get Adornments Button...");
//			GameSparksManager.Instance().GetAdornments(new List<int>(){ 0,1,2,3,4,5}, (_adornments)=>{
//				foreach(Adornment ad  in _adornments){
//					ad.Print();
//				}
//			}, (_errorString)=>{
//				
//			});
//		});


//		get_adornment_bttn.onClick.AddListener (() => {
//			Debug.Log ("UIM| Clicked On Get Adornment Button...");
//			GameSparksManager.Instance ().GetAdornment (int.Parse (check_adornment_id.text), (_adornment) => {
//				_adornment.Print ();
//			}, (_error) => {
//                Debug.LogError("UIM| "+_error.errorMessage.ToString());
//			});
//
//		});
//		get_outfit_bttn.onClick.AddListener (() => {
//			Debug.Log ("UIM| Clicked On Get Outfit Button...");
//			GameSparksManager.Instance ().GetOutfit (character_id, (_outfit) => {
//				_outfit.Print ();
//			}, (_error) => {
//                Debug.LogError("UIM| "+_error.errorMessage.ToString());
//			});
//		});
		set_outfit_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Set Outfit Button...");

            // THE FOLLOWING IS AN EXAMPLE OF AN OUTFIT USED FOR TESTING //
//            Outfit outfit = new Outfit();
//            outfit.isPlayerOutfit = true;
//            outfit.skinColor = Color.gray;
//            outfit.hairColor = Color.blue;
//            outfit.reactiveEyelids = true;
//            outfit.eyes = new Adornment(){ name = "eyes"};
//            outfit.mouth = new Adornment(){ name = "mouth"};
//            outfit.hair = new Adornment(){ name = "hair"};
//            outfit.shirt = new Adornment(){ name = "shirt"};
//            outfit.pants = new Adornment(){ name = "pants"};
//            outfit.shoes = new Adornment(){ name = "shoes"};
//            outfit.wristFront = new Adornment(){ name = "wrist"};
//            outfit.bangs = new Adornment(){ name = "bangs"};
//            outfit.helmet = new Adornment(){ name = "helmet"};
//            outfit.facial = new Adornment(){ name = "facial"};
//            outfit.makeup = new Adornment(){ name = "makeup"};
//            outfit.marks = new Adornment(){ name = "marks"};
//            outfit.overshirt = new Adornment(){ name = "overshirt"};
//            outfit.overpants = new Adornment(){ name = "overpants"};
//            outfit.backhandItem = new Adornment(){ name = "backhandItem"};
//            outfit.hat = new Adornment(){ name = "hat"};
//            outfit.pack = new Adornment(){ name = "pack"};

            Outfit outfit = new Outfit();
            outfit.isPlayerOutfit = true;
            outfit.skinColor = Color.gray;
            outfit.hairColor = Color.blue;
            outfit.reactiveEyelids = true;
            outfit.eyes = new Adornment(){ name = eyes.text };
            outfit.mouth = new Adornment(){ name = mouth.text };
            outfit.hair = new Adornment(){ name = hair.text };
            outfit.shirt = new Adornment(){ name = shirt.text };
            outfit.pants = new Adornment(){ name = pants.text };
            outfit.shoes = new Adornment(){ name = shoes.text };
            outfit.wristFront = new Adornment(){ name = wristfont.text };
            outfit.bangs = new Adornment(){ name = bangs.text };
            outfit.helmet = new Adornment(){ name = helmet.text };
            outfit.facial = new Adornment(){ name = facial.text };
            outfit.makeup = new Adornment(){ name = makeup.text };
            outfit.marks = new Adornment(){ name = marks.text };
            outfit.overshirt = new Adornment(){ name = overshirt.text };
            outfit.overpants = new Adornment(){ name = overpants.text };
            outfit.backhandItem = new Adornment(){ name = backhand.text };
            outfit.hat = new Adornment(){ name = hat.text };
            outfit.pack = new Adornment(){ name = pack.text };


			GameSparksManager.Instance ().SetOutfit (character_id, outfit, () => {
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});

        set_outfit_panel.SetActive(false);
        create_outfit_bttn.onClick.AddListener (() => {
            Debug.Log("UIM| Clicked On Create Outfit Bttn");

            eyes.readOnly = false;
            mouth.readOnly = false;
            hair.readOnly = false;
            shirt.readOnly = false;
            pants.readOnly = false;
            shoes.readOnly = false;
            wristfont.readOnly = false;
            bangs.readOnly = false;
            helmet.readOnly = false;
            facial.readOnly = false;
            makeup.readOnly = false;
            marks.readOnly = false;
            overshirt.readOnly = false;
            overpants.readOnly = false;
            backhand.readOnly = false;
            hat.readOnly = false;
            pack.readOnly = false;

            outfit_title.text = "Create Outfit";
            set_outfit_bttn.gameObject.SetActive(true);
            set_outfit_panel.SetActive(true);
        });
        outfit_back_bttn.onClick.AddListener (() => {
            Debug.Log("UIM| Clicked On Outfit Back Bttn");
            set_outfit_panel.SetActive(false);
        });
        get_outfit_bttn.onClick.AddListener (() => {
            Debug.Log("UIM| Clicked On Get Outfit Bttn");

            eyes.readOnly = true;
            mouth.readOnly = true;
            hair.readOnly = true;
            shirt.readOnly = true;
            pants.readOnly = true;
            shoes.readOnly = true;
            wristfont.readOnly = true;
            bangs.readOnly = true;
            helmet.readOnly = true;
            facial.readOnly = true;
            makeup.readOnly = true;
            marks.readOnly = true;
            overshirt.readOnly = true;
            overpants.readOnly = true;
            backhand.readOnly = true;
            hat.readOnly = true;
            pack.readOnly = true;

            outfit_title.text = "Load Outfit";
            set_outfit_bttn.gameObject.SetActive(false);
            set_outfit_panel.SetActive(true);

            GameSparksManager.Instance ().GetOutfit(character_id, (_outfit) => {
            }, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
            });

        });
		#endregion

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
		scene_states.SetActive(false);
        scenes.SetActive(false);
		playerDetails.SetActive (false);
		inbox.SetActive (false);
		islands.SetActive (false);
		characters.SetActive (false);
		outfits.SetActive (false);
        non_auth.SetActive(false);
		_panel.SetActive (true);
         

		if (_panel.gameObject.name == "character_panel") {
			Debug.Log ("UIM| Loading Player Details...");
			GameSparksManager.Instance ().GetLevelAndExperience (character_id, (_level, _xp) => {
				xpText.text = _xp.ToString();
				levelText.text = _level.ToString();
				Debug.Log("XP:"+_xp+", Level:"+_level);
			}, (_error)=>{
				Debug.LogWarning("UIM| "+_error.ToString());
			});
		}

	}
}





