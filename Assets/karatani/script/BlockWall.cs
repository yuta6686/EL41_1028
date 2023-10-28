using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockWall : MonoBehaviour
{
    [SerializeField,Header("デフォルトの飛ばす力")] private float breakForce = 10.0f;
    [SerializeField,Header("ブロックの質量")] private float blockMass = 1.0f;
    [SerializeField,Header("ブロックの抵抗")] private float blockDrag = 0.1f;

    private List<Rigidbody> blockRb = new List<Rigidbody>();
    private List<Transform> blockTf = new List<Transform>();

    // Start is called before the first frame update
    private void Start()
    {
        var myTransform = transform;
        foreach(Transform child in myTransform)
        {
            Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.mass = blockMass;
            rb.drag = blockDrag;
            blockRb.Add(rb);
            blockTf.Add(child);
        }
    }

    public void BreakWall(Vector3 hitPosition)
    {
        BreakWall(hitPosition, breakForce);
    }

    public void BreakWall(Vector3 hitPosition, float force)
    {
        int index = 0;
        foreach (Rigidbody rb in blockRb)
        {
            blockTf[index].parent = null;
            blockTf[index].gameObject.AddComponent<BoxCollider>();
            rb.isKinematic = false;
            Vector3 forceVec = (blockTf[index].position - hitPosition).normalized;
            rb.AddForce(forceVec * force, ForceMode.Force);

            Destroy(rb.gameObject, 3);
            index++;
        }

        Destroy(this.gameObject);
    }
}
