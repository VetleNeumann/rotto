using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    GameObject rotto;
    [SerializeField]
    float yOffset = 11f;
    [SerializeField]
    float xOffset = 0f;
    [SerializeField]
    float zOffset = 0f;
    [SerializeField]
    float xRotation = 80f;
    [SerializeField]
    bool diagonalView = false;

	Renderer prevRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rotto = GameObject.FindGameObjectWithTag("Player");
        rotto.GetComponent<PlayerController>().diagonalView = diagonalView;
    }

    void OnValidate()
    {
		rotto = GameObject.FindGameObjectWithTag("Player");
		transform.rotation = Quaternion.Euler(new Vector3(xRotation, diagonalView ? 45f : 0, 0));
		transform.position = rotto.transform.position + new Vector3(0, yOffset, zOffset);

        rotto.GetComponent<PlayerController>().diagonalView = diagonalView;
	}

    // Update is called once per frame
    void Update()
    {
		// Camera follow rotto
        transform.position = rotto.transform.position + new Vector3(xOffset, yOffset, zOffset);

		WallFade();
    }

	void OnDrawGizmos()
	{
		if (rotto != null)
		{
			Ray ray = new Ray(transform.position, rotto.transform.position - transform.position);
			//Gizmos.DrawRay(ray);
			Gizmos.DrawLine(transform.position, rotto.transform.position);
		}
	}

	void WallFade()
	{
		Ray ray = new Ray(transform.position, rotto.transform.position - transform.position);
		if (Physics.Raycast(ray, out RaycastHit hitInfo))
		{
			Renderer renderer = hitInfo.transform.GetComponent<Renderer>();

			if (hitInfo.transform.CompareTag("Wall") && (prevRenderer == null || prevRenderer != renderer) && renderer != null)
			{
				prevRenderer = renderer;
				SetFade(prevRenderer, 0, 1);
				//prevRenderer.material.SetFloat("_Transparency", 0);
			}
			else if (prevRenderer != null && renderer != prevRenderer)
			{
				//prevRenderer.material.SetFloat("_Transparency", 1.0f);
				SetFade(prevRenderer, 1, 1);
				prevRenderer = null;
			}
		}
	}

	void SetFade(Renderer renderer, float value, float speed)
	{
		float c = renderer.material.GetFloat("_Transparency");
		StopCoroutine("SetFadeRoutine");
		StartCoroutine(SetFadeRoutine(renderer, c, value, speed));
	}

	IEnumerator SetFadeRoutine(Renderer renderer, float c, float n, float s)
	{
		for (int i = 0; i <= 10 / s; i++)
		{
			float p = i / 10f;
			float lerpedValue = Mathf.Lerp(c, n, p);

			renderer.material.SetFloat("_Transparency", lerpedValue);
			yield return new WaitForSeconds(0);
		}
		yield return null;
	}
}
