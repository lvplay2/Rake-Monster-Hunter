using UnityEngine;
using UnityEngine.UI;

public class MedKitUsing : Item
{
	public FP_Input playerInput;

	public Bar bar;

	public TextCounter counter;

	public Image btnImg;

	public float useTime = 3f;

	public int count = 1;

	public int countMax = 3;

	public Creature creature;

	public RewardForMedKit reward;

	protected float currTime;

	protected bool isNowUsing;

	protected Heals heals;

	protected Color btnPositiveColor = new Color(1f, 1f, 1f, 1f);

	protected Color btnNegativeColor = new Color(1f, 1f, 1f, 0.05f);

	protected override void Start()
	{
		base.Start();
		heals = creature.GetComponent<Heals>();
		UpdateCounter();
	}

	public void StartUsingMedKit()
	{
		if (count <= 0)
		{
			if (!reward.gameObject.activeSelf)
			{
				reward.Show();
			}
			else
			{
				reward.Hide();
			}
		}
		else if (!isNowUsing && heals.hp < heals.hpMax)
		{
			bar.gameObject.SetActive(true);
			isNowUsing = true;
			currTime = useTime;
		}
	}

	public int TakeAll()
	{
		int num = countMax - count;
		count += num;
		UpdateCounter();
		return num;
	}

	public bool TakeOne()
	{
		if (count < countMax)
		{
			count++;
			UpdateCounter();
			return true;
		}
		return false;
	}

	public override void UpdateCounter()
	{
		if ((bool)counter)
		{
			counter.SetValue(count + "/" + countMax);
		}
		if (count > 0)
		{
			btnImg.color = btnPositiveColor;
		}
		else
		{
			btnImg.color = btnNegativeColor;
		}
	}

	private void Update()
	{
		if (!isNowUsing)
		{
			return;
		}
		if ((double)playerInput.MoveInput().sqrMagnitude > 0.01 || playerInput.Run() || playerInput.Shoot())
		{
			currTime = 3f;
			bar.SetValue(currTime / useTime);
			return;
		}
		currTime -= Time.deltaTime;
		bar.SetValue(currTime / useTime);
		if (currTime <= 0f)
		{
			bar.gameObject.SetActive(false);
			count--;
			creature.Heal(1f);
			isNowUsing = false;
			UpdateCounter();
		}
	}
}
