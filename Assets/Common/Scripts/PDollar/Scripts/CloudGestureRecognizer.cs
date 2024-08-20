using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using PDollarGestureRecognizer;
using System.Runtime.CompilerServices;

public class CloudGestureRecognizer : MonoBehaviour {

	public Transform gestureOnScreenPrefab;
	public GameObject stylus;
	private bool gestureStarted = false;
	private bool isDrawing = false;

	private List<Gesture> trainingSet = new List<Gesture>();

	private List<Point> points = new List<Point>();
	private int strokeId = -1;

	private Vector3 virtualKeyPosition = Vector2.zero;
	private Rect drawArea;
	private BoxCollider drawSpace;

	private RuntimePlatform platform;
	private int vertexCount = 0;

	private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();
	private LineRenderer currentGestureLineRenderer;

	private bool recognized;

	void Start () {

		drawArea = new Rect(0, 0, Screen.width - Screen.width / 3, Screen.height);
		drawSpace = GameObject.Find("GestureArea").GetComponent<BoxCollider>();
		
		//Load pre-made gestures
		TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");
		foreach (TextAsset gestureXml in gesturesXml)
        {
            Debug.Log("Gesture Loaded: " + gestureXml.name);
            trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));
        }

        //Load user custom gestures
		/*
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.xml");
		foreach (string filePath in filePaths)
			trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));
		*/
	}

    public void StartGesture()
    {
        Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation) as Transform;
        currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();

        gestureLinesRenderer.Add(currentGestureLineRenderer);

        vertexCount = 0;

		Debug.Log("Started Gesture");
		gestureStarted = true;
    }

	public void FinishGesture()
	{
		if (gestureStarted)
		{
			Debug.Log("Gesture Finished - Checking...");
			gestureStarted = false;
			recognized = true;

			Gesture candidate = new Gesture(points.ToArray());
			Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

			string message = gestureResult.GestureClass + " " + gestureResult.Score;

			strokeId = -1;
			points.Clear();

			Debug.Log(message);
		}
    }

	public void StartDrawing()
	{
		if (gestureStarted)
		{
			isDrawing = true;

			++strokeId;

			Debug.Log("Drawing Stroke " + strokeId);

			Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation) as Transform;
			currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();

			gestureLinesRenderer.Add(currentGestureLineRenderer);

			vertexCount = 0;
		}
    }

    public void FinishDrawing()
	{
		if (isDrawing)
		{
			Debug.Log("Finished Stroke " + strokeId);
			isDrawing = false;
		}
    }

	void Update () {

		virtualKeyPosition = new Vector3(stylus.transform.position.x, stylus.transform.position.y, stylus.transform.position.z);

		if (drawSpace.bounds.Contains(virtualKeyPosition)) {

			if (isDrawing) {
				points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, -virtualKeyPosition.z, strokeId));

                var spritePoints = new Vector3[currentGestureLineRenderer.positionCount];
                currentGestureLineRenderer.positionCount += ++vertexCount;


				currentGestureLineRenderer.SetPositions(spritePoints);
			}
		}
	}
}
