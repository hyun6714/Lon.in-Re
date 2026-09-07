using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI frameText;
    [SerializeField] TextMeshProUGUI rebirthText;
    [SerializeField] TextMeshProUGUI eventTitleText;
    [SerializeField] TextMeshProUGUI eventCostText;
    [SerializeField] TextMeshProUGUI specialCoinText;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI goldSecText;
    [SerializeField] TextMeshProUGUI gameNumText;
    [SerializeField] TextMeshProUGUI peopleNumText;
    
    [SerializeField] Button levelUpBtn;
    [SerializeField] Button marketBtn;
    [SerializeField] Button gameBtn;
    [SerializeField] Button artifactBtn;
    [SerializeField] Button peopleBtn;
    [SerializeField] Button rebirthBtn;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

    }

}
