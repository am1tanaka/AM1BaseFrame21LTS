using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame.Assets;

namespace AM1.BaseFrame.Demo
{
    public class DemoPlayer : MonoBehaviour
    {
        public static DemoPlayer Instance { get; private set; }

        [Tooltip("走る速さ"), SerializeField]
        float runSpeed = 3f;

        [Tooltip("重力加速度"), SerializeField]
        float gravity = -40;

        [Tooltip("段差"), SerializeField]
        float step = 0.2f;

        /// <summary>
        /// ジャンプ速度
        /// </summary>
        static float JumpVelocity => 8;

        /// <summary>
        /// ジャンプしてからこの秒数中は減速しない
        /// </summary>
        static float JumpUpSeconds => 0.2f;

        /// <summary>
        /// ジャンプを開始した時間
        /// </summary>
        float jumpTime;

        /// <summary>
        /// 接触し続けることを避けるための余白
        /// </summary>
        static float CollideMargine => 0.01f;

        /// <summary>
        /// ミスの高さ
        /// </summary>
        static float MissHeight => -6;

        /// <summary>
        /// ミスした時の上昇速度
        /// </summary>
        static float MissVelocityY => 25;

        /// <summary>
        /// 接触判定用のBoxCollider2D
        /// </summary>
        BoxCollider2D boxCollider;

        public enum State
        {
            /// <summary>
            /// スタート待ち。重力含めて一切処理しない
            /// </summary>
            WaitStart,

            /// <summary>
            /// 重力落下+走る。クリックかスペースキーでジャンプ
            /// </summary>
            Run,

            /// <summary>
            /// 転落
            /// </summary>
            Miss,

            /// <summary>
            /// クリア。クリアアニメ
            /// </summary>
            Clear,
        }

        /// <summary>
        /// 現在の状態
        /// </summary>
        public State CurrentState { get; private set; }

        /// <summary>
        /// アニメの種類
        /// </summary>
        enum AnimState
        {
            Stand,
            Run,
            Jump,
            Miss,
            Clear
        }

        /// <summary>
        /// 公開用速度
        /// </summary>
        public Vector2 Velocity => velocity;

        /// <summary>
        /// 速度
        /// </summary>
        Vector2 velocity;

        /// <summary>
        /// 着地しているか
        /// </summary>
        [HideInInspector]
        public bool IsGrounded { get; private set; }

        /// <summary>
        /// ジャンプキーが押された
        /// </summary>
        [HideInInspector]
        public bool IsJumpKeyDown;

        /// <summary>
        /// ジャンプキーが押されている
        /// </summary>
        [HideInInspector]
        public bool IsJumpKey;

        Animator anim;
        Rigidbody2D rb;
        int blockLayer;

        private void Awake()
        {
            Instance = this;
            anim = GetComponent<Animator>();
            anim.SetInteger("State", (int)AnimState.Jump);
            rb = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            blockLayer = LayerMask.GetMask("Block");
        }

        void Update()
        {
            IsJumpKeyDown |= Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1");
            IsJumpKey = Input.GetButton("Jump") || Input.GetButton("Fire1");
        }

        private void FixedUpdate()
        {
            switch (CurrentState)
            {
                case State.WaitStart:
                    IsJumpKeyDown = false;
                    break;

                case State.Run:
                    UpdateRun();
                    break;

                case State.Miss:
                    UpdateMiss();
                    break;

                case State.Clear:
                    UpdateClear();
                    break;
            }
        }

        void UpdateRun()
        {
            Jump();
            Vector2 nextPosition = SideMove();
            nextPosition = Fall(nextPosition);
            rb.MovePosition(nextPosition);
            if (IsGrounded)
            {
                anim.SetInteger("State", (int)AnimState.Run);
            }
            else
            {
                anim.SetInteger("State", (int)AnimState.Jump);
            }

            // ミス判定
            if (transform.position.y < MissHeight)
            {
                ToMiss();
            }
        }

        void UpdateMiss()
        {
            if ((velocity.y > 0) || (transform.position.y > MissHeight))
            {
                velocity.y += Time.deltaTime * gravity;
                rb.MovePosition((Vector2)transform.position + Time.deltaTime * velocity);
            }
        }

        void UpdateClear()
        {
            var nextPosition = Fall(transform.position);
            rb.MovePosition(nextPosition);
        }

        /// <summary>
        /// ジャンプ開始と長押し終了判定
        /// </summary>
        void Jump()
        {
            // ジャンプ開始
            if (IsGrounded && IsJumpKeyDown)
            {
                velocity.y = JumpVelocity;
                jumpTime = Time.time;
                SEPlayer.Play(SEPlayer.SE.Jump);
            }

            // 長押しキャンセル
            if (!IsJumpKey)
            {
                jumpTime = 0;
            }

            IsJumpKeyDown = false;
        }

        /// <summary>
        /// ゲームが開始したことを教えてもらう
        /// </summary>
        public void GameStart()
        {
            CurrentState = State.Run;
            velocity = runSpeed * Vector2.right;
            anim.SetInteger("State", (int)AnimState.Run);
        }

        /// <summary>
        /// 現在位置から横方向に移動させて、接触した後の候補座標を返します。
        /// </summary>
        /// <returns>横移動を適用後の座標</returns>
        Vector2 SideMove()
        {
            var resultPosition = (Vector2)transform.position + Time.deltaTime * velocity.x * Vector2.right;

            // 横移動が殆どなければ何もしない
            if (Mathf.Approximately(velocity.x, 0f))
            {
                return resultPosition;
            }

            // 衝突判定
            var result = Physics2D.BoxCast(
                (Vector2)transform.position + boxCollider.offset,
                boxCollider.size, 0, Vector2.right, Time.deltaTime * velocity.x, blockLayer
                );

            // 接触なしなら横移動確定
            if (result.collider == null)
            {
                return resultPosition;
            }

            // 逆方向なら横移動確定
            float collideDir = result.point.x - (transform.position.x + boxCollider.offset.x);
            if (collideDir * velocity.x <= 0)
            {
                return resultPosition;
            }

            // 足元の段差を取得
            float stepHeight = GetStepHeight();
            if (stepHeight > step)
            {
                // 移動不可なので接触点までで停止
                resultPosition.x = result.point.x - Mathf.Sign(velocity.x) * (CollideMargine + 0.5f * boxCollider.size.x - boxCollider.offset.x);
            }
            else
            {
                // 進めて段差を登る
                resultPosition.y += stepHeight;
                IsGrounded = true;
                velocity.y = 0;
            }
            return resultPosition;
        }

        /// <summary>
        /// 段差の高さを確認する。
        /// </summary>
        /// <returns>越えられる時、段差の高さ。越えられない時は越えられない高さを返す</returns>
        float GetStepHeight()
        {
            float dir = Mathf.Sign(velocity.x);

            // 頭の高さのRayで接触があれば移動不可
            float dist = dir * (0.5f * boxCollider.size.x + Time.deltaTime * velocity.x + CollideMargine);
            Vector2 head = transform.position;
            head.y += boxCollider.offset.y + 0.5f * boxCollider.size.y;
            var result = Physics2D.Raycast(head, Vector2.right, dist, blockLayer);
            if (result.collider != null)
            {
                // 接触があったので移動不可
                return 2f * step;
            }

            // 頭の高さで移動した先から足元にRayを飛ばして足元の高さを取得
            head.x += dist;
            result = Physics2D.Raycast(head, Vector2.down, boxCollider.size.y, blockLayer);
            if (result.collider == null)
            {
#if UNITY_EDITOR
                // 衝突がなければ歩ける(本来はないはず)
                Debug.Log($"頭の位置から下に床なし。本来はないはず from={head.x}, {head.y} dist={boxCollider.size.y}");
#endif
                return 0;
            }

            // 段差を計算して返す
            float stepH = result.point.y - (transform.position.y + boxCollider.offset.y - 0.5f * boxCollider.size.y);
            return stepH;
        }

        /// <summary>
        /// 重力落下処理
        /// </summary>
        /// <param name="nextPosition">次の候補座標</param>
        /// <returns>着地修正後の座標</returns>
        Vector2 Fall(Vector2 nextPosition)
        {
            // 長押ししていない時、重力加速
            if ((Time.time - jumpTime) > JumpUpSeconds)
            {
                velocity.y += Time.deltaTime * gravity;
            }
            IsGrounded = false;
            var resultPosition = nextPosition;
            resultPosition.y += Time.deltaTime * velocity.y;

            // 落下
            if (velocity.y < 0)
            {
                var result = Physics2D.BoxCast(
                    nextPosition + boxCollider.offset,
                    boxCollider.size, 0, Vector2.up,
                    Time.deltaTime * velocity.y - CollideMargine, blockLayer);
                if (result.collider != null)
                {
                    // 接触座標が下なら着地
                    if (result.point.y <= nextPosition.y + boxCollider.offset.y)
                    {
                        // 接触したので着地
                        IsGrounded = true;
                        velocity.y = 0;
                        resultPosition.y = result.point.y + CollideMargine + 0.5f * boxCollider.size.y - boxCollider.offset.y;
                    }
                }
            }
            // 上昇
            else
            {
                // 上昇中は頭をぶつけた判定
                var result = Physics2D.BoxCast(
                    nextPosition + boxCollider.offset,
                    boxCollider.size, 0, Vector2.up,
                    Time.deltaTime * velocity.y + CollideMargine, blockLayer);
                if (result.collider != null)
                {
                    // 接触が上なら頭ぶつけ
                    if (result.point.y >= nextPosition.y + boxCollider.offset.y)
                    {
                        // 頭をぶつけたので速度0
                        velocity.y = 0;
                        resultPosition.y = result.point.y - CollideMargine - 0.5f * boxCollider.size.y - boxCollider.offset.y;
                    }
                }
            }

            return resultPosition;
        }

        /// <summary>
        /// ミスした時に呼び出す。
        /// </summary>
        void ToMiss()
        {
            SEPlayer.Play(SEPlayer.SE.Miss);
            CurrentState = State.Miss;
            anim.SetInteger("State", (int)AnimState.Miss);
            velocity = MissVelocityY * Vector2.up;
            GameBehaviour.Instance.ToGameover();
        }

        /// <summary>
        /// クリア処理
        /// </summary>
        void ToClear()
        {
            CurrentState = State.Clear;
            anim.SetInteger("State", (int)AnimState.Clear);
            velocity = Vector2.zero;
            GameBehaviour.Instance.ToClear();
        }

        /// <summary>
        /// クリアアニメからジャンプ時に呼び出す
        /// </summary>
        public void JumpSE()
        {
            SEPlayer.Play(SEPlayer.SE.Jump);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (CurrentState != State.Run) return;

            if (collision.CompareTag("Goal"))
            {
                ToClear();
            }
        }
    }
}