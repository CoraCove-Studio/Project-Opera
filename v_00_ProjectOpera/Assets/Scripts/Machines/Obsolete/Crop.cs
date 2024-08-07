using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    ObjectPooler objectPooler;
    [SerializeField] private float collectionDuration = 5.0f;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;

    private void OnEnable()
    {
        startPosition = transform.position;
    }

    public void SetObjectPoolerReference(ObjectPooler reference)
    {
        objectPooler = reference;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TagManager.PLAYER))
        {
            print("player detected");
            //method to add to player's resource count
            //gameObject.transform.parent = objectPooler.transform;
            gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag(TagManager.COLLECTION))
        {
            print("collector detected");
            StartCoroutine(MoveToEndPosition(other.gameObject));
        }
    }

    private IEnumerator MoveToEndPosition(GameObject player)
    {
        float elapsedTime = 0f;

        while (elapsedTime < collectionDuration)
        {
            transform.position = Vector3.Lerp(startPosition, player.transform.position, elapsedTime / collectionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
    }
}
