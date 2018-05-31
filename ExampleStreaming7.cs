/**
Hi. this is REALLY Rough code. Beware.  vid here; https://youtu.be/JXY_zJetxQM
*/

using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;
using System.Collections.Generic;
using UnityEngine.UI;

// For Read Write to Logs and Memory Files
using System;
using System.IO;

// For Text to speech (not sure if we should declare here or only in ETTS 
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Connection;



public class ExampleStreaming : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [SerializeField]
	private string _username_STT;
    [SerializeField]
    private string _password_STT;
    [SerializeField]
    private string _url_STT;
    #endregion

	#region PLEASE SET THESE VARIABLES IN THE INSPECTOR
	[SerializeField]
	private string _username_TTS;
	[SerializeField]
	private string _password_TTS;
	[SerializeField]
	private string _url_TTS;
	#endregion



	// https://console.bluemix.net/docs/services/text-to-speech/SSML-transformation.html#transformation

	public int voicebox_prosody_pitch; // = 200;  // HZ
	public int voicebox_pitch_range; //  = 0;  // -100% to 100%
	public int voicebox_glottal_tension; // = 0; 	  // -100% to 100%
	public int voicebox_breathiness; // = 0;  // -100% to 100%
	public int voicebox_rate = 0;  // -100% to 100%
	//public int voicebox_timbre  // "Sunrise" or "Breeze"
	public int voicebox_timbre_extent; // = 50; // -0% to 100%



    private int _recordingRoutine = 0;
    private string _microphoneID = null;
    private AudioClip _recording = null;
    private int _recordingBufferSize = 1;
    private int _recordingHZ = 22050;

    private SpeechToText _speechToText;
	private TextToSpeech _textToSpeech;

	// Used for System to update LARGE FONT Text in the UX on operation 
	public Text ResultsField;  //  Maps to Text Streaming - from Raw and Original Example file
	public Text System_Mode;  ///   System Mode - (1) Diagnostics  (2) Teach_and_Learn   (3) Factoid  (4) Conversation
	public Text Agent_Status; ///  Agent Status and Emotional state (confused, apologetic, listening)
	public Text Voicebox_Status;  // Voicebox defaults

	// To enable TTS calls from this code
	string _testString  = "<speak version=\"1.0\"><prosody pitch=\\\"250Hz\\\"><express-as type=\"GoodNews\">Great news!</express-as></prosody></speak>"; 

	string _createdCustomizationId;
	CustomVoiceUpdate _customVoiceUpdate;
	string _customizationName = "unity-example-customization";
	string _customizationLanguage = "en-US";
	string _customizationDescription = "A text to speech voice customization created within Unity.";
	string _testWord = "Watson";

	private bool _synthesizeTested = false;
	private bool _getVoicesTested = false;
	private bool _getVoiceTested = false;
	private bool _getPronuciationTested = false;
	private bool _getCustomizationsTested = false;
	private bool _createCustomizationTested = false;
	private bool _deleteCustomizationTested = false;
	private bool _getCustomizationTested = false;
	private bool _updateCustomizationTested = false;
	private bool _getCustomizationWordsTested = false;
	private bool _addCustomizationWordsTested = false;
	private bool _deleteCustomizationWordTested = false;
	private bool _getCustomizationWordTested = false;






    void Start()
    {
        
		/// TTS TEST ON START
		LogSystem.InstallDefaultReactors();
		//  Create credential and instantiate service

		Credentials credentials_TTS = new Credentials(_username_TTS, _password_TTS, _url_TTS);
		_textToSpeech = new TextToSpeech(credentials_TTS);

		_testString = "<speak version=\"1.0\"><express-as type=\"GoodNews\">Hi! I'm MacKenzie! A Unity demo for the IBM Watson Software Development Kit </express-as></speak>";
		Runnable.Run (Examples ());

		/// STT BEGIN
		Credentials credentials_STT = new Credentials(_username_STT, _password_STT, _url_STT);
		_speechToText = new SpeechToText(credentials_STT);
        Active = true;

        StartRecording();

		Agent_Status.text = "Listening";
		System_Mode.text = "mode: listen-learn";

		voicebox_prosody_pitch = 200;  // 150 hZ to 350 hz
		voicebox_pitch_range = 0;  // -100% to 100%
		voicebox_glottal_tension = 25; 	  // -100% to 100%
		voicebox_breathiness = 50;  // -100% to 100%
		voicebox_rate = 0;  // -100% to 100%
		voicebox_timbre_extent = 50; // -0% to 100%
		//voicebox_timbre  // "Sunrise" or "Breeze"



		//// Check if LOG file exists (later, we dont need to do this all the time, but here for now.  OPTIMIZE LATER
		string path = @"MacKenzie_Conversation_Log.txt";
		if (!File.Exists (path)) {
			// Create a file to write to if the file does not exist
			using (StreamWriter sw = File.CreateText (path)) {
				sw.WriteLine (string.Concat ("timestamp,","mode,","transcript"));
				sw.WriteLine (string.Concat (System.DateTime.Now.ToLongTimeString (), "-", System.DateTime.Now.ToString ("MM/dd/yyyy"), ",log_initialized,", "MacKenzie_Conversation_Log Initialized"));
			}
		}


    }


	/// TEMP CODE
	public void TalkieTalkie(){

		//Log.Debug("red", "");
		//Runnable.Run (Examples ("blue"));
	}

	public void TextTalk(string str){
		//Runnable.Run (Examples (_testString2));
		//Runnable.Run (Examples ("blue"));
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
				foreach (var alt in res.alternatives) {
					string text = string.Format ("{0} ({1}, {2:0.00})\n", alt.transcript, res.final ? "Final" : "Interim", alt.confidence);
					Log.Debug ("ExampleStreaming.OnRecognize()", text);
					ResultsField.text = text;




					///// HACK LOG INSERT BEGINS


					if (ResultsField.text.Contains ("Final")) // needs to be final or ECHO happens
					{
						
						if (alt.transcript.Contains ("echo") || alt.transcript.Contains ("repeat") )
						{
							_testString = alt.transcript;
							_testString = _testString.Replace ("echo", "");
							_testString = _testString.Replace ("repeat", "");
							Runnable.Run (Examples ());
						}


						else if (alt.transcript.Contains ("connect") && (System_Mode.text == "mode: conversation")) {
							/// Count Transcript lines

							_testString = "<speak version=\"1.0\">" +
								" No conversation endoints are currently enabled.  If you enable them, we can talk more." +  // Later - CONNECT to conversation service
								"</speak>";
							Runnable.Run (Examples ());
						}

						else if (alt.transcript.Contains ("question") && (System_Mode.text == "mode: factoid")) {
							/// Count Transcript lines

							_testString = "<speak version=\"1.0\">" +
								" No factoid endoints are currently enabled.  If you enable them, we can do some Q&A." +  // Later - CONNECT to factoid service
								"</speak>";
							Runnable.Run (Examples ());
						}


						/// DIAGNOSTIC ZONE - for checking on Agent status
						///
						else if (alt.transcript.Contains ("status") && (System_Mode.text == "mode: diagnostic")) {
							/// Count Transcript lines
							System.IO.StreamReader reader = System.IO.File.OpenText(@"MacKenzie_Conversation_Log.txt");
							int iLineCount = 0;
							while (reader.Peek() >= 0)
							{	reader.ReadLine();
								++iLineCount;  }
							reader.Close();		

							_testString = "<speak version=\"1.0\">" +
								"I am in " +
								System_Mode.text +
								" and,  " +
								"There are "+ iLineCount +
								" utterances in my transcript log." +
								" This session has been active for " +
								Mathf.Round(Time.realtimeSinceStartup) +
								" seconds." +
								" Emotionally I am feeling Neutral" +  // FOR NOW HARDWIRE, later, do a movign window of this https://www.youtube.com/watch?v=fOfFrGsNwHo
								"</speak>";
							Runnable.Run (Examples ());
						}


						else if (alt.transcript.Contains ("utterance summary") && (System_Mode.text == "mode: diagnostic")) {
							/// Count Transcript lines
							System.IO.StreamReader reader = System.IO.File.OpenText(@"MacKenzie_Conversation_Log.txt");

							// Number of lines in log
//							int iLineCount = 0;
//							while (reader.Peek() >= 0)
//							{	reader.ReadLine();
//								++iLineCount;  
//							}
//							reader.Close();		

							// Number of lines in log (merge with above later)
							int iTypeCount_ll = 0; // counter of the thing we care about
							int iTypeCount_diag = 0; // counter of the thing we care about
							int iTypeCount_conv = 0; // counter of the thing we care about
							string reader_string;  // place holder for each string from each line

							while ((reader_string = reader.ReadLine()) != null)
							{
								if (reader_string.Contains("listen-learn"))
								{
									++iTypeCount_ll;                                                        
								}
								if (reader_string.Contains("conversation"))
								{
									++iTypeCount_conv;                                                        
								}        
								if (reader_string.Contains("diagnostic"))
								{
									++iTypeCount_diag;                                                        
								}        
							} 
							reader.Close();		
						 

							_testString = "<speak version=\"1.0\">" +
								" For utterance type listen-learn, I counted " +
								iTypeCount_ll +
								" occurances." +
								" There are " +
								iTypeCount_conv +
								" conversation utterances.  There are also " +
								iTypeCount_diag +
								" dialog utterances" +
								"</speak>";
							Runnable.Run (Examples ());
						}

						// HARD WIRED FOR NOW
						// HARD WIRED FOR NOW - ICE CREAM but later Focus "FOCUS" keyword and can get Sentiment
						else if (alt.transcript.Contains ("log query") && alt.transcript.Contains ("ice cream") && (System_Mode.text == "mode: diagnostic")) {
							/// Count Transcript lines
							System.IO.StreamReader reader = System.IO.File.OpenText(@"MacKenzie_Conversation_Log.txt");
							int iTypeCount_focus = 0; // counter of the thing we care about
							string reader_string;  // place holder for each string from each line

							while ((reader_string = reader.ReadLine()) != null)
							{
								if (reader_string.Contains("ice cream"))
								{
									++iTypeCount_focus;                                                        
								}
    							// later do Sentiment by Keyword or Entity in FOCUS
							} 
							reader.Close();		


							_testString = "<speak version=\"1.0\">" +
								" For FOCUS keyword Ice Cream, I counted " +
								iTypeCount_focus +
								" occurances.  And the sentiment was very positive" +  // LATER - call Sentiment by Keyword/ENtitty (hardwired now)
								"</speak>";
							Runnable.Run (Examples ());
						}



						else if (alt.transcript.Contains ("emotional state") && (System_Mode.text == "mode: diagnostic")) {
							/// Count Transcript lines

							_testString = "<speak version=\"1.0\">" +
								" Emotionally I am feeling Neutral." +  // Later - CONNECT to emotional levels (volumetric representation of state) moving window
								"</speak>";
							Runnable.Run (Examples ());
						}


							
						// https://console.bluemix.net/docs/services/text-to-speech/SSML-elements.html#elements
						// https://console.bluemix.net/docs/services/text-to-speech/SSML-expressive.html#expressive 

						else if (alt.transcript.Contains ("voicebox") || alt.transcript.Contains ("voice box"))
						{	
							_testString = "Voice traits have been modified.";

							if ( alt.transcript.Contains ("increase") && (alt.transcript.Contains ("pitch") || alt.transcript.Contains ("frequency") ))
							{
								voicebox_prosody_pitch = voicebox_prosody_pitch + 50;  // 150 hZ to 350 hz
								voicebox_pitch_range = voicebox_pitch_range + 25;  // -100% to 100%
								
							}
							else if (alt.transcript.Contains ("increase") && alt.transcript.Contains ("rate") )
							{
								voicebox_rate = voicebox_rate + 25;  // -100% to 100%
							}
							else if (alt.transcript.Contains ("increase") && alt.transcript.Contains ("tension") )
							{
								voicebox_glottal_tension = voicebox_glottal_tension + 25; 	  // -100% to 100%
								voicebox_breathiness = voicebox_breathiness + 25;  // -100% to 100%
								voicebox_timbre_extent = voicebox_timbre_extent + 10; // -0% to 100%
							}
							else if (alt.transcript.Contains ("decrease") && (alt.transcript.Contains ("pitch") || alt.transcript.Contains ("frequency") ))
							{
								voicebox_prosody_pitch = voicebox_prosody_pitch - 50;  // 150 hZ to 350 hz
								voicebox_pitch_range = voicebox_pitch_range - 25;  // -100% to 100%

							}
							else if (alt.transcript.Contains ("decrease") && alt.transcript.Contains ("rate") )
							{
								voicebox_rate = voicebox_rate - 25;  // -100% to 100%
							}
							else if (alt.transcript.Contains ("decrease") && alt.transcript.Contains ("tension") )
							{
								voicebox_glottal_tension = voicebox_glottal_tension - 25; 	  // -100% to 100%
								voicebox_breathiness = voicebox_breathiness - 25;  // -100% to 100%
								voicebox_timbre_extent = voicebox_timbre_extent - 10; // -0% to 100%
							}
							else if (alt.transcript.Contains ("reset") )
							{
								voicebox_prosody_pitch = 250;  //  HZ
								voicebox_pitch_range = 0;  // -100% to 100%
								voicebox_glottal_tension = 0; 	  // -100% to 100%
								voicebox_breathiness = 0;  // -100% to 100%
								voicebox_rate = 0;  // -100% to 100%
								voicebox_timbre_extent = 50; // -0% to 100%
							}

							else if (alt.transcript.Contains ("settings") )
							{
								_testString =
								" Voicebox settings.  Pitch is " +
								voicebox_prosody_pitch +
								" hertz. The speech rate is " +
								voicebox_rate +
								". And the glottal tension is " +
								voicebox_glottal_tension; 
							} 

							else 
							{
								_testString = "My voice is my passport";  // if no changes
							}



							Voicebox_Status.text = 
							"voicebox:" +
							"\n pitch" +
							voicebox_prosody_pitch +
							"hz \n pitch_range" +
							voicebox_pitch_range +
							"\n glottal_tension" +
							voicebox_glottal_tension +
							"\n breathiness" +
							voicebox_breathiness +
							"\n rate" +
							voicebox_rate +
							"\n timbre_extent" +
							voicebox_timbre_extent;


							/// WRAP TEST STRING IN THE SSML/TTS FIXINS THAT MAKE IT SPECIAL
							_testString = "<speak version=\"1.0\">" +
								"<prosody pitch=\"" +
								voicebox_prosody_pitch +
								"Hz\">" +
								"<voice-transformation type=\"Custom\" " +
								"glottal_tension=\"" +
								voicebox_glottal_tension +
								"%\" pitch_range=\"" +
								voicebox_pitch_range +
								"%\" rate=\"" +
								voicebox_rate +
								"%\" timbre_extent=\"" +
								voicebox_timbre_extent +
								"%\"> " +
								_testString +
								"</voice-transformation>" +
								"</prosody>" +
								"</speak>";
							/// AND FIRE TTS THAT USES PUBLIC _testString
							Runnable.Run (Examples ());

						

						} // end of voicebox
							




						// check if user is changing SYSTEM MODE
						else if ((alt.transcript.Contains ("switch") || alt.transcript.Contains ("change")) && alt.transcript.Contains ("mode")) 
							{
							if (ResultsField.text.Contains ("diagnostic")) { 
									System_Mode.text = "mode: diagnostic";
									_testString = "<speak version=\"1.0\">Diagnostic Mode Engaged</speak>";
									Runnable.Run (Examples ());
								}
							else if (ResultsField.text.Contains ("listen") || ResultsField.text.Contains ("learn"))  { 
									_testString = "<speak version=\"1.0\">Listen Learn Mode Engaged</speak>";
									Runnable.Run (Examples ());
									System_Mode.text = "mode: listen-learn";
								}
							else if (ResultsField.text.Contains ("conversation")) {
									System_Mode.text = "mode: conversation";
								_testString = "<speak version=\"1.0\"><express-as type=\"GoodNews\">Conversation Mode Engaged!</express-as></speak>";
								Runnable.Run (Examples ());

								}
							else if (ResultsField.text.Contains ("factoid")) {
									_testString = "<speak version=\"1.0\">Factoid Mode Engaged</speak>";
									Runnable.Run (Examples ());
									System_Mode.text = "mode: factoid";
								}
							}



						//string _testString = "<speak version=\"1.0\"><prosody pitch=\"350Hz\">This is Text to Speech!</prosody></speak>";
						//string _testString = "<speak version=\"1.0\"><prosody pitch=\"350Hz\">hi</prosody></speak>";
						// Good news and sorrow and uncertainty - ref https://console.bluemix.net/docs/services/text-to-speech/SSML-expressive.html#expressive
						// <express-as type="GoodNews">This is Text to Speech!</express-as>
						//string _testString = "<speak version=\"1.0\"><express-as type=\"Apology\">I am terribly sorry for the quality of service you have received.</express-as></speak>";
						//string _testString = "<speak version=\"1.0\"><express-as type=\"Uncertainty\">Can you please explain it again? I am not sure I understand.</express-as></speak>";
						//string _testString = "<speak version=\"1.0\"><prosody pitch=\\\"350Hz\\\"><express-as type=\"Uncertainty\">Can you please explain it again? I am confused and I'm not sure I understand.</express-as></prosody></speak>";


						//// WRITE TO LOG FILE - time-date and MODE and transcript (final)	 - we're also logging her responses right now. may mute later (or tag)
						string path = @"MacKenzie_Conversation_Log.txt";
						using (StreamWriter sw = File.AppendText (path)) {
						sw.WriteLine (string.Concat (System.DateTime.Now.ToLongTimeString (), "-", System.DateTime.Now.ToString ("MM/dd/yyyy"), ",",System_Mode.text,",", alt.transcript));
						}
								
					} 
					////// HACK LOG INSERT ENDS


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








	public IEnumerator Examples()   ///// RYAN CHECK - changed to Public / from private and may need to declare call examplestts.exampels from other code
	{
		//_testString = str; 

		//  Synthesize
		Log.Debug("ExampleTextToSpeech.Examples()", "Attempting synthesize.");
		_textToSpeech.Voice = VoiceType.en_US_Allison;
		_textToSpeech.ToSpeech(HandleToSpeechCallback, OnFail, _testString, true);
		while (!_synthesizeTested)
			yield return null;

		//	Get Voices
		Log.Debug("ExampleTextToSpeech.Examples()", "Attempting to get voices.");
		_textToSpeech.GetVoices(OnGetVoices, OnFail);
		while (!_getVoicesTested)
			yield return null;

		//	Get Voice
		Log.Debug("ExampleTextToSpeech.Examples()", "Attempting to get voice {0}.", VoiceType.en_US_Allison);
		_textToSpeech.GetVoice(OnGetVoice, OnFail, VoiceType.en_US_Allison);
		while (!_getVoiceTested)
			yield return null;

		//	Get Pronunciation
		Log.Debug("ExampleTextToSpeech.Examples()", "Attempting to get pronunciation of {0}", _testWord);
		_textToSpeech.GetPronunciation(OnGetPronunciation, OnFail, _testWord, VoiceType.en_US_Allison);
		while (!_getPronuciationTested)
			yield return null;
//
//		//  Get Customizations
//		Log.Debug("ExampleTextToSpeech.Examples()", "Attempting to get a list of customizations");
//		_textToSpeech.GetCustomizations(OnGetCustomizations, OnFail);
//		while (!_getCustomizationsTested)
//			yield return null;
//
//		//  Create Customization
//		Log.Debug("ExampleTextToSpeech.Examples()", "Attempting to create a customization");
//		_textToSpeech.CreateCustomization(OnCreateCustomization, OnFail, _customizationName, _customizationLanguage, _customizationDescription);
//		while (!_createCustomizationTested)
//			yield return null;
//
//		//  Get Customization
//		Log.Debug("ExampleTextToSpeech.Examples()", "Attempting to get a customization");
//		if (!_textToSpeech.GetCustomization(OnGetCustomization, OnFail, _createdCustomizationId))
//			Log.Debug("ExampleTextToSpeech.Examples()", "Failed to get custom voice model!");
//		while (!_getCustomizationTested)
//			yield return null;

//		//  Update Customization
//		Log.Debug("ExampleTextToSpeech.Examples()", "Attempting to update a customization");
//		Word[] wordsToUpdateCustomization =
//		{
//			new Word()
//			{
//				word = "hello",
//				translation = "hullo"
//			},
//			new Word()
//			{
//				word = "goodbye",
//				translation = "gbye"
//			},
//			new Word()
//			{
//				word = "hi",
//				translation = "ohioooo"
//			}
//		};
//
//		_customVoiceUpdate = new CustomVoiceUpdate()
//		{
//			words = wordsToUpdateCustomization,
//			description = "My updated description",
//			name = "My updated name"
//		};

		if (!_textToSpeech.UpdateCustomization(OnUpdateCustomization, OnFail, _createdCustomizationId, _customVoiceUpdate))
			Log.Debug("ExampleTextToSpeech.Examples()", "Failed to update customization!");
		while (!_updateCustomizationTested)
			yield return null;

//		//  Get Customization Words
//		Log.Debug("ExampleTextToSpeech.Examples()", "Attempting to get a customization's words");
//		if (!_textToSpeech.GetCustomizationWords(OnGetCustomizationWords, OnFail, _createdCustomizationId))
//			Log.Debug("ExampleTextToSpeech.GetCustomizationWords()", "Failed to get {0} words!", _createdCustomizationId);
//		while (!_getCustomizationWordsTested)
//			yield return null;
//
//		//  Add Customization Words
//		Log.Debug("ExampleTextToSpeech.Examples()", "Attempting to add words to a customization");
//		Word[] wordArrayToAddToCustomization =
//		{
//			new Word()
//			{
//				word = "bananna",
//				translation = "arange"
//			},
//			new Word()
//			{
//				word = "orange",
//				translation = "gbye"
//			},
//			new Word()
//			{
//				word = "tomato",
//				translation = "tomahto"
//			}
//		};
//
//		Words wordsToAddToCustomization = new Words()
//		{
//			words = wordArrayToAddToCustomization
//		};
//
//		if (!_textToSpeech.AddCustomizationWords(OnAddCustomizationWords, OnFail, _createdCustomizationId, wordsToAddToCustomization))
//			Log.Debug("ExampleTextToSpeech.AddCustomizationWords()", "Failed to add words to {0}!", _createdCustomizationId);
//		while (!_addCustomizationWordsTested)
//			yield return null;
//
//		//  Get Customization Word
//		Log.Debug("ExampleTextToSpeech.Examples()", "Attempting to get the translation of a custom voice model's word.");
//		string customIdentifierWord = wordsToUpdateCustomization[0].word;
//		if (!_textToSpeech.GetCustomizationWord(OnGetCustomizationWord, OnFail, _createdCustomizationId, customIdentifierWord))
//			Log.Debug("ExampleTextToSpeech.GetCustomizationWord()", "Failed to get the translation of {0} from {1}!", customIdentifierWord, _createdCustomizationId);
//		while (!_getCustomizationWordTested)
//			yield return null;

		//  Delete Customization Word
		Log.Debug("ExampleTextToSpeech.Examples()", "Attempting to delete customization word from custom voice model.");
		string wordToDelete = "goodbye";
		if (!_textToSpeech.DeleteCustomizationWord(OnDeleteCustomizationWord, OnFail, _createdCustomizationId, wordToDelete))
			Log.Debug("ExampleTextToSpeech.DeleteCustomizationWord()", "Failed to delete {0} from {1}!", wordToDelete, _createdCustomizationId);
		while (!_deleteCustomizationWordTested)
			yield return null;

		//  Delete Customization
		Log.Debug("ExampleTextToSpeech.Examples()", "Attempting to delete a customization");
		if (!_textToSpeech.DeleteCustomization(OnDeleteCustomization, OnFail, _createdCustomizationId))
			Log.Debug("ExampleTextToSpeech.DeleteCustomization()", "Failed to delete custom voice model!");
		while (!_deleteCustomizationTested)
			yield return null;

		Log.Debug("ExampleTextToSpeech.Examples()", "Text to Speech examples complete.");
	}

	void HandleToSpeechCallback(AudioClip clip, Dictionary<string, object> customData = null)
	{
		PlayClip(clip);
	}

	private void PlayClip(AudioClip clip)
	{
		if (Application.isPlaying && clip != null)
		{
			GameObject audioObject = new GameObject("AudioObject");
			AudioSource source = audioObject.AddComponent<AudioSource>();
			source.spatialBlend = 0.0f;
			source.loop = false;
			source.clip = clip;
			source.Play();

			Destroy(audioObject, clip.length);

			_synthesizeTested = true;
		}
	}

	private void OnGetVoices(Voices voices, Dictionary<string, object> customData = null)
	{
		Log.Debug("ExampleTextToSpeech.OnGetVoices()", "Text to Speech - Get voices response: {0}", customData["json"].ToString());
		_getVoicesTested = true;
	}

	private void OnGetVoice(Voice voice, Dictionary<string, object> customData = null)
	{
		Log.Debug("ExampleTextToSpeech.OnGetVoice()", "Text to Speech - Get voice  response: {0}", customData["json"].ToString());
		_getVoiceTested = true;
	}

	private void OnGetPronunciation(Pronunciation pronunciation, Dictionary<string, object> customData = null)
	{
		Log.Debug("ExampleTextToSpeech.OnGetPronunciation()", "Text to Speech - Get pronunciation response: {0}", customData["json"].ToString());
		_getPronuciationTested = true;
	}

//	private void OnGetCustomizations(Customizations customizations, Dictionary<string, object> customData = null)
//	{
//		Log.Debug("ExampleTextToSpeech.OnGetCustomizations()", "Text to Speech - Get customizations response: {0}", customData["json"].ToString());
//		_getCustomizationsTested = true;
//	}
//
//	private void OnCreateCustomization(CustomizationID customizationID, Dictionary<string, object> customData = null)
//	{
//		Log.Debug("ExampleTextToSpeech.OnCreateCustomization()", "Text to Speech - Create customization response: {0}", customData["json"].ToString());
//		_createdCustomizationId = customizationID.customization_id;
//		_createCustomizationTested = true;
//	}

	private void OnDeleteCustomization(bool success, Dictionary<string, object> customData = null)
	{
		Log.Debug("ExampleTextToSpeech.OnDeleteCustomization()", "Text to Speech - Delete customization response: {0}", customData["json"].ToString());
		_createdCustomizationId = null;
		_deleteCustomizationTested = true;
	}

//	private void OnGetCustomization(Customization customization, Dictionary<string, object> customData = null)
//	{
//		Log.Debug("ExampleTextToSpeech.OnGetCustomization()", "Text to Speech - Get customization response: {0}", customData["json"].ToString());
//		_getCustomizationTested = true;
//	}

	private void OnUpdateCustomization(bool success, Dictionary<string, object> customData = null)
	{
		Log.Debug("ExampleTextToSpeech.OnUpdateCustomization()", "Text to Speech - Update customization response: {0}", customData["json"].ToString());
		_updateCustomizationTested = true;
	}

//	private void OnGetCustomizationWords(Words words, Dictionary<string, object> customData = null)
//	{
//		Log.Debug("ExampleTextToSpeech.OnGetCustomizationWords()", "Text to Speech - Get customization words response: {0}", customData["json"].ToString());
//		_getCustomizationWordsTested = true;
//	}

	private void OnAddCustomizationWords(bool success, Dictionary<string, object> customData = null)
	{
		Log.Debug("ExampleTextToSpeech.OnAddCustomizationWords()", "Text to Speech - Add customization words response: {0}", customData["json"].ToString());
		_addCustomizationWordsTested = true;
	}

	private void OnDeleteCustomizationWord(bool success, Dictionary<string, object> customData = null)
	{
		Log.Debug("ExampleTextToSpeech.OnDeleteCustomizationWord()", "Text to Speech - Delete customization word response: {0}", customData["json"].ToString());
		_deleteCustomizationWordTested = true;
	}

	private void OnGetCustomizationWord(Translation translation, Dictionary<string, object> customData = null)
	{
		Log.Debug("ExampleTextToSpeech.OnGetCustomizationWord()", "Text to Speech - Get customization word response: {0}", customData["json"].ToString());
		_getCustomizationWordTested = true;
	}

	private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
	{
		Log.Error("ExampleTextToSpeech.OnFail()", "Error received: {0}", error.ToString());
	}
}
