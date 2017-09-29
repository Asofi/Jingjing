using UnityEngine;

public class ULTRA_DEATH_SOUND_TRIGGER : MonoBehaviour {

    #region Private Variables
    #endregion

    #region Public Variables
    #endregion

    #region MonoBehavior Messages

    private void OnTriggerEnter2D(Collider2D collision) {
        AudioManager.PlayAudio("Death");
}

    #endregion

    #region Event Messages
    #endregion

    #region Public Functions
    #endregion

    #region Private Functions
    #endregion
}
