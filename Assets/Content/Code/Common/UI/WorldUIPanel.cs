using UnityEngine;
using System.Collections;

public class WorldUIPanel : Singleton<WorldUIPanel>
{
    public UIPanel Panel;
    public UIFont BookFont01;
    public UIFont SpeechFont01;
    public Vector3 BookFont01Size = new Vector3(1,1,1);
    public Vector3 SpeechFont01Size = new Vector3(1,1,1);
    public UISlicedSprite SpeechBubbleTemplate;

    private void Start()
    {
        if (!Panel)
        {
            Panel = GetComponent<UIPanel>();
        }
    }


}
