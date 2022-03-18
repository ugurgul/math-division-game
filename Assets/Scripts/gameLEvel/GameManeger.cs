using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GameManeger : MonoBehaviour
{
    [SerializeField]
    private GameObject karePrefab;
    [SerializeField] 
    private Transform karelerPaneli;
    [SerializeField]
    private GameObject sonucPaneli;
    [SerializeField]
    private Text soruText;


    [SerializeField]
    AudioSource audioSource;

    public AudioClip butonSesi;
    
    private GameObject[] karelerDizisi = new GameObject[25];

    [SerializeField]
    private Transform soruPaneli;

    GameObject gecerliKare;

    [SerializeField]
    private Sprite[] kareSprites;

    List<int> bolumDegerleriListesi = new List<int>();

    int bolenSayi, bolunenSayi;
    int kacinciSoru;
    int butonDegeri;
    int dogruSonuc;
    int kalanHak;

    string sorununZorlukDerecesi;

    kalanHaklarManager kalanHaklarManager1;
    puanManager puanManager1;

    bool butonaBasilsinmi;




    private void Awake() {

        audioSource = GetComponent<AudioSource>();

        sonucPaneli.GetComponent<RectTransform>().localScale = Vector3.zero;
        kalanHak = 3;
        kalanHaklarManager1 = Object.FindObjectOfType<kalanHaklarManager>();
        puanManager1 = Object.FindObjectOfType<puanManager>();

        kalanHaklarManager1.KalanHaklariKontrolEt(kalanHak);
    }



    void Start()
    {
        butonaBasilsinmi=false;
        soruPaneli.GetComponent<RectTransform>().localScale = Vector3.zero;
        kareleriOlustur();
        
    }

    public void kareleriOlustur(){
        for(int i=0;i<25;i++){
            GameObject kare = Instantiate(karePrefab,karelerPaneli);
            kare.transform.GetChild(1).GetComponent<Image>().sprite = kareSprites[Random.Range(0,kareSprites.Length)];
            kare.transform.GetComponent<Button>().onClick.AddListener(()=> ButonaBasildi());
            karelerDizisi[i] = kare;
        }
        BolumDegerleriniTexteYazdir();
        StartCoroutine(DoFadeRoutine());

        Invoke("soruPaneliniAc",2f);

    }
    
    void ButonaBasildi(){

        

        if(butonaBasilsinmi){
        butonDegeri = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text);
        gecerliKare = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        SonucuKontrolEt();
        audioSource.PlayOneShot(butonSesi);
        }
    
    }

    void SonucuKontrolEt(){

        if(butonDegeri == dogruSonuc){
              
            gecerliKare.transform.GetChild(1).GetComponent<Image>().enabled = true;
            gecerliKare.transform.GetChild(0).GetComponent<Text>().text = "";
            gecerliKare.transform.GetComponent<Button>().interactable = false;

            puanManager1.PuaniArtir(sorununZorlukDerecesi);
            bolumDegerleriListesi.RemoveAt(kacinciSoru);

            if(bolumDegerleriListesi.Count > 0)
            {
               soruPaneliniAc();
            }
            else
            {
                OyunBitti();
            }
           
        }
        else{
            kalanHak--;
            kalanHaklarManager1.KalanHaklariKontrolEt(kalanHak);
        }

        if(kalanHak <= 0 )
        {
            OyunBitti();
        }

    }

    void OyunBitti (){
       butonaBasilsinmi = false;
       sonucPaneli.GetComponent<RectTransform>().DOScale(1,0.4f).SetEase(Ease.InBack);

    }

    IEnumerator DoFadeRoutine(){
        foreach(var kare in karelerDizisi){
            kare.GetComponent<CanvasGroup>().DOFade(1,0.2f);
            yield return new WaitForSeconds(0.06f);
        }
    }

    void BolumDegerleriniTexteYazdir(){

        foreach(var kare in karelerDizisi){

            int rastgeleDeger = Random.Range(1,13);
            bolumDegerleriListesi.Add(rastgeleDeger);
            kare.transform.GetChild(0).GetComponent<Text>().text = rastgeleDeger.ToString();
        }
    }

    void soruPaneliniAc()
    {
        soruyuSor();
        butonaBasilsinmi=true;
        soruPaneli.GetComponent<RectTransform>().DOScale(1,0.4f).SetEase(Ease.OutBack);
    }

    void soruyuSor(){

        bolenSayi = Random.Range(2,11);
        kacinciSoru = Random.Range(0,bolumDegerleriListesi.Count);
        dogruSonuc = bolumDegerleriListesi[kacinciSoru];
        bolunenSayi = bolenSayi * dogruSonuc;

        if(bolunenSayi <= 40)
        {
        sorununZorlukDerecesi = "kolay";
        }
        else if (bolunenSayi >40 && bolunenSayi<=80)
        {
            sorununZorlukDerecesi = "orta";
        }
        else
        {
            sorununZorlukDerecesi = "zor";
        }

        soruText.text = bolunenSayi.ToString() + " : " + bolenSayi.ToString();

    }



}
