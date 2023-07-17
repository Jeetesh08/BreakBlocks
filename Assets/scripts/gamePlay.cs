using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gamePlay : MonoBehaviour
{
    public GameObject BlockBustingFX;
    public AudioClip blockHit;
    public AudioClip wallHit;
    public AudioClip secondBall;
    public AudioClip sweepingSound;
    public bool collidedToF1 = false;
    public bool collidedToF2 = false;
    public static gamePlay instance;
    Vector2 prevPos;
    bool checkLevelNow = true;
    void MoveAllDownAddNewLevel()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float ratio = (float)screenHeight / (float)screenWidth;
        float downUnit = (1.777777778f * 0.55f) / ratio;
        if (GameManager.instance.addNewLevel)
        {
            for (int i = 0; i < GameManager.instance.BlockParent.transform.childCount; i++)
            {
                Vector2 oldPos = GameManager.instance.BlockParent.transform.GetChild(i).position;
                Vector2 newPos = new Vector2(oldPos.x, oldPos.y - downUnit);
                GameManager.instance.BlockParent.transform.GetChild(i).position = newPos;
            }
            for(int j =0; j< GameManager.instance.f1_gameobjectParent.transform.childCount; j++)
            {
                Vector2 oldPos = GameManager.instance.f1_gameobjectParent.transform.GetChild(j).position;
                Vector2 newPos = new Vector2(oldPos.x, oldPos.y - downUnit);
                GameManager.instance.f1_gameobjectParent.transform.GetChild(j).position = newPos;
            }
            for (int j = 0; j < GameManager.instance.f2_gameobjectParent.transform.childCount; j++)
            {
                Vector2 oldPos = GameManager.instance.f2_gameobjectParent.transform.GetChild(j).position;
                Vector2 newPos = new Vector2(oldPos.x, oldPos.y - downUnit);
                GameManager.instance.f2_gameobjectParent.transform.GetChild(j).position = newPos;
            }
            if(PlayerPrefs.HasKey("myLevel"))
            {
                PlayerPrefs.SetInt("myLevel", PlayerPrefs.GetInt("myLevel") + 1);
            }
            
            GameObject newBall = Instantiate(GameManager.instance.Ball, GameManager.instance.mainBall.transform.position, Quaternion.identity);
            newBall.transform.parent = GameManager.instance.BallParent.transform;
            GameManager.instance.players.Add(newBall);
            GameManager.instance.CreateLevel();
            PlayerPrefs.SetInt("spawn", 0);
            GameManager.instance.addNewLevel = false;
        }
        if(PlayerPrefs.HasKey("myLevel"))
        {
            GameManager.instance.SaveProgress(GameManager.instance.BlockParent);
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("f1") || collision.CompareTag("f1done") && !collidedToF1))
        {
            transform.GetChild(0).gameObject.SetActive(true);
            collidedToF1 = true;
            collision.gameObject.tag = "f1done";
            GameManager.instance.playThisSound(secondBall);
        }
        if((collision.CompareTag("f2") || collision.CompareTag("f2done") && !collidedToF2))
        {
            GameManager.instance.playThisSound(sweepingSound);
            collidedToF1 = true;
            collision.gameObject.tag = "f2done";
            string target = collision.gameObject.transform.GetChild(1).tag;
            float targetPos;
            if(target == "horizontalSwipe")
            {
                collision.gameObject.GetComponent<LineRenderer>().enabled = true;
                LineRenderer lr = collision.gameObject.GetComponent<LineRenderer>();
                lr.SetPosition(0, new Vector2(-5, collision.gameObject.transform.position.y));
                lr.SetPosition(1, new Vector2(5, collision.gameObject.transform.position.y));
                StartCoroutine(hideLinerenderer(collision.gameObject));
                targetPos = collision.gameObject.transform.position.y;
                for(int i = 0; i < GameManager.instance.BlockParent.transform.childCount; i++)
                {
                    if(GameManager.instance.BlockParent.transform.GetChild(i).transform.position.y == targetPos)
                    {
                        if ((int.Parse(GameManager.instance.BlockParent.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>().text) > 1))
                        {
                            GameManager.instance.score++;
                            GameManager.instance.BlockParent.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>().text = (int.Parse(GameManager.instance.BlockParent.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>().text) - 1).ToString();
                        }
                        else
                        {
                            Destroy(GameManager.instance.BlockParent.transform.GetChild(i).gameObject);
                        }
                    }
                }
            }
            else
            {
                collision.gameObject.GetComponent<LineRenderer>().enabled = true;
                LineRenderer lr = collision.gameObject.GetComponent<LineRenderer>();
                lr.SetPosition(0, new Vector2(collision.gameObject.transform.position.x, -10));
                lr.SetPosition(1, new Vector2(collision.gameObject.transform.position.x, 10));
                StartCoroutine(hideLinerenderer(collision.gameObject));
                targetPos = collision.gameObject.transform.position.x;
                for (int i = 0; i < GameManager.instance.BlockParent.transform.childCount; i++)
                {
                    if (GameManager.instance.BlockParent.transform.GetChild(i).transform.position.x == targetPos)
                    {
                        if((int.Parse(GameManager.instance.BlockParent.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>().text) > 1))
                        {
                            GameManager.instance.score++;
                            GameManager.instance.BlockParent.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>().text = (int.Parse(GameManager.instance.BlockParent.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>().text) - 1).ToString();
                        }
                        else
                        {
                            Destroy(GameManager.instance.BlockParent.transform.GetChild(i).gameObject);
                        }

                        
                    }
                }
            }
            GameManager.instance.playThisSound(secondBall);
        }
    }
    IEnumerator hideLinerenderer(GameObject object_)
    {
        yield return new WaitForSeconds(0.3f);
        object_.GetComponent<LineRenderer>().enabled = false;
        
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("bottomWall") && !GameManager.instance.MainBallDown)
        {
            if (!this.gameObject.transform.GetChild(0).gameObject.activeSelf)
            {
                this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                Vector2 pos = this.transform.position;
                Vector2 newPos = new Vector2(pos.x, GameManager.instance.base_.y);
                this.transform.position = newPos;
                GameManager.instance.GetMainBall(this.gameObject);
                GameManager.instance.addNewLevel = true;
                GameManager.instance.clampSpeedNow = false;
            }
            else
            {
                this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }

        }
        if (collision.collider.CompareTag("bottomWall") && GameManager.instance.MainBallDown)
        {
            if (!this.gameObject.transform.GetChild(0).gameObject.activeSelf)
            {
                snapAllBalls(this.gameObject);
                this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
            else
            {
                this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }

        }
        if (collision.collider.CompareTag("block"))
        {
            GameObject sfx = Instantiate(GameManager.instance.SoundChild, GameManager.instance.SoundParent.transform);
            sfx.AddComponent<AudioSource>();
            AudioSource temp = sfx.GetComponent<AudioSource>();
            temp.clip = blockHit;
            temp.Play();
            Destroy(sfx, 1);
            GameObject block = collision.collider.gameObject;
            string count = block.transform.GetChild(0).GetChild(0).GetComponent<Text>().text;
            float countVal = int.Parse(count);
            GameManager.instance.score++;
            if (countVal > 1)
            {

                if (block.transform.GetChild(0).GetChild(0).GetComponent<Text>().fontSize == 50)
                {
                    block.transform.GetChild(0).GetChild(0).GetComponent<Text>().fontSize = 30;
                }
                StartCoroutine(changeFontSize(block.transform.GetChild(0).GetChild(0).gameObject));

                block.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = (countVal - 1).ToString();
                if (PlayerPrefs.GetInt("colorBlock") != 5)
                {
                    PlayerPrefs.SetInt("colorBlock", PlayerPrefs.GetInt("colorBlock") + 1);
                }
                else if (PlayerPrefs.GetInt("colorBlock") == 5)
                {
                    PlayerPrefs.SetInt("colorBlock", 1);
                }
                switch (PlayerPrefs.GetInt("colorBlock"))
                {
                    case 1:
                        collision.collider.gameObject.GetComponent<SpriteRenderer>().color = GameManager.instance.color1;
                        break;
                    case 2:
                        collision.collider.gameObject.GetComponent<SpriteRenderer>().color = GameManager.instance.color2;
                        break;
                    case 3:
                        collision.collider.gameObject.GetComponent<SpriteRenderer>().color = GameManager.instance.color3;
                        break;
                    case 4:
                        collision.collider.gameObject.GetComponent<SpriteRenderer>().color = GameManager.instance.color4;
                        break;
                    case 5:
                        collision.collider.gameObject.GetComponent<SpriteRenderer>().color = GameManager.instance.color5;
                        break;
                    case 6:
                        collision.collider.gameObject.GetComponent<SpriteRenderer>().color = GameManager.instance.color6;
                        break;
                }

            }
            else if (countVal <= 1)
            {
                GameObject FX = Instantiate(BlockBustingFX, block.transform);
                collision.collider.gameObject.layer = 3;
                collision.collider.gameObject.GetComponent<SpriteRenderer>().color = GameManager.instance.color6;
                GameObject sfx2 = Instantiate(GameManager.instance.SoundChild, GameManager.instance.SoundParent.transform);
                sfx2.AddComponent<AudioSource>();
                AudioSource temp2 = sfx2.GetComponent<AudioSource>();
                temp2.clip = wallHit;
                StartCoroutine(latePlay(temp2));
                Destroy(sfx2, 1);
                FX.transform.parent = GameManager.instance.FX.transform;
                Destroy(block, 0.3f);
                Destroy(FX, 4);
            }
        }
    }
    IEnumerator latePlay(AudioSource aSource)
    {
        yield return new WaitForSeconds(0.2f);
        aSource.Play();

    }
    private void Update()
    {
        if (PlayerPrefs.GetInt("spawn") >= 1 && !GameManager.instance.projectiling)
        {
            MoveAllDownAddNewLevel();
        }
        if(checkLevelNow)
        {
            prevPos = this.gameObject.transform.position;
            StartCoroutine(checkSameLevelPos(prevPos));
        }
    }
    IEnumerator checkSameLevelPos(Vector2 prevPos)
    {
        checkLevelNow = false;
        yield return new WaitForSeconds(1);
        if(this.gameObject.transform.position.y < prevPos.y + 0.05f && this.gameObject.transform.position.y > prevPos.y - 0.05f)
        {
            if(this.gameObject.tag != "f1")
            {
                this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.005f;
            }
        }
        else
        {
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        }
        checkLevelNow = true;
    }
    void snapAllBalls(GameObject ballToSnap)
    {
        Vector2 mainBallPos = GameManager.instance.mainBall.transform.position;
        ballToSnap.transform.position = mainBallPos;
    }
    IEnumerator changeFontSize(GameObject text)
    {
        yield return new WaitForSeconds(0.1f);
        if(text)
        {
            text.GetComponent<Text>().fontSize = 50;
        }
        
    }
    IEnumerator destroyBlock(GameObject block)
    {
        yield return new WaitForSeconds(0.49f);
        Destroy(block);
    }
}
