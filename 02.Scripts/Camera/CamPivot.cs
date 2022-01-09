using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPivot : MonoBehaviour
{
    public PlayerCtrl player;
    [SerializeField]
    private Camera cam;
    Vector3 originPos;
    Quaternion SPPos;
    Quaternion originRot;
    private void Awake()
    {
        originRot = Quaternion.Euler(0, 0f, 0);
        SPPos = new Quaternion(-15.79f, -133f, 0,0);
    }
    private void Start()
    {
        originPos = cam.transform.localPosition;
        transform.rotation = originRot;
    }

    private void FixedUpdate()
    {
        Vector3 pos = this.transform.position;
        Vector3 min = player.transform.position + new Vector3(0, 0.8f, 0);
        this.transform.position = Vector3.Slerp(pos, min, 0.05f);
    }

    public void ShakeCam(float time, float amount)
    {
        StartCoroutine(ShakeCamCoroutine(time, amount));
    }
    public void SPCam()
    {
        StartCoroutine(MoveCamSPSkill());
    }
    private IEnumerator MoveCamSPSkill()
    {
        float timer = 0;
        while (timer <= 2.5f)
        {
            timer += Time.unscaledDeltaTime;
            Time.timeScale = 0.1f;

            transform.rotation = Quaternion.Slerp(transform.rotation, SPPos, Time.unscaledDeltaTime * 0.02f);
            cam.fieldOfView = 45f;

            yield return null;
        }
        Time.timeScale = 1f;
        cam.fieldOfView = 60f;
        transform.rotation = originRot;
        yield break;
    }

    private IEnumerator ShakeCamCoroutine(float time, float amount)
    {
        float timer = 0;

        while(timer <= time)
        {
            cam.transform.localPosition = (Vector3)Random.insideUnitCircle * amount + originPos;

            timer += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = originPos;
    }
}
