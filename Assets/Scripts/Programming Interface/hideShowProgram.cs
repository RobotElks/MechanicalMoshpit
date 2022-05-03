using UnityEngine;
using UnityEngine.UI;

public class hideShowProgram : MonoBehaviour
{

    public GameObject Panel;
    public Button hideShowButton;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = hideShowButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void TaskOnClick()
    {
        if(Panel.activeSelf){
            Panel.SetActive(false);
        }
        else{
            Panel.SetActive(true);
        }
    }
}
