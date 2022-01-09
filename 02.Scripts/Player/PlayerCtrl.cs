using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
public class PlayerCtrl : MonoBehaviour
{
    public Transform Cam;
    public Animator ani;
    private Vector3 moveDir = Vector3.zero;
    private bool Movecan;

    public string currentMapName;
    
    private PlayerStatement playerStatement;
    private PlayerAtk playerAtk;

    [Header("사운드")]
    public AudioClip step;
    private AudioSource audioSource;
    private void Awake()
    {
        //inDungeon.SetActive(false);
        playerStatement = GameManager.Instance.playerStatement;
        audioSource = GetComponent<AudioSource>();
        playerAtk = transform.GetComponent<PlayerAtk>();
    }
    /***********************************************************/
    public void Move(Vector2 inputDir) // 캐릭터이동
    {
        if(inputDir != null)
        {
            Vector2 moveInput = inputDir;

            bool isMove = moveInput.magnitude != 0;
            Movecan = playerAtk.atkCount == 0;//!playerStatement.dead && playerAtk.atkCount == 0;
            ani.SetFloat("MoveSpeed", moveInput.magnitude);
            ani.SetFloat("Horizontal", moveInput.magnitude);
            ani.SetFloat("Vertical", moveInput.magnitude);
            if (isMove && Movecan)
            {
                Vector3 lookForward =
                    new Vector3(Cam.forward.x, 0f, Cam.forward.z).normalized;
                Vector3 lookRight =
                    new Vector3(Cam.right.x, 0f, Cam.right.z).normalized;

                moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

                transform.position += moveDir * Time.deltaTime * 1.5f;
                transform.rotation = Quaternion.Slerp(this.transform.rotation,
                   Quaternion.LookRotation(moveDir), 0.5f);
                playerStatement.currentState = PlayerStatement.State.Move;
            }
        
        }
        else
        {
            Movecan = false;
            playerStatement.currentState = PlayerStatement.State.Idle;
            return;
        }
    }
    private void WalkStepClip()
    {
        if(playerStatement.currentState == PlayerStatement.State.Move &&  ani.GetFloat("MoveSpeed") < 0.3334f)
        {
            audioSource.PlayOneShot(step, 0.3f);
        }
    }
    private void RunStepClip()
    {
        if (playerStatement.currentState == PlayerStatement.State.Move &&  ani.GetFloat("MoveSpeed") < 0.9f && ani.GetFloat("MoveSpeed") > 0.33333f)
        {
            audioSource.PlayOneShot(step, 0.3f);
        }
    }
    private void SprintStepClip()
    {
        if (playerStatement.currentState == PlayerStatement.State.Move && ani.GetFloat("MoveSpeed") >= 0.9f)
        {
            audioSource.PlayOneShot(step, 0.3f);
        }
    }

    private void StepClip()
    {
        audioSource.PlayOneShot(step, 1f);
    }
    /************************************************************************/
}