using UnityEngine;
using System.Collections;
using GameSparks.Core;
using System;






public class AuthResponse
{
    public string[] characterIDs;
    public string lastCharacterID;
    public bool hasParentEmail;

    public AuthResponse(string[] characterIDs, string lastCharacterID, bool hasParentEmail)
    {
        this.characterIDs = characterIDs;
        this.lastCharacterID = lastCharacterID;
        this.hasParentEmail = hasParentEmail;
    }

    public void Print()
    {
        Debug.Log("Last Char:" + lastCharacterID + ", Characters:" + characterIDs.Length + ", hasParentEmail:" + hasParentEmail);
    }
}


public class ParentEmailStatus
{

    public ParentEmailStatus(string email, DateTime dateAdded, string status, DateTime verifiedDate)
    {
        this.email = email;
        this.dateAdded = dateAdded;
        this.status = status;
        this.verifiedDate = verifiedDate;
    }

    public ParentEmailStatus(string email, DateTime dateAdded, string status)
    {
        this.email = email;
        this.dateAdded = dateAdded;
        this.status = status;
    }

    string email, status;
    DateTime dateAdded, verifiedDate;

    public void Print()
    {
        Debug.Log("Email:" + email + ", Status:" + status + ", Verified:" + verifiedDate + ", Added:" + dateAdded);
    }
}


public class Adornment
{
    int adornment_id, assetbundle_id;
    string name;
    Restriction[] restrictions;

    public Adornment(int adornment_id, string name, int assetbundle_id, Restriction[] restrictions)
    {
        this.adornment_id = adornment_id;
        this.assetbundle_id = assetbundle_id;
        this.name = name;
        this.restrictions = restrictions;
    }

    public void Print()
    {
        Debug.Log("ID:" + adornment_id + ", Name:" + name + ", Bundle_ID:" + assetbundle_id + ", Restrictions:" + restrictions.Length);
    }

    public class Restriction
    {
        string restriction_type;
        int min_level, max_level;

        public Restriction(string restriction_type, int min_level, int max_level)
        {
            this.min_level = min_level;
            this.max_level = max_level;
            this.restriction_type = restriction_type;
        }

        public void Print()
        {
            Debug.Log("Type:" + restriction_type + ", Min Level:" + min_level + ", Max Level:" + max_level);
        }
    }
}

public class Outfit
{
    public int outfit_id;
    public string skin_color, hair_color;
    public string shirt, pants, hair, face_mark, helmet;

    public Outfit(int outfit_id, string skin_color, string hair_color, string shirt, string pants, string  hair, string face_mark, string helmet)
    {
        this.outfit_id = outfit_id;
        this.skin_color = skin_color;
        this.hair_color = hair_color;
        this.shirt = shirt;
        this.pants = pants;
        this.hair = hair;
        this.face_mark = face_mark;
        this.helmet = helmet;
    }

    public Outfit(string skin_color, string hair_color, string shirt, string pants, string  hair, string face_mark, string helmet)
    {
        this.skin_color = skin_color;
        this.hair_color = hair_color;
        this.shirt = shirt;
        this.pants = pants;
        this.hair = hair;
        this.face_mark = face_mark;
        this.helmet = helmet;
    }

    public void Print()
    {
        Debug.Log("ID:" + outfit_id + ", skin:" + skin_color.ToString() + ", hair:" + hair_color.ToString() + ", shirt:" + shirt + ", pants:" + pants + ", hair:" + hair + ", mask:" + face_mark + ", helmet:" + helmet);
    }
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


public class SceneState
{
    string type, direction;
    int lastx, lasty;

    public SceneState(string type, string direction, int lastx, int lasty)
    {
        this.type = type;
        this.direction = direction;
        this.lastx = lastx;
        this.lasty = lasty;
    }

    public void Print()
    {
        Debug.Log("Type:" + type + ", Direction:" + direction + ", LastX:" + lastx + ", LastY:" + lasty);
    }

    public GSRequestData ToGSData()
    {
        GSRequestData data = new GSRequestData();
        data.AddString("type", this.type);
        data.AddString("direction", this.direction);
        data.AddNumber("lastx", this.lastx);
        data.AddNumber("lasty", this.lasty);
        Debug.Log(data.JSON);
        return data;
    }
}


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