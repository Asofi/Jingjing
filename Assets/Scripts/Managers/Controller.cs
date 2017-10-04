using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public Rigidbody2D LeftBate;
    public Rigidbody2D RightBate;

    [SerializeField]
    float bateSpeed = 1000f;			///< скорость биты вверх
	[SerializeField]
    float bateRelease = -500f;			///< скорость биты вниз
	[SerializeField]
    float maxBateAngle = 45;			///< максимальный угол поднятия биты
	[SerializeField]
    float minBateAngle = 0;             ///< минимальный угол биты

    /// какая бита используется
    private enum Bate
    {
        LEFT,							///< правая
		RIGHT							///< левая
	}

    /** 
		Произвести удар выбранной битой
		\param[in] Bate выбранная бита
	*/
    private void Bump(Bate bate)
    {
        float zAngle = 0;
        if (bate == Bate.LEFT)
        {
            zAngle = LeftBate.transform.eulerAngles.z;
            zAngle += bateSpeed * Time.fixedDeltaTime;
            zAngle = zAngle > maxBateAngle ? maxBateAngle : zAngle;
            LeftBate.MoveRotation(zAngle);
        }
        else
        {
            zAngle = RightBate.transform.eulerAngles.z;
            zAngle -= bateSpeed * Time.fixedDeltaTime;
            // \todo : fix this condition
            if (zAngle < 360 - maxBateAngle && zAngle > maxBateAngle)
                zAngle = 360 - maxBateAngle;
            /////////////////
            RightBate.MoveRotation(zAngle);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!SuperManager.Instance.GM.IsStarted)
            return;
        Release();
        // мышка
#if UNITY_EDITOR
        if (Input.GetMouseButton(0) &&
                    Input.mousePosition.x < Screen.width / 2)
                {
                    Bump(Bate.LEFT);
                }
                else if (Input.GetMouseButton(0) &&
                      Input.mousePosition.x > Screen.width / 2)
                {
                    Bump(Bate.RIGHT);
                }

        if (Input.GetKey(KeyCode.Slash))
            Bump(Bate.RIGHT);
        if (Input.GetKey(KeyCode.Z))
            Bump(Bate.LEFT);

#else

        // мобильное устройство
        var touches = Input.touches;
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (touches[i].position.x < Screen.width / 2)
                        Bump(Bate.LEFT);
                    else if (touches[i].position.x > Screen.width / 2)
                        Bump(Bate.RIGHT);
                    if (touches[i].phase == TouchPhase.Began)
                    {
                        if (PlayerPrefs.GetInt("choosed_skin") != 2) {
                            AudioManager.PlayAudio("Mechanizms");
                        }
                    }
                }
#endif
    }

    private void Release()
    {
        // \todo : optimize this
        float zAngleRight = RightBate.transform.eulerAngles.z;
        zAngleRight -= bateRelease * Time.fixedDeltaTime;
        if (zAngleRight > minBateAngle && zAngleRight < maxBateAngle)
            zAngleRight = minBateAngle;
        RightBate.MoveRotation(zAngleRight);

        float zAngleLeft = LeftBate.transform.eulerAngles.z;
        zAngleLeft += bateRelease * Time.fixedDeltaTime;
        if (zAngleLeft < minBateAngle) zAngleLeft = minBateAngle;
        LeftBate.MoveRotation(zAngleLeft);
        ///////////////
    }
}
