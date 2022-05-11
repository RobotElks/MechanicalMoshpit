//using UnityEngine;
//using UnityEngine.Networking;
//using UnityEngine.Networking.Match;
//namespace Unity.Netcode
//{
//    public class CustomManager : NetworkManager
//    {

//        // Server callbacks
//        public override void OnServerConnect(NetworkConnection conn)
//        {
//            Debug.Log("A client connected to the server: " + conn);
//        }

//        public override void OnServerDisconnect(NetworkConnection conn)
//        {
//            NetworkServer.DestroyPlayersForConnection(conn);
//            if (conn.lastError != NetworkError.Ok)
//            {
//                if (LogFilter.logError) { Debug.LogError("ServerDisconnected due to error: " + conn.lastError); }
//            }
//            Debug.Log("A client disconnected from the server: " + conn);
//        }

//    }
//}
