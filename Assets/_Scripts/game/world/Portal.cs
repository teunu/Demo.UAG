using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public static bool blocked;
    
    static Dictionary<string, Portal> portals = new Dictionary<string, Portal>();

    [Header("This Portal")]
    [SerializeField] Vector2 A;
    [SerializeField] Vector2 B;
    [SerializeField] string reference;

    [Header("Destination")]
    [SerializeField] string DestinationScene;
    [SerializeField] string DestinationPortal;



    private void Awake()
    {
        portals.TryAdd(reference, this);   
    }

    private void OnDestroy()
    {
        portals.Remove(reference);
    }

    public static async Task CrossPortal(string scene, string other)
    {
        await GameSceneHandler.SwitchGameScene(scene);

        if (portals.TryGetValue(other, out Portal destination))
            await destination.Arrive();
    }

    public async void PortalToScene(string scene, string other_portal)
    {
        Send();
        _ = CrossPortal(scene, other_portal);
    }

    public async UniTask Send()
    {
        //Animate Character from A to B
        blocked = true;
        PlayerController.instance.body.isKinematic = true;

        Vector2 player = PlayerController.instance.transform.position;
        Vector2 destination = new Vector2(transform.position.x + B.x, player.y);

        int frames = 0;
        const int time = 25;
        while (frames++ < time)
        {
            PlayerController.instance.body.MovePosition(Vector2.Lerp(player, destination, (float)frames / (float)time));
            await UniTask.Yield();
        }

        PlayerController.instance.body.isKinematic = false;
        blocked = false;
    }

    public async Task Arrive()
    {
        blocked = true;
        PlayerController.instance.body.isKinematic = true;

        Vector2 start = new Vector2(transform.position.x + A.x, transform.position.y + A.y);
        Vector2 destination = new Vector2(transform.position.x + B.x, start.y);

        //Animate Character from B to A
        int frames = 0;
        const int time = 25;
        while (frames++ < time)
        {
            PlayerController.instance.body.MovePosition(Vector2.Lerp(destination, start, (float)frames / (float)time));
            await UniTask.Yield();
        }

        PlayerController.instance.body.isKinematic = false;
        blocked = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Pop!");
        if (blocked) { return; }
        if (collision.attachedRigidbody == PlayerController.instance.body) { PortalToScene(DestinationScene, DestinationPortal); }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawLine(A, B);
        Gizmos.color = Color.yellow; Gizmos.DrawSphere(A, 0.1f);
        Gizmos.color = Color.cyan; Gizmos.DrawSphere(B, 0.1f);
    }

}
