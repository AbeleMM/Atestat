using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    private SpriteRenderer sprite;
    protected bool canMove;

	protected virtual void Start() {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
        sprite = rb2D.gameObject.GetComponent<SpriteRenderer>();
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if(hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end, xDir, yDir));
            return true;
        }

        return false;
    }
	
    protected IEnumerator SmoothMovement(Vector3 end, int xDir, int yDir)
    {
        if (rb2D.gameObject.tag == "Player" && yDir == 0)
                rb2D.GetComponentInParent<Animator>().SetTrigger("playerRun");

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        while(sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        canMove = Move(xDir, yDir, out hit);

        if ((rb2D.gameObject.tag == "Player" && ((sprite.flipX == false && xDir == -1) || (sprite.flipX == true && xDir == 1))) || (rb2D.gameObject.tag == "Enemy" && ((sprite.flipX == false && xDir == 1) || (sprite.flipX == true && xDir == -1))))
            sprite.flipX = !sprite.flipX;

        if (hit.transform == null)
            return;

        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent, xDir, yDir);
    }

    protected abstract void OnCantMove<T>(T component, int xDir, int yDir)
        where T : Component;
}
