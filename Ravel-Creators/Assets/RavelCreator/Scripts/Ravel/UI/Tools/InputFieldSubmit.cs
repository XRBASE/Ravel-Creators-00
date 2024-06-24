using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TMP_InputField))]
public class InputFieldSubmit : MonoBehaviour {
	public UnityEvent<string> onSubmit;
	
	private TMP_InputField _field;

	private void Awake() {
		_field = GetComponent<TMP_InputField>();
		
		_field.onSubmit.AddListener(OnSubmit);
	}

	private void OnDestroy() {
		_field.onSubmit.RemoveListener(OnSubmit);
	}

	private void OnSubmit(string value) {
		onSubmit?.Invoke(value);
	}
}
