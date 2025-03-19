using UnityEngine;

public class randomrotation : MonoBehaviour {

    Vector3 rotationAxis;

    float speed;

    Transform mineTransform;

	void OnEnable ()
    {
        this.speed = Random.Range(1.0f, 3.0f);
        this.mineTransform = transform;
        this.rotationAxis = Random.insideUnitSphere;
	}

	void Update ()
    {
        this.mineTransform.Rotate(this.rotationAxis, this.speed * Time.deltaTime * Mathf.Rad2Deg);
    }
}
