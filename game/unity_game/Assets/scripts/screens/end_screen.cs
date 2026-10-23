using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class END_SCREEN : MonoBehaviour
{
    [Header("components")]
    [SerializeField] SESSION_DATA session_data;
    [SerializeField] TMP_Text score_value_text;
    [SerializeField] TMP_Text time_value_text;

    

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        this.score_value_text.text = this.session_data.score.ToString();
        int minutes = Mathf.FloorToInt(this.session_data.time / 60);
        float seconds = this.session_data.time % 60;
        this.time_value_text.text = $"{minutes:D2}:{seconds:00.000}";

        LeaderboardManager.SaveResult(
    session_data.player_name,
    session_data.level_name,
    session_data.time,
    session_data.score);
    }
    
    public void load_main_menu()
    {
        SceneManager.LoadScene(GLOBAL.main_menu_scene_number);
    }
}
