using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Product : MonoBehaviour
{
    ObjectPooler objectPooler;
    [SerializeField] private float collectionDuration = 5.0f;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private ResourceTypes resourceType;

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
            GameManager.Instance.AddResourceToPlayer(1, resourceType);
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
        startPosition = transform.position;

        while (elapsedTime < collectionDuration)
        {
            transform.position = Vector3.Lerp(startPosition, player.transform.position, elapsedTime / collectionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }
}
