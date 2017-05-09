// mu (myu) Max-Unity Interoperability Toolkit
// Ivica Ico Bukvic <ico@vt.edu> <http://ico.bukvic.net>
// Ji-Sun Kim <hideaway@vt.edu>
// Keith Wooldridge <kawoold@vt.edu>
// With thanks to Denis Gracanin

// Virginia Tech Department of Music
// DISIS Interactive Sound & Intermedia Studio
// Collaborative for Creative Technologies in the Arts and Design

// Copyright DISIS 2008.
// mu is distributed under the GPL license v3 (http://www.gnu.org/licenses/gpl.html)
// modified by Benedict Carey 2016 for use in SpectraScore VR

using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;



public class JitReceive : MonoBehaviour {

	public int portNo;
    public GameObject Scripts;
    public GameObject rollercoaster;
	public int maxObjects;
	public int pReps = 1;
	public int nPlanetOffset = 0;
	public GameObject Sphere1;

	//struct for pointing to various objects
	private struct objectList {
		public string objName;
		public GameObject objPointer;

		public void set(string s, GameObject gp) {
			objName = s;
			objPointer = gp;
		}
	}
	private int numObjects;
	private objectList[] o;

	//custom calls pointer
	JitCustomEvents jitCustom;

	private TcpClient incoming_client;
	private NetworkStream netStream;
	private TcpListener server;
	private bool waiting;
	public string wSphere;

	// Use this for initialization
	void Start () {
		if (portNo == 0) portNo = 32005;
		if (maxObjects == 0) maxObjects = 1024;
		waiting = false;
		server = new TcpListener(IPAddress.Any, portNo);
		server.Start();
		numObjects = 0;
		o = new objectList[maxObjects];
		jitCustom = (JitCustomEvents)GetComponent("JitCustomEvents");
	}
	
	// Update is called once per frame
	void Update () {
		string s;
		string[] values;

		if (server.Pending()) {
			incoming_client = server.AcceptTcpClient();
			netStream = incoming_client.GetStream();

			waiting = true;
		}
		while (waiting && netStream.DataAvailable) {
			try {
				int numread = 0;
				byte[] tmpbuf = new byte[1024];
				numread = netStream.Read(tmpbuf, 0, tmpbuf.Length);

				s = Encoding.ASCII.GetString(tmpbuf, 0, numread);
				s = s.Replace("\n","");
				values = s.Split(';');

				if (values.Length > 1) {
					for (int i = 0; i < (values.Length-1); i++) {
						Parse(values[i]);
					}
				}
				else Parse(values[0]);
			}
			//Called when netStream fails to read from the stream.
			catch (IOException e) {
				waiting = false;
				netStream.Close();
				incoming_client.Close();
			}
			//Called when netStream has been closed already.
			catch (ObjectDisposedException e) {
				waiting = false;
				incoming_client.Close();
			}
		}
	}
		
	void Parse(string toParse) {

		GameObject target = null;
		bool found = false;
		string[] values = toParse.Split (' ');

		Debug.Log (values [2]);

        if (values[1] == "1")
        {
            Scripts.GetComponent<loadLevel>().reLoad();
        }
        else if (values[0] == "2")
        {
            Scripts.GetComponent<InstantiateTrack>().numberOfNodes = int.Parse(values[1]);
            Scripts.GetComponent<InstantiateTrack>().minX = int.Parse(values[2]);
            Scripts.GetComponent<InstantiateTrack>().maxX = int.Parse(values[3]);
            Scripts.GetComponent<InstantiateTrack>().minY = int.Parse(values[4]);
            Scripts.GetComponent<InstantiateTrack>().maxY = int.Parse(values[5]);
            Scripts.GetComponent<InstantiateTrack>().PlaceNodes();
        }
        else if (values[1] == "3")
        {
            rollercoaster.GetComponent<RollerCoasterPlanes>().enabled = true;
        }
    }
	void custom(GameObject tgt, string method, string[] val) {
		int sz = val.Length;
		float[] param = new float[sz];
		for (int i = 0; i < val.Length; i++) {
			param[i] = (float)System.Convert.ToDouble(val[i]);
		}
		jitCustom.run (tgt, System.Convert.ToInt32(method), param);
	}

	void scale(GameObject tgt, string xVal, string yVal, string zVal) {
		Vector3 newScale = new Vector3((float)System.Convert.ToDouble(xVal),
			(float)System.Convert.ToDouble(yVal), (float)System.Convert.ToDouble(zVal));
		tgt.transform.localScale += newScale;		
	}

	void absoluteScale(GameObject tgt, string xVal, string yVal, string zVal) {
		Vector3 newScale = new Vector3((float)System.Convert.ToDouble(xVal),
			(float)System.Convert.ToDouble(yVal), (float)System.Convert.ToDouble(zVal));
		tgt.transform.localScale = newScale;
	}

	void reposition(GameObject tgt, string xLoc, string yLoc, string zLoc) {
		Vector3 newLoc = new Vector3((float)System.Convert.ToDouble(xLoc),
			(float)System.Convert.ToDouble(yLoc), -(float)System.Convert.ToDouble(zLoc));
		tgt.transform.position = newLoc;
	}

	void move(GameObject tgt, string xVal, string yVal, string zVal) {
		tgt.transform.Translate((float)System.Convert.ToDouble(xVal), 
			(float)System.Convert.ToDouble(yVal), -(float)System.Convert.ToDouble(zVal));
	}

	void rotate(GameObject tgt, string xVal, string yVal, string zVal) {

		tgt.transform.Rotate((float)System.Convert.ToDouble(xVal), 
			(float)System.Convert.ToDouble(yVal), (float)System.Convert.ToDouble(zVal));
	}

	void absoluteRotate(GameObject tgt, string xVal, string yVal, string zVal) {
		float toX = (float)System.Convert.ToDouble(xVal);
		float toY = (float)System.Convert.ToDouble(yVal);
		float toZ = (float)System.Convert.ToDouble(zVal);
		Quaternion rot = Quaternion.identity;
		rot.eulerAngles = new Vector3(toX, 180-toY, toZ);
		tgt.transform.rotation = rot;
	}
}
