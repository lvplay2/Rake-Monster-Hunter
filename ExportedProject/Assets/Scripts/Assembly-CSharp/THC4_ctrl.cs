using UnityEngine;

public class THC4_ctrl : MonoBehaviour
{
	private Animator anim;

	private CharacterController controller;

	private int battle_state;

	public float speed = 6f;

	public float runSpeed = 3f;

	public float turnSpeed = 60f;

	public float gravity = 20f;

	private Vector3 moveDirection = Vector3.zero;

	private float w_sp;

	private float r_sp;

	private void Start()
	{
		anim = GetComponent<Animator>();
		controller = GetComponent<CharacterController>();
		w_sp = speed;
		r_sp = runSpeed;
		battle_state = 0;
		runSpeed = 1f;
	}

	private void Update()
	{
		if (Input.GetKey("1"))
		{
			anim.SetInteger("battle", 0);
			battle_state = 0;
			runSpeed = 1f;
		}
		if (Input.GetKey("2"))
		{
			anim.SetInteger("battle", 1);
			battle_state = 1;
			runSpeed = r_sp;
		}
		if (Input.GetKey("3"))
		{
			anim.SetInteger("battle", 2);
			battle_state = 2;
			runSpeed = r_sp;
		}
		if (Input.GetKey("up"))
		{
			anim.SetInteger("moving", 1);
		}
		else
		{
			anim.SetInteger("moving", 0);
		}
		if (Input.GetKey("down") && battle_state < 2)
		{
			anim.SetInteger("moving", 12);
			runSpeed = 1f;
		}
		if (Input.GetKeyUp("down"))
		{
			anim.SetInteger("moving", 0);
			if (battle_state == 1)
			{
				runSpeed = 1f;
			}
			else if (battle_state > 1)
			{
				runSpeed = r_sp;
			}
		}
		if (Input.GetMouseButtonDown(0))
		{
			anim.SetInteger("moving", 2);
		}
		if (Input.GetMouseButtonDown(1))
		{
			anim.SetInteger("moving", 3);
		}
		if (Input.GetMouseButtonDown(2))
		{
			anim.SetInteger("moving", 4);
		}
		if (Input.GetKeyDown("x"))
		{
			anim.SetInteger("moving", 5);
		}
		if (Input.GetKeyDown("space"))
		{
			anim.SetInteger("moving", 6);
		}
		if (Input.GetKeyDown("c"))
		{
			anim.SetInteger("moving", 7);
		}
		if (Input.GetKeyDown("u"))
		{
			if (battle_state < 2)
			{
				int num = Random.Range(0, 2);
				if (num == 1)
				{
					anim.SetInteger("moving", 8);
				}
				else
				{
					anim.SetInteger("moving", 9);
				}
			}
			else if (battle_state == 2)
			{
				int num2 = Random.Range(0, 2);
				if (num2 == 1)
				{
					anim.SetInteger("moving", 10);
				}
				else
				{
					anim.SetInteger("moving", 11);
				}
			}
		}
		if (Input.GetKeyDown("i") && battle_state < 2)
		{
			anim.SetInteger("moving", 13);
		}
		if (Input.GetKeyDown("i") && battle_state == 2)
		{
			anim.SetInteger("moving", 14);
		}
		if (controller.isGrounded)
		{
			moveDirection = base.transform.forward * Input.GetAxis("Vertical") * speed * runSpeed;
			float axis = Input.GetAxis("Horizontal");
			base.transform.Rotate(0f, axis * turnSpeed * Time.deltaTime, 0f);
		}
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
	}
}
