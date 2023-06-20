using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        ConnectedToServer();
    }
    
    private void ConnectedToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        // Debug.Log("Try To Connect To Server...");
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        PhotonNetwork.JoinOrCreateRoom("DefaultRoom", roomOptions, TypedLobby.Default);
        // Debug.Log("Connected To Server.");
    }

    public override void OnCreatedRoom()
    {
        
        // Debug.Log($"Opened up a new room (named \"{PhotonNetwork.CurrentRoom.Name}\").");
        base.OnCreatedRoom();
    }

    public override void OnJoinedRoom()
    {
        // Debug.Log("Joined a room.");
        base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Debug.Log("New player joined the room");
        base.OnPlayerEnteredRoom(newPlayer);
    }
}