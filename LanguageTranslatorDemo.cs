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
	
	public Text ResponseTextField_es;
	public Text ResponseTextField_fr;
	public Text ResponseTextField_it;
	public Text ResponseTextField_de;
	public Text ResponseTextField_pt;
	public Text ResponseTextField_ar;
	public Text ResponseTextField_ja;


	
	private LanguageTranslator _languageTranslator;
	private string _translationModel_en_es = "en-es"; //spanish
	private string _translationModel_en_fr = "en-fr"; //french
	private string _translationModel_en_it = "en-it"; //italian
	private string _translationModel_en_de = "en-de"; //german
	private string _translationModel_en_pt = "en-pt"; //portuguese
	private string _translationModel_en_ar = "en-ar"; //arabic
	private string _translationModel_en_ja = "en-ja"; // japanese


	// All the language options here https://console.bluemix.net/docs/services/language-translator/translation-models.html
	// Use this for initialization
	void Start ()
	{
		LogSystem.InstallDefaultReactors();

		Credentials languageTranslatorCredentials = new Credentials () 
			{
				Username = "f57f599c-45c5-402b-a998-######",
				Password = "62#####",
				Url = "https://gateway.watsonplatform.net/language-translator/api"
			};

		_languageTranslator = new LanguageTranslator(languageTranslatorCredentials);

		//Translate("Where is the library?");
	}


	public void Translate_es(string text)
	{
		_languageTranslator.GetTranslation(OnTranslate_es, OnFail, text, _translationModel_en_es);
	}

	private void OnTranslate_es(Translations response, Dictionary<string, object> customData)
	{
		ResponseTextField_es.text = response.translations[0].translation;
	}


	// yes, i'm a terrible person for doing the code this way
	public void Translate_fr(string text)
	{
		_languageTranslator.GetTranslation(OnTranslate_fr, OnFail, text, _translationModel_en_fr);
	}

	private void OnTranslate_fr(Translations response, Dictionary<string, object> customData)
	{
		ResponseTextField_fr.text = response.translations[0].translation;
	}


	// yes, i'm a terrible person for doing the code this way
	public void Translate_it(string text)
	{
		_languageTranslator.GetTranslation(OnTranslate_it, OnFail, text, _translationModel_en_it);
	}

	private void OnTranslate_it(Translations response, Dictionary<string, object> customData)
	{
		ResponseTextField_it.text = response.translations[0].translation;
	}


	// yes, i'm a terrible person for doing the code this way
	public void Translate_de(string text)
	{
		_languageTranslator.GetTranslation(OnTranslate_de, OnFail, text, _translationModel_en_de);
	}

	private void OnTranslate_de(Translations response, Dictionary<string, object> customData)
	{
		ResponseTextField_de.text = response.translations[0].translation;
	}




	// yes, i'm a terrible person for doing the code this way
	public void Translate_pt(string text)
	{
		_languageTranslator.GetTranslation(OnTranslate_pt, OnFail, text, _translationModel_en_pt);
	}

	private void OnTranslate_pt(Translations response, Dictionary<string, object> customData)
	{
		ResponseTextField_pt.text = response.translations[0].translation;
	}


	// yes, i'm a terrible person for doing the code this way
	public void Translate_ar(string text)
	{
		_languageTranslator.GetTranslation(OnTranslate_ar, OnFail, text, _translationModel_en_ar);
	}

	private void OnTranslate_ar(Translations response, Dictionary<string, object> customData)
	{
		ResponseTextField_ar.text = response.translations[0].translation;
	}

	// yes, i'm a terrible person for doing the code this way
	public void Translate_ja(string text)
	{
		_languageTranslator.GetTranslation(OnTranslate_ja, OnFail, text, _translationModel_en_ja);
	}

	private void OnTranslate_ja(Translations response, Dictionary<string, object> customData)
	{
		ResponseTextField_ja.text = response.translations[0].translation;
	}




	private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
	{
		Log.Debug ("LanguageTranslatorDemo.OnFail()", "Error: {0}", error.ToString ());
	}
}
