using UnityEngine;
using System.Collections;
using GameSparks.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;


//public void LoadQuest(string character_id, string quest_id,  onLoadQuest onLoadQuest, onRequestFailed onRequestFailed)
//{
//    Debug.Log("GMS| Fetching Quest Info...");
//    new GameSparks.Api.Requests.LogEventRequest().SetEventKey("loadQuest")
//        .SetEventAttribute("character_id", character_id)
//        .SetEventAttribute("quest_id", quest_id)
//        .SetDurable(true)
//        .Send((response) =>
//            {
//                if (!response.HasErrors)
//                {
//                    if(onLoadQuest != null && response.ScriptData.GetGSData("questProgress") != null)
//                    {
//                        GSData questData = response.ScriptData.GetGSData("questProgress");
//                        QuestData newQuest = new QuestData();
//
//                        foreach(var questField in typeof(QuestData).GetFields())
//                        {
//                            if(questField.FieldType == typeof(string) && !questField.IsNotSerialized)
//                            {
//                                questField.SetValue(newQuest, questData.GetString(questField.Name));
//                            }
//                            else if(questField.FieldType == typeof(bool) && !questField.IsNotSerialized)
//                            {
//                                questField.SetValue(newQuest, questData.GetBoolean(questField.Name).GetValueOrDefault(false));
//                            }
//                            else if(questField.FieldType == typeof(int) && !questField.IsNotSerialized)
//                            {
//                                questField.SetValue(newQuest, questData.GetNumber(questField.Name).GetValueOrDefault(0));
//                            }
//                            else
//                            {
//                                if(questField.FieldType == typeof(List<string>) && !questField.IsNotSerialized)
//                                {
//                                    questField.SetValue(newQuest, questData.GetStringList(questField.Name));
//                                }
//                                else if(questField.FieldType == typeof(List<StageData>) && !questField.IsNotSerialized)
//                                {
//                                    List<GSData> respStageList = questData.GetGSDataList("stages");
//                                    List<StageData> stageList = new List<StageData>();
//                                    foreach(GSData gs_stage in respStageList)
//                                    {
//                                        StageData stage = new StageData();
//                                        foreach(var stageField in typeof(StageData).GetFields())
//                                        {
//                                            if(stageField.FieldType == typeof(string) && !stageField.IsNotSerialized)
//                                            {
//                                                stageField.SetValue(stage, gs_stage.GetString(stageField.Name));
//                                            }
//                                            else if(stageField.FieldType == typeof(bool) && !stageField.IsNotSerialized)
//                                            {
//                                                stageField.SetValue(stage, gs_stage.GetBoolean(stageField.Name).GetValueOrDefault(false));
//                                            }
//                                            else if(stageField.FieldType == typeof(int) && !stageField.IsNotSerialized)
//                                            {
//                                                stageField.SetValue(stage, gs_stage.GetNumber(stageField.Name).GetValueOrDefault(0));
//                                            }
//                                            else
//                                            {
//                                                if(stageField.FieldType == typeof(List<string>) && !stageField.IsNotSerialized)
//                                                {
//                                                    stageField.SetValue(stage, gs_stage.GetStringList(stageField.Name));
//                                                }
//                                                else if(stageField.FieldType == typeof(List<QuestStep>)  && !stageField.IsNotSerialized)
//                                                {
//                                                    List<GSData> respStepList = gs_stage.GetGSDataList("steps");
//                                                    List<QuestStep> stepList = new List<QuestStep>();
//                                                    foreach(GSData gs_step in respStepList)
//                                                    {
//                                                        QuestStep step = new QuestStep();
//                                                        foreach(var stepField in typeof(QuestStep).GetFields())
//                                                        {
//                                                            if(stepField.FieldType == typeof(string)  && !stepField.IsNotSerialized)
//                                                            {
//                                                                stepField.SetValue(step, gs_step.GetString(stepField.Name));
//                                                            }
//                                                        }
//                                                        stepList.Add(step);
//                                                    }
//                                                    stage.SetSteps(stepList);
//                                                }
//                                            }
//                                        }
//                                        stageList.Add(stage);
//                                    }
//                                    newQuest.SetStages(stageList);
//                                }
//                            }
//                        }
//                        onLoadQuest(newQuest);
//                    }
//                }
//                else
//                {
//                    if (onRequestFailed != null)
//                    {
//                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
//                    }
//                }
//            });
//}


public class Scene
{
    int scene_id, island_id;
    string asset_bundle;
    Vector2 start_location;
    Connection[] connections;

    public Scene(int scene_id, int island_id,string asset_bundle,Vector2 start_location, Connection[] connections)
    {
        this.scene_id = scene_id;
        this.island_id = island_id;
        this.asset_bundle = asset_bundle;
        this.start_location = start_location;
        this.connections = connections;
    }

    public class Connection
    {
        int scene_id;
        Vector2 start_location;

        public Connection(int scene_id, Vector2 start_location)
        {
            this.scene_id = scene_id;
            this.start_location = start_location;
        }

        public void Print()
        {
            Debug.Log("ID:"+scene_id+", Start Location"+start_location);
        }
    }
   
    public void Print()
    {
        Debug.Log("ID:"+scene_id+", Island ID:"+island_id+", Asset Bundle:"+asset_bundle+", Start Location"+start_location+", Connections:"+connections.Length);
    }


}

[Serializable()]
public class Outfit {
  
    public bool isPlayerOutfit;
    public Color skinColor, hairColor;
    public bool reactiveEyelids;
    public Hair hair;
    public Shirt shirt;
    public Pants pants;
    public Shoes shoes;
    public WristFront wristFront;
    public Bangs bangs;
    public Helmet helmet;
    public Facial facial;
    public Makeup makeup;
    public Marks marks;
    public Overshirt overshirt;
    public Overpants overpants;
    public BackhandItem backhandItem;
    public Hat hat;
    public Pack pack;




    public void Print()
    {
        Debug.Log("Hair:"+hair.name+", shirt:"+shirt.name+", pants:"+pants.name+", shoes:"+shoes.name);
        Debug.Log("Wrist:"+wristFront.name+", bangs:"+bangs.name+", helmet:"+helmet.name+", facial:"+facial.name);
        Debug.Log("Makeup:"+makeup.name+", marks:"+marks.name+", overshirt:"+overshirt.name+", overpants:"+overpants.name);
        Debug.Log("BackHandItem:"+backhandItem.name+", hat:"+hat.name+", pack:"+pack.name);

        Debug.Log("Hair:"+hair.asset_bundle_url+", shirt:"+shirt.asset_bundle_url+", pants:"+pants.asset_bundle_url+", shoes:"+shoes.asset_bundle_url);
        Debug.Log("Wrist:"+wristFront.asset_bundle_url+", bangs:"+bangs.asset_bundle_url+", helmet:"+helmet.asset_bundle_url+", facial:"+facial.asset_bundle_url);
        Debug.Log("Makeup:"+makeup.asset_bundle_url+", marks:"+marks.asset_bundle_url+", overshirt:"+overshirt.asset_bundle_url+", overpants:"+overpants.asset_bundle_url);
        Debug.Log("BackHandItem:"+backhandItem.asset_bundle_url+", hat:"+hat.asset_bundle_url+", pack:"+pack.asset_bundle_url);
    }    
}

public class OutfitPrototype
{
    public bool isPlayerOutfit;

    public Color skinColor, hairColor;
    public bool reactiveEyelids;

    public List<AdornmentPrototype> adornmentList;

    public OutfitPrototype()
    {
        adornmentList = new List<AdornmentPrototype>();
    }
}

public class AdornmentPrototype
{
    public string type, name, url;

    public AdornmentPrototype(string type, string url)
    {
        this.name = name;
        this.url = url;
    }

    public AdornmentPrototype()
    {
    }
}


public class BackhandItem : Adornment
{
}
public class Bangs : Adornment
{
}
public class Facial : Adornment
{
}
public class Hair : Adornment
{
}
public class Hat : Adornment
{
}
public class Helmet : Adornment
{
}
public class Makeup : Adornment
{
}
public class Marks : Adornment
{
}
public class Overpants : Adornment
{
}
public class Overshirt : Adornment
{
}
public class Pack : Adornment
{
}
public class Pants : Adornment
{
}
public class Shirt : Adornment
{
}
public class Shoes : Adornment
{
}
public class WristFront : Adornment
{
}

[Serializable()]
public class Adornment  
{
    public string asset_bundle_url;
    public string name;
    public bool isAvailable = true;

}




public class Item
{
    int item_id;
    string name, icon, representation, isSpecial, equipped;

    public Item(int item_id, string name, string icon, string equipped, string isSpecial, string representation)
    {
        this.item_id = item_id;
        this.name = name;
        this.icon = icon;
        this.equipped = equipped;
        this.isSpecial = isSpecial;
        this.representation = representation;
    }

    public void Print()
    {
        Debug.Log("Item ID: " + item_id + ", Name:" + name + ", Icon:" + icon + ", Equipped:" + equipped + ", isSpecial:" + isSpecial + ", Rsp:" + representation);
    }
}




//public class StateData
//{
//    public List<SceneState> states;
//
//    public StateData()
//    {
//        states = new List<SceneState>();
//    }
//
//    public void addState(SceneState state)
//    {
//        states.Add(state);
//    }
//
//    public GSRequestData ToGSData()
//    {
//        GSRequestData data = new GSRequestData();
//
//        List<GSData> dataList = new List<GSData>();
//
//        foreach (SceneState state in states)
//        {
//            dataList.Add(state.ToGSData());
//        }
//
//        data.AddObjectList("list", dataList);
//
//        return data;
//    }
//
//    public void Print()
//    {
//        foreach (SceneState state in states)
//        {
//            state.Print();
//        }
//    }
//}

//
//public abstract class SceneState
//{
//    public string type;
//
//    public abstract GSRequestData ToGSData();
//    public abstract void Print();
//}
//
//public class PositionState : SceneState
//{
//    string direction;
//    int lastx, lasty;
//
//    public PositionState(string type, string direction, int lastx, int lasty)
//    {
//        this.type = type;
//        this.direction = direction;
//        this.lastx = lastx;
//        this.lasty = lasty;
//    }
//
//    public override void Print()
//    {
//        Debug.Log("Type:" + type + ", Direction:" + direction + ", LastX:" + lastx + ", LastY:" + lasty);
//    }
//
//    public override GSRequestData ToGSData()
//    {
//        GSRequestData data = new GSRequestData();
//        data.AddString("type", this.type);
//        data.AddString("direction", this.direction);
//        data.AddNumber("lastx", this.lastx);
//        data.AddNumber("lasty", this.lasty);
//        Debug.Log(data.JSON);
//        return data;
//    }
//}
//
//public class LockState : SceneState
//{
//    string state;
//    public LockState(string type, string state)
//    {
//        this.type = type;
//        this.state = state;
//    }
//
//    public override void Print()
//    {
//        Debug.Log("Type:" + type + ", State:" + this.state);
//    }
//
//    public override GSRequestData ToGSData()
//    {
//        GSRequestData data = new GSRequestData();
//        data.AddString("type", this.type);
//        data.AddString("state", this.state);
//        Debug.Log(data.JSON);
//        return data;
//    }
//}


public class Island
{
    string name, description;
    int island_id, initial_scene_id;
    string[] urls;
    Gate[] gates;

    public Island(int island_id, string name, string description, Gate[] gates, string[] urls)
    {
        this.island_id = island_id;
        this.name = name;
        this.description = description;
        this.gates = gates;
        this.urls = urls;
    }

    public void Print()
    {
        Debug.Log("Island ID:" + island_id + ", Name:" + name + ", Desc:" + description + ", urls:" + urls.Length + ", Gates:" + gates.Length);
    }



    public class Gate
    {
        string gate_type, start_date, end_date, product_id;
        int min_level, max_level;

        public Gate(string gate_type, string start_date, string end_date, int min_level, int max_level, string product_id)
        {
            this.gate_type = gate_type;
            this.start_date = start_date;
            this.end_date = end_date;
            this.min_level = min_level;
            this.max_level = max_level;
            this.product_id = product_id;
        }

        public void Print()
        {
            Debug.Log("Type:" + gate_type + ", Start:" + start_date + ", End:" + end_date + ", Min:" + min_level + ", Max:" + max_level + ", Product ID:" + product_id);
        }
    }
}

public class Character
{
    int level, experience;
    string character_id, name, gender;

    public Character(string character_id, int level, int experience, string name, string gender)
    {
        this.character_id = character_id;
        this.level = level;
        this.experience = experience;
        this.name = name;
        this.gender = gender;
    }

    public void Print()
    {
        Debug.Log("ID:" + character_id + ", Name:" + name + ", Gender:" + gender + ", Level:" + level + ", XP:" + experience);
    }
}


public class InboxMessage
{
    string messageId, senderName, senderID, header, body;
    GSData payload;

    public InboxMessage(string _messageId, string _senderName, string _senderID, string _header, string _body, GSData _payload)
    {
        this.messageId = _messageId;
        this.senderName = _senderName;
        this.senderID = _senderID;
        this.header = _header;
        this.body = _body;
        this.payload = _payload;
    }

    public void Print()
    {
        Debug.Log("Message ID: " + messageId + ", Header: " + header + ", Body: " + body + ", Sender ID: " + senderID + ", Sender Name: " + senderName + ", Payload:" + payload.JSON);
    }
}

[Serializable()]
public class QuestData
{
    public QuestData()
    {
    }

    public QuestData(string questID, string name, string descrp, bool isActive, bool isComplete)
    {
        this.questID = questID;
        this.name = name;
        this.description = descrp;
        this.isActive = isActive;
        this.isComplete = isComplete;
    }

    public void SetStages(List<StageData> stages)
    {
        this.stages = stages;
    }

    public void SetRewards(List<string> rewards)
    {
        this.rewards = rewards;
    }


    public string questID;
    [NonSerialized]
    public string name;
    public string description;
    public bool isActive;
    public List<StageData> stages;
    public List<string> rewards;
    public bool isComplete;



    public void Print()
    {
        Debug.Log("Quest ID:"+questID+", Name:"+name+", isActive:"+isActive+", Complete:"+isComplete);
        Debug.Log("Description:"+description);
        if(stages != null)
        {
            foreach(StageData stage in stages)
            {
                Debug.Log("Stage:"+stage.stageID+", Next Stage:"+stage.nextStageID+", isActive:"+stage.isActive+", Initial:"+stage.isInitial+",isComplete:"+stage.isComplete);
                if(stage.rewards != null)
                {
                    foreach(string reward in stage.rewards)
                    {
                        Debug.Log("Stage Reward:"+reward);
                    }
                }
                if(stage.steps != null)
                {
                    foreach(QuestStep step in stage.steps)
                    {
                        Debug.Log("Steps, ID:"+step.stepID+", ObjectID:"+step.objectID+", ObjectiveID:"+step.objectiveID+", Complete:"+step.isComplete+", Mandatory"+step.mandatory);
                    }
                }
            }   
        }
        if(rewards != null)
        {
            foreach(string reward in rewards)
            {
                Debug.Log("Reward:"+reward);
            }
        }
    }
}

[Serializable()]
public class StageData
{
    public StageData()
    {
    }


    public void SetSteps(List<QuestStep> steps)
    {
        this.steps = steps;
    }

    public void SetRewards(List<string> rewards)
    {
        this.rewards = rewards;
    }

    public StageData(string stageID, string nextStageID, string name, bool isActive, bool isComplete, bool isInitial)
    {
        this.stageID = stageID;
        this.nextStageID = nextStageID;
        this.name = name;
        this.isActive = isActive;
        this.isComplete = isComplete;
        this.isInitial = isInitial;
    }

    public string stageID;
    public string nextStageID;
    public string name;
    public bool isActive;
    public List<QuestStep> steps;
    public List<string> rewards;
    public bool isComplete;
    public bool isInitial;
}

[Serializable()]
public class QuestStep 
{

    public QuestStep()
    {
    }

    public QuestStep(string objectiveID, int count, string stepID, string stepType, bool mandatory, bool isComplete)
    {
        this.objectiveID = objectiveID;
        this.count = count;
        this.stepID = stepID;
        this.mandatory = mandatory;
        this.isComplete = isComplete;
        this.stepType = stepType;
    }

    public QuestStep(string objectId, string stepID, string stepType, bool mandatory, bool isComplete)
    {
        
        this.objectID = objectId;
        this.stepID = stepID;
        this.mandatory = mandatory;
        this.isComplete = isComplete;
        this.stepType = stepType;
    }

    public string objectiveID;
    public string objectID;
    public int count;
    public string stepID;
    public string stepType;
    public bool mandatory;
    public bool isComplete = false;

}


public class ThingA
{
    public List<string> myStringyList;

    public override string ToString(){
        if(myStringyList != null)
        {
            Debug.Log("myStringyList -> "+myStringyList.Count);
        }
        return null;
    }
}

public class ThingB
{
    public bool isBool;
    public string isString;
    public int isInt;

    public override string ToString(){
        Debug.Log("isBool:"+isBool+", isString:"+isString+", isInt:"+isInt);
        return null;
    }
}

public class ThingC
{
    public string[] myStringyArray;

    public override string ToString(){
        if(myStringyArray != null)
        {
            Debug.Log("myStringyArray -> "+myStringyArray.Length);
        }
        return null;
    }
}
    

public class ThingE
{
    public int[]  lotsOfInts;

    public override string ToString(){
        if(lotsOfInts != null)
        {
            Debug.Log("lotsOfInts -> "+lotsOfInts.Length);
        }
        return null;
    }
}

public class ThingF
{
    public float myFloat = 1.040404040f;
    public float[]  lotsOfFloats;

    public override string ToString(){
        if(lotsOfFloats != null)
        {
            Debug.LogError("lotsOfFloats -> "+lotsOfFloats.Length);
        }
        Debug.LogError("OVERRIDE");
        return null;
    }
}

public class SceneState
{
    public int island_id;
    public int scene_id;
    public string character_id;
    public Dictionary<string, object> states;

    public void Print()
    {
        Debug.Log("Char_id:"+character_id+", Island_id:"+island_id+", Scene_id:"+scene_id);
        if(states != null)
        {
            foreach(var elem in states)
            {
                Debug.LogWarning(elem.Value);
            }
        }
    }

    public SceneState(string character_id, int island_id, int scene_id)
    {
        this.character_id = character_id;
        this.island_id =  island_id;
        this.scene_id = scene_id;
    }

    public SceneState()
    {
    }

    public SceneState(string character_id, int island_id, int scene_id, Dictionary<string, object> states)
    {
        this.character_id = character_id;
        this.island_id = island_id;
        this.scene_id = scene_id;
        this.states = states;
    }
}