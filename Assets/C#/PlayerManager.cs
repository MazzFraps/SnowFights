using Assets.Player;
using UnityEngine;

namespace Assets
{
    public class PlayerManager : MonoBehaviour {

        public int[] playerList;
        public int playerNumber;

        public Movement player1;
        public Movement player2;
        public Movement player3;
        public Movement player4;
	

        // Use this for initialization
        void Start () {
            playerList[0] = 1;
            playerList[1] = 2;
            playerList[2] = 3;
            playerList[3] = 4;
        }
	
        // Update is called once per frame
        void Update () {
            if(Input.anyKeyDown) {
                //Debug.Log(Input.anyKey);
            }
        }

        void AddPlayer (int player) {
		
        }
    }
}
