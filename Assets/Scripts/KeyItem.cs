using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

        if (this.name == "key1(Clone)")
        {
            GameObject.Find("Game").GetComponent<GameBoard>().board[1, 20] = this.gameObject;
        }else if(this.name == "key2(Clone)")
        {
            GameObject.Find("Game").GetComponent<GameBoard>().board[26, 19] = this.gameObject;
        }
        else if (this.name == "key3(Clone)")
        {
            GameObject.Find("Game").GetComponent<GameBoard>().board[2, 1] = this.gameObject;
        }
        else if (this.name == "key4(Clone)")
        {
            GameObject.Find("Game").GetComponent<GameBoard>().board[8, 25] = this.gameObject;
        }
        else if (this.name == "key5(Clone)")
        {
            GameObject.Find("Game").GetComponent<GameBoard>().board[21, 5] = this.gameObject;
        }

    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
