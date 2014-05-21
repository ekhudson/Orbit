using UnityEngine;
using System.Collections;

    /// <summary>
    /// Title: Grendel Engine
    /// Author: Elliot Hudson
    /// Date: Jan 10, 2012
    /// 
    /// Filename: BaseObject.cs
    /// 
    /// Summary: Extends MonoBehavior and provides
    /// usual functionality for game design (such as easy hiding/disabling of objects)
    /// as well as storing reference to commonly used components (ie. transform/gameobject).
    /// Primarily, the BaseObject class is concerned with the technical functions
    /// of the base Monobehavior scripts, providing direct access and control
    /// to those scripts. Things such as Health and Damage, however, are handled in the
    /// BaseActor class which extends BaseObject. 
    /// 
    /// </summary>

public class BaseObject : MonoBehaviour
{
    //PUBLIC VARIABLES
    public bool DebugMode = false;
    public bool GameObjectActiveOnStart = true;
    public bool RigidBodyAwakeOnStart = false;
    //public int UpdatesPerSecond = 30; //not particularly relevant for all objects? Probably belongs on BaseActor
    
    //PROTECTED REFERENCES
    protected Transform mTransform;
    protected GameObject mGameObject;
    protected Renderer mRenderer;
    protected Collider mCollider;
	protected BoxCollider mBoxCollider;
    protected Rigidbody mRigidbody;
    protected int mInstanceID;
	protected ParticleSystem mParticleSystem;
    
    //PROTECTED VARIABLES
    protected bool mIsHidden = false;
    protected EditorObject mLastActivator = null;
    protected Collider mLastCollider = null;
    
    //PRIVATE VARIABLES
    private float mUpdateInterval;
        
    //ACCESSORS
    public Transform BaseTransform
    {
        get { return mTransform; }
        set { mTransform = value; }
    }
    
    public GameObject BaseGameObject
    {
        get { return mGameObject; }
        set { mGameObject = value; }
    }
    
    public Renderer BaseRenderer
    {
        get { return mRenderer; }
        set { mRenderer = value; }
    }
    
    public Collider BaseCollider
    {
        get { return mCollider; }
        set { mCollider = value; }
    }
    
    public Rigidbody BaseRigidbody
    {
        get { return mRigidbody; }
        set { mRigidbody = value; }
    }

	public ParticleSystem BaseParticleSystem
	{
		get {  if (mParticleSystem == null){ mParticleSystem = gameObject.particleSystem; } return mParticleSystem; }
		set { mParticleSystem = value; }
	}
    
    public int BaseInstanceID
    {
        get { return mInstanceID; }
        set { mInstanceID = value; }
    }
    
    public EditorObject LastActivator
    {
        get { return mLastActivator; }
        set { mLastActivator = value; }
    }
    
    public Collider LastCollider
    {
        get { return mLastCollider; }
        set { mLastCollider = value; }
    }
    
    public bool IsHidden
    {
        get { return mIsHidden; }
        set { mIsHidden = value; }
    }
    
    virtual public void ToggleScript()
    {        
        if (this.enabled) { this.enabled = false; }
        else {this.enabled = true; }        
    }
    
    virtual protected void Awake ()
    {
        //grab component references
        mTransform = transform;
        mGameObject = gameObject;
        mRenderer = renderer;
        mCollider = collider;
		mBoxCollider = collider as BoxCollider;
        mRigidbody = rigidbody;
		mParticleSystem = particleSystem;
        mInstanceID = gameObject.GetInstanceID();
        
        //calculate the update interval (assuming ideal target of 60 fps)
        //mUpdateInterval = UpdatesPerSecond / 60f;
        
        //check conditions
        if (mRigidbody != null && !RigidBodyAwakeOnStart) { mRigidbody.Sleep(); }
        if (!GameObjectActiveOnStart) { mGameObject.SetActive(false); }
                
        //start coroutines
        if (mRenderer != null) { StartCoroutine( CheckIsHidden() ); } 
    }
    
    // Use this for initialization
    virtual protected void Start () 
    {
    
    }
        
    IEnumerator CheckIsHidden()
    {        
        while(true)
        {
            if (mIsHidden && mRenderer.enabled) { mRenderer.enabled = false; }
            else if (!mIsHidden && !mRenderer.enabled) { mRenderer.enabled = true; }
            yield return new WaitForSeconds(mUpdateInterval);
        }        
    }}
