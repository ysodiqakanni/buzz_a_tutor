﻿using bat.data;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bat
{
    public class BlackboardHub : Hub
    {
        //Connect to Group

        static List<Participant> ConnectedUsers = new List<Participant>();

        //public void JoinGroup(string groupName)
        //{
        //    Groups.Add(Context.ConnectionId, groupName);
        //    //Clients.Group(groupName).getSnapShot();
        //}

        public void JoinGroup(string groupName, string userId, string userName, string isHost, string isHaveControl)
        {
            if (ConnectedUsers.Count(u => u.UserId == userId && u.GroupName == groupName) == 0)
                ConnectedUsers.Add(new Participant()
                {
                    ConnectionId = Context.ConnectionId,
                    UserId = userId,
                    UserName = userName,
                    Status = "Online",
                    IsHost = isHost,
                    IsHaveControl = isHaveControl,
                    GroupName = groupName,
                });
            else
            {
                var index = ConnectedUsers.FindIndex(p => p.UserId == userId && p.GroupName == groupName);
                ConnectedUsers[index].ConnectionId = Context.ConnectionId;
                ConnectedUsers[index].IsHaveControl = isHaveControl;
                ConnectedUsers[index].IsHost = isHost;
                ConnectedUsers[index].Status = "Online";
            }
            Groups.Add(Context.ConnectionId, groupName);
            RefreshList();
        }

        public void RefreshList()
        {
            Clients.All.refreshList(ConnectedUsers);
        }

        public void UpdateModel(ChalkModel clientModel)
        {
            clientModel.LastUpdatedBy = Context.ConnectionId;
            // Update the Chalk model within our broadcaster
            Clients.Group(clientModel.Group, clientModel.LastUpdatedBy).UpdateChalk(clientModel);
        }

        public void UpdateBBText(textModel textModel)
        {
            textModel.LastUpdatedBy = Context.ConnectionId;
            // Update the Chalk model within our broadcaster
            Clients.Group(textModel.Group, textModel.LastUpdatedBy).BBText(textModel);
        }
        public void UpdateList(BBListUpdate Update)
        {
            // Update the Chalk model within our broadcaster
            Clients.Group(Update.Group).UpdateList(Update);
        }

        public void BoardImage(ImageModel imageModel)
        {
            Clients.Group(imageModel.Group, Context.ConnectionId).BoardImage(imageModel);
        }

        public void UploadSnapshot(string snapshot, string groupName)
        {
            var teacherConnectionId = retriveTeacherContextId(groupName);
            if (teacherConnectionId != "")
                Clients.Group(groupName, teacherConnectionId).loadSnapShot(snapshot);
        }

        public void UploadSnapshotOnInit(string snapshot, string groupName)
        {
            var teacherConnectionId = retriveTeacherContextId(groupName);
            if (teacherConnectionId != "")
                Clients.Group(groupName, teacherConnectionId).loadOnInitWithSnapShot(snapshot);
        }

        public void GetTeacherSnapshot(string groupName)
        {
            var teacherConnectionId = retriveTeacherContextId(groupName);
            if (teacherConnectionId != "")
                Clients.Client(teacherConnectionId).getTeacherSnapshot();
            else
                Clients.Client(Context.ConnectionId).loadOnInitWithoutSnapShot();
        }

        public void UploadShape(string shapeString, string previousShapeId, string groupName)
        {
            var teacherConnectionId = retriveTeacherContextId(groupName);
            if (teacherConnectionId != "")
                Clients.Group(groupName, teacherConnectionId).loadShape(shapeString, previousShapeId);
        }

        public void ClearBoard(string groupName)
        {
            var teacherConnectionId = retriveTeacherContextId(groupName);
            if (teacherConnectionId != "")
                Clients.Group(groupName, teacherConnectionId).clearBoard();
        }

        public void UndoAction(string groupName)
        {
            var teacherConnectionId = retriveTeacherContextId(groupName);
            if (teacherConnectionId != "")
                Clients.Group(groupName, teacherConnectionId).undoAction();
        }

        public void RedoAction(string groupName)
        {
            var teacherConnectionId = retriveTeacherContextId(groupName);
            if (teacherConnectionId != "")
                Clients.Group(groupName, teacherConnectionId).redoAction();
        }

        public void PanAction(string coordinates,string groupName)
        {
            var teacherConnectionId = retriveTeacherContextId(groupName);
            if (teacherConnectionId != "")
                Clients.Group(groupName, teacherConnectionId).panAction(coordinates);
        }

        public void ColorChange(string colorType, string colorValue, string groupName)
        {
            var teacherConnectionId = retriveTeacherContextId(groupName);
            if (teacherConnectionId != "")
                Clients.Group(groupName, teacherConnectionId).colorChange(colorType, colorValue);
        }

        public void AssignHandle(string Group, string connectionId,string snapshotString)
        {
            var index = ConnectedUsers.FindIndex(p => p.ConnectionId == connectionId);
            if (index != -1)
                ConnectedUsers[index].IsHaveControl = "true";
            string LastUpdatedBy = Context.ConnectionId;
            Clients.Client(connectionId).AssignHandle(snapshotString);
            RemoveOthersHandler(Group, connectionId, LastUpdatedBy, snapshotString);
            RefreshList(LastUpdatedBy);
        }

        public void RemoveOthersHandler(string Group, string connectionId, string LastUpdatedBy, string snapshotString)
        {
            //Clients.Group(Group, connectionId).RemoveHandle(snapshotString);
            var index = ConnectedUsers.FindIndex(p => p.ConnectionId == connectionId);
            for (int i = 0; i < ConnectedUsers.Count; i++)
            {
                if (i != index)
                    ConnectedUsers[i].IsHaveControl = "false";
            }
        }
        public void RemoveHandle(string Group, string connectionId,string snapshotString)
        {
            Clients.Client(connectionId).RemoveHandle(snapshotString);
            var index = ConnectedUsers.FindIndex(p => p.ConnectionId == connectionId);
            if (index != -1)
                ConnectedUsers[index].IsHaveControl = "false";
            string LastUpdatedBy = Context.ConnectionId;
            AssignHandle(Group, LastUpdatedBy, snapshotString);
            RefreshList(LastUpdatedBy);
        }

        public void RefreshList(string connectionId)
        {
            Clients.Client(connectionId).refreshList(ConnectedUsers);
        }

        private string retriveTeacherContextId(string groupName)
        {
            string teacherContextId = "";
            var index = ConnectedUsers.FindIndex(p => p.IsHost == "true" && p.GroupName == groupName);
            if (index != -1)
                teacherContextId = ConnectedUsers[index].ConnectionId;
            return teacherContextId;
        }

    }

    public class Participant
    {
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }

        public string IsHost { get; set; }

        public string IsHaveControl { get; set; }

        public string GroupName { get; set; }
    }

    public class ChalkModel
    {
        // We declare Left and Top as lowercase with 
        // JsonProperty to sync the client and server models
        [JsonProperty("oX")]
        public double oX { get; set; }
        [JsonProperty("oY")]
        public double oY { get; set; }
        [JsonProperty("nX")]
        public double nX { get; set; }
        [JsonProperty("nY")]
        public double nY { get; set; }
        [JsonProperty("lineWidth")]
        public string lineWidth { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
        [JsonProperty("group")]
        public string Group { get; set; }
        // We don't want the client to get the "LastUpdatedBy" property
        [JsonIgnore]
        public string LastUpdatedBy { get; set; }
    }
    public class textModel
    {
        [JsonProperty("oX")]
        public double oX { get; set; }
        [JsonProperty("oY")]
        public double oY { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("group")]
        public string Group { get; set; }
        [JsonIgnore]
        public string LastUpdatedBy { get; set; }
    }

    public class BBListUpdate
    {
        [JsonProperty("group")]
        public string Group { get; set; }
        [JsonProperty("update")]
        public bool Update { get; set; }
    }

    public class ImageModel
    {
        [JsonProperty("group")]
        public string Group { get; set; }
        [JsonProperty("clear")]
        public bool clear { get; set; }
        [JsonProperty("imageId")]
        public int ImageId { get; set; }
    }

    public class ChatHub : Hub
    {
        public Lesson lesson { get; set; }
        public ChatRecord chatRecord { get; set; }
        //Connect to Group
        public void JoinGroup(string groupName, string name)
        {
            Groups.Add(Context.ConnectionId, groupName);
            Save(groupName, "Classroom", name + " has joined.");
            Clients.Group(groupName).broadcastMessage("Classroom", name + " has joined.");
        }


        public void Send(MyMessage message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.Group(message.GroupName).broadcastMessage(message.Name, message.Msg);
            Save(message.GroupName, message.Name, message.Msg);
        }


        public void Save(string groupName, string name, string message)
        {
            // Save message to DB
            using (var conn = new dbEntities())
            {
                var id = Int32.Parse(groupName);

                this.lesson = conn.Lessons.FirstOrDefault(a => a.ID == id);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");

                this.chatRecord = new ChatRecord()
                {
                    Lesson_ID = id,
                    Chat_User = name,
                    Char_Message = message,
                    DateTime = DateTime.Now.ToUniversalTime(),
                };
                conn.ChatRecords.Add(this.chatRecord);
                conn.SaveChanges();
            }
        }
    }


    public class MyMessage
    {
        public string Name { get; set; }
        public string Msg { get; set; }
        public string GroupName { get; set; }
    }
}