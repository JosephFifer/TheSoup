using UnityEngine;

public class FingerPointer : MonoBehaviour
{
    private GameObject m_RightIndexTip;
    public GameObject m_FingerPointer;

    private bool isEnabled = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_RightIndexTip = GameObject.Find("R_IndexTip");
    }

    public void SetFingerPointerEnabled(bool enabled)
    {
        isEnabled = enabled;
        if (enabled)
        {
            Instantiate(m_FingerPointer, m_RightIndexTip.transform.position, Quaternion.identity);
            m_FingerPointer.SetActive(true);
        } else
        {
            m_FingerPointer.SetActive(false);
            Destroy(m_FingerPointer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {
            m_FingerPointer.transform.SetPositionAndRotation(m_RightIndexTip.transform.position, m_RightIndexTip.transform.rotation);
        }
    }
}
