﻿using UnityEngine;
using System.Collections;

public partial class Menu : MonoBehaviour {

	//main menu BUTTONS rects
	LTRect startButtonRect;
	LTRect optionsButtonRect;
	LTRect exitButtonRect;
	LTRect creditsButtonRect;
	//start menu button rect
	LTRect continueButtonRect;
	LTRect newGameButtonRect;
	LTRect loadButtonRect;
	//OPTIONS RECTS
		//to do
	//GENERAL RECTS
	LTRect yesButtonRect;
	LTRect noButtonRect;	
	LTRect previousButtonRect;
	//LABELS RECTS
	LTRect doYouWantToStartNewGame;
	LTRect doYouwantToExitLabelRect;
	//texture Rects
	Rect creditsTextureRect;
	Rect guiTextureRect;

void goToMenu(MenuSelector nextMenu){
		//hide old buttons
	switch (currMenu) {
		case MenuSelector.MAIN:
			hideButtons(-1,0,0,startButtonRect,optionsButtonRect,exitButtonRect,creditsButtonRect);
			break;
		case MenuSelector.EXIT:
			hideButtons(1,0,0,doYouwantToExitLabelRect,yesButtonRect,noButtonRect);
			break;
		case MenuSelector.PLAY:
			hideButtons(1,0,0,continueButtonRect,newGameButtonRect,loadButtonRect);
			hideButtons(0,0.3f,0,previousButtonRect);
			break;
		case MenuSelector.OPTIONS:
			hideButtons(0,0.3f,0,previousButtonRect);
			//to do
			break;
		case MenuSelector.STARTNEWGAME:
			hideButtons(1,0,0,doYouWantToStartNewGame,yesButtonRect,noButtonRect);
			break;
		}
		currMenu = nextMenu;
		//show new buttons
	switch (currMenu) {
		case MenuSelector.MAIN:
			showButtons(1,0,0,startButtonRect,optionsButtonRect,exitButtonRect,creditsButtonRect);
			break;
		case MenuSelector.EXIT:
			showButtons(-1,0,0,doYouwantToExitLabelRect,yesButtonRect,noButtonRect);
			 break;
		case MenuSelector.PLAY:
			showButtons(-1,0,0,continueButtonRect,newGameButtonRect,loadButtonRect);
			showButtons(0,-0.3f,0.5f,previousButtonRect);
			break;
		case MenuSelector.OPTIONS:
			showButtons(0,-0.3f,0.5f,previousButtonRect);
			//to do
			break;
		case MenuSelector.STARTNEWGAME:
			showButtons(-1,0,0,doYouWantToStartNewGame,yesButtonRect,noButtonRect);
			break;
		}
	}
	void setUpRects()
	{
		float verticalButtonsPos = Screen.height / 2 - 0.5f * height;
		float verticalOffset = 1.3f * height;
		float horizontallOffset = 1.3f * width;
		float horizontalButtonPos = Screen.width / 2 - 0.5f * width;
		//textures setup
		guiTextureRect = new Rect (0, 0, Screen.width, Screen.height);
		creditsTextureRect = new Rect (Screen.width / 10, Screen.height / 10, Screen.width*0.8f, Screen.height * 0.8f);
		//MAIN BUTTONS SETUP
		startButtonRect   = new LTRect (horizontalButtonPos,verticalButtonsPos-verticalOffset, width, height);
		optionsButtonRect = new LTRect (horizontalButtonPos,verticalButtonsPos, width, height);
		exitButtonRect    = new LTRect (horizontalButtonPos,verticalButtonsPos+verticalOffset, width, height);
		creditsButtonRect = new LTRect (horizontalButtonPos + 1.2f*horizontallOffset, verticalButtonsPos + 1.6f*verticalOffset, width/2, height/2);
		//GENERAL BUTTONS SETUP
		yesButtonRect = new LTRect (horizontalButtonPos-0.5f*horizontallOffset+ScrWidth,verticalButtonsPos,width,height);
		noButtonRect = new LTRect (horizontalButtonPos+0.5f*horizontallOffset+ScrWidth,verticalButtonsPos,width,height);
		previousButtonRect = new LTRect (horizontalButtonPos - 0.8f*horizontallOffset, verticalButtonsPos + 1.6f*verticalOffset+0.3f*ScrHeight, width/2, height/2);
		//PLAY BUTTONS SETUP
		continueButtonRect = new LTRect (horizontalButtonPos+ScrWidth,verticalButtonsPos-verticalOffset, width, height);
		newGameButtonRect = new LTRect (horizontalButtonPos+ScrWidth,verticalButtonsPos, width, height);
		loadButtonRect = new LTRect (horizontalButtonPos+ScrWidth,verticalButtonsPos+verticalOffset, width, height);
		//LABELS RECTS
		doYouwantToExitLabelRect = new LTRect (horizontalButtonPos+ScrWidth, verticalButtonsPos-0.4f*verticalOffset, width, 0.4f*height);
		doYouWantToStartNewGame = new LTRect (horizontalButtonPos+ScrWidth, verticalButtonsPos-0.4f*verticalOffset, width, 0.4f*height);
	}

	IEnumerator moveRects(float interval,float timeToMove,float delay,float endValX/*o ile przesunac w prawo relatywne do ekranu*/,
	                      float endValY/*o ile przesunac w gore relatywne do ekranu*/,params LTRect[] rectts)
	{
		yield return new WaitForSeconds(delay);
		foreach (var rectt in rectts) 
		{
			LeanTween.move (rectt, new Vector2 (rectt.rect.x + ScrWidth * endValX, rectt.rect.y + ScrHeight*endValY), timeToMove).setEase (LeanTweenType.easeOutQuad);
			yield return new WaitForSeconds(interval);
		}
	}
	void showButtons(float directionX, float directionY,float additonaldelay,params LTRect[] rectts)
	{
		StartCoroutine(moveRects(0.2f,0.25f,0.2f+additonaldelay,directionX,directionY,rectts));
	}
	void hideButtons(float directionX,float directionY,float additonaldelay,params LTRect[] rectts)
	{
		StartCoroutine(moveRects(0.2f,0.25f,additonaldelay,directionX,directionY,rectts));
	}
}