using UnityEngine;
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

	public GameObject achievements, daily_spin, currency, asset_bundle, items, scenes, scene_states, menu, playerDetails, inbox, islands, characters, outfits, non_auth, quests, test_user;

    public Button logoutBttn;
    public Button goto_achievements_bttn, goto_dailyspin_bttn, goto_currency_bttn, goto_asset_bundle_bttn, goto_testuser_bttn, goto_quests_bttn, goto_nonAuth_bttn, goto_scenes_bttn, goto_sceneState_bttn, goto_characters_bttn, goto_outfits_bttn, goto_islands_bttn, goto_chars_bttn, goto_items_bttn, goto_player_bttn, goTo_player_inbox, goTo_menu_bttn1, goTo_menu_bttn2, goTo_menu_bttn3, goTo_menu_bttn4, goTo_menu_bttn5, goTo_menu_bttn6, goTo_menu_bttn7, goTo_menu_bttn8, goTo_menu_bttn9, goTo_menu_bttn10, goTo_menu_bttn11, goTo_menu_bttn12,  goTo_menu_bttn13, goTo_menu_bttn14, goTo_menu_bttn15;

	public Button clearLog_bttn;
	public InputField auth_password_input, auth_username_txt;
	public Button auth_bttn;
	public InputField reg_username_txt, reg_password, reg_displayname, reg_age, reg_gender;
	public Text server_version_txt;
	public Button reg_bttn;

	public Button getScene_bttn, setScene_bttn, enterScene_bttn;
	public InputField getscene_island_id, getscene_scene_id, set_scene_id, set_island_id, set_x, set_y, set_type, set_direction;

	public GameObject blockout_panel;

    // PLAYER OPTIONS //
	public Button parent_email_bttn, get_parent_emai_bttn, delete_parent_email_bttn, change_password_bttn, check_username_bttn;
	public InputField parent_email_input, old_password, new_password, check_username, check_username_suggestions;



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
    public InputField marks, makeup, hair, facial, shirt, helmet, pants, bangs, shoes, overshirt, wristfont, overpants, backhand, hat, pack, adornment_id;
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

    public Button set_quests_bttn, get_quests_bttn;

    public Button test_user_bttn;
    public InputField  test_username, test_displayName, test_age, test_gender, test_password;

    public Button submit_asset_bundle_bttn, get_asset_bundle_bttn;
    public InputField asset_bundle_id, created_by, admin_username, admin_password, file_path_1, get_asset_bundle_id;
    public GameObject uploadingPanel;

    public InputField currency_character_id, no_moonstones_convert_field, currency_event_name_field, purchase_item_id, get_purchasable_limit, get_purchasable_offset, get_purchasable_tag;
    public Button convert_to_coins_bttn, add_currency_event_complete_bttn, purchase_item_bttn, get_balance_bttn, get_purchasable_items_bttn;
    public Text to_coins;

    public Button get_daily_spin_bttn, choose_daily_bonus_bttn;
    public InputField daily_spin_character_id, choose_daily_bonus_id;

    public Button award_ach_bttn;
    public InputField award_ach_name;

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
        // --> EXAMPLE, THIS WILL BE TRIGGERED IF THE SEVER WAS UPATED WHILE THE PLAYER IS CONNECTED //
        GameSparksManager.Instance ().OnServerVersionMessage += (serverVersion) => {
            serverVersion.Print();
        };

        GameSparksManager.Instance ().OnCurrencyBalanceMessage += (balance) => {
            balance.Print();
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

        logoutBttn.onClick.AddListener (() => {
            GameSparksManager.Instance().Logout(()=>{
                Debug.Log("UIM| Player Logged Out...");
                BringPanelForward (menu);
                blockout_panel.SetActive(true);
            },(_error)=>{
                Debug.Log("UIM|"+ _error.errorMessage.ToString());
            });
        });

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
        goto_quests_bttn.onClick.AddListener (() => {
            Debug.Log ("Selected Quests Options...");
            BringPanelForward (quests);
        });
        goto_testuser_bttn.onClick.AddListener (() => {
            Debug.Log ("Selected Test User Options...");
            BringPanelForward (test_user);
        });
        goto_asset_bundle_bttn.onClick.AddListener (() => {
            Debug.Log ("Selected Asset Bundle Options...");
            BringPanelForward (asset_bundle);
        });
        goto_currency_bttn.onClick.AddListener (() => {
            Debug.Log ("Selected Currency Options...");
            BringPanelForward (currency);
        });
        goto_dailyspin_bttn.onClick.AddListener (() => {
            Debug.Log ("Selected Daily Spin Options...");
            BringPanelForward (daily_spin);
        });

        goto_achievements_bttn.onClick.AddListener (() => {
            Debug.Log ("Selected Achievements Options...");
            BringPanelForward (achievements);
        });

        goTo_menu_bttn15.onClick = goTo_menu_bttn14.onClick = goTo_menu_bttn13.onClick = goTo_menu_bttn10.onClick = goTo_menu_bttn12.onClick = goTo_menu_bttn11.onClick = goTo_menu_bttn2.onClick = goTo_menu_bttn3.onClick = goTo_menu_bttn9.onClick = goTo_menu_bttn4.onClick = goTo_menu_bttn5.onClick = goTo_menu_bttn8.onClick = goTo_menu_bttn6.onClick = goTo_menu_bttn7.onClick = goTo_menu_bttn1.onClick;
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
				GameSparksManager.Instance ().GetServerVersion ((_versionServer) => {
                    _versionServer.Print();
                    Debug.Log("UIM| Version Date:"+_versionServer.published.ToString());
                    server_version_txt.text = "Server Version: " + _versionServer.currentVersion;
					blockout_panel.SetActive (false);
				}, (_error, _serverVersion) => {
                    blockout_panel.SetActive (false);
                    _serverVersion.Print();
                    Debug.LogError("UIM| Error:"+_error.errorMessage.ToString());
				});
            }, (_authFailed) => {
                Debug.LogError("UIM| "+_authFailed.errorMessage.ToString());

                if(_authFailed.isPop1Player != string.Empty){
                    Debug.LogWarning("Pop1 User: "+_authFailed.isPop1Player);
                }

                _authFailed.Print();
               
			});
		});

		reg_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on registration button");
			GameSparksManager.Instance ().Register (reg_username_txt.text, reg_displayname.text, reg_password.text, int.Parse(reg_age.text), reg_gender.text,  (_playerID) => {
				// here is an example of how we can use the event callbacks. //
				// in this case, i will remove the menu-option blocker if the player has authenticated successfully //
				server_version_txt.text = "Server Version: Requesting....";
				GameSparksManager.Instance ().GetServerVersion ((_serverVersion) => {
                    _serverVersion.Print();
                    Debug.Log("UIM| Version Date:"+_serverVersion.published.ToString());
                    server_version_txt.text = "Server Version: " + _serverVersion.currentVersion;
					blockout_panel.SetActive (false);
				}, (_error, _serverVersion)=>{
                    _serverVersion.Print();
                    Debug.LogError("UIM| "+_error.ToString());
                    blockout_panel.SetActive (false);
				});
				}, (_error) => {
                Debug.LogError ("UIM| Error: " + _error.errorMessage.ToString());

			});
		});

        check_username_bttn.onClick.AddListener (() => {
            Debug.Log ("UIM| Clicked on registration button");
            GameSparksManager.Instance ().CheckUsername(check_username.text, int.Parse(check_username_suggestions.text),  
                (_checkUsernameResp) => {
                _checkUsernameResp.Print();
//                if(_checkUsernameResp.availableName != string.Empty && _checkUsernameResp.availableName != null)
//                {
//                    Debug.Log("UIM| Username Available: "+_checkUsernameResp.availableName);
//                }
//                if(_checkUsernameResp.suggestedNames.Length > 0)
//                {
//                    for(int i = 0; i < _checkUsernameResp.suggestedNames.Length; i++)
//                    {
//                        Debug.Log("UIM| Suggestion ["+(i+1)+"] -> "+_checkUsernameResp.suggestedNames[i]);
//                    }
//                }
//                Debug.Log("Is Pop 1 Player: "+_checkUsernameResp.ispop);
//                Debug.Log("Is Pop 2 Player: "+_isPop2Player);
            }, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
            }
                );
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
			GameSparksManager.Instance ().UseItem (character_id, use_item_id.text, (_item_id) => {
				Debug.Log ("Item ID: " + _item_id);
				// and request the inventory again to make sure it was picked up //
				GameSparksManager.Instance ().GetInventory (character_id, null, null);
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});

		equip_item_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Equip-Item button");
			GameSparksManager.Instance ().EquipItem (character_id, equip_item_id.text, equip_location.text, (_item_id) => {
				Debug.Log ("Item ID: " + _item_id);
				// and request the inventory again to make sure it was picked up //
				GameSparksManager.Instance ().GetInventory (character_id, null, null);
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});
			
		add_item_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Pickup-Item button");
			GameSparksManager.Instance ().PickUpItem (character_id, pickup_item_id.text, pickup_scene_id.text, (_itemID) => {
				Debug.Log ("UIM| Picked Up Item:" + _itemID);
				// and request the inventory again to make sure it was picked up //
				GameSparksManager.Instance ().GetInventory (character_id, null, null);
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});

		remove_item_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Drop-Item button");
			GameSparksManager.Instance ().RemoveItem (character_id, pickup_item_id.text, (_itemID) => {
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
			GameSparksManager.Instance ().GetSceneState (character_id, getscene_island_id.text, getscene_scene_id.text, (_states) => {
				// callback will have the scene-state which was returned //
				// you can use it from here //
				_states.Print ();
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());

			});
		});

		enterScene_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on Enter Scene Button");
			GameSparksManager.Instance ().EnterScene (character_id, getscene_island_id.text, (island_id, scene_id) => {
				// on entered scene successfully //
				Debug.Log ("Scene ID: " + scene_id + ", Island ID: " + island_id);
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());

			});
		});

		setScene_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked on set scene button");




            Dictionary<string, object> newDic = new Dictionary<string, object>();
//        
            newDic.Add("myString", "Yo!");
            newDic.Add("myint" , 10);
            newDic.Add("test_elem", new TestElement());
            newDic.Add("thingB", new ClassB());
            newDic.Add("thingC", new ClassC());
            newDic.Add("thingE", new ClassD());
            newDic.Add("thingF", new ClassE());

            SceneState newSceneState = new SceneState(character_id, 1, 1, newDic);
            GameSparksManager.Instance ().SetSceneState (character_id, set_island_id.text, set_scene_id.text, newSceneState, null, null);
		});

        get_scenes_bttn.onClick.AddListener (() => {
            Debug.Log ("UIM| Clicked on Enter Scene Button");
            GameSparksManager.Instance ().GetScenes (character_id, get_scenes_island_id.text, (_sceneList) => {
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

        change_password_bttn.onClick.AddListener (() => {
            Debug.Log ("UIM| Clicked On Get Parent Email Button...");
            GameSparksManager.Instance ().ChangePassword(old_password.text, new_password.text, () => {
                Debug.Log("UIM| Password Set...");
            }, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
            });
        });

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
            Debug.Log ("UIM| Clicked On Delete Parent Email Button...");
            GameSparksManager.Instance ().DeleteParentEmail (() => {
                Debug.Log ("UIM| Email Deleted...");
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
			// we are going to send the JSON { "quest-trigger" : "message-GameSparksErrorMessage" }
			GSRequestData payloadData = new GSRequestData ();
			payloadData.AddString ("quest-trigger", "message-GameSparksErrorMessage");
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
            GameSparksManager.Instance ().ReadMessage (message_id.text, () => {
                // message was deleted
            }, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
            });
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
			GameSparksManager.Instance ().VisitIsland (character_id, player_island_id.text, (_islandID) => {
				Debug.Log ("Island ID:" + _islandID);

			}, (_error) => { // if there was an error, there will be "error":{"@visitIsland":"invalid-island-id"}
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});

		leave_island_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Leave Island Button...");
			GameSparksManager.Instance ().LeaveIsland (character_id, player_island_id.text, () => {
				// visit to island successful //
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});

		complete_island_bttn.onClick.AddListener (() => {
			Debug.Log ("UIM| Clicked On Complete Island Button...");
			GameSparksManager.Instance ().CompleteIsland (character_id, player_island_id.text, () => {
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
            outfit.hair = new Hair(){ name = hair.text };
            outfit.shirt = new Shirt(){ name = shirt.text };
            outfit.pants = new Pants(){ name = pants.text };
            outfit.shoes = new Shoes(){ name = shoes.text };
            outfit.wristFront = new WristFront(){ name = wristfont.text };
            outfit.bangs = new Bangs(){ name = bangs.text };
            outfit.helmet = new Helmet(){ name = helmet.text };
            outfit.facial = new Facial(){ name = facial.text };
            outfit.makeup = new Makeup(){ name = makeup.text };
            outfit.marks = new Marks(){ name = marks.text };
            outfit.overshirt = new Overshirt(){ name = overshirt.text };
            outfit.overpants = new Overpants(){ name = overpants.text };
            outfit.backhandItem = new BackhandItem(){ name = backhand.text };
            outfit.hat = new Hat(){ name = hat.text };
            outfit.pack = new Pack(){ name = pack.text };


			GameSparksManager.Instance ().SetOutfit (character_id, outfit, () => {
			}, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		});

        set_outfit_panel.SetActive(false);
        create_outfit_bttn.onClick.AddListener (() => {
            Debug.Log("UIM| Clicked On Create Outfit Bttn");

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

            GameSparksManager.Instance ().GetOutfit(character_id, (_outfitPrototype) => {

                foreach(AdornmentPrototype ad  in _outfitPrototype.adornmentList)
                {
                    Debug.Log(ad.type+":"+ad.name+"\n url:"+ad.url);
                    switch(ad.type){
                        case "hair":
                            hair.text = ad.url;
                            break;
                        case "shirt":
                            shirt.text = ad.url;
                            break;
                        case "pants":
                            pants.text = ad.url;
                            break;
                        case "shoes": 
                            shoes.text = ad.url;
                            break;
                        case "bangs":
                            bangs.text = ad.url;
                            break;
                        case "helmet":
                            helmet.text = ad.url;
                            break;
                        case "facial":
                            facial.text = ad.url;
                            break;
                        case "makeup":
                            makeup.text = ad.url;
                            break;
                        case "marks":
                            marks.text = ad.url;
                            break;
                        case "overshirt":
                            overshirt.text = ad.url;
                            break;
                        case "overpants":
                            overpants.text = ad.url;
                            break;
                        case "hat":
                            hat.text = ad.url;
                            break;
                        case "pack":
                            pack.text = ad.url;
                            break;

                    }
                }
                Debug.Log("Hair color: " + _outfitPrototype.hairColor.ToString());
                Debug.Log("Skin color: " + _outfitPrototype.skinColor.ToString());
                Debug.Log("isPlayerOutfit: " + _outfitPrototype.isPlayerOutfit);
                Debug.Log("reactiveEyelids: " + _outfitPrototype.reactiveEyelids);


            }, (_error) => {
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
            });

        });


        is_adornments_available_bttn.onClick.AddListener (() => {
            Debug.Log("UIM| Clicked Is Available Button...");
            GameSparksManager.Instance().IsAdornmentAvailable(character_id, adornment_id.text, (_isAvail)=>{
                Debug.Log("UIM| Is Available:"+_isAvail);
            }, (_error)=>{
                Debug.Log("UIM| "+_error.errorMessage.ToString());
            });
        });

        get_adornment_bttn.onClick.AddListener (() => {
            Debug.Log("UIM| Clicked Get Adornment Button...");
            GameSparksManager.Instance().GetAdornment(adornment_id.text, (_adornment)=>{
                Debug.Log("UIM| URL:"+_adornment.url);
            }, (_error)=>{
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
            });

//            GameSparksManager.Instance().GetAdornments(new List<string>(){ "makeup", "shoes", "shirt"}, (_adornments)=>{
//                foreach(AdornmentPrototype ad in _adornments)
//                {
//                    Debug.Log(ad.url);
//                }
//            }, (_error) =>{
//                Debug.Log("UIM| "+_error.errorMessage.ToString());
//            });
        });




		#endregion

        #region Quests
        StageData stage1 = new StageData("201", "202", "stage 201", true, true, false);
        stage1.SetRewards(new List<string>(){ "test1", "test2", "test3" });
        stage1.SetSteps(new List<QuestStep>(){
            new QuestStep("PickHammer", 10, "1", "AquireObject", false, false),
            new QuestStep("PickAnAxe", 6, "2", "AquireObject", true, false)
        });

        StageData stage2 = new StageData("202", "", "stage 202", false, false, false);
        stage2.SetRewards(new List<string>(){ "test1", "test2", "test3" });
        stage2.SetSteps(new List<QuestStep>(){
            new QuestStep("FixThePlane", "2", "CompleteObjective", true, false),
            new QuestStep("LearnToFly", "3", "CompleteObjective", true, false)
        });

        QuestData mockQuest = new QuestData("1", "myQuest", "Gobbledygook Quest", true, false);
        mockQuest.SetRewards(new List<string>(){ "test1", "test2", "test3" });
        mockQuest.SetStages(new List<StageData>()
            {
                stage1, stage2
            });

        set_quests_bttn.onClick.AddListener (() => {
        Debug.Log("UIM| Clicked On Set Quest Button...");
            GameSparksManager.Instance().SaveQuest(character_id, mockQuest, () =>{
                
            }, (_error) =>{
                
            });
        });

        get_quests_bttn.onClick.AddListener (() => {
            Debug.Log("UIM| Clicked On Load Quest Button...");
            GameSparksManager.Instance().LoadQuest(character_id, "1", (_quest) =>{
                _quest.Print();
            }, (_error) =>{

            });
        });
        #endregion

        #region Test User
        test_user_bttn.onClick.AddListener (() => {
            Debug.Log("UIM| Clicked On Create Test User Button...");
            GameSparksManager.Instance().RegisterTestAccount(test_username.text, 
                test_displayName.text, test_password.text, int.Parse(test_age.text), test_gender.text, () =>{
//                _quest.Print();
            }, (_error, _checkUsername) =>{
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
                _checkUsername.Print();
            });
        });
        #endregion

        #region Asset Bundle

        submit_asset_bundle_bttn.onClick.AddListener (() => {
            Debug.Log("Clicked On Sumitt Asset Bundle Button...");

            List<string> filePaths = new List<string>();
            filePaths.Add(file_path_1.text);
            uploadingPanel.SetActive(true);
            GameSparksDownloadablesManager.SubmitAssetBundle(asset_bundle_id.text, created_by.text, admin_username.text, admin_password.text, file_path_1.text, 
                (_assetBundle)=>{
                _assetBundle.Print();
                uploadingPanel.SetActive(false);
            }, (_error)=>{
                uploadingPanel.SetActive(false);
                Debug.LogError(_error.errorMessage.ToString());
            });
        });

        get_asset_bundle_bttn.onClick.AddListener (() => {
            Debug.Log("Clicked On Sumitt Asset Bundle Button...");

            GameSparksManager.Instance().GetAssetBundle(get_asset_bundle_id.text, (_assetbundle) =>{
                _assetbundle.Print();
            }, (_error)=>{
                Debug.LogError(_error.errorMessage.ToString());
            });

//            List<string> assetBundleList = new List<string>();
//            assetBundleList.Add("test_a");
//            GameSparksManager.Instance().GetAssetBundles(assetBundleList, (_assetbundles) =>{
//                for(int i = 0; i < _assetbundles.Length; i++)
//                {
//                    _assetbundles[i].Print();
//                }
//
//            }, (_error)=>{
//                Debug.LogError(_error.errorMessage.ToString());
//            });

        });

        #endregion 

        #region Currency

        convert_to_coins_bttn.onClick.AddListener(() => {
            Debug.Log("Clicked On Convert To Coins Button...");
            GameSparksManager.Instance().ConvertToCoins(int.Parse(no_moonstones_convert_field.text), (_balance)=>{
                to_coins.text = _balance.coin_delta.ToString();
                Debug.Log(_balance.coin_delta+"<- Coins Converted To MoonStones ->"+_balance.moonstone_delta);
                _balance.Print();
            }, (_error) =>{
                Debug.LogError(_error.errorMessage.ToString());
            });
        });
           

        purchase_item_bttn.onClick.AddListener(() => {
            Debug.Log("Clicked On Purchase Item ID Button...");
            GameSparksManager.Instance().PurchaseItem(purchase_item_id.text, (_balance)=>{
                _balance.Print();
            }, (_error) =>{
                Debug.LogError(_error.errorMessage.ToString());
            });
        });

        get_balance_bttn.onClick.AddListener(() => {
            Debug.Log("Clicked On Get Balance Button...");
            GameSparksManager.Instance().GetBalance((_balance)=>{
                _balance.Print();
            }, (_error) =>{
                Debug.LogError(_error.errorMessage.ToString());
            });
        });

        get_purchasable_items_bttn.onClick.AddListener(() => {
            Debug.Log("Clicked On Get Purchasable Items Button...");
            if(get_purchasable_tag.text == string.Empty)
            {
                GameSparksManager.Instance().GetPurchasableItems(int.Parse(get_purchasable_offset.text), int.Parse(get_purchasable_limit.text),  (_items)=>{
                    for(int i = 0; i < _items.Length; i++)
                    {
                        _items[i].Print();
                    }
                }, (_error) =>{
                    Debug.LogError(_error.errorMessage.ToString());
                });
            }
            else
            {
                GameSparksManager.Instance().GetPurchasableItems(new string[]{ get_purchasable_tag.text }, int.Parse(get_purchasable_offset.text), int.Parse(get_purchasable_limit.text),  (_items)=>{
                    for(int i = 0; i < _items.Length; i++)
                    {
                        _items[i].Print();
                    }
                }, (_error) =>{
                    Debug.LogError(_error.errorMessage.ToString());
                });
            }


        });

        #endregion

        #region Daily Spin

        get_daily_spin_bttn.onClick.AddListener(() => {
            Debug.Log("Clicked Get Daily Spin Button...");
            GameSparksManager.Instance().GetDailyBonusList(daily_spin_character_id.text, (_dailySpinList)=>{
                _dailySpinList.Print();
            }, (_error) =>{
                Debug.LogError(_error.errorMessage.ToString());
            });
        });

        choose_daily_bonus_bttn.onClick.AddListener(() => {
            Debug.Log("Clicked Choose Daily Bonus Button...");
            GameSparksManager.Instance().ChooseDailyBonus(daily_spin_character_id.text, choose_daily_bonus_id.text, (_bonus, _currency)=>{
                _bonus.Print();
                _currency.Print();
            }, (_error) =>{
                Debug.LogError(_error.errorMessage.ToString());
            });
        });


        #endregion 

        #region Achievement

        award_ach_bttn.onClick.AddListener(() => {
            Debug.Log("Clicked Award Achievement Button...");
            GameSparksManager.Instance().AwardAchievement(award_ach_name.text, (_currency) =>{
                _currency.Print();
            }, (_error) =>{
                Debug.LogError(_error.errorMessage.ToString());
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
        quests.SetActive(false);
        test_user.SetActive(false);
        asset_bundle.SetActive(false);
        currency.SetActive(false);
        daily_spin.SetActive(false);
        achievements.SetActive(false);
		_panel.SetActive (true);
         

		if (_panel.gameObject.name == "character_panel") {
			Debug.Log ("UIM| Loading Player Details...");
			GameSparksManager.Instance ().GetLevelAndExperience (character_id, (_level, _xp) => {
				xpText.text = _xp.ToString();
				levelText.text = _level.ToString();
				Debug.Log("XP:"+_xp+", Level:"+_level);
			}, (_error)=>{
                Debug.LogError("UIM| "+_error.errorMessage.ToString());
			});
		}

	}
}





