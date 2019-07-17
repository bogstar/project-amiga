using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Exists as a MonoBehaviour in a battle scene.
public class InputManager : MonoBehaviour
{
	public List<PlayerCombatObject> playersToManage;

	public Transform GUI;

	public CameraController cameraController;
	public DisplayCard displayCard;

	public Texture2D panCursor;
	public Vector2 panCursorOffset;
	public Texture2D orbitCursor;
	public Vector2 orbitCursorOffset;

	MouseState mouseState;

	Vector3 currentMousePos;
	Vector3 lastMousePos;
	Vector3 currentMousePosPix;
	Vector3 lastMousePosPix;

	PlayerHand currentHand;

	EventSystem eventSystem;
	CombatManager battleManager;


	void Awake()
	{
		eventSystem = EventSystem.current;
		battleManager = FindObjectOfType<CombatManager>();
	}

	void Start()
	{
		HideAllTargettingButtons();
		UnhoverOverCard();
		ChangeMouseState(MouseState.Free);
	}

	void Update()
	{
		// Calculate zoom.
		cameraController.targetZoom = Input.mouseScrollDelta.y;

		foreach (var enemy in battleManager.enemies)
		{
			enemy.ClickableTile.Refresh();
		}
		foreach (var player in battleManager.players)
		{
			player.ClickableTile.Refresh();
		}

		// Mouse cursor is over UI AND is not in any state.
		if (eventSystem.IsPointerOverGameObject() && mouseState == MouseState.Free)
		{
			// Let's get all the info on what is under the mouse.
			PointerEventData pointer = new PointerEventData(eventSystem);
			pointer.position = Input.mousePosition;
			List<RaycastResult> raycastResults = new List<RaycastResult>();

			// All results are stored in the raycastResults list.
			// Let's see if we need any of those.
			eventSystem.RaycastAll(pointer, raycastResults);

			// We need to see if we are over player hand.
			List<PlayerHand> playerHand = LookForPlayerHand(raycastResults);

			// If there is only one player hand under the cursor,
			// we can go ahead and tell the hand to update itself.
			if (playerHand.Count == 1)
			{
				currentHand = playerHand[0];

				if(currentHand != null)
				{
					List<CardInHand> cardsUnderMouse = currentHand.GetAllCards(raycastResults);

					CardInHand selectedCard;

					currentHand.UpdateXD(cardsUnderMouse, out selectedCard);

					if (selectedCard != null)
					{
						DisplayCard(true, selectedCard);
					}
				}
			}
			// There are more hands under cursor OR there aren't any.
			else
			{
				UnhoverOverCard();
			}

			// If the mouse is over GUI, break out.
			foreach (var rr in raycastResults)
			{
				if(rr.gameObject.transform.root == GUI)
				{
					return;
				}
			}
		}
		// Cursor is not in the UI OR is in another state.
		else if (!eventSystem.IsPointerOverGameObject() && mouseState == MouseState.Free)
		{
			UnhoverOverCard();

			Ray ray = cameraController.camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			int layerMask = 1 << (int)Layer.Clickable;

			foreach (var enemy in battleManager.enemies)
			{
				enemy.clickableTileClickable = true;

				/*
				if (battleManager.multipleSpecificMode)
				{
					foreach (var entity in battleManager.multipleSpecificModeList)
					{
						((EnemyCombatObject)entity).clickableTileClickable = false;
						entity.ClickableTile.Selected = true;
						((EnemyCombatObject)entity).ClickableTile.GetComponent<Renderer>().material.color = Color.cyan;
					}
				}
				*/
			}

			if (Physics.Raycast(ray, out hit, 100f, layerMask, QueryTriggerInteraction.Collide))
			{
				HealthEntityCombatObject entityHit = hit.transform.gameObject.GetComponentInParent<HealthEntityCombatObject>();

				if (entityHit != null)
				{
					if(entityHit is PlayerCombatObject)
					{
						entityHit.ClickableTile.MouseOver(true);
					}
					if (entityHit is EnemyCombatObject)
					{
						if(battleManager.currentSelectedCard.card.abilityStepsWithTargetingData[battleManager.stepNumber].targetingData.targets == TargetingData_Base.Target.Multiple)
						{
							foreach (var enemy in battleManager.GetNearbyEnemies(((EnemyCombatObject)entityHit), battleManager.currentSelectedCard.card.abilityStepsWithTargetingData[battleManager.stepNumber].targetingData.numberOfTargets))
							{
								enemy.ClickableTile.MouseOver(true);
							}
						}
						else if (battleManager.multipleSpecificMode)
						{
							foreach (var entity in battleManager.multipleSpecificModeList)
							{
								((EnemyCombatObject)entity).ClickableTile.GetComponent<Renderer>().material.color = Color.cyan;
							}
						}
						entityHit.ClickableTile.MouseOver(true);
					}

					if (Input.GetMouseButtonDown(0))
					{
						if(entityHit is EnemyCombatObject)
						{
							if (((EnemyCombatObject)entityHit).clickableTileClickable)
							{
								ButtonInput_SelectTarget(entityHit);
							}
						}
						else
						{
							ButtonInput_SelectTarget(entityHit);
						}
					}
				}
			}
		}

		HandleCameraTransform();
	}

	void LateUpdate()
	{
		cameraController.UpdateCameraTransform();
		lastMousePos = Utility.GetCursorInWorldPosition(Input.mousePosition, cameraController.camera, 0);
		lastMousePosPix = Input.mousePosition;
	}

	public void ButtonInput_SelectTarget(HealthEntityCombatObject entity)
	{
		battleManager.AddTarget(entity, HideAllTargettingButtons);
	}
	
	public void ButtonInput_SelectCard(CardGameObject cardGO, PlayerCombatObject player)
	{
		// Player has selected a card from the hand.

		// Let's first unselect all player's cards.
		foreach(var p in battleManager.players)
		{
			p.hand.UnselectCard();
			p.hand.RecalculateCardsPosition();
		}

		// Ignore if the player does not have control over the card.
		if (!playersToManage.Contains(player))
		{
			//return;
		}

		// Pass the info to Battle Manager.
		battleManager.SelectCard(cardGO, player, player.hand.SelectCard, player.hand.RecalculateCardsPosition);
	}

	void HideAllTargettingButtons()
	{
		foreach (var p in battleManager.players)
		{
			p.AllowTargetable(false);
		}

		battleManager.enemyManager.SetEnemiesTargetable(false, battleManager.enemies);
	}

	void HandleCameraTransform()
	{
		if (mouseState == MouseState.Free)
		{
			if (Input.GetMouseButtonDown(2))
			{
				ChangeMouseState(MouseState.Pan);
			}
			else if (Input.GetMouseButtonDown(1))
			{
				ChangeMouseState(MouseState.Orbit);
			}
		}
		else if (mouseState == MouseState.Pan)
		{
			if (Input.GetMouseButtonUp(2))
			{
				if (Input.GetMouseButton(1))
				{
					ChangeMouseState(MouseState.Orbit);
				}
				else
				{
					ChangeMouseState(MouseState.Free);
				}
			}
			currentMousePos = Utility.GetCursorInWorldPosition(Input.mousePosition, cameraController.camera, 0);
			cameraController.targetRigPosition = currentMousePos - lastMousePos;
		}
		else if (mouseState == MouseState.Orbit)
		{
			if (Input.GetMouseButtonUp(1))
			{
				if (Input.GetMouseButton(2))
				{
					ChangeMouseState(MouseState.Pan);
				}
				else
				{
					ChangeMouseState(MouseState.Free);
				}
			}
			currentMousePosPix = Input.mousePosition;
			cameraController.targetRigRotation = currentMousePosPix - lastMousePosPix;
		}
	}

	void DisplayCard(bool display, CardInHand card)
	{
		if (display)
		{
			displayCard.gameObject.SetActive(true);
			displayCard.Display(card.GetComponent<CardGameObject>());
		}
		else
		{
			displayCard.gameObject.SetActive(false);
		}
	}

	void UnhoverOverCard()
	{
		foreach (var p in FindObjectOfType<CombatManager>().players)
		{
			p.hand.UnhoverOverCard();
			p.hand.RecalculateCardsPosition();
		}
		if (currentHand != null)
		{
			currentHand = null;
		}
		DisplayCard(false, null);
	}

	void HoverOverCard(CardInHand card)
	{
		UnhoverOverCard();
		currentHand = card.hand;
		displayCard.gameObject.SetActive(true);
		displayCard.Display(card.GetComponent<CardGameObject>());
		currentHand.RecalculateCardsPosition();
	}

	List<PlayerHand> LookForPlayerHand(List<RaycastResult> raycastResults)
	{
		List<PlayerHand> playerHandsFound = new List<PlayerHand>();

		foreach (var r in raycastResults)
		{
			if (r.gameObject.GetComponent<CardInHand>() != null)
			{
				PlayerHand playerHandFound = r.gameObject.GetComponent<CardInHand>().GetComponentInParent<PlayerHand>();

				if (!playerHandsFound.Contains(playerHandFound))
				{
					playerHandsFound.Add(playerHandFound);
				}
			}
		}

		return playerHandsFound;
	}

	void ChangeMouseState(MouseState newState)
	{
		mouseState = newState;
		switch (newState)
		{
			case MouseState.Free:
				cameraController.shouldOrbit = false;
				cameraController.shouldPan = false;
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
				break;
			case MouseState.Orbit:
				cameraController.shouldOrbit = true;
				cameraController.targetRigPosition = Vector3.zero;
				lastMousePosPix = Input.mousePosition;
				Cursor.SetCursor(orbitCursor, orbitCursorOffset, CursorMode.Auto);
				break;
			case MouseState.Pan:
				cameraController.shouldPan = true;
				cameraController.targetRigRotation = Vector3.zero;
				lastMousePos = Utility.GetCursorInWorldPosition(Input.mousePosition, cameraController.camera, 0);
				Cursor.SetCursor(panCursor, panCursorOffset, CursorMode.Auto);
				break;
		}
	}

	public enum MouseState { Free, Pan, Orbit }
}