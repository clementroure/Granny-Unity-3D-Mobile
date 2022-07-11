using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class Interact : MonoBehaviour {


    [Header("Interact Settings")]
    [HideInInspector]
    public GameControll m_gameController;
    [Tooltip("Distance of ray to interact")]
    public float rayDistance;
    [Tooltip("Layers to interact (default as obstacle)")]
    public LayerMask interactLayers;

    [Tooltip("Tags for interact")]
    public string interactTag;
    [Header("UI Settings")]
    private PlayerController player;

    [Header("Pictures Paper Settings")]
    public Transform m_examineTransform;
    public Transform m_paperScreenTransform;
    public Transform m_itemScreenTransform;
    public float m_moveSpeed;
    public float m_paperShowTime;
    public float m_itemShowTime;
    bool m_readPaper;
    bool m_takeItem;
    PicturePaper m_paper;
    Item m_item;
    int m_readState;
    int m_itemTakeState;


    private void Start()
    {
        m_gameController = FindObjectOfType<GameControll>();
        player = m_gameController.player;
    }

    private void Update()
    {
        if (!player.locked)
        {
            if (m_gameController.m_mobileTouchInput)
            {

                for (var i = 0; i < Input.touchCount; ++i)
                {                  
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                    {
                        if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                        {
                            RaycastHit hot;
                            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                            if (Physics.Raycast(ray, out hot, rayDistance, interactLayers))
                            {
                                if (hot.transform.gameObject.tag == interactTag)
                                {
                                    CheckRaycastedObject(hot.transform.gameObject, -1);
                                }
                            }
                        }

                    }
                }

            }

            if (!m_gameController.m_mobileTouchInput)
            {
                if (CrossPlatformInputManager.GetButtonDown("Interact"))
                {
                    if (EventSystem.current.IsPointerOverGameObject())    // is the touch on the GUI
                    {
                        return;
                    }
                    RaycastHit hot;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hot, rayDistance, interactLayers))
                    {
                        if (hot.transform.gameObject.tag == interactTag)
                        {
                            CheckRaycastedObject(hot.transform.gameObject,-1);
                        }
                    }
                }
            }
        }
        if (m_readPaper)
        {
            ReadPaper();
        }

        if (m_takeItem)
        {
            TakingItem();
        }
    }

    private void ReadPaper()
    {
        if(m_readPaper && m_paper != null)
        {
            if(m_readState == 0)
            {
                m_paper.transform.position = Vector3.Slerp(m_paper.transform.position, m_examineTransform.position, m_moveSpeed * Time.deltaTime);
                m_paper.transform.rotation = Quaternion.Slerp(m_paper.transform.rotation, m_examineTransform.rotation, m_moveSpeed * Time.deltaTime);
                float dist = Vector3.Distance(m_paper.transform.position, m_examineTransform.position);
                float ang = Quaternion.Angle(m_paper.transform.rotation, m_examineTransform.rotation);

                if (dist <= 0.5 && ang <= 0.5)
                {
                    m_paper.transform.position =m_examineTransform.position;
                    m_paper.transform.rotation = m_examineTransform.rotation;
                    m_readState = 1;
                    m_paper.ReadPaper();
                    StartCoroutine(WaitForPaperShow());

                    m_gameController.SetEffect(m_paper.m_pictureEffectType);

                }
            }

            if(m_readState == 2)
            {
                m_paper.transform.position = Vector3.Slerp(m_paper.transform.position, m_paperScreenTransform.position, m_moveSpeed * Time.deltaTime);
                m_paper.transform.rotation = Quaternion.Slerp(m_paper.transform.rotation, m_paperScreenTransform.rotation, m_moveSpeed * Time.deltaTime);
                float dist = Vector3.Distance(m_paper.transform.position, m_paperScreenTransform.position);
                float ang = Quaternion.Angle(m_paper.transform.rotation, m_paperScreenTransform.rotation);

                if (dist <= 1 && ang <= 1)
                {
                    m_gameController.m_nextPicture = m_paper.m_pictureIconUI;
                    m_readState = 0;
                    m_readPaper = false;
                    Destroy(m_paper.gameObject);
                    m_paper = null;                
                    m_gameController.AddPaperPicture();
                }
            }
        }
    }

    private void TakingItem()
    {
        if (m_takeItem && m_item != null)
        {
            if (m_itemTakeState == 0)
            {
                m_item.transform.position = Vector3.Slerp(m_item.transform.position, m_examineTransform.position, m_moveSpeed * Time.deltaTime);
                m_item.transform.rotation = Quaternion.Slerp(m_item.transform.rotation, m_examineTransform.rotation, m_moveSpeed * Time.deltaTime);
                float dist = Vector3.Distance(m_item.transform.position, m_examineTransform.position);
                float ang = Quaternion.Angle(m_item.transform.rotation, m_examineTransform.rotation);

                if (dist <= 0.5 && ang <= 0.5)
                {
                    m_item.transform.position = m_examineTransform.position;
                    m_item.transform.rotation = m_examineTransform.rotation;
                    m_itemTakeState = 1;
                    m_gameController.ShowTip(m_item.itemID,6);
                    StartCoroutine(WaitForItemShow());

                }
            }

            if (m_itemTakeState == 2)
            {
                m_item.transform.position = Vector3.Slerp(m_item.transform.position, m_itemScreenTransform.position, m_moveSpeed * Time.deltaTime);
                m_item.transform.rotation = Quaternion.Slerp(m_item.transform.rotation, m_itemScreenTransform.rotation, m_moveSpeed * Time.deltaTime);
                float dist = Vector3.Distance(m_item.transform.position, m_itemScreenTransform.position);
                float ang = Quaternion.Angle(m_item.transform.rotation, m_itemScreenTransform.rotation);

                if (dist <= 1 && ang <= 1)
                {

                    m_itemTakeState = 0;
                    m_gameController.inventory.AddItem(m_item.itemID, m_item.itemCount);
                    m_takeItem = false;
                    Destroy(m_item.gameObject);
                    m_item = null;                 
                }
            }
        }
    }

    public void DropItemCheck(int id)
    {
        if (!player.locked)
        {
            if (m_gameController.m_mobileTouchInput)
            {
               RaycastHit hot;
               Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
               if (Physics.Raycast(ray, out hot, rayDistance, interactLayers))
               {
                 if (hot.transform.gameObject.tag == interactTag)
                 {
                   CheckRaycastedObject(hot.transform.gameObject,id);
                 }
               }

            }

            if (!m_gameController.m_mobileTouchInput)
            {
                
                RaycastHit hot;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hot, rayDistance, interactLayers))
                {
                  if (hot.transform.gameObject.tag == interactTag)
                  {
                            CheckRaycastedObject(hot.transform.gameObject, id);
                  }
                }
                
            }
        }
    }

    private void CheckRaycastedObject(GameObject target, int dragID)
    {
        

            if (target.GetComponent<InteractObject>())
            {
               if(target.GetComponent<InteractObject>().m_itemToInteractId == -1 )
               {
                target.GetComponent<InteractObject>().Interact();
               }
              else
               {
                 if (dragID == target.GetComponent<InteractObject>().m_itemToInteractId)
                 {
                    target.GetComponent<InteractObject>().Interact();

                    if (target.GetComponent<InteractObject>().m_removeItemAfterUse)
                    {
                        m_gameController.inventory.RemoveItem(target.GetComponent<InteractObject>().m_itemToInteractId, 1);
                    }
                 }else
                 {
                    if (!target.GetComponent<InteractObject>().m_used || !target.GetComponent<InteractObject>().m_useOneTime)
                    {
                        m_gameController.ShowTip(target.GetComponent<InteractObject>().m_itemToInteractId, 0);
                    }
                }
               }
            }

            if (target.GetComponent<Item>() && !m_takeItem && !m_readPaper)
            {
                AudioSource.PlayClipAtPoint(target.GetComponent<Item>().pickupSound,transform.position);          
                m_takeItem = true;
                m_item = target.GetComponent<Item>();
                target.transform.parent = m_examineTransform;
            }

            if (target.GetComponent<SecurityComputer>())
            {
                m_gameController.SetSecurityCameras();
            }

            if (target.GetComponent<PicturePaper>() && !m_readPaper && !m_takeItem)
            {
                target.GetComponent<PicturePaper>().ShowPaper();
                AudioSource.PlayClipAtPoint(target.GetComponent<PicturePaper>().m_pickupSound, transform.position);
                m_readPaper = true;
                m_paper = target.GetComponent<PicturePaper>();
                target.transform.parent = m_examineTransform;
            }

            if (target.GetComponent<Door>())
            {
                
               target.GetComponent<Door>().Interact();
                
            }

        if (target.GetComponent<AutomaticDoor>())
        {
            if (!target.GetComponent<AutomaticDoor>().m_jamned)
            {
                if (target.GetComponent<AutomaticDoor>().m_locked)
                {
                    if (dragID == target.GetComponent<AutomaticDoor>().m_keyId)
                    {
                        m_gameController.inventory.RemoveItem(target.GetComponent<AutomaticDoor>().m_keyId, 1);
                        target.GetComponent<AutomaticDoor>().Unlock();
                    }else
                    {
                        
                        m_gameController.ShowTip(target.GetComponent<AutomaticDoor>().m_keyId, 0);
                    }

                }else
                {
                    if(target.GetComponent<AutomaticDoor>().m_needPower)
                    {
                        m_gameController.ShowTip(-1, 5);
                    }
                }
            }else
            {
                m_gameController.ShowTip(-1,1);
            }
        }

    }

    private IEnumerator WaitForPaperShow()
    {

        yield return new WaitForSeconds(m_paperShowTime);
        m_readState = 2;

    }

    private IEnumerator WaitForItemShow()
    {

        yield return new WaitForSeconds(m_itemShowTime);
        m_itemTakeState = 2;

    }
}
