using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using PDollarGestureRecognizer;
using System.Runtime.CompilerServices;

public class CloudGestureRecognizerSprite : MonoBehaviour {

	public Transform gestureOnScreenPrefab;
	public GameObject stylus;
	private bool isDrawing = false;

	private List<Gesture> trainingSet = new List<Gesture>();

	private List<Point> points = new List<Point>();
	private int strokeId = -1;

	private Vector3 virtualKeyPosition = Vector2.zero;
	private Rect drawArea;
	private BoxCollider drawSpace;

	private RuntimePlatform platform;

	private bool recognized;

	void Start () {

		drawSpace = GameObject.Find("GestureArea").GetComponent<BoxCollider>();
		
		//Load pre-made gestures
		TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM");
		foreach (TextAsset gestureXml in gesturesXml)
		{
			Debug.Log("BORK "+gestureXml.name);
			trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));
		}
/*
		//Load user custom gestures
		string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.xml");
		foreach (string filePath in filePaths)
			trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));
*/
	}

	public void SetDrawing(bool drawing)
	{
		isDrawing = drawing;
		if(!drawing)
		{
            recognized = true;

            Gesture candidate = new Gesture(points.ToArray());
            Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

            Debug.Log(gestureResult.GestureClass + " " + gestureResult.Score);
        }
	}

	public void SaveGesture()
	{
		string newGestureName = "shmendrick";
        string fileName = String.Format("{0}/{1}-{2}.xml", Application.persistentDataPath, newGestureName, DateTime.Now.ToFileTime());

        GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
        Debug.Log("SAVING "+fileName);
        trainingSet.Add(new Gesture(points.ToArray(), newGestureName));

        newGestureName = "";
    }

	void Update () {

		virtualKeyPosition = new Vector3(stylus.transform.position.x, stylus.transform.position.y, stylus.transform.position.z);

		if (drawSpace.bounds.Contains(virtualKeyPosition)) {

			if (isDrawing) {

				if (recognized) {

					recognized = false;
					strokeId = -1;

					points.Clear();
				}

				++strokeId;

				points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, -virtualKeyPosition.z, strokeId));
			}
		}
	}
}
