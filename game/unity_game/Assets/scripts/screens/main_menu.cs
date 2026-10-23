using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.IO;
using TMPro;
using System.Xml;

public class MAIN_MENU : MonoBehaviour
{
    [Header("configuration")]
    [SerializeField] GameObject level_button_prefab;
    [SerializeField] GameObject control_listing_prefab;
    [SerializeField] Color warning_color = new Color(1f, 1f, 0f);
    [SerializeField] Color error_color = new Color(1f, 0f, 0f);
    
    [Header("components")]
    [SerializeField] SESSION_DATA session_data;
    [SerializeField] TMP_InputField input_field;
    [SerializeField] TMP_Text input_feedback_text;
    [SerializeField] GameObject naming_menu;
    [SerializeField] GameObject selection_menu;
    [SerializeField] GameObject level_buttons_content_object;
    [SerializeField] GameObject control_listings_content_object;



    void Start()
    {
        this.input_field.onValueChanged.AddListener((text) => this.validate_name(text));

        this.create_control_listings();

        this.create_level_buttons();
    }
    


    public void create_control_listings()
    {
        InputActionMap character_actions = InputSystem.actions.FindActionMap("character");
        foreach (var input_action in character_actions.actions)
        {
            GameObject control_listing_object = Instantiate(this.control_listing_prefab, this.control_listings_content_object.transform);
            control_listing_object.transform.Find("name").GetComponent<TMP_Text>().text = $"{input_action.name}:";
            control_listing_object.transform.Find("value").GetComponent<TMP_Text>().text = input_action.GetBindingDisplayString() switch
            {
                "Delta" => "mouse",
                "W|Up Arrow/A|Left Arrow/S|Down Arrow/D|Right Arrow" => "wasd",
                "LMB" => "left mouse click",
                "E" => "e",
                "RMB" => "right mouse click",
                "Scroll" => "scroll",
                "\t" => "tab",
                _ => input_action.GetBindingDisplayString(),
            };
        }
    }

    public void create_level_buttons()
    {
    TextAsset[] levelFiles = Resources.LoadAll<TextAsset>("Levels");

    if (levelFiles.Length == 0)
    {
        Debug.LogError("No level files found in Resources/Levels");
        return;
    }


    Debug.Log($"Levels found: {levelFiles.Length}");

    foreach (TextAsset levelFile in levelFiles)
    {
        string fileName = levelFile.name + ".xml";

        Debug.Log($"Filename: '{fileName}'");

        if (fileName.Contains(',') || fileName.Contains('\n'))
        {
            Debug.Log($"'{fileName}' file name contains ',' or '\\n'");
            continue;
        }

        Debug.Log("Log 1...");

        GameObject levelButtonObject =
            Instantiate(level_button_prefab, level_buttons_content_object.transform);

        levelButtonObject.GetComponent<BUTTON>().set_text(fileName);

        Debug.Log("Log 2...");

        levelButtonObject.GetComponent<Button>()
            .onClick.AddListener(() => load_level(fileName));

        Debug.Log("Log 3...");
    }
}

    public void init_level_buttons()
    {
        foreach (Transform button_object in level_buttons_content_object.transform)
        {
            string prerequisite = this.get_prerequisite(button_object.GetComponent<BUTTON>().get_text());
            if (prerequisite != null && !player_meets_prerequisite(prerequisite))
            {
                button_object.gameObject.SetActive(false);
            }
            else
            {
                button_object.gameObject.SetActive(true);
            }
        }
    }
    
    public string get_prerequisite(string level_name)
    {
    TextAsset levelFile = Resources.Load<TextAsset>(
"Levels/" + Path.GetFileNameWithoutExtension(level_name));

    if (levelFile == null)
    {
        Debug.LogError($"Level '{level_name}' not found.");
        return null;
    }

    using StringReader stringReader = new StringReader(levelFile.text);
    using XmlReader reader = XmlReader.Create(stringReader);
    while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "prerequisite")
            {
                return reader.ReadElementContentAsString();
            }
        }
        return null;
    }

public bool player_meets_prerequisite(string prerequisite)
{
    return LeaderboardManager.IsLevelCompleted(
        this.session_data.player_name,
        prerequisite);
}

bool validate_name(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            this.input_feedback_text.text = "name can not be empty.";
            this.input_feedback_text.color = this.error_color;
            return false;
        }
        if (name.Contains(',') || name.Contains('\n'))
        {
            this.input_feedback_text.text = "name can not contain ',' or '\\n'.";
            this.input_feedback_text.color = this.error_color;
            return false;
        }
        if (name_is_in_use(name))
        {
            this.input_feedback_text.text = $"'{name}' is already in use";
            this.input_feedback_text.color = this.warning_color;
            return true;
        }
        this.input_feedback_text.text = "";
        return true;
    }

bool name_is_in_use(string name)
{
    return LeaderboardManager.PlayerExists(name);
}

public void save_name()
    {
        string name = this.input_field.text;
        if (this.validate_name(name))
        {
            this.session_data.player_name = name;
        LeaderboardManager.RegisterPlayer(name);
        this.init_level_buttons();
            this.naming_menu.SetActive(false);
            this.selection_menu.SetActive(true);
        }
    }
    
    public void load_level(string level_name)
    {
        this.session_data.level_name = level_name;
        SceneManager.LoadScene(GLOBAL.level_scene_number);
    }
    
    public void quit()
    {
        Application.Quit();
    }
}
    