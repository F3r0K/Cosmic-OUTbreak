using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FogOfWarManager : MonoBehaviour
{



    #region EDITOR
    [SerializeField]
    [Min(1)]
    [Tooltip("The width of the map in worldspace")]
    int m_MapWidth = 128;

    [SerializeField]
    [Min(1)]
    [Tooltip("The height of the map in worldspace")]
    int m_MapHeight = 128;

    [SerializeField]
    [Min(1)]
    [Tooltip("The resolution of the fog of war texture in pixels")]
    int m_Resolution = 256;
    int m_ResolutionHeight;

    [SerializeField]
    private ShaderReferencesScriptableObject m_ShaderReferencesObject;
    [SerializeField]
    private Transform m_FogPlane;

    [Space]
    [SerializeField]
    float m_FogUpdateInterval = 0.02f;
    [SerializeField]
    [Range(1,10)]
    float m_FogAppearSpeed = 3f;
    [SerializeField]
    [Tooltip("Should the vision dissapear when no vision components are in range, or should the discovered parts of the map stay visible")]
    bool m_VisionDissapear=false;

    [SerializeField]
    [Range(0,10)]
    int m_BlurIterations = 2;


    [SerializeField]
    Color m_FogColor=Color.black;
    [SerializeField]
    [Tooltip("The final result as a Fog Of War mask, Can be used to write a custom fog shader")]
    private RenderTexture m_FogOfWarOutput;
    [SerializeField]
    bool m_LogWarnings = true;

    #endregion


    public static FogOfWarManager INSTANCE;

    Material m_FogMaterial;
    Material m_BlurMaterial;
    float m_CellSize;


    Dictionary<Collider,List<Vector2Int>> m_VisionBlockers = new Dictionary<Collider, List<Vector2Int>>();

    Texture2D m_VisionBlockingTexture;

    RenderTexture m_FogOfWarResultRenderTexture;
    RenderTexture m_FogOfWarInputRenderTexture;
    RenderTexture m_VisionBlockingRenderTexture;
    RenderTexture m_BlurRenderTexture;

    FogOfWarComputeHandler m_FogOfWarComputeHandler;
    UpscaleFogComputeHandler m_UpscaleFogOfWarHandler;
    LerpTextureComputeHandler m_LerpComputeShaderHandler;
    FogOfWarCheckComputeHandler m_FogOfWarCheckComputeHandler;

    private bool m_Initialized = false;


    void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }

        //Find the blur material to gaussian blur
        m_BlurMaterial = new Material(Shader.Find("Blur"));
        
        Initialize();

        CheckRenderTexture();

        InvokeRepeating("UpdateFogOfWar", 0.0f, m_FogUpdateInterval);
        InvokeRepeating("LerpTexture", 0.0f, 0.02f);

        m_Initialized = true;
    }

    private void Initialize()
    {
        //Get the material of the fog plane
        if (m_FogPlane)
            m_FogMaterial = m_FogPlane.GetComponent<Renderer>().material;
        else if (m_LogWarnings)
            Debug.LogWarning("You have not assigned a fog plane. You can ignore this message if you use a custom fog shader.");

        if(m_FogMaterial)
            m_FogMaterial.SetColor("_Color", m_FogColor);

        //Get the resolution height
        float ResolutionScale = (float)m_Resolution / (float)m_MapWidth;
        float resolutionheight = m_MapHeight * ResolutionScale;
        m_ResolutionHeight = (int)resolutionheight;

        m_CellSize = 1 / ResolutionScale;

        //Create the vision blocking texture
        m_VisionBlockingTexture = new Texture2D(width: m_Resolution, height: m_ResolutionHeight, textureFormat: TextureFormat.R16, mipCount: 1, linear: true);

        //Create the vision blocking render texture
        m_VisionBlockingRenderTexture = new RenderTexture(m_Resolution, m_ResolutionHeight, 1, RenderTextureFormat.R16);
        m_VisionBlockingRenderTexture.enableRandomWrite = true;
        m_VisionBlockingRenderTexture.Create();

        //Create the blur render texture
        m_BlurRenderTexture = new RenderTexture(m_Resolution*4, m_ResolutionHeight*4, 1, RenderTextureFormat.ARGB64);
        m_BlurRenderTexture.enableRandomWrite = true;
        m_BlurRenderTexture.Create();


        //Create and initialize the compute handlers
        m_FogOfWarComputeHandler = new FogOfWarComputeHandler(m_ShaderReferencesObject.m_FogOfWarComputeShader, m_Resolution, m_ResolutionHeight, m_MapWidth, m_MapHeight, transform.position, m_VisionBlockingRenderTexture);
        m_UpscaleFogOfWarHandler = new UpscaleFogComputeHandler(m_ShaderReferencesObject.m_UpscaleComputeShader, m_Resolution, m_ResolutionHeight);
        m_LerpComputeShaderHandler = new LerpTextureComputeHandler(m_ShaderReferencesObject.m_LerpComputeShader, m_Resolution * 4, m_ResolutionHeight * 4, m_FogAppearSpeed, m_VisionDissapear);
        m_FogOfWarCheckComputeHandler = new FogOfWarCheckComputeHandler(m_ShaderReferencesObject.m_FogOfWarCheckComputeShader);

        m_FogOfWarComputeHandler.Initialize();
        m_UpscaleFogOfWarHandler.Initialize();
        m_LerpComputeShaderHandler.Initialize();
        m_FogOfWarCheckComputeHandler.Initialize();
    }

    void CleanupComputeHandlers() {

        if (m_FogOfWarComputeHandler!=null)
            m_FogOfWarComputeHandler.Dispose();
        if (m_UpscaleFogOfWarHandler != null)
            m_UpscaleFogOfWarHandler.Dispose();
        if (m_LerpComputeShaderHandler != null)
            m_LerpComputeShaderHandler.Dispose();
        if (m_FogOfWarCheckComputeHandler != null)
            m_FogOfWarCheckComputeHandler.Dispose();
    }

    private void CheckRenderTexture() {
        if (m_FogOfWarOutput && (m_LogWarnings))
        {
            if (m_FogOfWarOutput.width != m_Resolution * 4)
                UnityEngine.Debug.LogError("The width of the fog of war output render texture should be " + m_Resolution * 4 + " but is " + m_FogOfWarOutput.width);
            if (m_FogOfWarOutput.height != m_ResolutionHeight * 4)
                UnityEngine.Debug.LogError("The height of the fog of war output render texture should be " + m_ResolutionHeight * 4 + " but is " + m_FogOfWarOutput.height);
        }
    }


    private void OnDestroy()
    {
        CleanupComputeHandlers();

        //Clear fog of war output
        Graphics.SetRenderTarget(m_FogOfWarOutput);
        GL.Clear(true, true, new Color(0,0,0,0));

        if (INSTANCE == this)
        {
            INSTANCE = null;
        }
    }

    RenderTexture Blur(RenderTexture source, int iterations)
    {
        Graphics.CopyTexture(source,m_BlurRenderTexture);

        RenderTexture blit = RenderTexture.GetTemporary((int)m_Resolution * 4, (int)m_ResolutionHeight * 4);
        for (int i = 0; i < iterations; i++)
        {
            Graphics.SetRenderTarget(blit);
            GL.Clear(true, true, Color.black);
            Graphics.Blit(m_BlurRenderTexture, blit, m_BlurMaterial);
            Graphics.SetRenderTarget(m_BlurRenderTexture);
            GL.Clear(true, true, Color.black);
            Graphics.Blit(blit, m_BlurRenderTexture, m_BlurMaterial);
        }
        RenderTexture.ReleaseTemporary(blit);
        return m_BlurRenderTexture;
    }

    void UpdateFogOfWar() {
        m_FogOfWarInputRenderTexture = m_FogOfWarComputeHandler.Run();
        UpscaleTexture();
    }

    void UpscaleTexture()
    {
        m_UpscaleFogOfWarHandler.SetFogOfWarInput(m_FogOfWarInputRenderTexture);
        m_FogOfWarResultRenderTexture=m_UpscaleFogOfWarHandler.Run();
    }

    void LerpTexture() {
        if (m_FogOfWarInputRenderTexture)
        {
            m_LerpComputeShaderHandler.SetLerpInput(m_FogOfWarResultRenderTexture);

            var FogOfWarMask = Blur(m_LerpComputeShaderHandler.Run(), m_BlurIterations);

            m_FogOfWarCheckComputeHandler.SetFogOfWarFinalTexture(FogOfWarMask);

            if(m_FogMaterial)
                m_FogMaterial.mainTexture = FogOfWarMask;


            if (m_FogOfWarOutput)
                Graphics.CopyTexture(FogOfWarMask, m_FogOfWarOutput);
        }
    }

    public void SetVisionBlocker(Collider visionblocker)
    {
        if (!m_VisionBlockers.ContainsKey(visionblocker))
            m_VisionBlockers.Add(visionblocker,new List<Vector2Int>());

        m_VisionBlockers[visionblocker].Clear();
        GetColliderPositions(visionblocker);
        RecalculateBlockingMap();
    }

    public void RemoveVisionBlocker(Collider visionblocker)
    {
        m_VisionBlockers.Remove(visionblocker);
        RecalculateBlockingMap();
    }

    void RecalculateBlockingMap()
    {
        // Reset all pixels color to black
        Color resetColor = Color.black;
        Color[] resetColorArray = m_VisionBlockingTexture.GetPixels();

        for (int i = 0; i < resetColorArray.Length; i++)
        {
            resetColorArray[i] = resetColor;
        }

        m_VisionBlockingTexture.SetPixels(resetColorArray);

        foreach (List<Vector2Int> collisionpositions in m_VisionBlockers.Values)
        {
            foreach (Vector2Int position in collisionpositions)
            {
                m_VisionBlockingTexture.SetPixel(position.x, position.y, Color.white);
            }
        }


        m_VisionBlockingTexture.Apply();
        Graphics.CopyTexture(m_VisionBlockingTexture, m_VisionBlockingRenderTexture);
    }

    void ResizeVisionBlockerMap() {
        foreach (Collider collider in m_VisionBlockers.Keys)
        {
            m_VisionBlockers[collider].Clear();
            GetColliderPositions(collider);
        }
        RecalculateBlockingMap();

    }

    void GetColliderPositions(Collider collider)
    {
        Bounds colliderbounds = collider.bounds;
        Vector2Int minBound = WorldPositionToMapPosition(collider.bounds.min);
        Vector2Int maxBound = WorldPositionToMapPosition(collider.bounds.max);

        int counter=0;
        for (int x = minBound.x; x <= maxBound.x; x++)
        {
            for (int y = minBound.y; y <= maxBound.y; y++)
            {
                if ((x < m_Resolution && x > 0) && (y < m_ResolutionHeight && y > 0))
                {
                    Vector3 worldposition = MapPositionToWorldPosition(new Vector2Int(x, y));
                    worldposition.y = collider.bounds.max.y + 1.0f;

                    Ray ray = new Ray(worldposition + new Vector3(m_CellSize / 2, 0.0f, m_CellSize / 2), Vector3.down);
                    RaycastHit raycastHit;
                    if (collider.Raycast(ray, out raycastHit, collider.bounds.size.y + 2))
                    {
                        m_VisionBlockers[collider].Add(new Vector2Int(x, y));
                    }
                    counter++;
                }

            }
        }
    }

    Vector2Int WorldPositionToMapPosition(Vector3 position)
    {
        position -= transform.position;
        position += new Vector3(m_MapWidth / 2, 0f, m_MapHeight / 2);
        position /= m_CellSize;
        return new Vector2Int((int)position.x, (int)position.z);
    }
    Vector3 MapPositionToWorldPosition(Vector2Int position)
    {
        Vector3 worldposition = new Vector3(position.x, 0, position.y);
        worldposition *= m_CellSize;
        worldposition += transform.position; 
        worldposition -= new Vector3(m_MapWidth / 2, 0f, m_MapHeight / 2);
        return worldposition;
    }

    public bool IsPositionInFog(Vector3 worldposition)
    {
        Vector2Int mapposition = WorldPositionToMapPosition(worldposition) * 4;
        return m_FogOfWarCheckComputeHandler.IsInFog(mapposition);
    }


    private void OnDrawGizmosSelected()
    {
        float ResolutionScale = (float)m_MapWidth / (float)m_Resolution;

        for (float x = 0.0f; x < m_MapWidth; x+= ResolutionScale)
        {
            Vector3 position1 = transform.position + new Vector3(x - m_MapWidth / 2, 0, -m_MapHeight / 2);
            Vector3 position2 = transform.position + new Vector3(x - m_MapWidth / 2, 0, m_MapHeight / 2);
            Gizmos.DrawLine(position1, position2);
        }
        for (float y = 0.0f; y < m_MapHeight; y+= ResolutionScale)
        {
            Vector3 position1 = transform.position + new Vector3(-m_MapWidth / 2, 0, y - m_MapHeight / 2);
            Vector3 position2 = transform.position + new Vector3(m_MapWidth / 2, 0, y - m_MapHeight / 2);
            Gizmos.DrawLine(position1, position2);
        }

    }

    private void OnValidate()
    {
        if (Application.isPlaying && m_Initialized) //if we change something in the editor we want to update the values for immediate feedback
        {
            CleanupComputeHandlers();
            Initialize();
            ResizeVisionBlockerMap();
        }

        if(m_FogPlane)
            m_FogPlane.transform.localScale = new Vector3((float)m_MapWidth / 10.0f, 1.0f, (float)m_MapHeight / 10.0f);
    }
}
