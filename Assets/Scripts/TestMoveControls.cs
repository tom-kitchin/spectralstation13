using UnityEngine;
using Implementers.Motion;

public class TestMoveControls : MonoBehaviour
{
    public GameObject testEntity;
    CanMove canMoveImpl;

    // Update is called once per frame
    void Update ()
    {
        if (testEntity != null)
        {
            if (canMoveImpl == null)
            {
                canMoveImpl = testEntity.GetComponent<CanMove>();
            }
            canMoveImpl.movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }
}
