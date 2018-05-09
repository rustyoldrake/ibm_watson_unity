using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IBM.Watson.DeveloperCloud.Services.LanguageTranslator.v2;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Connection;

public class LanguageTranslatorDemo : MonoBehaviour
{
	public Text ResponseTextField;
	
	private LanguageTranslator _languageTranslator;
	private string _translationModel = "en-es";

	// Use this for initialization
	void Start ()
	{
		LogSystem.InstallDefaultReactors();

		Credentials languageTranslatorCredentials = new Credentials () 
			{
				Username = "f5xxxxx",
				Password = "62xxxxx",
				Url = "https://gateway.watsonplatform.net/language-translator/api"
			};

		_languageTranslator = new LanguageTranslator(languageTranslatorCredentials);

		Translate("Where is the library?");
	}

	public void Translate(string text)
	{
		_languageTranslator.GetTranslation(OnTranslate, OnFail, text, _translationModel);
	}

	private void OnTranslate(Translations response, Dictionary<string, object> customData)
	{
		ResponseTextField.text = response.translations[0].translation;
	}


	private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
	{
		Log.Debug ("LanguageTranslatorDemo.OnFail()", "Error: {0}", error.ToString ());
	}
}

// thanks Taj  - https://www.youtube.com/watch?v=cVaMlv7iZ1s&index=6&list=PLZDyxLlNKRY8MdgiUq45cZztn_f0EVz6c 
