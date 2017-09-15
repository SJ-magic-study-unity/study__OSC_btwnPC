/************************************************************
************************************************************/
#define USE_QUEUE

/************************************************************
************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityOSC; // SJ


/************************************************************
************************************************************/

public class OSCController : MonoBehaviour {
	/****************************************
	****************************************/
	// params for mac.
	public string serverId = "win";
	public string Ip_SendTo = "10.0.0.3";
	public int Port_SendTo = 12345;
	
	public string clientId = "mac";
	public int ReceivePort = 12345;
	
	// params for win.
	/*
	public string serverId = "mac";
	public string Ip_SendTo = "10.0.0.2";
	public int Port_SendTo = 12345;
	
	public string clientId = "win";
	public int ReceivePort = 12345;
	*/

	/* */
	public KeyCode debugKey = KeyCode.S;
	public string  debugMessage = "/mouse";
	
	private long latestTimeStamp = 0;
	private string label = "saijo";
	
#if USE_QUEUE
	private Queue queue;
#endif	
	
	/****************************************
	****************************************/
	
	// Use this for initialization
	void Start () {
		OSCHandler.Instance.Init(this.clientId, this.Ip_SendTo, this.Port_SendTo, this.serverId, this.ReceivePort);
		
#if USE_QUEUE
		queue = new Queue();
        queue = Queue.Synchronized(queue);

        // パケット受信時のイベントハンドラを登録
        OSCHandler.Instance.PacketReceivedEvent += OnPacketReceived_;
#endif

	}
	
#if USE_QUEUE
	void OnPacketReceived_(OSCServer server, OSCPacket packet) {
		/********************
		for safety
		********************/
		const int QueueLimitSize = 100;
		if(QueueLimitSize < queue.Count){
			queue.Clear();
		}
			
		/********************
		********************/
        queue.Enqueue(packet);
    }
#endif

	// Update is called once per frame
	void Update () {
		/********************
		send
		********************/
		// if (Input.GetMouseButtonDown(0))
		/*
		if (Input.GetKeyDown(this.debugKey))
		{
			Debug.Log("SendMessage");
			
			var sampleVals = new List<int>(){1, 2, 3}; // 2つ以上のparameterを送信(型は同じである必要あり)
			OSCHandler.Instance.SendMessageToClient(this.clientId, debugMessage, sampleVals);
		}
		*/
		
		// var sampleVals = new List<float>(){Input.mousePosition.x, Input.mousePosition.y}; // 2つ以上のparameterを送信(型は同じである必要あり)
		var sampleVals = new List<float>(){(int)(Input.mousePosition.x), (int)(Input.mousePosition.y)}; // 2つ以上のparameterを送信(型は同じである必要あり)
		OSCHandler.Instance.SendMessageToClient(this.clientId, debugMessage, sampleVals);
		
		/********************
		receive
		********************/
#if USE_QUEUE
		while (0 < queue.Count) {
            OSCPacket packet = queue.Dequeue() as OSCPacket;
            if (packet.IsBundle()) {
                // OSCBundleの場合
                OSCBundle bundle = packet as OSCBundle;
                foreach (OSCMessage msg in bundle.Data) {
                    // メッセージの中身にあわせた処理
                }
            } else {
                // OSCMessageの場合はそのまま変換
                OSCMessage msg = packet as OSCMessage;
				
                // メッセージの中身にあわせた処理
				if(msg.Address == "/mouse"){
					label = "(" + msg.Data[0] + ", " + msg.Data[1] + ")";
					
				}else if(msg.Address == "/test"){
					Debug.Log(	"Receive : "
								+ msg.TimeStamp
								+ ", "
								+ msg.Address
								+ ", "
								+ msg.Data[0]);
				}

			}
        }
		
#else
		
		OSCHandler.Instance.UpdateLogs();
		
		foreach (KeyValuePair<string, ServerLog> item in OSCHandler.Instance.Servers)
		{
			if (item.Value.packets.Count == 0) continue; // 1つもmessage packetがない.
			
			int latestPacketIndex = item.Value.packets.Count - 1; // 複数ある時は、一番最後に積まれたpacketを使用.
			
			if (this.latestTimeStamp == item.Value.packets[latestPacketIndex].TimeStamp) continue;	// 時間が経過していない.
			this.latestTimeStamp = item.Value.packets[latestPacketIndex].TimeStamp;
			
			if(item.Value.packets[latestPacketIndex].Address == "/mouse"){
				label = "(" + item.Value.packets[latestPacketIndex].Data[0] + ", " + item.Value.packets[latestPacketIndex].Data[1] + ")";
				
			}else{
				Debug.Log(	"Receive : "
							+ item.Key // "server id" which was set at "CreateServer(serverId, clientPort);" in Init()(in OSCHandler.cs).
							+ ", "
							+ item.Value.packets.Count
							+ ", "
							+ item.Value.packets[latestPacketIndex].TimeStamp
							+ ", "
							+ item.Value.packets[latestPacketIndex].Address
							+ ", "
							+ item.Value.packets[latestPacketIndex].Data[0]);
							
				// Debug.Log(	"Receive : "
				//			+ item.Value.packets[latestPacketIndex].TimeStamp
				//			+ ","
				//			+ item.Value.packets[latestPacketIndex].Address
				//			+ ","
				//			+ item.Value.packets[latestPacketIndex].Data[0]);
			}
			
		}
		
#endif

	}
	
	void OnGUI()
	{
		GUI.color = Color.black;
		
		GUI.Label(new Rect(15, 15, 100, 30), label);
	}
}
