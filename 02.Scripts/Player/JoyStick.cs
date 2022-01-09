using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class JoyStick : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerDownHandler
{
    [SerializeField]
    public RectTransform stick;
    [SerializeField]
    private RectTransform rectTransform;

    [SerializeField, Range(10, 140)]
    private float stickRange;

    private Vector2 inputDir;
    public bool isInput;

    Vector2 startPos = Vector2.zero;

    private PlayerCtrl player;

   
    private void Awake()
    {
        startPos = rectTransform.anchoredPosition;
       
    }
    void Start()
    {
        player = GameManager.Instance.player;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(!GameManager.Instance.playerStatement.dead)
        {
            rectTransform.position = eventData.position;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        CtrlStick(eventData);
        isInput = true;

    }
    
    public void OnDrag(PointerEventData eventData)
    {
        CtrlStick(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(GameManager.Instance.playerStatement.dead)
        {
            return;
        }

        stick.anchoredPosition = Vector2.zero;
        rectTransform.anchoredPosition = startPos;
        isInput = false;
        player.Move(Vector2.zero);
        player.ani.Play("Sprint_Stop");
    }

    private void CtrlStick(PointerEventData eventData)
    {
        Vector2 inputPos = eventData.position - rectTransform.anchoredPosition;
        Vector2 inputVector =
            inputPos.magnitude < stickRange ? inputPos : inputPos.normalized * stickRange;
        stick.anchoredPosition = inputVector;
        inputDir = inputVector / stickRange;
    }

    private void InputCtrlVector()
    {
        player.Move(inputDir);
    }

    void Update()
    {
        if (isInput && GameManager.Instance.playerStatement.currentState != PlayerStatement.State.Attack)
        {
            InputCtrlVector();
        }
        else
        {
            stick.anchoredPosition = Vector2.zero;
            rectTransform.anchoredPosition = startPos;
            player.Move(Vector2.zero);
        }
    }

    
}
