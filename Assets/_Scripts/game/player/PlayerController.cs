using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public Rigidbody2D body { get; private set; }
    public PlayerInput input { get; private set; }

    float height { get { return state == LocomotionStates.crouch ? 0.3f : 0.65f; } }
    float moveforce { get { return 50; } }

    float max_speed { get {
            switch (state)
            {
                default: return 2.5f;
                case LocomotionStates.crouch: return 0.4f;
            }
        }
    }
    const float jump_power = 16f;

    //Colliders
    [SerializeField] BoxCollider2D sensor_ground;
    public BoxCollider2D body_collider { get; private set; }

    //Touching
    public bool grounded;
    public bool wall_left;
    public bool wall_right;
    public bool top_confined
    {
        get
        {
            RaycastHit2D hit_head_l = Physics2D.Raycast(transform.position + new Vector3(-0.1f, 0.5f), Vector2.up, 0.1f, LayerMask.GetMask("Static World"));
            RaycastHit2D hit_head_r = Physics2D.Raycast(transform.position + new Vector3(0.1f, 0.5f), Vector2.up, 0.1f, LayerMask.GetMask("Static World"));
            if (hit_head_l.collider != null || hit_head_r.collider != null)
            {
                return true;
            }
            return false;
        }
    }

    public LocomotionStates state;
    public bool can_jump;
    public bool coyote;
    public bool freefall;
    public bool immune;
    public bool controllable;

    //Events
    public delegate void ControllerEvent();
    public ControllerEvent OnHurt;
    public ControllerEvent OnJump;

    private void Awake()
    {
        instance = this;
        can_jump = true;

        SceneManager.LoadSceneAsync("HUD", LoadSceneMode.Additive);
        OnHurt += PlayerData.GameOverListener;

        input = GetComponent<PlayerInput>();
        body = GetComponent<Rigidbody2D>();
        body_collider = GetComponent<BoxCollider2D>();
    }

    private void OnDestroy()
    {
        OnHurt -= PlayerData.GameOverListener;
    }

    private void FixedUpdate()
    {
        state = grounded ? LocomotionStates.normal : LocomotionStates.air;
        grounded = sensor_ground.IsTouchingLayers(LayerMask.GetMask("Static World"));

        if (!grounded && body.velocity.y < -12)
            freefall = true;
        else if (freefall)
            FreefallImpact();

        if (!grounded && state != LocomotionStates.air)
        { CoyoteTime(); }

        HazardChecker();
        HandleInput();
        AntiSpider();
    }

    void HandleInput()
    {
        if (PlayerInput.InInteractionMenus || !controllable) { return; }
        float f_moveforce = grounded ? moveforce : moveforce * 0.7f;

        if (state != LocomotionStates.air || (state == LocomotionStates.crouch && !top_confined))
            state = input.crouch ? LocomotionStates.crouch : LocomotionStates.normal;
        if (input.jump && can_jump && (grounded || coyote)) { Jump(); }

        Move();

        body_collider.size = state == LocomotionStates.crouch ? new Vector2(0.35f, 0.15f) : new Vector2(0.25f, 0.5f);
        body_collider.offset = state == LocomotionStates.crouch ? new Vector2(0f, 0.115f) : new Vector2(0f, 0.285f);
    }

    void Move()
    {
        if (input.move.x < 0 && wall_left) { return; }
        else if (input.move.x > 0 && wall_right) { return; }

        body.AddForce(new Vector2(input.move.x * moveforce, 0));

        if (body.velocity.x > max_speed)
        {
            body.velocity = new Vector2(max_speed, body.velocity.y);
        }
        else if (body.velocity.x < -max_speed)
        {
            body.velocity = new Vector2(-max_speed, body.velocity.y);
        }
    }

    async UniTask Jump() {
        sensor_ground.enabled = false;
        can_jump = false;

        OnJump?.Invoke();

        //Start off with a little power
        body.velocity = new Vector2(body.velocity.x, jump_power / 3);

        float jump_time = 0.5f;
        for (int t = 0; t < 15; t++)
        {
            if (!input.jump || top_confined) { break; }

            float force = jump_power * Mathf.Pow(jump_time, 1.9f);
            body.velocity = new Vector2(body.velocity.x, body.velocity.y + (1f/30f * force) * 10);

            jump_time -= Time.fixedDeltaTime;
            await UniTask.Yield();
        }

        sensor_ground.enabled = true;

        while (!grounded) { await UniTask.Yield(); }

        can_jump = true;
    }

    void AntiSpider()
    {
        Vector2 top = (Vector2)transform.position + new Vector2(0, height);
        Vector2 bottom = (Vector2)transform.position + new Vector2(0, 0.05f);
        RaycastHit2D hit_lt = Physics2D.Raycast(top, Vector2.left, 0.32f, LayerMask.GetMask("Static World"));
        RaycastHit2D hit_rt = Physics2D.Raycast(top, -Vector2.left, 0.32f, LayerMask.GetMask("Static World"));
        RaycastHit2D hit_ld = Physics2D.Raycast(bottom, Vector2.left, 0.3f, LayerMask.GetMask("Static World"));
        RaycastHit2D hit_rd = Physics2D.Raycast(bottom, -Vector2.left, 0.3f, LayerMask.GetMask("Static World"));

        wall_left = (hit_lt.collider != null || hit_ld.collider != null);
        wall_right = (hit_rt.collider != null || hit_rd.collider != null);

        if (wall_left)
            body.velocity = new Vector2(Mathf.Clamp(body.velocity.x, 0, 10), body.velocity.y);
        if (wall_right)
            body.velocity = new Vector2(Mathf.Clamp(body.velocity.x, -10, 0), body.velocity.y);
    }

    void HazardChecker()
    {
        if (body_collider.IsTouchingLayers(LayerMask.GetMask("Hazard")))
            GetHurt(transform);
    }

    public async UniTask GetHurt(Transform source)
    {
        if (immune) { return; } //Immune to hurt

        PlayerData.health--;

        OnHurt?.Invoke();
        Immunity();             //Set immunity

        Vector2 v = body.velocity;
        body.velocity = Vector2.zero;

        #region Time Stop
        await UniTask.DelayFrame(2);    //Allow the game to play for 2 more frames
        Time.timeScale = 0.1f;
        await UniTask.DelayFrame(6);    //Pause the game for 6 frames to show the impact
        Time.timeScale = 1;
        #endregion

        if (source == transform)
            body.velocity = new Vector2(v.x /3 , (jump_power / 3) * 2);
        //else (use source as knockback)
    }

    async UniTask Immunity()
    {
        immune = true;
        await UniTask.DelayFrame(15);
        immune = false;
    }

    async UniTask CoyoteTime()
    {
        for (int t = 0; t < 3; t++) 
        {
            coyote = true;
            await UniTask.Yield();
        }
        coyote = false;
    }

    async void FreefallImpact()
    {
        freefall = false;

        body.velocity = Vector2.zero;
        await FreezeControl(15);

        //Await a random number before jumping up again. When any input is read, then finish.
        for (int t = 0; t < 60; t++) {
            if (input.crouch || input.jump || Mathf.Abs(input.move.x) > 0.1f) return;
            if (body.velocity.y < -0.1f) return;
            await UniTask.Yield();
        }

        //"Jump"
        body.velocity = new Vector2(body.velocity.x, jump_power / 2);
        OnJump?.Invoke();

    }

    //Freeze Control for specified amount of frames, such as after freefall
    async UniTask FreezeControl(int frames)
    {
        controllable = false;

        for (int t = 0; t < frames; t++)
        {
            await UniTask.Yield();
        }
        controllable = true;
    }

    public enum LocomotionStates
    {
        normal,
        air,
        crouch
    }
}
