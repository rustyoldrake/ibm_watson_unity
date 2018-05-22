/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;
using System.Collections.Generic;
using UnityEngine.UI;

public class ExampleStreaming : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [SerializeField]
    private string _username;
    [SerializeField]
    private string _password;
    [SerializeField]
    private string _url;
    #endregion

    public Text ResultsField;

    private int _recordingRoutine = 0;
    private string _microphoneID = null;
    private AudioClip _recording = null;
    private int _recordingBufferSize = 1;
    private int _recordingHZ = 22050;

    private SpeechToText _speechToText;



	public MeshRenderer octopusMeshRenderer;

	public Material color_red;
	public Material color_green;
	public Material color_blue;
	public Material color_yellow;
	public Material color_white;

	public float speed;
	public float roto;
	public float moto_x;
	public float moto_y;




    void Start()
    {
        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        _speechToText = new SpeechToText(credentials);
        Active = true;

        StartRecording();


		transform.position = new Vector3(0.0f, 0.0f, 0.0f);
		roto = 0.0f;
		moto_x = 0.0f;
		moto_y = 0.0f;
		Vector3 orginalPosition = octopusMeshRenderer.transform.position; // for reset



    }

    public bool Active
    {
        get { return _speechToText.IsListening; }
        set
        {
            if (value && !_speechToText.IsListening)
            {
                _speechToText.DetectSilence = true;
                _speechToText.EnableWordConfidence = true;
                _speechToText.EnableTimestamps = true;
                _speechToText.SilenceThreshold = 0.01f;
                _speechToText.MaxAlternatives = 0;
                _speechToText.EnableInterimResults = true;
                _speechToText.OnError = OnError;
                _speechToText.InactivityTimeout = -1;
                _speechToText.ProfanityFilter = false;
                _speechToText.SmartFormatting = true;
                _speechToText.SpeakerLabels = false;
                _speechToText.WordAlternativesThreshold = null;
                _speechToText.StartListening(OnRecognize, OnRecognizeSpeaker);
            }
            else if (!value && _speechToText.IsListening)
            {
                _speechToText.StopListening();
            }
        }
    }

    private void StartRecording()
    {
        if (_recordingRoutine == 0)
        {
            UnityObjectUtil.StartDestroyQueue();
            _recordingRoutine = Runnable.Run(RecordingHandler());
        }
    }

    private void StopRecording()
    {
        if (_recordingRoutine != 0)
        {
            Microphone.End(_microphoneID);
            Runnable.Stop(_recordingRoutine);
            _recordingRoutine = 0;
        }
    }

    private void OnError(string error)
    {
        Active = false;

        Log.Debug("ExampleStreaming.OnError()", "Error! {0}", error);
    }

    private IEnumerator RecordingHandler()
    {
        Log.Debug("ExampleStreaming.RecordingHandler()", "devices: {0}", Microphone.devices);
        _recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);
        yield return null;      // let _recordingRoutine get set..

        if (_recording == null)
        {
            StopRecording();
            yield break;
        }

        bool bFirstBlock = true;
        int midPoint = _recording.samples / 2;
        float[] samples = null;

        while (_recordingRoutine != 0 && _recording != null)
        {
            int writePos = Microphone.GetPosition(_microphoneID);
            if (writePos > _recording.samples || !Microphone.IsRecording(_microphoneID))
            {
                Log.Error("ExampleStreaming.RecordingHandler()", "Microphone disconnected.");

                StopRecording();
                yield break;
            }

            if ((bFirstBlock && writePos >= midPoint)
              || (!bFirstBlock && writePos < midPoint))
            {
                // front block is recorded, make a RecordClip and pass it onto our callback.
                samples = new float[midPoint];
                _recording.GetData(samples, bFirstBlock ? 0 : midPoint);

                AudioData record = new AudioData();
				record.MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples));
                record.Clip = AudioClip.Create("Recording", midPoint, _recording.channels, _recordingHZ, false);
                record.Clip.SetData(samples, 0);

                _speechToText.OnListen(record);

                bFirstBlock = !bFirstBlock;
            }
            else
            {
                // calculate the number of samples remaining until we ready for a block of audio, 
                // and wait that amount of time it will take to record.
                int remaining = bFirstBlock ? (midPoint - writePos) : (_recording.samples - writePos);
                float timeRemaining = (float)remaining / (float)_recordingHZ;

                yield return new WaitForSeconds(timeRemaining);
            }

        }

        yield break;
    }

    private void OnRecognize(SpeechRecognitionEvent result, Dictionary<string, object> customData)
    {
        if (result != null && result.results.Length > 0)
        {
            foreach (var res in result.results)
            {
                foreach (var alt in res.alternatives)
                {
                    string text = string.Format("{0} ({1}, {2:0.00})\n", alt.transcript, res.final ? "Final" : "Interim", alt.confidence);
                    Log.Debug("ExampleStreaming.OnRecognize()", text);
                    ResultsField.text = text;


					if (alt.transcript.Contains("green") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
					{
						octopusMeshRenderer.material = color_green;
					}

					if (alt.transcript.Contains("red") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
					{
						octopusMeshRenderer.material = color_red;
					}

					if (alt.transcript.Contains("blue") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
					{
						octopusMeshRenderer.material = color_blue;
					}
					if (alt.transcript.Contains("yellow") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
					{
						octopusMeshRenderer.material = color_yellow;
					}

					if (alt.transcript.Contains("rotate") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
					{
						roto = 1.0f; 
						octopusMeshRenderer.transform.Rotate(Time.deltaTime, roto, 0);
						//octopusMeshRenderer.transform.Translate(moto_y, 0,0);

						//roto = -1.0f; 

					}

					if (alt.transcript.Contains("stop") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
					{
						roto = 0.0f; 
						octopusMeshRenderer.transform.Rotate(Time.deltaTime, roto, 0);
						//octopusMeshRenderer.transform.Translate(moto_y, 0,0);
					}

					if (alt.transcript.Contains("reset") && ResultsField.text.Contains("Final")) // needs to be final or ECHO happens
					{
						octopusMeshRenderer.transform.localPosition = new Vector3(0, 0, 0);

						//gameObject.transform.Rotate(0, -90, 0);  // works 
						octopusMeshRenderer.transform.Rotate(0, -90, 0);  // works (ish)

						roto = 0.0f;
						moto_x = 0.0f;
						moto_y = 0.0f;

					}





                }

                if (res.keywords_result != null && res.keywords_result.keyword != null)
                {
                    foreach (var keyword in res.keywords_result.keyword)
                    {
                        Log.Debug("ExampleStreaming.OnRecognize()", "keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time);
                    }
                }

                if (res.word_alternatives != null)
                {
                    foreach (var wordAlternative in res.word_alternatives)
                    {
                        Log.Debug("ExampleStreaming.OnRecognize()", "Word alternatives found. Start time: {0} | EndTime: {1}", wordAlternative.start_time, wordAlternative.end_time);
                        foreach(var alternative in wordAlternative.alternatives)
                            Log.Debug("ExampleStreaming.OnRecognize()", "\t word: {0} | confidence: {1}", alternative.word, alternative.confidence);
                    }
                }
            }
        }
    }

    private void OnRecognizeSpeaker(SpeakerRecognitionEvent result, Dictionary<string, object> customData)
    {
        if (result != null)
        {
            foreach (SpeakerLabelsResult labelResult in result.speaker_labels)
            {
                Log.Debug("ExampleStreaming.OnRecognize()", string.Format("speaker result: {0} | confidence: {3} | from: {1} | to: {2}", labelResult.speaker, labelResult.from, labelResult.to, labelResult.confidence));
            }
        }
    }

	// Update is called once per frame
	void Update () {


		//float step = speed * Time.deltaTime;
		//transform.position = Vector3.MoveTowards(transform.position, target.position, step);

		/// COLOR
		if (Input.GetKeyDown (KeyCode.R)) {
			octopusMeshRenderer.material = color_red;
		}
		if (Input.GetKeyDown (KeyCode.G)) {
			octopusMeshRenderer.material = color_green;
		}
		if (Input.GetKeyDown (KeyCode.B)) {
			octopusMeshRenderer.material = color_blue;
		}
		if (Input.GetKeyDown (KeyCode.Y)) {
			octopusMeshRenderer.material = color_yellow;
		}



		/// MOVEMENT


		/// Navigation
		/// https://www.youtube.com/watch?v=mC9BfAqwU2Q
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");
		Vector3 direction = new Vector3 (vertical, 0, horizontal);
		octopusMeshRenderer.transform.Translate (direction.normalized * Time.deltaTime * speed);

		/// Rotataion Update
		octopusMeshRenderer.transform.Rotate(Time.deltaTime, roto, 0);
		octopusMeshRenderer.transform.Translate(moto_y, 0,0);


		// Rotate the object around its local X axis at 1 degree per second
		//transform.Rotate(Time.deltaTime, 0, 0);
		// ...also rotate around the World's Y axis
		//transform.Rotate(0, Time.deltaTime, 0, Space.World);


		// Keystrokes to toggle on/off rotation
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			roto = 1.0f; 
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			roto = 0.0f;
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			roto = -1.0f;
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			moto_y = 0.1f;
		}		
		if (Input.GetKeyDown (KeyCode.C)) {
			moto_y = -0.1f;
		}

		/// REset octopus
		if (Input.GetKeyDown (KeyCode.Space)) {


			octopusMeshRenderer.transform.localPosition = new Vector3(0, 0, 0);

			//gameObject.transform.Rotate(0, -90, 0);  // works 
			octopusMeshRenderer.transform.Rotate(0, -90, 0);  // works (ish)

			roto = 0.0f;
			moto_x = 0.0f;
			moto_y = 0.0f;

		}
	} // update

}
