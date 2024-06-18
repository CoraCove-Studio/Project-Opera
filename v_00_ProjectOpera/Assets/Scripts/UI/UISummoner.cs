using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// goes on parent canvas when animations are made
// needs controller
// needs inverse and verse animations

public class UISummoner : MonoBehaviour
{
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float delay = 0.1f;
    [SerializeField] private bool showing = false;

    private Animator[] children;
    // Start is called before the first frame update
    void Start()
    {
        children = GetComponentsInChildren<Animator>();

        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetBool("Shown", showing);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = Camera.main.transform.position - transform.position;

        if (delta.magnitude < minDistance)
        {
            if (showing) return;
            StartCoroutine("ActivateInTurn");
        }
        else
        {
            if (!showing) return;
            StartCoroutine("DeactivateInTurn");
        }
    }

    public IEnumerator ActivateInTurn()
    {
        showing = true;

        yield return new WaitForSeconds(delay);

        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetBool("Shown", true);
            yield return new WaitForSeconds(delay);
        }
    }

    public IEnumerator DeactivateInTurn()
    {
        showing = false;

        yield return new WaitForSeconds(delay);

        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetBool("Shown", false);
            yield return new WaitForSeconds(delay);
        }
    }
}
