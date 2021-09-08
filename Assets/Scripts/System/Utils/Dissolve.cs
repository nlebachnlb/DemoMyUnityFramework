using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Dissolve : MonoBehaviour
{
	Material material;

	bool isDissolving = false;

	[Range(0f, 1f)]
	public float fade = 1f;

	void Start()
	{
		// Get a reference to the material
		material = GetComponent<SpriteRenderer>().sharedMaterial;
	}

	protected virtual void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
		// {
		// 	isDissolving = true;
		// }

		// if (isDissolving)
		// {
		// 	fade -= Time.deltaTime;

		// 	if (fade <= 0f)
		// 	{
		// 		fade = 0f;
		// 		isDissolving = false;
		// 	}

		// 	// Set the property
		// 	material.SetFloat("_Fade", fade);
		// }
		material.SetFloat("_Fade", fade);
    }
}
