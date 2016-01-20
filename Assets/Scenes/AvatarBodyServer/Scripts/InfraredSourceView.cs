using UnityEngine;
using System.Collections;

public class InfraredSourceView : MonoBehaviour 
{
    public GameObject MultiSourceManager;
    private MultiSourceManager _MultiManager;
    //public GameObject InfraredSourceManager;
    //private InfraredSourceManager _InfraredManager;
    
    void Start () 
    {
        gameObject.renderer.material.SetTextureScale("_MainTex", new Vector2(-1, 1));
    }
    
    void Update()
    {

        if (MultiSourceManager == null)
        {
            return;
        }

        _MultiManager = MultiSourceManager.GetComponent<MultiSourceManager>();
        if (_MultiManager == null)
        {
            return;
        }

        gameObject.renderer.material.mainTexture = _MultiManager.GetInfraredTexture();
    
    //    if (InfraredSourceManager == null)
    //    {
    //        return;
    //    }
        
    //    _InfraredManager = InfraredSourceManager.GetComponent<InfraredSourceManager>();
    //    if (_InfraredManager == null)
    //    {
    //        return;
    //    }
    
    //    gameObject.renderer.material.mainTexture = _InfraredManager.GetInfraredTexture();
    }
}
