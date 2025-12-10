using Unity.Netcode;

using UnityEngine;



public class NetworkUI : MonoBehaviour

{

    void OnGUI()

    {

        //接続切ったあとは何もしない

        if (NetworkManager.Singleton == null)
        {

            return;

        }



        //まだホストでもクライアントでもない（ゲームが始まってない）

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)

        {

            if (GUILayout.Button("ホスト"))

            {

                //ホストとしてゲーム開始

                NetworkManager.Singleton.StartHost();

            }



            if (GUILayout.Button("クライアント"))

            {

                //クライアントとして接続

                NetworkManager.Singleton.StartClient();

            }

        }

    }

}